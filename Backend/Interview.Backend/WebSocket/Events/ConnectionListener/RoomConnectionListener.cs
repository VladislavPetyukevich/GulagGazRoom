using System.Collections.Concurrent;
using System.Collections.Immutable;
using Interview.Backend.Auth;
using Interview.Domain.Connections;
using Interview.Domain.Events;
using Interview.Infrastructure.Chat;
using Microsoft.Extensions.Options;

namespace Interview.Backend.WebSocket.Events.ConnectionListener;

public class RoomConnectionListener : IActiveRoomSource, IConnectionListener
{
    private ConcurrentDictionary<Guid, ImmutableList<WebSocketConnectDetail>> _activeRooms = new();

    private readonly ChatBotAccount _chatBotAccount;
    private readonly IRoomEventDispatcher _roomEventDispatcher;
    private readonly ILogger<RoomConnectionListener> _logger;
    private readonly ConcurrentDictionary<Guid, TwitchChatClient> _twitchClients = new();

    public RoomConnectionListener(IOptions<ChatBotAccount> chatBotAccount, IRoomEventDispatcher roomEventDispatcher, ILogger<RoomConnectionListener> logger)
    {
        _chatBotAccount = chatBotAccount.Value;
        _roomEventDispatcher = roomEventDispatcher;
        _logger = logger;
    }

    public ICollection<Guid> ActiveRooms => _activeRooms.Where(e => e.Value.Count > 0).Select(e => e.Key).ToList();

    public async Task OnConnectAsync(WebSocketConnectDetail detail, CancellationToken cancellationToken)
    {
        await Task.Yield();
        _activeRooms.AddOrUpdate(
            detail.Room.Id,
            roomId =>
            {
                try
                {
                    var client = new TwitchChatClient(roomId, _roomEventDispatcher);
                    client.Connect(_chatBotAccount.Username, _chatBotAccount.AccessToken, detail.Room.TwitchChannel);
                    _twitchClients.TryAdd(roomId, client);
                    _logger.LogInformation("Start listen new room {RoomId} {TwitchChannel}", roomId, detail.Room.TwitchChannel);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unable connect to twitch {RoomId} {TwitchChannel}", roomId, detail.Room.TwitchChannel);
                }

                return ImmutableList.Create(detail);
            },
            (_, list) => list.Add(detail));
    }

    public async Task OnDisconnectAsync(WebSocketConnectDetail detail, CancellationToken cancellationToken)
    {
        await Task.Yield();
        _activeRooms.AddOrUpdate(
            detail.Room.Id,
            s => ImmutableList<WebSocketConnectDetail>.Empty,
            (roomId, list) =>
            {
                var newList = list.Remove(detail);
                if (newList.Count == 0 && _twitchClients.TryRemove(roomId, out var client))
                {
                    try
                    {
                        client.Dispose();
                    }
                    catch (Exception e)
                    {
                        // ignore
                    }
                }

                return newList;
            });
    }
}
