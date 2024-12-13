namespace SmartAttendanceSystem.Application.ServicesImplementation;

public class UserService(UserManager<ApplicationUser> userManager, IStudentService studentService) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IStudentService _studentService = studentService;

    public async Task<Result<StudentProfileResponse>> GetStudentProfileAsync(string userId)
    {
        var userResult = await _studentService.StudentAttendance(userId);

        if (userResult.IsFailure)
            return Result.Failure<StudentProfileResponse>(userResult.Error);

        var profileResponse = userResult.Value.Adapt<StudentProfileResponse>();

        return Result.Success(profileResponse);
    }

    public async Task<Result<InstructorProfileResponse>> GetInstructorProfileAsync(string userId)
        => Result.Success(await _userManager.Users.Where(x => x.Id == userId).ProjectToType<InstructorProfileResponse>().FirstAsync());
}
