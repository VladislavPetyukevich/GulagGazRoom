using Interview.Domain.Events;
using Interview.Domain.Events.Events;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Interview.Infrastructure.Chat;

public class TwitchChatClient : IDisposable
{
    private readonly Guid _roomId;
    private readonly IRoomEventDispatcher _eventDispatcher;
    private readonly TwitchClient _client;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public TwitchChatClient(Guid roomId, IRoomEventDispatcher eventDispatcher)
    {
        _roomId = roomId;
        _eventDispatcher = eventDispatcher;
        _cancellationTokenSource = new CancellationTokenSource();
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30),
        };
        var customClient = new WebSocketClient(clientOptions);
        _client = new TwitchClient(customClient);
        _client.OnMessageReceived += ClientOnMessageReceived;
    }

    public void Connect(string username, string accessToken, string channelChat)
    {
        var credentials = new ConnectionCredentials(username, accessToken);
        _client.Initialize(credentials, channelChat);
        _client.Connect();
    }

    public void Dispose()
    {
        try
        {
            _cancellationTokenSource.Cancel();
        }
        catch
        {
            // ignore
        }

        _client.OnMessageReceived -= ClientOnMessageReceived;
        try
        {
            _client.Disconnect();
        }
        catch
        {
            // ignore
        }

        _eventDispatcher.DropEventsAsync(_roomId).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private void ClientOnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        var message = e.ChatMessage.Message ?? string.Empty;
        var payload = new EventPayload(message, e.ChatMessage.Username);
        var @event = new RoomEvent<EventPayload>(_roomId, EventType.ChatMessage, payload);
        _eventDispatcher.WriteAsync(@event, _cancellationTokenSource.Token);

        if (message.Contains("он"))
        {
            _client.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username}, дай бог здаровъя тебе");
        }
    }

    public sealed class EventPayload
    {
        public string Message { get; }

        public string Nickname { get; }

        public EventPayload(string message, string nickname)
        {
            Message = message;
            Nickname = nickname;
        }
    }
}
