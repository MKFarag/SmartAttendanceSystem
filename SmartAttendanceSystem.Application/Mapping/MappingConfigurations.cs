namespace SmartAttendanceSystem.Application.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);

        config.NewConfig<ApplicationUser, StudentResponse>()
            .Map(dest => dest.Id, src => src.StudentInfo!.Id)
            .Map(dest => dest.Level, src => src.StudentInfo!.Level)
            .Map(dest => dest.DeptName, src => src.StudentInfo!.Department.Name);
        
        config.NewConfig<Student, StudentResponse>()
            .Map(dest => dest.Name, src => src.User.Name)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.DeptName, src => src.Department.Name);
    }
}
