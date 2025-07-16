using Microsoft.AspNetCore.WebUtilities;

namespace SmartAttendanceSystem.Application.Services;

public class UserService
    (IEmailTemplateService emailTemplateService, ILogger<UserService> logger, IUnitOfWork unitOfWork) : IUserService
{
    private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;
    private readonly ILogger<UserService> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    #region For dashboard

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _unitOfWork.Users.GetAllProjectionWithRolesAsync<UserResponse>(false, cancellationToken);

    public async Task<Result<UserResponse>> GetAsync(string userId)
    {
        if (await _unitOfWork.Users.GetAsync(userId) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.NotFound);

        var roles = await _unitOfWork.Users.GetRolesAsync(user);

        var response = (user, roles).Adapt<UserResponse>();

        return Result.Success(response);
    }

    public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var allowedRoles = await _unitOfWork.Roles.FindAllProjectionAsync
            (r => !r.IsDefault, r => r.Name, true, cancellationToken);

        if (request.Roles.Except(allowedRoles).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var user = request.Adapt<ApplicationUser>();

        var result = await _unitOfWork.Users.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _unitOfWork.Users.AddToRolesAsync(user, request.Roles);

            var response = (user, request.Roles).Adapt<UserResponse>();

            return Result.Success(response);
        }

        var error = result.Errors.First();
        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.AnyAsync(x => x.Email == request.Email && x.Id != userId, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        var allowedRoles = await _unitOfWork.Roles.FindAllProjectionAsync
            (r => !r.IsDefault, r => r.Name, true, cancellationToken);

        if (request.Roles.Except(allowedRoles).Any())
            return Result.Failure(UserErrors.InvalidRoles);

        if (await _unitOfWork.Users.GetAsync(userId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        user = request.Adapt(user);

        var result = await _unitOfWork.Users.UpdateAsync(user);

        if (result.Succeeded)
        {
            await _unitOfWork.Users.BulkDeleteAllRolesAsync(userId, cancellationToken);

            await _unitOfWork.Users.AddToRolesAsync(user, request.Roles);

            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ToggleStatusAsync(string userId)
    {
        if (await _unitOfWork.Users.GetAsync(userId) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        user.IsDisabled = !user.IsDisabled;

        var result = await _unitOfWork.Users.UpdateAsync(user);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UnlockAsync(string userId)
    {
        if (await _unitOfWork.Users.GetAsync(userId) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        if (!await _unitOfWork.Users.IsLockedOutAsync(user))
            return Result.Failure(UserErrors.NotLockedUser);

        var result = await _unitOfWork.Users.SetLockoutEndDateAsync(user, null);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    #endregion

    #region For user profile

    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userResponse = await _unitOfWork.Users.FindProjectionAsync<UserProfileResponse>
            (x => x.Id == userId, cancellationToken);

        return userResponse is not null
            ? Result.Success(userResponse)
            : Result.Failure<UserProfileResponse>(UserErrors.NotFound);
    }

    public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Users.ExistsAsync(userId, cancellationToken))
            return Result.Failure(UserErrors.NotFound);

        await _unitOfWork.Users.BulkNameUpdateAsync(userId, request.Name, cancellationToken);

        if (await _unitOfWork.Users.GetAsync(userId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        user.Name = request.Name;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        if (await _unitOfWork.Users.GetAsync(userId) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        var result = await _unitOfWork.Users.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ChangeEmailRequestAsync(string userId, string newEmail)
    {
        newEmail = _unitOfWork.Users.NormalizeEmail(newEmail);

        if (await _unitOfWork.Users.AnyAsync(x => x.NormalizedEmail == newEmail && x.Id != userId))
            return Result.Failure(UserErrors.DuplicatedEmail);

        if (await _unitOfWork.Users.GetAsync(userId) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        if (user.NormalizedEmail == newEmail)
            return Result.Failure(UserErrors.SameEmail);

        var code = await _unitOfWork.Users.GenerateChangeEmailTokenAsync(user, newEmail);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Change email code: {code}", code);

        _emailTemplateService.SendConfirmationLink(user, code);

        return Result.Success();
    }

    public async Task<Result> ConfirmChangeEmailAsync(string userId, ConfirmChangeEmailRequest request)
    {
        if (await _unitOfWork.Users.GetAsync(userId) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        var oldEmail = user.Email;
        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _unitOfWork.Users.ChangeEmailAsync(user, request.NewEmail, code);
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
        {
            _emailTemplateService.SendChangeEmailNotification(user, oldEmail!, DateTime.UtcNow);

            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    #endregion
}