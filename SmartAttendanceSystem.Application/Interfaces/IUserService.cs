namespace SmartAttendanceSystem.Application.Interfaces;

public interface IUserService
{
    Task<Result<StudentProfileResponse>> GetStudentProfileAsync(string userId);

    Task<Result<InstructorProfileResponse>> GetInstructorProfileAsync(string userId);

    Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);

    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
