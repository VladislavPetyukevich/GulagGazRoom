using Interview.Domain;
using Interview.Domain.Questions;
using Interview.Domain.Rooms;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Database;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; } = null!;

    public DbSet<Question> Questions { get; } = null!;

    public DbSet<Room> Rooms { get; } = null!;

    public DbSet<Role> Roles { get; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        ModifyFieldByState(UpdateCreateDate, EntityState.Added);
        ModifyFieldByState(ModifiedUpdateDate, EntityState.Modified);

        return base.SaveChanges();
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
