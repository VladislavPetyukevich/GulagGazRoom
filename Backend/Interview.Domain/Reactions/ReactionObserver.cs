namespace Interview.Domain.Reactions;

public sealed class ReactionObserver : SendObserverBase<Reaction>
{
    private readonly IMessageSender _messageSender;

    public ReactionObserver(IMessageSender messageSender)
    {
        _messageSender = messageSender;
    }

    protected override Task SendAsync(Reaction value, CancellationToken cancellationToken)
    {
        return _messageSender.SendAsync(value, cancellationToken);
    }
}
