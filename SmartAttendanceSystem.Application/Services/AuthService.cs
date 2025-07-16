using Microsoft.AspNetCore.WebUtilities;

namespace SmartAttendanceSystem.Application.Services;

public class AuthService
    (IEmailTemplateService emailTemplateService, IOptions<InstructorPassword> instructorPassword, ISignInService signInService,
    IJwtProvider jwtProvider, ILogger<AuthService> logger, IUnitOfWork unitOfWork) : IAuthService
{
    private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;
    private readonly InstructorPassword _instructorPassword = instructorPassword.Value;
    private readonly ISignInService _signInService = signInService;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private readonly int _refreshTokenExpiryDays = 14;
    private readonly int _confirmationCodeExpiryMinutes = 10;

    #region Login

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var result = await _signInService.PasswordSignInAsync(user, password, false, true);

        if (result.Succeeded)
        {
            var (userRoles, userPermissions) = await _unitOfWork.Users.GetRolesAndPermissionsAsync(user, cancellationToken);

            // NOTE: We close the login for students for now (Upon the instructor's request)
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

            await _unitOfWork.Users.UpdateAsync(user);

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

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        if (_jwtProvider.ValidateToken(token) is not { } userId)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (await _unitOfWork.Users.GetAsync(userId, cancellationToken) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        if (user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive) is not { } userRefreshToken)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (userRoles, userPermissions) = await _unitOfWork.Users.GetRolesAndPermissionsAsync(user, cancellationToken);

        var (NewToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _unitOfWork.Users.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email, user.Name, NewToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        if (_jwtProvider.ValidateToken(token) is not { } userId)
            return Result.Failure(UserErrors.InvalidJwtToken);

        if (await _unitOfWork.Users.GetAsync(userId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.InvalidJwtToken);

        if (user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive) is not { } userRefreshToken)
            return Result.Failure(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(user);

        return Result.Success();
    }

    #endregion

    #region Register

    public async Task<Result> RegisterAsync(RegisterRequest request, bool confirmWithLink = true, CancellationToken cancellationToken = default)
    {
        if (!request.InstructorPassword.Equals(_instructorPassword.Value))
            return Result.Failure(UserErrors.InvalidRolePassword);

        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

        var result = await _unitOfWork.Users.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var code = await GenerateEmailConfirmationCode(user, confirmWithLink);

            if (confirmWithLink)
                _emailTemplateService.SendConfirmationLink(user, code);
            else
                _emailTemplateService.SendConfirmationCode(user, code, _confirmationCodeExpiryMinutes);

            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string code)
    {
        if (await _unitOfWork.Users.GetAsync(userId) is not { } user)
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

                result = await _unitOfWork.Users.UpdateAsync(user);
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

            result = await _unitOfWork.Users.ConfirmEmailAsync(user, code);
        }

        if (result.Succeeded)
        {
            await _unitOfWork.Users.AddToRoleAsync(user, DefaultRoles.Instructor.Name);

            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ResendConfirmationEmailAsync(string email, bool confirmWithLink = true)
    {
        if (await _unitOfWork.Users.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = await GenerateEmailConfirmationCode(user, confirmWithLink);

        if (confirmWithLink)
            _emailTemplateService.SendConfirmationLink(user, code);
        else
            _emailTemplateService.SendConfirmationCode(user, code, _confirmationCodeExpiryMinutes);

        return Result.Success();
    }

    #endregion

    #region Forget Password

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if (await _unitOfWork.Users.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed with { StatusCode = StatusCodes.Status400BadRequest });

        var code = await _unitOfWork.Users.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Reset code: {code}", code);

        _emailTemplateService.SendResetPassword(user, code);

        return Result.Success();

    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _unitOfWork.Users.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _unitOfWork.Users.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(new IdentityError
            {
                Code = UserErrors.InvalidToken.Code,
                Description = UserErrors.InvalidToken.Description
            });
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
            code = await _unitOfWork.Users.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        }
        else
        {
            code = Random.Shared.Next(100000, 999999).ToString();

            user.EmailConfirmationCode = code;
            user.EmailConfirmationCodeExpiration = DateTime.UtcNow.AddMinutes(_confirmationCodeExpiryMinutes);
            await _unitOfWork.Users.UpdateAsync(user);
        }

        _logger.LogInformation("Confirm code: {code}", code);

        return code;
    }

    #endregion
}
