using System.Reflection;

namespace SmartAttendanceSystem.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    #region DbSet

    public DbSet<Attendance> Attendances { get; set; } = default!;
    public DbSet<Course> Courses { get; set; } = default!;
    public DbSet<Department> Departments { get; set; } = default!;
    public DbSet<Student> Students { get; set; } = default!;

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        #region Change delete behavior

        var CascadeFk = builder.Model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(Fk => Fk.DeleteBehavior == DeleteBehavior.Cascade && !Fk.IsOwnership);

        foreach (var Fk in CascadeFk)
            Fk.DeleteBehavior = DeleteBehavior.Restrict;

        #region Configure Specific Cascade Delete Relationships

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.StudentInfo)
            .WithOne(s => s.User)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Student>()
            .HasMany(s => s.Attendances)
            .WithOne(a => a.Student)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #endregion

    }
}
