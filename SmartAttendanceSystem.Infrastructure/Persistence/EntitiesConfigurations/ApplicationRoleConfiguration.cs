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
                Id = DefaultRoles.MemberRoleId,
                Name = DefaultRoles.Member,
                NormalizedName = DefaultRoles.Member.ToUpper(),
                ConcurrencyStamp = DefaultRoles.MemberRoleConcurrencyStamp,
                IsDefault = true
            },
            new ApplicationRole
            {
                Id = DefaultRoles.StudentRoleId,
                Name = DefaultRoles.Student,
                NormalizedName = DefaultRoles.Student.ToUpper(),
                ConcurrencyStamp = DefaultRoles.StudentRoleConcurrencyStamp
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
