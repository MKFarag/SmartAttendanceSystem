namespace SmartAttendanceSystem.Application.Abstraction.DTOsServices;

public interface IUserService
{
    Task<Result<StudentProfileResponse>> GetStudentProfileAsync(string userId);

    Task<Result<InstructorProfileResponse>> GetInstructorProfileAsync(string userId);
}
