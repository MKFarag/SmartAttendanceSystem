using SmartAttendanceSystem.Infrastructure.Persistence.IdentityEntities;

namespace SmartAttendanceSystem.Application.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, dest => dest.Email);
    }
}
