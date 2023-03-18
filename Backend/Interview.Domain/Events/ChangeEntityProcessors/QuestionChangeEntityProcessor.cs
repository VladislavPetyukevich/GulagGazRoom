using Interview.Domain.Events.Events;
using Interview.Domain.Questions;
using Interview.Domain.Repository;

namespace Interview.Domain.Events.ChangeEntityProcessors;

public class QuestionChangeEntityProcessor : IChangeEntityProcessor
{
    private readonly IRoomEventDispatcher _eventDispatcher;

    public QuestionChangeEntityProcessor(IRoomEventDispatcher eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;
    }

    public ValueTask ProcessAddedAsync(IReadOnlyCollection<Entity> entities, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask ProcessModifiedAsync(IReadOnlyCollection<(Entity Original, Entity Current)> entities, CancellationToken cancellationToken = default)
    {
        foreach (var (originalEntity, currentEntity) in entities)
        {
            if (originalEntity is not Question original || currentEntity is not Question current)
            {
                continue;
            }

            foreach (var roomId in _eventDispatcher.ActiveRooms)
            {
                await _eventDispatcher.WriteAsync(CreateEvent(current, original, roomId), cancellationToken);
            }
        }
    }

    private IRoomEvent CreateEvent(Question current, Question original, Guid roomId)
    {
        return new RoomEvent<EventPayload>(
            roomId,
            EventType.ChangeQuestion,
            new EventPayload(current.Id, original.Value, current.Value));
    }

    public sealed class EventPayload
    {
        public Guid QuestionId { get; }

        public string OldValue { get; }

        public string NewValue { get; }

        public EventPayload(Guid questionId, string oldValue, string newValue)
        {
            QuestionId = questionId;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
