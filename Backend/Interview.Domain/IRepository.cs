namespace Interview.Domain;

public interface IRepository<T> 
    where T : Entity
{
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);
    ValueTask<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}