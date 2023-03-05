using Bogus;
using Interview.Domain.Events;

namespace Interview.Backend.Controllers.WebSocket
{
    public class JobWriter : BackgroundService
    {
        private IRoomEventDispatcher _dispatcher;

        public JobWriter(IRoomEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var roomIds = new[]
            {
                Guid.Parse("2E87B752-84FD-475C-B7C4-719C72D9B01A"),
                Guid.Parse("2E87B752-84FD-475C-B7C4-239C72D9B02A"),
            };
            var id = 0;
            var faker = new Faker<CustomEvent>()
                .RuleFor(e => e.Type, e => e.PickRandom<EventType>())
                .RuleFor(e => e.Value, e => (++id).ToString())
                .RuleFor(e => e.RoomId, e => e.PickRandom(roomIds));

            while (true)
            {
                var currentEvent = faker.Generate(Random.Shared.Next(1, 3));
                foreach (var customEvent in currentEvent)
                {
                    await _dispatcher.WriteAsync(customEvent, default);
                }

                await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1, 5)), default);
            }
        }
    }

    public sealed class CustomEvent : IWebSocketEvent
    {
        public Guid RoomId { get; set; }

        public EventType Type { get; set; }

        public string Value { get; set; }
    }
}
