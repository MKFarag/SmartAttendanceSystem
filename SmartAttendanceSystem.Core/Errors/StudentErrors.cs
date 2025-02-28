namespace SmartAttendanceSystem.Core.Errors;

public static class StudentErrors
{
    public static readonly Error NotFount =
        new("Student.NotFount", "No student found", StatusCodes.Status404NotFound);

    public static readonly Error CourseNotFound =
        new("Student.CourseNotFound", "No course found by this id", StatusCodes.Status404NotFound);

    public static readonly Error NotAddedCourse =
        new("Student.NotAddedCourse", "No student has added this course", StatusCodes.Status404NotFound);

    public static readonly Error AlreadyRegistered =
        new("Student.AlreadyRegistered", "Attendance for this week has been pre-registered", StatusCodes.Status400BadRequest);

    public static readonly Error NoCourses =
        new("Student.NoCourses", "No courses added", StatusCodes.Status404NotFound);

    public static readonly Error AlreadyHaveFp =
        new("Student.AlreadyHaveFp", "This student has a fingerprint Id already", StatusCodes.Status409Conflict);

    public static Error CourseNotAdded(int id) =>
        new("Student.CourseNotAdded", $"The course of id {id} has not been added to your courses", StatusCodes.Status404NotFound);
}
