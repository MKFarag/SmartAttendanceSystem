namespace SmartAttendanceSystem.Application.Services;

public class RoleAskService
    (IOptions<InstructorRoleSettings> instructorOptions,
    UserManager<ApplicationUser> userManager,
    IDepartmentService departmentService) : IRoleAskService
{
    private readonly InstructorRoleSettings _instructorOptions = instructorOptions.Value;
    private readonly IDepartmentService _departmentService = departmentService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    //INSTRUCTOR
    public async Task<Result> RoleAskAsync(string UserId, InstructorRoleAskRequest request)
    {
        if (!request.Password.Equals(_instructorOptions.Password))
            return Result.Failure(UserErrors.InvalidRolePassword);

        if (await _userManager.FindByIdAsync(UserId) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.NotFount);

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains(DefaultRoles.Student.Name))
            return Result.Failure(UserErrors.NoPermission);

        if (roles.Contains(DefaultRoles.Instructor.Name))
            return Result.Failure(UserErrors.AlreadyInRole);

        var result = await _userManager.AddToRoleAsync(user, DefaultRoles.Instructor.Name);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        return Result.Success();
    }

    //STUDENT
    public async Task<Result> RoleAskAsync(string UserId, StudentRoleAskRequest request)
    {
        if (!await _departmentService.AnyAsync(x => x.Id == request.DepartmentId))
            return Result.Failure(GlobalErrors.IdNotFound("Departments"));

        if (await _userManager.FindByIdAsync(UserId) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.NotFount);

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains(DefaultRoles.Instructor.Name))
            return Result.Failure(UserErrors.NoPermission);

        if (roles.Contains(DefaultRoles.Student.Name))
            return Result.Failure(UserErrors.AlreadyInRole);

        user.StudentInfo = new Student
        {
            DepartmentId = request.DepartmentId,
            Level = request.Level,
        };

        var result = await _userManager.AddToRoleAsync(user, DefaultRoles.Student.Name);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        return Result.Success();
    }
}
