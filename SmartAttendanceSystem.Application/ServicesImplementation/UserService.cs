namespace SmartAttendanceSystem.Application.ServicesImplementation;

public class UserService(UserManager<ApplicationUser> userManager, IStudentService studentService) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IStudentService _studentService = studentService;

    public async Task<Result<StudentProfileResponse>> GetStudentProfileAsync(string userId)
    {
        var userResult = await _studentService.StudentAttendance(UserId: userId);

        if (userResult.IsFailure)
            return Result.Failure<StudentProfileResponse>(userResult.Error);

        var profileResponse = userResult.Value.Adapt<StudentProfileResponse>();

        return Result.Success(profileResponse);
    }

    public async Task<Result<InstructorProfileResponse>> GetInstructorProfileAsync(string userId)
        => Result.Success(await _userManager.Users.Where(x => x.Id == userId).ProjectToType<InstructorProfileResponse>().FirstAsync());

    public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        //var user = await _userManager.FindByIdAsync(userId);
        //user = request.Adapt(user);
        //await _userManager.UpdateAsync(user!);

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
}
