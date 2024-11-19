using System.Reflection;

namespace SmartAttendanceSystem.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    #region DbSet


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
}
