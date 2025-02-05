namespace SmartAttendanceSystem.Application.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        #region User

        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);

        #endregion

        #region Student

        config.NewConfig<Student, StudentResponse>()
            .Map(dest => dest.Name, src => src.User.Name)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.Department, src => src.Department.Name)
            .Map(dest => dest.Courses, src => src.Attendances!.Select(x => x.Course.Name));

        #region Attendance

        config.NewConfig<Student, StudentAttendanceResponse>()
            .Map(dest => dest.Name, src => src.User.Name)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.CourseAttendances.Select(x => x.Course), src => src.Attendances!.Select(x => x.Course))
            .Map(dest => dest.CourseAttendances.Select(x => x.Total), src => src.Attendances!.Select(x => x.Total));
            

        #region ByCourse

        config.NewConfig<Attendance, CourseAttendanceResponse>()
            .Map(dest => dest.Id, src => src.Student.Id)
            .Map(dest => dest.Name, src => src.Student.User.Name)
            .Map(dest => dest.Level, src => src.Student.Level)
            .Map(dest => dest.DepartmentName, src => src.Student.Department.Name)
            .Map(dest => dest.Total, src => src.Total);

        #endregion

        #region ByWeek

        config.NewConfig<Attendance, WeekAttendanceResponse>()
            .Map(dest => dest.Id, src => src.Student.Id)
            .Map(dest => dest.Name, src => src.Student.User.Name)
            .Map(dest => dest.Level, src => src.Student.Level)
            .Map(dest => dest.DepartmentName, src => src.Student.Department.Name)
            .Map(dest => dest.Attend, src => src.Weeks!.Week(MapContext.Current!.Get<int>("weekNum")));

        #endregion

        #region FpData

        config.NewConfig<Student, StdAttendAction>()
            .Map(dest => dest.Name, src => src.User.Name)
            .Map(dest => dest.DepartmentName, src => src.Department.Name);

        #endregion

        #endregion

        #region Profile

        config.NewConfig<ApplicationUser, StudentProfileResponse>()
            .Map(dest => dest.Level, src => src.StudentInfo!.Level)
            .Map(dest => dest.StudentId, src => src.StudentInfo!.Id)
            .Map(dest => dest.Department, src => src.StudentInfo!.Department);

        config.NewConfig<StudentAttendanceResponse, StudentProfileResponse>()
            .Map(dest => dest.StudentId, src => src.Id)
            .Map(dest => dest.Courses, src => src.CourseAttendances);

        #endregion

        #endregion
    }
}
