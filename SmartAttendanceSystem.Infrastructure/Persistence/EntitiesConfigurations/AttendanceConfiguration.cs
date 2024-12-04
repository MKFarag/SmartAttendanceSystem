namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder
            .HasIndex(x => new { x.StudentId, x.CourseId })
            .IsUnique();

        builder
            .OwnsOne(x => x.Weeks)
            .ToTable("Weeks")
            .WithOwner()
            .HasForeignKey("AttendanceId");
    }
}
