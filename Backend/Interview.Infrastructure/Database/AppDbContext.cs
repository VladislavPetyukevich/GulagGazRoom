using Interview.Domain;
using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.Rooms;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }

    private AppDbContext()
    {
    }

    public DbSet<User> Users { get; private set; } = null!;

    public DbSet<Question> Questions { get; private set; } = null!;

    public DbSet<Room> Rooms { get; private set; } = null!;

    public DbSet<Role> Roles { get; private set; } = null!;

    public DbSet<Reaction> Reactions { get; private set; } = null!;

    public override int SaveChanges()
    {
        BeforeSaveChanges();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        BeforeSaveChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        BeforeSaveChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private static void UpdateCreateDate(Entity entity)
    {
        entity.CreateDate = DateTime.UtcNow;
        entity.UpdateDate = DateTime.UtcNow;
    }

    private static void ModifiedUpdateDate(Entity entity) => entity.UpdateDate = DateTime.UtcNow;

    private void BeforeSaveChanges()
    {
        ModifyFieldByState(UpdateCreateDate, EntityState.Added);
        ModifyFieldByState(ModifiedUpdateDate, EntityState.Modified);
    }

    private void ModifyFieldByState(Action<Entity> action, EntityState entityState)
    {
        foreach (var entityEntry in ChangeTracker.Entries())
        {
            if (entityEntry.State != entityState)
            {
                continue;
            }

            if (entityEntry.Entity is Entity entity)
            {
                action(entity);
            }
        }
    }
}
