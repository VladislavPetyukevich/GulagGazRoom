using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using Interview.Backend.WebSocket.Events.Handlers;
using Interview.Domain.RoomParticipants;

namespace Interview.Backend.WebSocket.Events.ConnectionListener;

public class VideoChatConnectionListener : IConnectionListener, IVideChatConnectionProvider
{
    private readonly ConcurrentDictionary<Guid, ImmutableList<Payload>> _store = new();
    private readonly ConcurrentDictionary<(Guid UserId, Guid RoomId), ImmutableList<System.Net.WebSockets.WebSocket>> _storeByUserAndRoom = new();
    private readonly ILogger<VideoChatConnectionListener> _logger;

    public VideoChatConnectionListener(ILogger<VideoChatConnectionListener> logger)
    {
        _logger = logger;
    }

    public Task OnConnectAsync(WebSocketConnectDetail detail, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task OnDisconnectAsync(WebSocketConnectDetail detail, CancellationToken cancellationToken)
    {
        var disconnectUser = false;
        _store.AddOrUpdate(
            detail.Room.Id,
            _ => ImmutableList<Payload>.Empty,
            (_, list) =>
            {
                var newList = list.Remove(new Payload(detail.User, detail.WebSocket), EqualityComparer.Instance);
                disconnectUser = newList.Count != list.Count;
                return newList;
            });
        _storeByUserAndRoom.AddOrUpdate(
            (detail.User.Id, detail.Room.Id),
            _ => ImmutableList<System.Net.WebSockets.WebSocket>.Empty,
            (_, list) =>
            {
                var newList = list.Remove(detail.WebSocket);
                disconnectUser = newList.Count != list.Count;
                return newList;
            });

        await Task.Yield();

        if (disconnectUser && TryGetConnections(detail.Room.Id, out var connections))
        {
            var payload = new { Id = detail.User.Id, };
            var sendEvent = new WebSocketEvent
            {
                Type = "user left",
                Payload = System.Text.Json.JsonSerializer.Serialize(payload),
            };
            var sendEventAsStr = System.Text.Json.JsonSerializer.Serialize(sendEvent);
            var sendEventAsBytes = Encoding.UTF8.GetBytes(sendEventAsStr);

            foreach (var (_, webSocket) in connections)
            {
                try
                {
                    await webSocket.SendAsync(sendEventAsBytes, WebSocketMessageType.Text, true, cancellationToken);
                }
                catch
                {
                    // ignore
                }
            }
        }
    }

    public async Task<bool> TryConnectAsync(SocketEventDetail detail, CancellationToken cancellationToken)
    {
        var participantRepository = detail.ScopedServiceProvider.GetRequiredService<IRoomParticipantRepository>();
        var roomParticipant = await participantRepository.FindByRoomIdAndUserId(detail.RoomId, detail.UserId, cancellationToken);
        if (roomParticipant is null)
        {
            _logger.LogWarning("Not found room participant {RoomId} {UserId}", detail.RoomId, detail.UserId);
            return false;
        }

        if (roomParticipant.Type != RoomParticipantType.Examinee && roomParticipant.Type != RoomParticipantType.Expert)
        {
            _logger.LogWarning("Not enough permissions to connect to video chat {RoomId} {UserId}", detail.RoomId, detail.UserId);
            return false;
        }

        _store.AddOrUpdate(
            detail.RoomId,
            _ => ImmutableList.Create(new Payload(detail.User, detail.WebSocket)),
            (_, list) => list.Add(new Payload(detail.User, detail.WebSocket)));
        _storeByUserAndRoom.AddOrUpdate(
            (detail.User.Id, detail.Room.Id),
            _ => ImmutableList.Create(detail.WebSocket),
            (_, list) => list.Add(detail.WebSocket));
        return true;
    }

    public bool TryGetConnections(Guid roomId, [NotNullWhen(true)] out IReadOnlyCollection<(User User, System.Net.WebSockets.WebSocket WebSocket)>? connections)
    {
        if (!_store.TryGetValue(roomId, out var subscriber) || subscriber.Count == 0)
        {
            connections = null;
            return false;
        }

        connections = subscriber.Select(e => (e.User, e.Connection)).ToList();
        return true;
    }

    public bool TryGetConnections(Guid userId, Guid roomId, [NotNullWhen(true)] out IReadOnlyCollection<System.Net.WebSockets.WebSocket>? connections)
    {
        if (!_storeByUserAndRoom.TryGetValue((userId, roomId), out var subscriber) || subscriber.Count == 0)
        {
            connections = null;
            return false;
        }

        connections = subscriber;
        return true;
    }

    private record Payload(User User, System.Net.WebSockets.WebSocket Connection);

    private sealed class EqualityComparer : IEqualityComparer<Payload>
    {
        public static readonly IEqualityComparer<Payload> Instance = new EqualityComparer();

        public bool Equals(Payload? x, Payload? y)
        {
            return x is not null &&
                   y is not null &&
                   ReferenceEquals(x.Connection, y.Connection);
        }

        public int GetHashCode(Payload obj) => obj.Connection.GetHashCode();
    }
}

public interface IVideChatConnectionProvider
{
    Task<bool> TryConnectAsync(SocketEventDetail detail, CancellationToken cancellationToken);

    bool TryGetConnections(
        Guid roomId,
        [NotNullWhen(true)] out IReadOnlyCollection<(User User, System.Net.WebSockets.WebSocket WebSocket)>? connections);

    bool TryGetConnections(
        Guid userId,
        Guid roomId,
        [NotNullWhen(true)] out IReadOnlyCollection<System.Net.WebSockets.WebSocket>? connections);
}
