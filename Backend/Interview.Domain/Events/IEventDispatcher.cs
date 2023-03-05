using Interview.Backend.Controllers.WebSocket;

namespace Interview.Domain.Events
{
    public interface IEventDispatcher
    {
        Task<IEnumerable<IWebSocketEvent>> ReadFromRoomsAsync(TimeSpan timeSpan);

        Task<IWebSocketEvent?> ReadAsync(Guid roomId, TimeSpan timeout);

        Task WriteAsync(IWebSocketEvent @event, CancellationToken cancellationToken = default);
    }
}
