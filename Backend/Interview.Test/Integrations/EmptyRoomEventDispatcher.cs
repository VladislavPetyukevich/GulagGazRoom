using Interview.Domain.Events;
using Interview.Domain.Events.Events;

namespace Interview.Test.Integrations
{
    public class EmptyRoomEventDispatcher : IRoomEventDispatcher
    {
        public IAsyncEnumerable<IRoomEvent> ReadAsync(TimeSpan timeSpan)
            => AsyncEnumerable.Empty<IRoomEvent>();

        public Task WriteAsync(IRoomEvent @event, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task DropEventsAsync(Guid roomId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task WaitAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
