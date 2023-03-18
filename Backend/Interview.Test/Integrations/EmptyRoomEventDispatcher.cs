using Interview.Domain.Events;
using Interview.Domain.Events.Events;

namespace Interview.Test.Integrations
{
    public class EmptyRoomEventDispatcher : IRoomEventDispatcher
    {
        public Task<IEnumerable<IRoomEvent>> ReadAsync(TimeSpan timeSpan)
        {
            return Task.FromResult(Enumerable.Empty<IRoomEvent>());
        }

        public Task WriteAsync(IRoomEvent @event, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task DropEventsAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
