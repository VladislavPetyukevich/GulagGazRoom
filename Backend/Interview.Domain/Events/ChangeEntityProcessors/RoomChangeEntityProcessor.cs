using Interview.Domain.Events.Events;
using Interview.Domain.Repository;
using Interview.Domain.RoomConfigurations;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms;

namespace Interview.Domain.Events.ChangeEntityProcessors
{
    public class RoomChangeEntityProcessor : IChangeEntityProcessor
    {
        private readonly IRoomEventDispatcher _eventDispatcher;

        public RoomChangeEntityProcessor(IRoomEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        public async ValueTask ProcessAddedAsync(IReadOnlyCollection<Entity> entities, CancellationToken cancellationToken)
        {
            foreach (var entity in entities.OfType<Room>())
            {
                var e = CreateEvent(entity, null);
                if (e is null)
                {
                    continue;
                }

                await _eventDispatcher.WriteAsync(e, cancellationToken);
            }
        }

        public async ValueTask ProcessModifiedAsync(IReadOnlyCollection<(Entity Original, Entity Current)> entities, CancellationToken cancellationToken)
        {
            foreach (var (originalEntity, currentEntity) in entities)
            {
                if (originalEntity is not Room original || currentEntity is not Room current)
                {
                    continue;
                }

                var e = CreateEvent(current, original);
                if (e is null)
                {
                    continue;
                }

                await _eventDispatcher.WriteAsync(e, cancellationToken);
            }
        }

        private static IRoomEvent? CreateEvent(Room current, Room? original)
        {
            if (current.Configuration is null)
            {
                return null;
            }

            if (original?.Configuration is null || original.Configuration.CodeEditorContent != current.Configuration.CodeEditorContent)
            {
                return new ChangeCodeEditorRoomEvent(current.Id, current.Configuration.CodeEditorContent);
            }

            return null;
        }

        public sealed class ChangeCodeEditorRoomEvent : RoomEvent
        {
            public ChangeCodeEditorRoomEvent(Guid roomId, string? value)
                : base(roomId, EventType.ChangeCodeEditor, value)
            {
            }
        }
    }
}
