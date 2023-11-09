using NSpecifications;

namespace Interview.Domain.Events.Storage;

public sealed class EmptyEventStorage : IEventStorage
{
    public Task AddAsync(StorageEvent @event, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<List<StorageEvent>> GetBySpecAsync(Spec<StorageEvent> spec, CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<StorageEvent>());
    }
}
