using System.Text.Json;

namespace Interview.Backend.WebSocket.Events.Handlers;

public class ConsoleHandler : IWebSocketEventHandler
{
    public Task HandleAsync(System.Net.WebSockets.WebSocket webSocket, WebSocketEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine("On receive event: {0}", JsonSerializer.Serialize(@event, new JsonSerializerOptions { WriteIndented = true, }));
        return Task.CompletedTask;
    }
}
