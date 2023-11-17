using System.Buffers;
using System.Net.WebSockets;
using System.Text.Json;
using Interview.Backend.WebSocket.Events.Handlers;
using Interview.Domain.Events.Storage;
using Microsoft.IO;

namespace Interview.Backend.WebSocket.Events;

public class WebSocketReader
{
    private readonly RecyclableMemoryStreamManager _manager;
    private readonly IWebSocketEventHandler[] _handlers;
    private readonly IEventStorage _eventStorage;

    public WebSocketReader(
        RecyclableMemoryStreamManager manager,
        IEnumerable<IWebSocketEventHandler> handlers,
        IEventStorage eventStorage)
    {
        _manager = manager;
        _eventStorage = eventStorage;
        _handlers = handlers.ToArray();
    }

    public async Task ReadAsync(
        User user,
        Room room,
        IServiceProvider scopedServiceProvider,
        System.Net.WebSockets.WebSocket webSocket,
        Action<Exception> onError,
        CancellationToken ct)
    {
        try
        {
            while (!webSocket.ShouldCloseWebSocket())
            {
                WebSocketEvent? deserializeResult = null;
                using (var buffer = new PoolItem(8192))
                {
                    using var ms = _manager.GetStream();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await webSocket.ReceiveAsync(buffer.Buffer, ct);
                        await ms.WriteAsync(buffer.Buffer.AsMemory(0, result.Count), ct);
                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    try
                    {
                        deserializeResult = await JsonSerializer.DeserializeAsync<WebSocketEvent>(ms, (JsonSerializerOptions?)null, ct);
                    }
                    catch (Exception e)
                    {
                        if (!webSocket.ShouldCloseWebSocket())
                        {
                            onError(e);
                        }
                    }
                }

                if (webSocket.ShouldCloseWebSocket())
                {
                    break;
                }

                if (deserializeResult is null)
                {
                    continue;
                }

                var storageEvent = new StorageEvent
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Payload = deserializeResult.Value,
                    RoomId = room.Id,
                    Stateful = false,
                    Type = deserializeResult.Type,
                };
                await _eventStorage.AddAsync(storageEvent, ct);
                var socketEventDetail = new SocketEventDetail(
                    scopedServiceProvider,
                    webSocket,
                    deserializeResult,
                    user,
                    room);
                var tasks = _handlers.Select(e => e.HandleAsync(socketEventDetail, ct));
                await Task.WhenAll(tasks);
            }
        }
        catch (Exception e)
        {
            if (!webSocket.ShouldCloseWebSocket())
            {
                onError(e);
            }
        }
    }

    private class PoolItem : IDisposable
    {
        public byte[] Buffer { get; }

        public PoolItem(int size)
        {
            Buffer = ArrayPool<byte>.Shared.Rent(size);
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Buffer);
        }
    }
}
