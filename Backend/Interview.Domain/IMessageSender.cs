using Interview.Domain.Chat;
using Interview.Domain.Reactions;

namespace Interview.Domain;

public interface IMessageSender
{
    Task SendAsync(ChatMessage chatMessage, CancellationToken cancellationToken);

    Task SendAsync(Reaction reaction, CancellationToken cancellationToken);
}
