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

        builder.HasOne(u => u.StudentInfo)
            .WithOne(s => s.User)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
