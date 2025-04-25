namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        int seedId = 1;

        var allPermissions = Permissions.GetAllPermissions();
        var adminClaims = new List<IdentityRoleClaim<string>>();

        var instructorPermissions = Permissions.GetInstructorPermissions();
        var instructorClaims = new List<IdentityRoleClaim<string>>();

        foreach (var permission in allPermissions)
            adminClaims.Add(new IdentityRoleClaim<string>
            {
                Id = seedId++,
                ClaimType = Permissions.Type,
                ClaimValue = permission,
                RoleId = DefaultRoles.Admin.Id,
            });

        foreach (var permission in instructorPermissions)
            instructorClaims.Add(new IdentityRoleClaim<string>
            {
                Id = seedId++,
                ClaimType = Permissions.Type,
                ClaimValue = permission,
                RoleId = DefaultRoles.Instructor.Id,
            });

        builder.HasData(adminClaims);
        builder.HasData(instructorClaims);
    }
}
