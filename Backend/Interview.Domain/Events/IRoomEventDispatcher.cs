using Interview.Backend.Controllers.WebSocket;

namespace Interview.Domain.Events
{
    public interface IRoomEventDispatcher
    {
        Task<IEnumerable<IWebSocketEvent>> ReadAsync(TimeSpan timeSpan);

        Task WriteAsync(IWebSocketEvent @event, CancellationToken cancellationToken = default);
    }
}
