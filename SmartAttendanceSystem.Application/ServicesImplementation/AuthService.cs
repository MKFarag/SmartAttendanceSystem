using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace SmartAttendanceSystem.Application.ServicesImplementation;

public class AuthService(
    IOptions<EmailConfirmationSettings> emailOptions,
    SignInManager<ApplicationUser> signInManager,
    IJwtProvider<ApplicationUser> jwtProvider,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IDepartmentService departmentService,
    ILogger<AuthService> logger,
    IEmailSender emailSender
    ) 
    : IAuthService<AuthResponse, RegisterRequest>
{
    #region InitializeTheFields

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly EmailConfirmationSettings _emailOptions = emailOptions.Value;
    private readonly IJwtProvider<ApplicationUser> _jwtProvider = jwtProvider;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IDepartmentService _deptService = departmentService;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ILogger<AuthService> _logger = logger;

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
            var (token, expiresIn) = _jwtProvider.GenerateToken(user);

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

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        if (_jwtProvider.ValidateToken(token) is not { } userId)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive) is not { } userRefreshToken)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (NewToken, expiresIn) = _jwtProvider.GenerateToken(user);
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

    #region Register

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

        #region AddStudentData

        if (request.IsStudent)
        {
            if (await _deptService.AnyAsync(x => x.Id == request.DeptId, cancellationToken))
                return Result.Failure(UserErrors.AddDeptRelation);

            user.StudentInfo = new Student
            {
                UserId = user.Id,
                Level = request.Level,
                DepartmentId = request.DeptId
            };
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
            return Result.Success();

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

        await _emailSender.SendEmailAsync(user.Email!, "✅ Smart Attendance System", emailBody);
    }

    #endregion
}
