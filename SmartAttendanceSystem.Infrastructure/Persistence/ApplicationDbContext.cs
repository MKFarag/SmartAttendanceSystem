using System.Reflection;

namespace SmartAttendanceSystem.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    #region DbSet

    public DbSet<Attendance> Attendances { get; set; } = default!;
    public DbSet<Course> Courses { get; set; } = default!;
    public DbSet<Department> Departments { get; set; } = default!;
    public DbSet<Student> Students { get; set; } = default!;

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);

        #region Change delete behavior

        var CascadeFk = builder.Model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(Fk => Fk.DeleteBehavior == DeleteBehavior.Cascade && !Fk.IsOwnership);

        foreach (var Fk in CascadeFk)
            Fk.DeleteBehavior = DeleteBehavior.Restrict;

        #endregion
    }

    #region Override-SaveChangesAsync

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries<ApplicationUser>();

        foreach (var entityEntry in entities)
        {
            var user = entityEntry.Entity;

            if (user.IsStudent && user.StudentInfo == null)
            {
                throw new InvalidOperationException("A user marked as a student must have associated student information");
            }
            if (!user.IsStudent && user.StudentInfo != null)
            {
                throw new InvalidOperationException("A user not marked as a student cannot have associated student information");
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
