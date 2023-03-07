using System.Net.WebSockets;
using System.Text;
using Interview.Backend.WebSocket.UserByRoom;
using Interview.Domain.Events;

namespace Interview.Backend.WebSocket;

public class EventSenderJob : BackgroundService
{
    private static TimeSpan ReadTimeout => TimeSpan.FromSeconds(30);

    private readonly IRoomEventDispatcher _roomEventDispatcher;
    private readonly UserByRoomEventSubscriber _userByRoomEventSubscriber;
    private readonly ILogger<EventSenderJob> _logger;

    public EventSenderJob(IRoomEventDispatcher roomEventDispatcher, UserByRoomEventSubscriber userByRoomEventSubscriber, ILogger<EventSenderJob> logger)
    {
        _roomEventDispatcher = roomEventDispatcher;
        _userByRoomEventSubscriber = userByRoomEventSubscriber;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogDebug("Start sending");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                IEnumerable<IWebSocketEvent>? events;
                try
                {
                    events = await _roomEventDispatcher.ReadAsync(ReadTimeout);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Read events");
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    continue;
                }

                foreach (var currentEvent in events)
                {
                    if (!_userByRoomEventSubscriber.TryGetSubscribers(currentEvent.RoomId, out var subscribers))
                    {
                        continue;
                    }

                    _logger.LogDebug("Start sending {Event}", currentEvent.Stringify());
                    await HandleSubscribersAsync(stoppingToken, subscribers, currentEvent);
                }
            }
        }
        finally
        {
            _logger.LogDebug("Stop sending");
        }
    }

    private async Task HandleSubscribersAsync(CancellationToken stoppingToken, UserByRoomSubscriberCollection users, IWebSocketEvent currentEvent)
    {
        foreach (var entry in users)
        {
            try
            {
                if (entry.WebSocket.CloseStatus.HasValue)
                {
                    await entry.WebSocket.CloseAsync(entry.WebSocket.CloseStatus.Value, entry.WebSocket.CloseStatusDescription, stoppingToken);
                    users.Remove(entry, stoppingToken);
                    continue;
                }

                var bytes = Encoding.UTF8.GetBytes(currentEvent.Stringify());
                await entry.WebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Send {Event}", currentEvent.Stringify());
            }
        }
    }
}
