using Microsoft.AspNetCore.WebUtilities;

namespace SmartAttendanceSystem.Application.Services;

public class AuthService

    #region Initialize Fields

    (IOptions<EmailConfirmationSettings> emailOptions,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IDepartmentService departmentService,
    ILogger<AuthService> logger,
    IJobScheduler jobScheduler,
    IDbContextHelper context,
    IJwtProvider jwtProvider,
    IEmailSender emailSender) : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly EmailConfirmationSettings _emailOptions = emailOptions.Value;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IDepartmentService _deptService = departmentService;
    private readonly IJobScheduler _jobScheduler = jobScheduler;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IDbContextHelper _context = context;
    private readonly int _refreshTokenExpiryDays = 14;

    #endregion

    #region Login

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

        if (result.Succeeded)
        {
            var (userRoles, userPermissions) = await _context.GetUserRolesAndPermissionsAsync(user, cancellationToken);

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

        return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);
    }

    #region RefreshToken

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        if (_jwtProvider.ValidateToken(token) is not { } userId)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive) is not { } userRefreshToken)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (userRoles, userPermissions) = await _context.GetUserRolesAndPermissionsAsync(user, cancellationToken);

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

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

        if (!request.IsStudent.HasValue)
            user.IsStudent = false;

        #region AddStudentData

        if (user.IsStudent)
        {
            if (!await _deptService.AnyAsync(x => x.Id == request.DeptId, cancellationToken))
                return Result.Failure(UserErrors.AddDeptRelation);

            user.StudentInfo = new Student
            {
                UserId = user.Id,
                Level = request.Level,
                DepartmentId = request.DeptId
            };

            //TODO: Fingerprint Register

        }
        #endregion

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirm code: {code}", code);

            await SendConfirmationEmail(user, code);

            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    #region Email Confirmation

    public async Task<Result> ConfirmEmailAsync(string userId, string code)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            if (user.IsStudent)
                await _userManager.AddToRoleAsync(user, DefaultRoles.Student);
            else
                await _userManager.AddToRoleAsync(user, DefaultRoles.Instructor);

            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ResendConfirmationEmailAsync(string email)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirm code: {code}", code);

        await SendConfirmationEmail(user, code);

        return Result.Success();
    }

    #endregion

    #endregion

    #region Forget Password

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        //Check Confirmation

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Reset code: {code}", code);

        await SendResetPasswordEmail(user, code);

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

    private async Task SendConfirmationEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            new Dictionary<string, string>
            {
                { EmailConfirmationSettings.PTitleName, _emailOptions.TitleName },
                { EmailConfirmationSettings.PTeamName, _emailOptions.TeamName },
                { EmailConfirmationSettings.PAddress, _emailOptions.Address },
                { EmailConfirmationSettings.PCity, _emailOptions.City },
                { EmailConfirmationSettings.PCountry, _emailOptions.Country },
                { EmailConfirmationSettings.PUserName,  user.Name},

                //FrontEnd should tell me where the user will go with what queries
                { EmailConfirmationSettings.PAction_url, $"{origin}/auth/emailConfirmation?userId={user.Id}$code={code}" }
            }
        );

        _jobScheduler.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Smart Attendance System", emailBody));

        await Task.CompletedTask;
    }

    private async Task SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ResetPassword",
            new Dictionary<string, string>
            {
                { EmailConfirmationSettings.PTitleName, _emailOptions.TitleName },
                { EmailConfirmationSettings.PTeamName, _emailOptions.TeamName },
                { EmailConfirmationSettings.PAddress, _emailOptions.Address },
                { EmailConfirmationSettings.PCity, _emailOptions.City },
                { EmailConfirmationSettings.PCountry, _emailOptions.Country },
                { EmailConfirmationSettings.PUserName,  user.Name},

                //FrontEnd should tell me where the user will go with what queries
                { EmailConfirmationSettings.PAction_url, $"{origin}/auth/forgetPassword?email={user.Email}$code={code}" }
            }
        );

        _jobScheduler.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Smart Attendance System", emailBody));

        await Task.CompletedTask;
    }

    #endregion
}
