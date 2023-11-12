using System.Linq.Expressions;
using Interview.Domain.Events.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

    public Task AddAsync(IStorageEvent @event, CancellationToken cancellationToken)
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

    public async Task<IReadOnlyCollection<IStorageEvent>> GetBySpecAsync(ISpecification<IStorageEvent> spec, CancellationToken cancellationToken)
    {
        var newSpec = (Expression<Func<RedisEvent, bool>>)Expression.Lambda(
            spec.Expression.Body,
            Expression.Parameter(typeof(RedisEvent), spec.Expression.Parameters[0].Name));
        var result = await _collection.Where(newSpec).ToListAsync(cancellationToken);
        return result;
    }

    public void Test()
    {
        var rAll = _collection.ToList();
        var r1 = _collection.Where(e => e.Type == "join video chat").ToList();
        var r2 = _collection.Where(e => e.Type == "all users").ToList();
        var r4 = _collection.Where(e => e.Payload!.Contains("11")).ToList();
        var r5 = _collection.Where(e => e.Payload!.Contains("Test")).ToList();
    }

    [Document(StorageType = StorageType.Json)]
    private class RedisEvent : IStorageEvent
    {
        [RedisIdField]
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
