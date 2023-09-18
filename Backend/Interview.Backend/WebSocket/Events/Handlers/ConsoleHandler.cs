using System.Text.Json;

namespace Interview.Backend.WebSocket.Events.Handlers;

public class ConsoleHandler : IWebSocketEventHandler
{
    public Task HandleAsync(SocketEventDetail detail, CancellationToken cancellationToken)
    {
        Console.WriteLine("On receive event: {0}", JsonSerializer.Serialize(detail.Event, new JsonSerializerOptions { WriteIndented = true, }));
        return Task.CompletedTask;
    }
}
