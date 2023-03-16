using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.Repository;
using Interview.Domain.RoomParticipants;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Interview.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Internal;

namespace Interview.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public ISystemClock SystemClock { get; set; } = new SystemClock();

    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; private set; } = null!;

    public DbSet<Question> Questions { get; private set; } = null!;

    public DbSet<Room> Rooms { get; private set; } = null!;

    public DbSet<Role> Roles { get; private set; } = null!;

    public DbSet<Reaction> Reactions { get; private set; } = null!;

    public DbSet<RoomParticipant> RoomParticipants { get; private set; } = null!;

    public DbSet<RoomQuestion> RoomQuestions { get; private set; } = null!;

    public DbSet<RoomQuestionReaction> RoomQuestionReactions { get; private set; } = null!;

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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly, type => type != typeof(RoleTypeConfiguration));
        modelBuilder.ApplyConfiguration(new RoleTypeConfiguration(SystemClock));
    }

    private void BeforeSaveChanges()
    {
        ModifyFieldByState(entity => entity.UpdateCreateDate(SystemClock.UtcNow.DateTime), EntityState.Added);
        ModifyFieldByState(entity => entity.UpdateUpdateDate(SystemClock.UtcNow.DateTime), EntityState.Modified);
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
