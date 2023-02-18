using Interview.Domain.Chat;
using Interview.Domain.Users;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;
using ChatMessage = Interview.Domain.Chat.ChatMessage;

namespace Interview.Infrastructure.Chat;

// TODO
public class TwitchChatMessageSource : IChatMessageSource
{
    private readonly Lazy<TwitchClient> _client;
    private readonly List<IObserver<ChatMessage>> _observers;
    private readonly IUserRepository _userRepository;

    public TwitchChatMessageSource(IClient client, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _client = new Lazy<TwitchClient>(() => new TwitchClient(client), LazyThreadSafetyMode.None);
        _observers = new List<IObserver<ChatMessage>>();
    }

    public IDisposable Subscribe(IObserver<ChatMessage> observer)
    {
        return new UnSubscriber(observer, _observers);
    }

    public void Dispose()
    {
        if (_client.IsValueCreated)
        {
            _client.Value.Disconnect();
        }

        _observers.Clear();
    }

    public void Connect(ConnectionCredentials credentials, string chanel)
    {
        var client = _client.Value;
        client.Initialize(credentials, chanel);
        client.OnError += OnError;
        client.OnMessageReceived += OnMessageReceived;
        client.OnDisconnected += OnOnDisconnected;
        client.Connect();
    }

    private void OnOnDisconnected(object? sender, OnDisconnectedEventArgs e)
    {
        var observers = _observers;
        foreach (var observer in observers)
        {
            observer.OnCompleted();
        }
    }

    private async void OnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        var observers = _observers;
        if (observers.Count == 0)
        {
            return;
        }

        var user = await _userRepository.FindByNicknameAsync(e.ChatMessage.Username);
        if (user == null)
        {
            return;
        }

        var chatMessage = new ChatMessage(user, e.ChatMessage.Message);
        foreach (var observer in observers)
        {
            observer.OnNext(chatMessage);
        }
    }

    private void OnError(object? sender, OnErrorEventArgs e)
    {
        var observers = _observers;
        foreach (var observer in observers)
        {
            observer.OnError(e.Exception);
        }
    }

    private sealed class UnSubscriber : IDisposable
    {
        private readonly IObserver<ChatMessage> _observer;
        private readonly List<IObserver<ChatMessage>> _observers;

        public UnSubscriber(IObserver<ChatMessage> observer, List<IObserver<ChatMessage>> observers)
        {
            _observer = observer;
            _observers = observers;
            observers.Add(observer);
        }

        public void Dispose()
        {
            _observers.Remove(_observer);
        }
    }
}
