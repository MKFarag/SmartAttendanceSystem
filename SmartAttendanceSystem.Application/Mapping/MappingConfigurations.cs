namespace SmartAttendanceSystem.Application.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        #region User

        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);

        config.NewConfig<CreateUserRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.EmailConfirmed, src => true)
            .Map(dest => dest.IsStudent, src => false);

        config.NewConfig<(ApplicationUser user, IList<string> roles), UserResponse>()
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Roles, src => src.roles);

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

        #endregion
    }
}
