namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder
            .HasIndex(s => s.FingerId)
            .IsUnique();

        builder
            .Property(s => s.UserId)
            .IsRequired();

        builder
            .HasIndex(s => s.UserId)
            .IsUnique();
    }
}
