using Microsoft.AspNetCore.WebUtilities;

namespace SmartAttendanceSystem.Application.Services;

public class AuthService

#region Initialize Fields

    (IOptions<EmailTemplateOptions> templateData,
    IOptions<InstructorPassword> instructorPassword,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    ILogger<AuthService> logger,
    IUserService userService,
    IJwtProvider jwtProvider,
    IEmailSender emailSender,
    IJobManager jobManager) : IAuthService
{
    private readonly InstructorPassword _instructorPassword = instructorPassword.Value;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly EmailTemplateOptions _templateData = templateData.Value;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IUserService _userService = userService;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IJobManager _jobManager = jobManager;

    private readonly int _refreshTokenExpiryDays = 14;
    private static readonly SemaphoreSlim _registrationLock = new(1, 1);

    #endregion

    #region Login

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var result = await _signInManager.PasswordSignInAsync(user, password, false, true);

        if (result.Succeeded)
        {
            var (userRoles, userPermissions) = await _userService.GetRolesAndClaimsAsync(user, cancellationToken);

            if (userRoles.Contains(DefaultRoles.Student.Name))
                return Result.Failure<AuthResponse>(UserErrors.NoPermission);

            var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(user.Id, user.Email, user.Name, token, expiresIn, refreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }

        var error = result.IsNotAllowed
            ? UserErrors.EmailNotConfirmed
            : result.IsLockedOut
            ? UserErrors.LockedUser
            : UserErrors.InvalidCredentials;

        return Result.Failure<AuthResponse>(error);
    }

    #region RefreshToken

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        if (_jwtProvider.ValidateToken(token) is not { } userId)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        if (user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive) is not { } userRefreshToken)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (userRoles, userPermissions) = await _userService.GetRolesAndClaimsAsync(user, cancellationToken);

        var (NewToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email, user.Name, NewToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        if (_jwtProvider.ValidateToken(token) is not { } userId)
            return Result.Failure(UserErrors.InvalidJwtToken);

        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.InvalidJwtToken);

        if (user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive) is not { } userRefreshToken)
            return Result.Failure(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    #endregion

    #endregion

    #region Register

    public async Task<Result<string>> RegisterAsync(RegisterRequest request, bool confirmWithLink = true, CancellationToken cancellationToken = default)
    {
        await _registrationLock.WaitAsync(cancellationToken);
        try
        {
            if (!request.InstructorPassword.Equals(_instructorPassword.Value))
                return Result.Failure<string>(UserErrors.InvalidRolePassword);

            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
                return Result.Failure<string>(UserErrors.DuplicatedEmail);

            var user = request.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var code = await GenerateEmailConfirmationCode(user, confirmWithLink);

                #if DEBUG
                _logger.LogInformation("Confirm code: {code}", code);
                #endif

                SendConfirmationEmail(user, code, confirmWithLink);

                return Result.Success(user.Id);
            }

            var error = result.Errors.First();
            return Result.Failure<string>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        finally
        {
            _registrationLock.Release();
        }
    }

    #region Email Confirmation

    public async Task<Result> ConfirmEmailAsync(string userId, string code)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        bool confirmWithCode = code.Length == 6 && user.IsEmailConfirmationCodeActive;

        IdentityResult result;

        if (confirmWithCode)
        {
            if (user.EmailConfirmationCode == code && user.IsEmailConfirmationCodeActive)
            {
                user.EmailConfirmed = true;
                user.EmailConfirmationCode = null;
                user.EmailConfirmationCodeExpiration = null;

                result = await _userManager.UpdateAsync(user);
            }
            else
                return Result.Failure(UserErrors.InvalidCode);
        }
        else
        {
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {
                return Result.Failure(UserErrors.InvalidCode);
            }

            result = await _userManager.ConfirmEmailAsync(user, code);
        }

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, DefaultRoles.Instructor.Name);

            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ResendConfirmationEmailAsync(string email, bool confirmWithLink = true)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = await GenerateEmailConfirmationCode(user, confirmWithLink);

        #if DEBUG
        _logger.LogInformation("Confirm code: {code}", code);
        #endif

        SendConfirmationEmail(user, code, confirmWithLink);

        return Result.Success();
    }

    #endregion

    #endregion

    #region Forget Password

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed with { StatusCode = StatusCodes.Status400BadRequest });

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        #if DEBUG
        _logger.LogInformation("Reset code: {code}", code);
        #endif

        SendResetPasswordEmail(user, code);

        return Result.Success();

    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }

    #endregion

    #region PrivatesMethods

    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private async Task<string> GenerateEmailConfirmationCode(ApplicationUser user, bool confirmWithLink)
    {
        string code;

        if (confirmWithLink)
        {
            code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        }
        else
        {
            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            code = (BitConverter.ToUInt32(bytes) % 900000 + 100000).ToString();

            user.EmailConfirmationCode = code;
            user.EmailConfirmationCodeExpiration = DateTime.UtcNow.AddMinutes(10);
            await _userManager.UpdateAsync(user);
        }

        _logger.LogInformation("Confirm code: {code}", code);

        return code;
    }

    private void SendConfirmationEmail(ApplicationUser user, string code, bool confirmWithLink)
    {
        string emailBody;

        if (confirmWithLink)
        {
            //var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmationLink",
                new Dictionary<string, string>
                {
                    { EmailTemplateOptions.Placeholders.BaseUrl, _templateData.BaseUrl },
                    { EmailTemplateOptions.Placeholders.TitleName, _templateData.TitleName },
                    { EmailTemplateOptions.Placeholders.TeamName, _templateData.TeamName },
                    { EmailTemplateOptions.Placeholders.Address, _templateData.Address },
                    { EmailTemplateOptions.Placeholders.City, _templateData.City },
                    { EmailTemplateOptions.Placeholders.Country, _templateData.Country },
                    { EmailTemplateOptions.Placeholders.SupportEmail, _templateData.SupportEmail},
                    { EmailTemplateOptions.Placeholders.UserName,  user.Name},

                    //FrontEnd should tell me where the user will go with what queries
                    //{ EmailTemplateOptions.Placeholders.Action_url, $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
                    { EmailTemplateOptions.Placeholders.Action_url, $"https://localhost:7120/public/users/confirm-email?UserId={user.Id}&Code={code}" }
                }
            );
        }
        else
        {
            emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmationCode",
                new Dictionary<string, string>
                {
                    { EmailTemplateOptions.Placeholders.BaseUrl, _templateData.BaseUrl },
                    { EmailTemplateOptions.Placeholders.TitleName, _templateData.TitleName },
                    { EmailTemplateOptions.Placeholders.TeamName, _templateData.TeamName },
                    { EmailTemplateOptions.Placeholders.Address, _templateData.Address },
                    { EmailTemplateOptions.Placeholders.City, _templateData.City },
                    { EmailTemplateOptions.Placeholders.Country, _templateData.Country },
                    { EmailTemplateOptions.Placeholders.SupportEmail, _templateData.SupportEmail},
                    { EmailTemplateOptions.Placeholders.UserName, user.Name},
                    { EmailTemplateOptions.Placeholders.Code, code}
                }
            );
        }

        _jobManager.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Api.Builder email confirmation", emailBody));
    }

    private void SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ResetPassword",
            new Dictionary<string, string>
            {
                { EmailTemplateOptions.Placeholders.BaseUrl, _templateData.BaseUrl },
                { EmailTemplateOptions.Placeholders.TitleName, _templateData.TitleName },
                { EmailTemplateOptions.Placeholders.TeamName, _templateData.TeamName },
                { EmailTemplateOptions.Placeholders.Address, _templateData.Address },
                { EmailTemplateOptions.Placeholders.City, _templateData.City },
                { EmailTemplateOptions.Placeholders.Country, _templateData.Country },
                { EmailTemplateOptions.Placeholders.SupportEmail, _templateData.SupportEmail},
                { EmailTemplateOptions.Placeholders.UserName,  user.Name},

                //FrontEnd should tell me where the user will go with what queries
                { EmailTemplateOptions.Placeholders.Action_url, $"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
            }
        );

        _jobManager.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Api.Builder reset password", emailBody));
    }

    #endregion
}
