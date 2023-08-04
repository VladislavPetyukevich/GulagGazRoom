using Interview.Domain.Events.ChangeEntityProcessors;
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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Internal;
using RoomConfiguration = Interview.Domain.RoomConfigurations.RoomConfiguration;

namespace Interview.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public ISystemClock SystemClock { get; set; } = new SystemClock();

    public IChangeEntityProcessor[] ChangeEntityProcessors { get; set; }

    public AppDbContext(DbContextOptions options, IEnumerable<IChangeEntityProcessor>? processors)
        : base(options)
    {
        ChangeEntityProcessors = processors?.ToArray() ?? Array.Empty<IChangeEntityProcessor>();
    }

    public DbSet<User> Users { get; private set; } = null!;

    public DbSet<Question> Questions { get; private set; } = null!;

    public DbSet<Room> Rooms { get; private set; } = null!;

    public DbSet<Role> Roles { get; private set; } = null!;

    public DbSet<Reaction> Reactions { get; private set; } = null!;

    public DbSet<RoomParticipant> RoomParticipants { get; private set; } = null!;

    public DbSet<RoomQuestion> RoomQuestions { get; private set; } = null!;

    public DbSet<RoomQuestionReaction> RoomQuestionReactions { get; private set; } = null!;

    public DbSet<RoomConfiguration> RoomConfiguration { get; private set; } = null!;

    public override int SaveChanges()
    {
        using (new SaveCookie(this, CancellationToken.None))
        {
            return base.SaveChanges();
        }
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        await using (new SaveCookie(this, cancellationToken))
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private readonly struct SaveCookie : IDisposable, IAsyncDisposable
    {
        private readonly AppDbContext _db;
        private readonly CancellationToken _cancellationToken;
        private readonly List<Entity>? _addedEntities;
        private readonly List<(Entity Original, Entity Current)>? _modifiedEntities;

        public SaveCookie(AppDbContext db, CancellationToken cancellationToken)
        {
            _db = db;
            _cancellationToken = cancellationToken;
            foreach (var entity in FilterByState(EntityState.Added))
            {
                entity.UpdateCreateDate(db.SystemClock.UtcNow.DateTime);
            }

            foreach (var entity in FilterByState(EntityState.Modified))
            {
                entity.UpdateUpdateDate(db.SystemClock.UtcNow.DateTime);
            }

            if (db.ChangeEntityProcessors.Length > 0)
            {
                _addedEntities = FilterByState(EntityState.Added).ToList();

                _modifiedEntities = FilterEntryByState(EntityState.Modified)
                    .Select(e =>
                    {
                        var original = (Entity)e.OriginalValues.ToObject();
                        return (original, e.Entity);
                    })
                    .ToList();
            }
        }

        public void Dispose()
        {
            if (_db.ChangeEntityProcessors.Length == 0 || _addedEntities == null || _modifiedEntities == null)
            {
                return;
            }

            foreach (var processor in _db.ChangeEntityProcessors)
            {
                processor.ProcessAddedAsync(_addedEntities, _cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
                processor.ProcessModifiedAsync(_modifiedEntities, _cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_db.ChangeEntityProcessors.Length == 0 || _addedEntities == null || _modifiedEntities == null)
            {
                return;
            }

            foreach (var processor in _db.ChangeEntityProcessors)
            {
                await processor.ProcessAddedAsync(_addedEntities, _cancellationToken);
                await processor.ProcessModifiedAsync(_modifiedEntities, _cancellationToken);
            }
        }

        private IEnumerable<Entity> FilterByState(EntityState entityState)
        {
            return FilterEntryByState(entityState).Select(e => e.Entity);
        }

        private IEnumerable<EntityEntry<Entity>> FilterEntryByState(EntityState entityState)
        {
            foreach (var entityEntry in _db.ChangeTracker.Entries<Entity>())
            {
                if (entityEntry.State != entityState)
                {
                    continue;
                }

                yield return entityEntry;
            }
        }
    }
}
