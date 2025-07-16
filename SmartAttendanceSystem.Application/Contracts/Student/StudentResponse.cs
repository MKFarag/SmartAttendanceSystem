namespace SmartAttendanceSystem.Application.Contracts.Student;

public record StudentResponse(
    int Id,
    string Name,
    string Email,
    int Level,
    string Department,
    IEnumerable<string>? Courses
);

/// <summary>
/// Simple version of StudentResponse
/// </summary>
public record StudentResponseV2(
    int Id,
    string Name,
    int Level,
    string Department
);

/// <summary>
/// A new version of StudentResponse for adding method and getting student who has no fingerId
/// </summary>
public record StudentResponseV3(
    int Id,
    string Name,
    string Email,
    int Level,
    string Department
);