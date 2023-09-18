using System.Net.WebSockets;
using System.Text;
using Interview.Backend.WebSocket.Events.ConnectionListener;
using Interview.Domain.Events;
using Interview.Domain.Events.Events;
using Interview.Domain.Events.Events.Serializers;

namespace Interview.Backend.WebSocket;

public class EventSenderJob : BackgroundService
{
    private static TimeSpan ReadTimeout => TimeSpan.FromSeconds(30);

    private readonly IRoomEventDispatcher _roomEventDispatcher;
    private readonly IWebSocketConnectionSource _webSocketConnectionSource;
    private readonly ILogger<EventSenderJob> _logger;
    private readonly IRoomEventSerializer _roomEventSerializer;

    public EventSenderJob(
        IRoomEventDispatcher roomEventDispatcher,
        IWebSocketConnectionSource webSocketConnectionSource,
        ILogger<EventSenderJob> logger,
        IRoomEventSerializer roomEventSerializer)
    {
        _roomEventDispatcher = roomEventDispatcher;
        _webSocketConnectionSource = webSocketConnectionSource;
        _logger = logger;
        _roomEventSerializer = roomEventSerializer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogDebug("Start sending");
            while (!stoppingToken.IsCancellationRequested)
            {
                IEnumerable<IRoomEvent>? events;
                try
                {
                    events = await _roomEventDispatcher.ReadAsync(ReadTimeout);
                }
                catch (Exception e)
                {
                    if (e is not OperationCanceledException)
                    {
                        _logger.LogError(e, "Read events");
                        await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                    }

                    continue;
                }

                foreach (var group in events.ToLookup(e => e.RoomId))
                {
                    if (!_webSocketConnectionSource.TryGetConnections(group.Key, out var subscribers) ||
                        subscribers.Count == 0)
                    {
                        continue;
                    }

                    foreach (var currentEvent in group)
                    {
                        var eventAsString = _roomEventSerializer.SerializeAsString(currentEvent);
                        var eventAsBytes = Encoding.UTF8.GetBytes(eventAsString);
                        _logger.LogDebug("Start sending {Event}", eventAsString);
                        await Parallel.ForEachAsync(subscribers, stoppingToken, async (entry, token) =>
                        {
                            try
                            {
                                if (!entry.ShouldCloseWebSocket())
                                {
                                    await entry.SendAsync(eventAsBytes, WebSocketMessageType.Text, true, token);
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Send {Event}", eventAsString);
                            }
                        });
                    }
                }

                _logger.LogDebug("Before wait async");
                await _roomEventDispatcher.WaitAsync(stoppingToken);
                _logger.LogDebug("After wait async");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "During sending events");
        }
        finally
        {
            _logger.LogDebug("Stop sending");
        }
    }
}
