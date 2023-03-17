using Interview.Domain;
using Interview.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using NSpecifications;
using X.PagedList;

namespace Interview.Infrastructure.Database;

public abstract class EfRepository<T> : IRepository<T>
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
        return Set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public Task<TRes?> FindByIdAsync<TRes>(Guid id, IMapper<T, TRes> mapper, CancellationToken cancellationToken = default)
    {
        return Set.Where(e => e.Id == id).Select(mapper.Expression).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<T?> FindByIdDetailedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set).FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public Task<TRes?> FindByIdDetailedAsync<TRes>(Guid id, IMapper<T, TRes> mapper, CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set).Where(e => e.Id == id).Select(mapper.Expression).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<T>> FindByIdsAsync(ICollection<Guid> id, CancellationToken cancellationToken = default)
    {
        return Set.Where(e => id.Contains(e.Id)).ToListAsync(cancellationToken);
    }

    public Task<List<T>> FindByIdsDetailedAsync(ICollection<Guid> id, CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set).Where(e => id.Contains(e.Id)).ToListAsync(cancellationToken);
    }

    public Task<List<TRes>> FindByIdsAsync<TRes>(ICollection<Guid> id, IMapper<T, TRes> mapper, CancellationToken cancellationToken = default)
    {
        return Set.Where(e => id.Contains(e.Id)).Select(mapper.Expression).ToListAsync(cancellationToken);
    }

    public Task<List<TRes>> FindByIdsDetailedAsync<TRes>(ICollection<Guid> id, IMapper<T, TRes> mapper, CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set).Where(e => id.Contains(e.Id)).Select(mapper.Expression).ToListAsync(cancellationToken);
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

    public Task<bool> IsExistsWithIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Set.AnyAsync(entity => entity.Id == id, cancellationToken);

    public Task<IPagedList<T>> GetPageAsync(ISpecification<T> specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return Set
            .OrderBy(entity => entity.Id)
            .Where(specification.Expression)
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<T>> GetPageDetailedAsync(
        ISpecification<T> specification,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set)
            .OrderBy(entity => entity.Id)
            .Where(specification.Expression)
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<T>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return Set.OrderBy(entity => entity.Id).ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<T>> GetPageDetailedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set)
            .OrderBy(entity => entity.Id)
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<TRes>> GetPageAsync<TRes>(
        ISpecification<T> specification,
        IMapper<T, TRes> mapper,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return Set
            .Where(specification.Expression)
            .OrderBy(entity => entity.Id)
            .Select(mapper.Expression)
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<TRes>> GetPageDetailedAsync<TRes>(
        ISpecification<T> specification,
        IMapper<T, TRes> mapper,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set)
            .Where(specification.Expression)
            .OrderBy(entity => entity.Id)
            .Select(mapper.Expression)
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<TRes>> GetPageAsync<TRes>(
        IMapper<T, TRes> mapper,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return Set
            .OrderBy(entity => entity.Id)
            .Select(mapper.Expression)
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<TRes>> GetPageDetailedAsync<TRes>(
        IMapper<T, TRes> mapper,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set)
            .OrderBy(entity => entity.Id)
            .Select(mapper.Expression)
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    protected abstract IQueryable<T> ApplyIncludes(DbSet<T> set);
}
