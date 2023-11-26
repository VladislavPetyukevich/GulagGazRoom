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

    public ValueTask AddAsync(IStorageEvent @event, CancellationToken cancellationToken)
    {
        var redisEvent = ToRedisEvent(@event);
        return new ValueTask(_collection.InsertAsync(redisEvent));
    }

    public async IAsyncEnumerable<IReadOnlyCollection<IStorageEvent>> GetBySpecAsync(ISpecification<IStorageEvent> spec, int chunkSize, CancellationToken cancellationToken)
    {
        var newSpec = BuildNewSpec(spec);
        var collection = _redis.RedisCollection<RedisEvent>(chunkSize);
        var result = await collection.Where(newSpec).ToListAsync();
        yield return new List<IStorageEvent>(result);
        var offset = 0;
        while (result.Count > 0)
        {
            offset += collection.ChunkSize;
            result = await collection.Where(newSpec).Skip(offset).ToListAsync();
            if (result.Count == 0)
            {
                break;
            }

            yield return new List<IStorageEvent>(result);
        }
    }

    public ValueTask DeleteAsync(IEnumerable<IStorageEvent> items, CancellationToken cancellationToken)
    {
        var redisEvents = items.Select(ToRedisEvent);
        return new ValueTask(_collection.DeleteAsync(redisEvents));
    }

    private static RedisEvent ToRedisEvent(IStorageEvent @event)
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
        return redisEvent;
    }

    private static Expression<Func<RedisEvent, bool>> BuildNewSpec(ISpecification<IStorageEvent> spec)
    {
        var param = Expression.Parameter(typeof(RedisEvent));
        var body = new Visitor<RedisEvent>(param).Visit(spec.Expression.Body);
        return Expression.Lambda<Func<RedisEvent, bool>>(body, param);
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

    private class Visitor<T> : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        public Visitor(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameter;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            // only properties are allowed if you use fields then you need to extend
            // this method to handle them
            if (node.Member.MemberType != System.Reflection.MemberTypes.Property)
            {
                return node;
            }

            // name of a member referenced in original expression in your
            // sample Id in mine Prop
            var memberName = node.Member.Name;

            // find property on type T (=PersonData) by name
            var otherMember = typeof(T).GetProperty(memberName);

            // visit left side of this expression p.Id this would be p
            var inner = Visit(node.Expression);
            return Expression.Property(inner, otherMember);
        }
    }
}
