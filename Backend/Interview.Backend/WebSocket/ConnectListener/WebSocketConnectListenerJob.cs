using System.Collections.Immutable;
using Interview.Backend.Auth;
using Interview.Domain.Connections;
using Interview.Domain.Events;
using Interview.Infrastructure.Chat;
using Microsoft.Extensions.Options;

namespace Interview.Backend.WebSocket.ConnectListener;

public class WebSocketConnectListenJob : BackgroundService
{
    private readonly ChatBotAccount _chatBotAccount;
    private readonly IConnectUserSource _user;
    private readonly IRoomEventDispatcher _roomEventDispatcher;
    private readonly ILogger<WebSocketConnectListenJob> _logger;
    private readonly Dictionary<Guid, TwitchChatClient> _twitchClients;

    public WebSocketConnectListenJob(IConnectUserSource user, IOptions<ChatBotAccount> chatBotAccount, IRoomEventDispatcher roomEventDispatcher, ILogger<WebSocketConnectListenJob> logger)
    {
        _user = user;
        _roomEventDispatcher = roomEventDispatcher;
        _logger = logger;
        _chatBotAccount = chatBotAccount.Value;
        _twitchClients = new Dictionary<Guid, TwitchChatClient>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            RemoveUnusedClients(_user.Source);
            var source = _user.Source;
            foreach (var (roomId, (_, twitchChanel)) in _user.Source)
            {
                try
                {
                    if (!ShouldKeepConnection(source, roomId) || _twitchClients.ContainsKey(roomId))
                    {
                        continue;
                    }

                    _logger.LogInformation("Start listen new room {RoomId} {TwitchChannel}", roomId, twitchChanel);
                    var client = new TwitchChatClient(roomId, _roomEventDispatcher);
                    client.Connect(_chatBotAccount.Username, _chatBotAccount.AccessToken, twitchChanel);
                    _twitchClients.Add(roomId, client);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Start listen new room {RoomId} {TwitchChannel}", roomId, twitchChanel);
                }
            }

            _logger.LogDebug("Before wait async");
            await _user.WaitAsync(stoppingToken);
            _logger.LogDebug("After wait async");
        }
    }

    private bool ShouldKeepConnection(IReadOnlyDictionary<Guid, (ImmutableHashSet<Guid> Users, string TwitchChannel)> source, Guid roomId)
        => source.TryGetValue(roomId, out var detail) && detail.Users.Count > 0;

    private void RemoveUnusedClients(IReadOnlyDictionary<Guid, (ImmutableHashSet<Guid> Users, string TwitchChannel)> source)
    {
        foreach (var (roomId, client) in _twitchClients.ToList())
        {
            try
            {
                if (ShouldKeepConnection(source, roomId))
                {
                    continue;
                }

                try
                {
                    _logger.LogInformation("Stop listen room {RoomId}", roomId);
                    client.Dispose();
                }
                catch (Exception)
                {
                    // ignore
                }

                _twitchClients.Remove(roomId);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "Stop listen room {RoomId}", roomId);
            }
        }
    }
}
