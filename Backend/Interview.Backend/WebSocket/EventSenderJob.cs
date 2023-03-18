using System.Net.WebSockets;
using System.Text;
using Interview.Backend.WebSocket.UserByRoom;
using Interview.Domain.Events;
using Interview.Domain.Events.Events;

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

    private static bool ShouldCloseWebSocket(UserSubscriber entry)
    {
        return entry.WebSocket.State is WebSocketState.Aborted or WebSocketState.Closed or WebSocketState.CloseReceived or WebSocketState.CloseSent ||
               entry.WebSocket.CloseStatus.HasValue;
    }

    private async Task HandleSubscribersAsync(CancellationToken stoppingToken, UserByRoomSubscriberCollection users, IRoomEvent currentEvent)
    {
        foreach (var entry in users)
        {
            try
            {
                if (ShouldCloseWebSocket(entry))
                {
                    try
                    {
                        var status = entry.WebSocket.CloseStatus ?? WebSocketCloseStatus.NormalClosure;
                        await entry.WebSocket.CloseAsync(status, entry.WebSocket.CloseStatusDescription, stoppingToken);
                    }
                    catch
                    {
                        // ignored
                    }

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
