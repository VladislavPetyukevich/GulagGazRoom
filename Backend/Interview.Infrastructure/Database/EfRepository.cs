using Interview.Domain;
using Microsoft.EntityFrameworkCore;
using NSpecifications;
using X.PagedList;

namespace Interview.Infrastructure.Database;

public class EfRepository<T> : IRepository<T>
    where T : Entity
{
    protected readonly AppDbContext Db;
    protected readonly DbSet<T> Set;

    public EfRepository(AppDbContext db)
    {
        Db = db;
        Set = db.Set<T>();
    }

    public Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Set.Add(entity);
        return Db.SaveChangesAsync(cancellationToken);
    }

    public Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set).FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public Task<List<T>> FindByIdsAsync(ICollection<Guid> id, CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set).Where(e => id.Contains(e.Id)).ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Set.Update(entity);
        return Db.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        Set.Remove(entity);
        return Db.SaveChangesAsync(cancellationToken);
    }

    public Task<IPagedList<T>> GetPageAsync(ISpecification<T> specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set).OrderBy(entity => entity.Id).Where(specification.Expression).ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<T>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set).OrderBy(entity => entity.Id).ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    protected virtual IQueryable<T> ApplyIncludes(DbSet<T> set) => set;
}
