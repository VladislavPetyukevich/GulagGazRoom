namespace Interview.Domain.Chat;

public sealed class ChatObserver : SendObserverBase<ChatMessage>
{
    private readonly IMessageSender _messageSender;
    
    public ChatObserver(IMessageSender messageSender)
    {
        _messageSender = messageSender;
    }

    protected override Task SendAsync(ChatMessage value, CancellationToken cancellationToken)
        => _messageSender.SendAsync(value, cancellationToken);
}