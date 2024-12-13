namespace SmartAttendanceSystem.Application.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);

        config.NewConfig<Student, StudentResponse>()
            .Map(dest => dest.Name, src => src.User.Name)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.Courses, src => src.Attendances!.Select(x => x.Course));

        config.NewConfig<Student, StdAttendanceByCourseResponse>()
            .Map(dest => dest.Name, src => src.User.Name)
            .Map(dest => dest.Email, src => src.User.Email);

        config.NewConfig<Attendance, StdAttendanceByCourseResponse>()
            .Map(dest => dest.Id, src => src.Student.Id)
            .Map(dest => dest.Name, src => src.Student.User.Name)
            .Map(dest => dest.Email, src => src.Student.User.Email)
            .Map(dest => dest.Level, src => src.Student.Level)
            .Map(dest => dest.Department, src => src.Student.Department)
            .Map(dest => dest.Total, src => src.Total);

        config.NewConfig<Attendance, StdAttendanceByWeekResponse>()
            .Map(dest => dest.Id, src => src.Student.Id)
            .Map(dest => dest.Name, src => src.Student.User.Name)
            .Map(dest => dest.Email, src => src.Student.User.Email)
            .Map(dest => dest.Level, src => src.Student.Level)
            .Map(dest => dest.Department, src => src.Student.Department)
            .Map(dest => dest.Attend, src => src.Weeks!.Week(MapContext.Current!.Get<int>("weekNum")));

        config.NewConfig<Student, StdAttendanceByWeekResponse>()
            .Map(dest => dest.Name, src => src.User.Name)
            .Map(dest => dest.Email, src => src.User.Email);

        config.NewConfig<Student, StudentAttendanceResponse>()
            .Map(dest => dest.Name, src => src.User.Name)
            .Map(dest => dest.Email, src => src.User.Email);

        config.NewConfig<ApplicationUser, StudentProfileResponse>()
            .Map(dest => dest.Level, src => src.StudentInfo!.Level)
            .Map(dest => dest.StudentId, src => src.StudentInfo!.Id)
            .Map(dest => dest.Department, src => src.StudentInfo!.Department);

        config.NewConfig<StudentAttendanceResponse, StudentProfileResponse>()
            .Map(dest => dest.StudentId, src => src.Id)
            .Map(dest => dest.Courses, src => src.CourseAttendances)
            .Map(dest => dest.Type, src => "Student");
        
        config.NewConfig<ApplicationUser, InstructorProfileResponse>()
            .Map(dest => dest.Type, src => "Instructor");
    }
}
