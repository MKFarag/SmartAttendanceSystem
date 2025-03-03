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

        //DefaultData
        builder.HasData(new ApplicationUser
        {
            Id = DefaultUser.Admin.Id,
            Name = DefaultUser.Admin.Name,
            Email = DefaultUser.Admin.Email,
            UserName = DefaultUser.Admin.Email,
            NormalizedEmail = DefaultUser.Admin.Email.ToUpper(),
            NormalizedUserName = DefaultUser.Admin.Email.ToUpper(),
            ConcurrencyStamp = DefaultUser.Admin.ConcurrencyStamp,
            SecurityStamp = DefaultUser.Admin.SecurityStamp,
            PasswordHash = DefaultUser.Admin.PasswordHash,
            EmailConfirmed = true
        });
    }
}
