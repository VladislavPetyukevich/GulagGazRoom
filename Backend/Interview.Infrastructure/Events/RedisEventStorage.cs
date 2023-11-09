using Interview.Domain.Events.Storage;
using Microsoft.EntityFrameworkCore;
using NSpecifications;
using Redis.OM;
using Redis.OM.Modeling;
using Redis.OM.Searching;

namespace Interview.Infrastructure.Events;

public class RedisEventStorage : IEventStorage
{
    private readonly RedisConnectionProvider _redis;
    private readonly IRedisCollection<RedisEvent> _collection;

    public RedisEventStorage(RedisEventStorageConfiguration configuration)
    {
        _redis = new RedisConnectionProvider(configuration.ConnectionString);
        _collection = _redis.RedisCollection<RedisEvent>();
    }

    public void CreateIndexes()
    {
        _redis.Connection.CreateIndex(typeof(RedisEvent));
    }

    public Task AddAsync(StorageEvent @event, CancellationToken cancellationToken)
    {
        var redisEvent = new RedisEvent
        {
            Id = @event.Id,
            RoomId = @event.RoomId,
            Type = @event.Type,
            Payload = @event.Payload,
            Stateful = @event.Stateful,
            CreatedAt = @event.CreatedAt,
        };
        return _collection.InsertAsync(redisEvent);
    }

    public Task<List<StorageEvent>> GetBySpecAsync(Spec<StorageEvent> spec, CancellationToken cancellationToken)
    {
        return _collection
            .Select(e => new StorageEvent
            {
                Id = e.Id,
                RoomId = e.RoomId,
                Type = e.Type,
                Payload = e.Payload,
                Stateful = e.Stateful,
                CreatedAt = e.CreatedAt,
            })
            .Where(spec.Expression)
            .ToListAsync(cancellationToken);
    }

    [Document(StorageType = StorageType.Json)]
    private class RedisEvent
    {
        [Indexed]
        public required Guid Id { get; set; }

        [Indexed]
        public required Guid RoomId { get; set; }

        [Indexed]
        public required string Type { get; set; }

        [Indexed]
        public required string? Payload { get; set; }

        [Indexed]
        public required bool Stateful { get; set; }

        [Indexed(Sortable = true)]
        public required DateTime CreatedAt { get; set; }
    }
}
