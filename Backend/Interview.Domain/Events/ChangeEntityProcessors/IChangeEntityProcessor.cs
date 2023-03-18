using Interview.Domain.Repository;

namespace Interview.Domain.Events.ChangeEntityProcessors;

public interface IChangeEntityProcessor
{
    ValueTask ProcessAddedAsync(IReadOnlyCollection<Entity> entity, CancellationToken cancellationToken);

    ValueTask ProcessModifiedAsync(IReadOnlyCollection<Entity> entity, CancellationToken cancellationToken);
}
