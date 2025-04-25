namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class DepartmentCoursesConfiguration : IEntityTypeConfiguration<DepartmentCourses>
{
    public void Configure(EntityTypeBuilder<DepartmentCourses> builder)
    {
        builder.HasKey(dc => new { dc.DepartmentId, dc.CourseId });

        // Seed data for DepartmentCourses with updated CourseIds
        var departmentCourses = new List<DepartmentCourses>();

        // Computer Science
        for (int i = 1; i <= 30; i++)
            departmentCourses.Add(new DepartmentCourses { DepartmentId = 1, CourseId = i });

        // Mathematics
        departmentCourses.Add(new DepartmentCourses { DepartmentId = 2, CourseId = 13 }); // Calculus 2
        departmentCourses.Add(new DepartmentCourses { DepartmentId = 2, CourseId = 14 }); // Ordinary Differential Equations
        departmentCourses.Add(new DepartmentCourses { DepartmentId = 2, CourseId = 19 }); // Abstract Algebra 1
        departmentCourses.Add(new DepartmentCourses { DepartmentId = 2, CourseId = 20 }); // Ordinary Differential Equation Theory
        
        for (int i = 31; i <= 51; i++)
            departmentCourses.Add(new DepartmentCourses { DepartmentId = 2, CourseId = i });

        // Physics
        departmentCourses.Add(new DepartmentCourses { DepartmentId = 3, CourseId = 1 });  // Linear Algebra 1
        departmentCourses.Add(new DepartmentCourses { DepartmentId = 3, CourseId = 2 });  // Numerical Analysis 1
        departmentCourses.Add(new DepartmentCourses { DepartmentId = 3, CourseId = 13 }); // Calculus 2
        departmentCourses.Add(new DepartmentCourses { DepartmentId = 3, CourseId = 14 }); // Ordinary Differential Equations
        departmentCourses.Add(new DepartmentCourses { DepartmentId = 3, CourseId = 33 }); // Solid Geometry
        departmentCourses.Add(new DepartmentCourses { DepartmentId = 3, CourseId = 44 }); // Mathematical Analysis

        for (int i = 52; i <= 73; i++)
            departmentCourses.Add(new DepartmentCourses { DepartmentId = 3, CourseId = i });

        // Chemistry
        for (int i = 74; i <= 99; i++)
            departmentCourses.Add(new DepartmentCourses { DepartmentId = 4, CourseId = i });

        builder.HasData(departmentCourses);
    }
}
