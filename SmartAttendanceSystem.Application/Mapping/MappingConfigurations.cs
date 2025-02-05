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
            .Map(dest => dest.CourseAttendances, src => src.Attendances);
            

        #region ByCourse

        config.NewConfig<Attendance, CourseAttendanceResponse>()
            .Map(dest => dest.Id, src => src.Student.Id)
            .Map(dest => dest.Name, src => src.Student.User.Name)
            .Map(dest => dest.Level, src => src.Student.Level)
            .Map(dest => dest.Department, src => src.Student.Department.Name)
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

        #endregion

        #region Profile

        config.NewConfig<(ApplicationUser user, IList<string> roles), StudentProfileResponse>()
            .Map(dest => dest.Name, src => src.user.Name)
            .Map(dest => dest.Email, src => src.user.Email)
            .Map(dest => dest.Level, src => src.user.StudentInfo!.Level)
            .Map(dest => dest.StudentId, src => src.user.StudentInfo!.Id)
            .Map(dest => dest.Department, src => src.user.StudentInfo!.Department)
            .Map(dest => dest.CourseAttendances, src => src.user.StudentInfo!.Attendances)
            .Map(dest => dest.Roles, src => src.roles);

        config.NewConfig<(ApplicationUser user, IList<string> roles), ProfileResponse>()
            .Map(dest => dest.Name, src => src.user.Name)
            .Map(dest => dest.Email, src => src.user.Email)
            .Map(dest => dest.Roles, src => src.roles);

        #endregion

        #endregion
    }
}
