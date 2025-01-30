namespace SmartAttendanceSystem.Core.CustomAttribute;

public class StudentCheckAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not ApplicationUser user)
            return false;

        if (user.IsStudent && user.StudentInfo == null)
        {
            ErrorMessage = "A user marked as a student must have associated student information";
            return false;
        }
        if (!user.IsStudent && user.StudentInfo != null)
        {
            ErrorMessage = "A user not marked as a student cannot have associated student information";
            return false;
        }

        return true;
    }
}
