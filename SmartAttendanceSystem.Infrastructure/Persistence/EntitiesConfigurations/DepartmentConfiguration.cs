namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(100);

        // Seed data
        builder.HasData(
            new Department { Id = 1, Name = "Computer Science" },
            new Department { Id = 2, Name = "Mathematics" },
            new Department { Id = 3, Name = "Physics" },
            new Department { Id = 4, Name = "Chemistry" }
        );
    }
}
