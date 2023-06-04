using System.Net.WebSockets;
using System.Text;
using Interview.Backend.WebSocket.UserByRoom;
using Interview.Domain.Events;
using Interview.Domain.Events.Events;
using Interview.Domain.Events.Events.Serializers;

namespace Interview.Backend.WebSocket;

public class EventSenderJob : BackgroundService
{
    private static TimeSpan ReadTimeout => TimeSpan.FromSeconds(30);

    private readonly IRoomEventDispatcher _roomEventDispatcher;
    private readonly UserByRoomEventSubscriber _userByRoomEventSubscriber;
    private readonly ILogger<EventSenderJob> _logger;
    private readonly IRoomEventSerializer _roomEventSerializer;

    public EventSenderJob(IRoomEventDispatcher roomEventDispatcher, UserByRoomEventSubscriber userByRoomEventSubscriber, ILogger<EventSenderJob> logger, IRoomEventSerializer roomEventSerializer)
    {
        _roomEventDispatcher = roomEventDispatcher;
        _userByRoomEventSubscriber = userByRoomEventSubscriber;
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
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    continue;
                }

                foreach (var currentEvent in events)
                {
                    if (!_userByRoomEventSubscriber.TryGetSubscribers(currentEvent.RoomId, out var subscribers))
                    {
                        continue;
                    }

                    var eventAsString = _roomEventSerializer.SerializeAsString(currentEvent);
                    _logger.LogDebug("Start sending {Event}", eventAsString);
                    await HandleSubscribersAsync(stoppingToken, subscribers, eventAsString);
                }

                _logger.LogDebug("Before wait async");
                await _roomEventDispatcher.WaitAsync(stoppingToken);
                _logger.LogDebug("After wait async");
            }
        }
        finally
        {
            _logger.LogDebug("Stop sending");
        }
    }

    private async Task HandleSubscribersAsync(CancellationToken stoppingToken, UserByRoomSubscriberCollection users, string eventAsStr)
    {
        await Parallel.ForEachAsync(users, stoppingToken, async (entry, token) =>
        {
            try
            {
                if (entry.WebSocket.ShouldCloseWebSocket())
                {
                    try
                    {
                        var status = entry.WebSocket.CloseStatus ?? WebSocketCloseStatus.NormalClosure;
                        await entry.WebSocket.CloseAsync(status, entry.WebSocket.CloseStatusDescription, token);
                    }
                    catch
                    {
                        // ignored
                    }

                    users.Remove(entry, token);
                    return;
                }

                var bytes = Encoding.UTF8.GetBytes(eventAsStr);
                await entry.WebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, token);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Send {Event}", eventAsStr);
            }
        });
    }
}
