using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Interview.Infrastructure.Chat
{
    public class TwitchChatClient
    {
        private readonly TwitchClient _client;

        public TwitchChatClient(string username, string accessToken, string channelChat)
        {
            var credentials = new ConnectionCredentials(username, accessToken);

            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30),
            };

            var customClient = new WebSocketClient(clientOptions);

            _client = new TwitchClient(customClient);

            _client.Initialize(credentials, channelChat);

            _client.OnLog += ClientOnLog;
            _client.OnJoinedChannel += ClientOnJoinedChannel;
            _client.OnMessageReceived += ClientOnMessageReceived;
            _client.OnConnected += ClientOnConnected;

            _client.Connect();
        }

        private void ClientOnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void ClientOnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void ClientOnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            _client.SendMessage(e.Channel, "Hey guys!");
        }

        private void ClientOnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.Contains("он"))
            {
                _client.SendMessage(e.ChatMessage.Channel, $"@{e.ChatMessage.Username}, дай бог здаровъя тебе");
            }
        }
    }
}
