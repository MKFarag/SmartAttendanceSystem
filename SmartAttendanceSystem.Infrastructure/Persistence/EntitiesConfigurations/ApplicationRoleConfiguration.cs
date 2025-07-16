namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData([
            new ApplicationRole
            {
                Id = DefaultRoles.Admin.Id,
                Name = DefaultRoles.Admin.Name,
                NormalizedName = DefaultRoles.Admin.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp
            },
            new ApplicationRole
            {
                Id = DefaultRoles.Student.Id,
                Name = DefaultRoles.Student.Name,
                NormalizedName = DefaultRoles.Student.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Student.ConcurrencyStamp
            },
            new ApplicationRole
            {
                Id = DefaultRoles.Instructor.Id,
                Name = DefaultRoles.Instructor.Name,
                NormalizedName = DefaultRoles.Instructor.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Instructor.ConcurrencyStamp,
                IsDefault = true
            },
            new ApplicationRole
            {
                Id = DefaultRoles.Graduate.Id,
                Name = DefaultRoles.Graduate.Name,
                NormalizedName = DefaultRoles.Graduate.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Graduate.ConcurrencyStamp
            }
        ]);
    }
}
