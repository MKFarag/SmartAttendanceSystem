namespace SmartAttendanceSystem.Application.Services;

public class UserService

    #region Initial

    (UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IRoleClaimManager roleClaimManager,
    IStudentService studentService,
    IDbContextManager context) : IUserService
{
    private readonly IRoleClaimManager _roleClaimManager = roleClaimManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly IStudentService _studentService = studentService;
    private readonly IDbContextManager _context = context;

    #endregion

    #region User

    #region Get

    public async Task<object> GetProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var roles = await _roleClaimManager.GetRolesAsync(userId, true, cancellationToken);

        if (roles.Contains(DefaultRoles.Student))
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
                    roles,
                    (user.Department).Adapt<DepartmentResponse>(),
                    courses
                );

            return response;
        }
        else
            return await _userManager.Users
                .Where(x => x.Id == userId)
                .Select(x => new ProfileResponse(x.Name, x.Email!, roles))
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

    #region Admin

    #region GetAll

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    => await (from u in _userManager.Users
              join ur in _context.UserRoles
              on u.Id equals ur.UserId
              join r in _roleManager.Roles
              on ur.RoleId equals r.Id into roles
              where !roles.Any(x => x.Name == DefaultRoles.NotActiveInstructor)
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


    #endregion

    #endregion
}
