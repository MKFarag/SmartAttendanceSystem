namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class DepartmentCoursesConfiguration : IEntityTypeConfiguration<DepartmentCourses>
{
    public void Configure(EntityTypeBuilder<DepartmentCourses> builder)
    {
        builder.HasKey(dc => new { dc.DepartmentId, dc.CourseId });
    }
}
