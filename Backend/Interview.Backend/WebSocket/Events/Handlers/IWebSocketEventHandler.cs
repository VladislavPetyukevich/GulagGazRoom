namespace Interview.Backend.WebSocket.Events.Handlers;

public interface IWebSocketEventHandler
{
    Task HandleAsync(SocketEventDetail detail, CancellationToken cancellationToken);
}

public record SocketEventDetail(IServiceProvider ScopedServiceProvider, System.Net.WebSockets.WebSocket WebSocket, WebSocketEvent Event, Guid UserId, Guid RoomId);
