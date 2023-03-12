using Bogus;
using Interview.Domain.Events;

namespace Interview.Backend.WebSocket;

public class JobWriter : BackgroundService
{
    private readonly IRoomEventDispatcher _dispatcher;
    private readonly IServiceProvider _serviceProvider;

    public JobWriter(IRoomEventDispatcher dispatcher, IServiceProvider serviceProvider)
    {
        _dispatcher = dispatcher;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var id = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            var roomIds = await GetRoomIdsAsync();
            if (roomIds.Count == 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                continue;
            }

            var faker = new Faker<CustomEvent>()
                .RuleFor(e => e.Type, e => e.PickRandom<EventType>())
                .RuleFor(e => e.Value, e => (++id).ToString())
                .RuleFor(e => e.RoomId, e => e.PickRandom(roomIds));

            var currentEvent = faker.Generate(Random.Shared.Next(1, 3));
            foreach (var customEvent in currentEvent)
            {
                await _dispatcher.WriteAsync(customEvent, default);
            }

            await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1, 5)), default);
        }
    }

    private async Task<ICollection<Guid>> GetRoomIdsAsync()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var rep = scope.ServiceProvider.GetRequiredService<IRoomRepository>();
        var rooms = await rep.GetPageAsync(1, 10);
        return rooms.Select(e => e.Id).ToList();
    }

    public sealed class CustomEvent : IWebSocketEvent
    {
        public Guid RoomId { get; set; }

        public EventType Type { get; set; }

        public string Value { get; set; }
    }
}
