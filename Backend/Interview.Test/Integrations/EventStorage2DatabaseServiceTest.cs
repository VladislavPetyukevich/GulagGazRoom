using FluentAssertions;
using Interview.Domain.Events;
using Interview.Domain.Events.Storage;
using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service;
using Interview.Infrastructure.Events;
using Interview.Infrastructure.RoomQuestions;
using Interview.Infrastructure.Rooms;
using Microsoft.Extensions.Logging.Abstractions;
using NSpecifications;
using X.PagedList;

namespace Interview.Test.Integrations;

public class EventStorage2DatabaseServiceTest
{
    [Fact]
    public async Task Process()
    {
        var clock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(clock);

        var room1 = new Room("Test 1", string.Empty) { Status = SERoomStatus.Close, };
        appDbContext.Rooms.Add(room1);
        appDbContext.Rooms.Add(new Room("Test 2", string.Empty) { Status = SERoomStatus.Close, });
        appDbContext.Rooms.Add(new Room("Test 3", string.Empty) { Status = SERoomStatus.Close, });
        appDbContext.Rooms.Add(new Room("Test 4", string.Empty) { Status = SERoomStatus.Close, });
        appDbContext.SaveChanges();
        var queuedRoomEvents = appDbContext.Rooms.Where(e => e.Id != room1.Id).Select(e => new QueuedRoomEvent { RoomId = e.Id, });
        appDbContext.QueuedRoomEvents.AddRange(queuedRoomEvents);
        appDbContext.SaveChanges();

        var eventStorage = new InMemoryEventStorage();
        var storageEvents = new StorageEvent[]
        {
            new StorageEvent
            {
                Id = Guid.NewGuid(),
                Payload = "1",
                Stateful = false,
                Type = "Test 1",
                CreatedAt = new DateTime(2000, 1, 15),
                RoomId = room1.Id,
            },
            new StorageEvent
            {
                Id = Guid.NewGuid(),
                Payload = "2",
                Stateful = true,
                Type = "Test 2",
                CreatedAt = new DateTime(2015, 12, 1),
                RoomId = room1.Id,
            },
            new StorageEvent
            {
                Id = Guid.NewGuid(),
                Payload = "3",
                Stateful = false,
                Type = "Test 3",
                CreatedAt = new DateTime(2023, 05, 04),
                RoomId = room1.Id,
            },
        };
        foreach (var storageEvent in storageEvents)
        {
            await eventStorage.AddAsync(storageEvent, CancellationToken.None);
        }

        var initialEvents = await appDbContext.RoomEvents.ToListAsync();
        var service = new EventStorage2DatabaseService(
            new QueuedRoomEventRepository(appDbContext),
            new DbRoomEventRepository(appDbContext),
            eventStorage,
            NullLogger<EventStorage2DatabaseService>.Instance);

        await service.ProcessAsync(CancellationToken.None);

        var actualEvents = await appDbContext.RoomEvents.ToListAsync();

        initialEvents.Should().BeEmpty();
        actualEvents.Should().HaveSameCount(storageEvents)
            .And.ContainSingle(e => IsSame(e, storageEvents[0]))
            .And.ContainSingle(e => IsSame(e, storageEvents[1]))
            .And.ContainSingle(e => IsSame(e, storageEvents[2]));
    }

    private bool IsSame(DbRoomEvent roomEvent, StorageEvent storageEvent)
    {
        return roomEvent.RoomId == storageEvent.RoomId &&
               roomEvent.Stateful == storageEvent.Stateful &&
               roomEvent.Payload == storageEvent.Payload &&
               roomEvent.Type == storageEvent.Type &&
               roomEvent.Id == storageEvent.Id;
    }

    private sealed class InMemoryEventStorage : IEventStorage
    {
        private readonly List<IStorageEvent> _storage = new();

        public ValueTask AddAsync(IStorageEvent @event, CancellationToken cancellationToken)
        {
            _storage.Add(@event);
            return ValueTask.CompletedTask;
        }

        public async IAsyncEnumerable<IReadOnlyCollection<IStorageEvent>> GetBySpecAsync(ISpecification<IStorageEvent> spec, int chunkSize, CancellationToken cancellationToken)
        {
            await foreach (var res in _storage.Where(spec.IsSatisfiedBy).Chunk(chunkSize).ToAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return res;
            }
        }

        public ValueTask DeleteAsync(IEnumerable<IStorageEvent> items, CancellationToken cancellationToken)
        {
            foreach (var e in items)
            {
                _storage.Remove(e);
            }
            
            return ValueTask.CompletedTask;
        }
    }
}
