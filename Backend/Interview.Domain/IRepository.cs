using NSpecifications;
using X.PagedList;

namespace Interview.Domain;

public interface IRepository<T>
    where T : Entity
{
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

    Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<T>> FindByIdsAsync(ICollection<Guid> id, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    Task<IPagedList<T>> GetPageAsync(ISpecification<T> specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<IPagedList<T>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
