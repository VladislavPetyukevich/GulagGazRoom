using Interview.Domain.Events.Events;
using Interview.Domain.Reactions;
using Interview.Domain.Repository;
using Interview.Domain.RoomQuestionReactions;

namespace Interview.Domain.Events.ChangeEntityProcessors;

public class RoomQuestionReactionChangeEntityProcessor : IChangeEntityProcessor
{
    private readonly IRoomEventDispatcher _eventDispatcher;

    public RoomQuestionReactionChangeEntityProcessor(IRoomEventDispatcher eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;
    }

    public async ValueTask ProcessAddedAsync(IReadOnlyCollection<Entity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities.OfType<RoomQuestionReaction>())
        {
            if (entity.Reaction.Type == ReactionType.Like)
            {
                await _eventDispatcher.WriteAsync(CreateEvent(entity, EventType.ReactionLike), cancellationToken);
            }
            else if (entity.Reaction.Type == ReactionType.Dislike)
            {
                await _eventDispatcher.WriteAsync(CreateEvent(entity, EventType.ReactionDislike), cancellationToken);
            }
        }
    }

    public ValueTask ProcessModifiedAsync(IReadOnlyCollection<Entity> entities, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    private IRoomEvent CreateEvent(RoomQuestionReaction entity, EventType type)
    {
        return new RoomEvent<RoomEventUserPayload>(
            entity.RoomQuestion.Room.Id,
            EventType.ReactionLike,
            new RoomEventUserPayload(entity.Sender.Id));
    }
}
