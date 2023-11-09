namespace Interview.Domain.Events.Sender;

public interface IEventSenderAdapter
{
    Task SendAsync<T>(T @event, IEventSender<T> sender, CancellationToken cancellationToken);
}
