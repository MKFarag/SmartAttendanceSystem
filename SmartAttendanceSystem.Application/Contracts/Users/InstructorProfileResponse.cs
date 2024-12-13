namespace SmartAttendanceSystem.Application.Contracts.Users;

public record InstructorProfileResponse(
    string Name,
    string Email,
    string Type
);
