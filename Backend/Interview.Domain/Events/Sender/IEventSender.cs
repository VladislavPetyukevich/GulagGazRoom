namespace Interview.Domain.Events.Sender;

public interface IEventSender<in T>
{
    Task SendAsync(T @event, CancellationToken cancellationToken);
}
