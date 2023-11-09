using NSpecifications;

namespace Interview.Domain.Events.Storage;

public interface IEventStorage
{
    Task AddAsync(StorageEvent @event, CancellationToken cancellationToken);

    Task<List<StorageEvent>> GetBySpecAsync(Spec<StorageEvent> spec, CancellationToken cancellationToken);
}
