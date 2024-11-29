
namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.Property(x => x.Code)
            .HasMaxLength(4);
    }
}
