using Interview.Domain.Repository;

namespace Interview.Domain.Events.ChangeEntityProcessors;

public interface IChangeEntityProcessor
{
    ValueTask ProcessAddedAsync(IReadOnlyCollection<Entity> entities, CancellationToken cancellationToken);

    ValueTask ProcessModifiedAsync(IReadOnlyCollection<(Entity Original, Entity Current)> entities, CancellationToken cancellationToken);
}
