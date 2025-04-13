namespace SmartAttendanceSystem.Application.Services;

public class UserService

#region Initial

    (UserManager<ApplicationUser> userManager,
    IStudentService studentService,
    IDbContextManager context,
    IRoleService roleService) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IStudentService _studentService = studentService;
    private readonly IRoleService _roleService = roleService;
    private readonly IDbContextManager _context = context;

    #endregion

    #region Admin (Dashboard)

    #region Get

    //GET ALL
    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    => await (from u in _userManager.Users
              join ur in _context.UserRoles
              on u.Id equals ur.UserId
              join r in _roleService.Roles
              on ur.RoleId equals r.Id into roles
              where !roles.Any(x => x.Name == DefaultRoles.Member.Name)
              select new
              {
                  u.Id,
                  u.Name,
                  u.Email,
                  u.IsDisabled,
                  Roles = roles.Select(x => x.Name!).ToList()
              })
              .GroupBy(u => new { u.Id, u.Name, u.Email, u.IsDisabled })
              .Select(u => new UserResponse
              (
                  u.Key.Id,
                  u.Key.Name,
                  u.Key.Email,
                  u.Key.IsDisabled,
                  u.SelectMany(x => x.Roles)
              ))
              .ToListAsync(cancellationToken);

    //GET
    public async Task<Result<UserResponse>> GetAsync(string Id)
    {
        if (await _userManager.FindByIdAsync(Id) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.NotFount);

        var roles = await _userManager.GetRolesAsync(user);

        var response = (user, roles).Adapt<UserResponse>();

        return Result.Success(response);
    }

    #endregion

    #region Add

    public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var allowedRoles = await _roleService.GetAllNamesAsync(cancellationToken: cancellationToken);

        if (request.Roles.Except(allowedRoles).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, request.Roles);

            var response = (user, request.Roles).Adapt<UserResponse>();

            return Result.Success(response);
        }

        var error = result.Errors.First();
        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    #endregion

    #region Update

    //UPDATE
    public async Task<Result> UpdateAsync(string Id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != Id, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        var allowedRoles = await _roleService.GetAllNamesAsync(cancellationToken: cancellationToken);

        if (request.Roles.Except(allowedRoles).Any())
            return Result.Failure(UserErrors.InvalidRoles);

        if (await _userManager.FindByIdAsync(Id) is not { } user)
            return Result.Failure(UserErrors.NotFount);

        user = request.Adapt(user);

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            await _context.UserRoles
                    .Where(x => x.UserId == Id)
                    .ExecuteDeleteAsync(cancellationToken);

            await _userManager.AddToRolesAsync(user, request.Roles);

            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    //TOGGLE STATUS
    public async Task<Result> ToggleStatusAsync(string Id)
    {
        if (await _userManager.FindByIdAsync(Id) is not { } user)
            return Result.Failure(UserErrors.NotFount);

        user.IsDisabled = !user.IsDisabled;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    //UNLOCK
    public async Task<Result> UnlockAsync(string Id)
    {
        if (await _userManager.FindByIdAsync(Id) is not { } user)
            return Result.Failure(UserErrors.NotFount);

        var result = await _userManager.SetLockoutEndDateAsync(user, null);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    #endregion

    #endregion

    #region User Profile (Public)

    #region Get

    public async Task<object> GetProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var stdCheck = await _context.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == DefaultRoles.Student.Id, cancellationToken: cancellationToken);

        if (stdCheck)
        {
            var user = await _userManager.Users
                .Where(x => x.Id == userId)
                .Select(x => new
                {
                    x.StudentInfo!.Id,
                    x.Name,
                    x.Email,
                    x.StudentInfo.Level,
                    x.StudentInfo.Department
                }
                )
                .AsNoTracking()
                .FirstAsync(cancellationToken);

            var courses = await _studentService.GetCoursesWithAttendancesDTOsAsync(user.Id, cancellationToken: cancellationToken);

            StudentProfileResponse response = new
                (
                    user.Id,
                    user.Name,
                    user.Email!,
                    user.Level,
                    (user.Department).Adapt<DepartmentResponse>(),
                    courses
                );

            return response;
        }

        return await _userManager.Users
            .Where(x => x.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .AsNoTracking()
            .FirstAsync(cancellationToken);
    }

    #endregion

    #region Update

    public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        await _userManager.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.Name, request.Name)
            );

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    #endregion

    #endregion

    #region Get User Roles & Claims

    public async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetRolesAndClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var userPermissions = await (from r in _roleService.Roles
                                     join p in _roleService.RoleClaims
                                     on r.Id equals p.RoleId
                                     where userRoles.Contains(r.Name!)
                                     select p.ClaimValue!)
                                     .Distinct()
                                     .ToListAsync(cancellationToken);

        return (userRoles, userPermissions);
    }

    #endregion
}
