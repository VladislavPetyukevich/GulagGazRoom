using Interview.Domain;
using Interview.Domain.Questions;
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

    public override int SaveChanges()
    {
        ModifyFieldByState(UpdateCreateDate, EntityState.Added);
        ModifyFieldByState(ModifiedUpdateDate, EntityState.Modified);

        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private static void UpdateCreateDate(Entity entity) => entity.CreateDate = DateTime.UtcNow;

    private static void ModifiedUpdateDate(Entity entity) => entity.UpdateDate = DateTime.UtcNow;

    private void ModifyFieldByState(Action<Entity> action, EntityState entityState)
    {
        foreach (var entity in ChangeTracker.Entries().Where(e => e.State == entityState).OfType<Entity>())
        {
            action(entity);
        }
    }
}
