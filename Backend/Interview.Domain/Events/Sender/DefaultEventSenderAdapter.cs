namespace Interview.Domain.Events.Sender;

public sealed class DefaultEventSenderAdapter : IEventSenderAdapter
{
    public Task SendAsync<T>(T @event, IEventSender<T> sender, CancellationToken cancellationToken)
        => sender.SendAsync(@event, cancellationToken);
}
