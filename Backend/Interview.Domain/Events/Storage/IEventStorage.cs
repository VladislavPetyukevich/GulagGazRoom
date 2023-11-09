using NSpecifications;

namespace Interview.Domain.Events.Storage;

public interface IEventStorage
{
    Task AddAsync(IStorageEvent @event, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<IStorageEvent>> GetBySpecAsync(ISpecification<IStorageEvent> spec, CancellationToken cancellationToken);
}
