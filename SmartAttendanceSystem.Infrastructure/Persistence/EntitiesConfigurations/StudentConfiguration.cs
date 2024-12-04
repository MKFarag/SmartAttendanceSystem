namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder
            .HasOne(s => s.User)
            .WithOne(u => u.StudentInfo)
            .HasForeignKey<Student>(s => s.UserId);
    }
}
