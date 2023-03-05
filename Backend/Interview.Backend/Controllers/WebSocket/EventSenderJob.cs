using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Interview.Domain.Events;

namespace Interview.Backend.Controllers.WebSocket
{
    public class EventSenderJob : BackgroundService
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly SubscribeUserProvider _subscribeUserProvider;

        public EventSenderJob(IEventDispatcher eventDispatcher, SubscribeUserProvider subscribeUserProvider)
        {
            _eventDispatcher = eventDispatcher;
            _subscribeUserProvider = subscribeUserProvider;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var currentEvent in await _eventDispatcher.ReadFromRoomsAsync(TimeSpan.FromSeconds(30)))
                {
                    var json = JsonSerializer.Serialize(currentEvent, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Converters =
                        {
                            new JsonStringEnumConverter()
                        }
                    });
                    var bytes = Encoding.UTF8.GetBytes(json);

                    if (!_subscribeUserProvider.TryGetUsers(currentEvent.RoomId, out var users))
                    {
                        continue;
                    }

                    try
                    {
                        foreach (var pair in users)
                        {
                            var socket = pair.Key;
                            try
                            {
                                if (socket.CloseStatus.HasValue)
                                {
                                    await socket.CloseAsync(socket.CloseStatus.Value, socket.CloseStatusDescription, stoppingToken);
                                    users.TryRemove(pair);
                                    pair.Value.SetCanceled(stoppingToken);
                                    continue;
                                }
                                            
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
}
