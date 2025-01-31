namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(new IdentityUserRole<string>
        {
            UserId = DefaultUser.AdminId,
            RoleId = DefaultRoles.AdminRoleId,
        });
    }
}
