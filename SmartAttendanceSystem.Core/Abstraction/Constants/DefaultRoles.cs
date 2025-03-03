namespace SmartAttendanceSystem.Core.Abstraction.Constants;

public static class DefaultRoles
{
    public partial class Admin
    {
        public const string Name = nameof(Admin);
        public const string Id = "0194ba0b-a50d-7568-b187-227d0faed2e9";
        public const string ConcurrencyStamp = "0194ba0b-a50d-7568-b187-227ea7268643";
    }

    public partial class Member
    {
        public const string Name = nameof(Member);
        public const string Id = "019519df-03ea-78aa-8c1f-86eccefa5aeb";
        public const string ConcurrencyStamp = "019519df-29b5-7992-a8e6-846793cb078f";
    }

    public partial class Student
    {
        public const string Name = nameof(Student);
        public const string Id = "0194ba0b-a50d-7568-b187-2279f6b03b05";
        public const string ConcurrencyStamp = "0194ba0b-a50d-7568-b187-227aba8bc12f";
    }

    public partial class Instructor
    {
        public const string Name = nameof(Instructor);
        public const string Id = "0194bd46-ceca-7ea4-9d02-9e268a031756";
        public const string ConcurrencyStamp = "0194bd47-1793-75b0-a10b-9de81d580d84";
    }
}
