namespace Interview.Domain.Chat;

public interface IChatMessageSource : IObservable<ChatMessage>, IDisposable
{
}
