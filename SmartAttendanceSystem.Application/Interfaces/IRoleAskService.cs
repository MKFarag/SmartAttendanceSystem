namespace SmartAttendanceSystem.Application.Interfaces;

public interface IRoleAskService
{
    //ASK FOR INSRUCTOR ROLE
    Task<Result> RoleAskAsync(string UserId, InstructorRoleAskRequest request);

    //ASK FOR STUDENT ROLE
    Task<Result> RoleAskAsync(string UserId, StudentRoleAskRequest request);
}
