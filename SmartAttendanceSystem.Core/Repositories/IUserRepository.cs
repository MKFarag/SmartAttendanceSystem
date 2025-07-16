namespace SmartAttendanceSystem.Core.Repositories;

public interface IUserRepository : IGenericRepository<ApplicationUser, string>
{
    /// <summary>Get all projections with roles for users</summary>
    Task<IEnumerable<TProjection>> GetAllProjectionWithRolesAsync<TProjection>(bool includeDefaultRole, CancellationToken cancellationToken = default)
        where TProjection : class;

    /// <summary>Get roles and permissions for a user</summary>
    Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetRolesAndPermissionsAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    /// <summary>Check if the email exists</summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Check if the email is already used by another user</summary>
    Task<bool> EmailExistsAsync(string email, string userId, CancellationToken cancellationToken = default);

    /// <summary>Bulk update the user's name by user id</summary>
    Task BulkNameUpdateAsync(string userId, string name, CancellationToken cancellationToken = default);

    /// <summary>Bulk delete for all roles for a specific user by user id</summary>
    Task BulkDeleteAllRolesAsync(string userId, CancellationToken cancellationToken = default);

    #region UserManager Methods

    /// <summary>Create a new user with a password</summary>
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);

    /// <summary>Update a user</summary>
    Task<IdentityResult> UpdateAsync(ApplicationUser user);

    /// <summary>Check if the user is locked out</summary>
    Task<bool> IsLockedOutAsync(ApplicationUser user);

    /// <summary>Set the lockout end date for a user</summary>
    Task<IdentityResult> SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd);

    /// <summary>Generate email confirmation token for a user</summary>
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);

    /// <summary>Confirm the user's email with a token</summary>
    Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);

    /// <summary>Generate a token for changing the user's email</summary>
    Task<string> GenerateChangeEmailTokenAsync(ApplicationUser user, string newEmail);

    /// <summary>Change the user's email with a token</summary>
    Task<IdentityResult> ChangeEmailAsync(ApplicationUser user, string newEmail, string token);

    /// <summary>Reset the user's password</summary>
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);

    /// <summary>Reset the user's password with a token</summary>
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);

    /// <summary>Change the user's password</summary>
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);

    /// <summary>Add a user to roles</summary>
    Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles);

    /// <summary>Add a user to a specific role</summary>
    Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);

    /// <summary>Get roles for a specific user</summary>
    Task<IList<string>> GetRolesAsync(ApplicationUser user);

    /// <summary>Find a user by email</summary>
    Task<ApplicationUser?> FindByEmailAsync(string email);

    /// <summary>Normalize the email address</summary>
    string NormalizeEmail(string email);

    /// <summary>Normalize the user's name</summary>
    string NormalizeName(string name);

    #endregion
}
