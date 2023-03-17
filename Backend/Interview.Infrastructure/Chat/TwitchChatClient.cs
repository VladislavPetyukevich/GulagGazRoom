using Interview.Domain.Events;
using Interview.Domain.Events.Events;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Interview.Infrastructure.Chat
{
    public class TwitchChatClient : IDisposable
    {
        private readonly Guid _roomId;
        private readonly IRoomEventDispatcher _eventDispatcher;
        private readonly TwitchClient _client;

        public TwitchChatClient(Guid roomId, IRoomEventDispatcher eventDispatcher)
        {
            _roomId = roomId;
            _eventDispatcher = eventDispatcher;
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
            _eventDispatcher.WriteAsync(new WebSocketEvent(_roomId, EventType.ChatMessage, e.ChatMessage.Message));
        }
    }
}
