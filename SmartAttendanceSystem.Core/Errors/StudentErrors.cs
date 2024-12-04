namespace SmartAttendanceSystem.Core.Errors;

public static class StudentErrors
{
    public static readonly Error IdNotFount =
        new("Student.IdNotFount", "No student found by this id", StatusCodes.Status404NotFound);

    public static readonly Error CourseNotFound =
        new("Student.CourseNotFound", "No course found by this id", StatusCodes.Status404NotFound);
    
    public static readonly Error NotAddedCourse =
        new("Student.NotAddedCourse", "No student has added this course", StatusCodes.Status404NotFound);
    
    public static readonly Error NoCourses =
        new("Student.NoCourses", "No courses added", StatusCodes.Status404NotFound);
    
    public static Error CourseNotAdded(int id) =>
        new("Student.CourseNotAdded", $"The course of id {id} has not been added to your courses", StatusCodes.Status404NotFound);
}
