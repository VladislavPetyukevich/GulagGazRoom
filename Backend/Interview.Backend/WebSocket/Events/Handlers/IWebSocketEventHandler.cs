namespace Interview.Backend.WebSocket.Events.Handlers;

public interface IWebSocketEventHandler
{
    Task HandleAsync(System.Net.WebSockets.WebSocket webSocket, WebSocketEvent @event, CancellationToken cancellationToken);
}
