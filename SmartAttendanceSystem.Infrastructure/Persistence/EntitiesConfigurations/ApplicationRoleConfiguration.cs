namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData([
            new ApplicationRole
            {
                Id = DefaultRoles.AdminRoleId,
                Name = DefaultRoles.Admin,
                NormalizedName = DefaultRoles.Admin.ToUpper(),
                ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp
            },
            new ApplicationRole
            {
                Id = DefaultRoles.StudentRoleId,
                Name = DefaultRoles.Student,
                NormalizedName = DefaultRoles.Student.ToUpper(),
                ConcurrencyStamp = DefaultRoles.StudentRoleConcurrencyStamp,
                IsDefault = true
            },
            new ApplicationRole
            {
                Id = DefaultRoles.InstructorRoleId,
                Name = DefaultRoles.Instructor,
                NormalizedName = DefaultRoles.Instructor.ToUpper(),
                ConcurrencyStamp = DefaultRoles.InstructorRoleConcurrencyStamp
            }
        ]);
    }
}
