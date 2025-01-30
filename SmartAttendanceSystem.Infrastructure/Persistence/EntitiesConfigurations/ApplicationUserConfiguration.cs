namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .OwnsMany(u => u.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");

        builder.Property(u => u.Name)
            .HasMaxLength(200);

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        //DefaultData
        builder.HasData(new ApplicationUser
        {
            Id = DefaultUser.AdminId,
            Name = DefaultUser.AdminName,
            Email = DefaultUser.AdminEmail,
            UserName = DefaultUser.AdminEmail,
            NormalizedEmail = DefaultUser.AdminEmail.ToUpper(),
            NormalizedUserName = DefaultUser.AdminEmail.ToUpper(),
            ConcurrencyStamp = DefaultUser.AdminConcurrencyStamp,
            SecurityStamp = DefaultUser.AdminSecurityStamp,
            PasswordHash = passwordHasher.HashPassword(null!, DefaultUser.AdminPassword),
            EmailConfirmed = true
        });
    }
}
