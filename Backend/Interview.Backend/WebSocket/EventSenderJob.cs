using System.Net.WebSockets;
using System.Text;
using Interview.Domain.Events;

namespace Interview.Backend.WebSocket;

public class EventSenderJob : BackgroundService
{
    private readonly IRoomEventDispatcher _roomEventDispatcher;

    private readonly UserRoomObservable _userRoomObservable;

    public EventSenderJob(IRoomEventDispatcher roomEventDispatcher, UserRoomObservable userRoomObservable)
    {
        _roomEventDispatcher = roomEventDispatcher;
        _userRoomObservable = userRoomObservable;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var currentEvent in await _roomEventDispatcher.ReadAsync(TimeSpan.FromSeconds(30)))
            {
                if (!_userRoomObservable.TryGetUsers(currentEvent.RoomId, out var users))
                {
                    continue;
                }

                try
                {
                    foreach (var entry in users)
                    {
                        var socket = entry.Key;

                        try
                        {
                            if (socket.CloseStatus.HasValue)
                            {
                                await socket.CloseAsync(socket.CloseStatus.Value, socket.CloseStatusDescription, stoppingToken);
                                users.TryRemove(entry);
                                entry.Value.SetCanceled(stoppingToken);
                                continue;
                            }

                            var bytes = Encoding.UTF8.GetBytes(currentEvent.Stringify());

                            await socket.SendAsync(
                                new ArraySegment<byte>(bytes, 0, bytes.Length),
                                WebSocketMessageType.Text,
                                true,
                                stoppingToken);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
