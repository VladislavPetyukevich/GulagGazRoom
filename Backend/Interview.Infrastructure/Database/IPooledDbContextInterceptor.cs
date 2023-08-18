using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Interview.Infrastructure.Database;

public interface IPooledDbContextInterceptor<in TContext>
    where TContext : DbContext
{
    void OnCreate(TContext dbContext);

    void OnReturn(TContext dbContext);
}

public class UserAccessorDbContextInterceptor : IPooledDbContextInterceptor<AppDbContext>
{
    private readonly IServiceProvider _serviceProvider;

    public UserAccessorDbContextInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void OnCreate(AppDbContext dbContext)
    {
        dbContext.LazyPreProcessors = new LazyPreProcessors(_serviceProvider);
    }

    public void OnReturn(AppDbContext dbContext)
    {
        dbContext.LazyPreProcessors = null!;
    }
}

public sealed class LazyPreProcessors
{
    private readonly Lazy<List<IEntityAdditionPreProcessor>> _addPreProcessors;

    private readonly Lazy<List<IEntityModifyPreProcessor>> _modifyPreProcessors;

    public LazyPreProcessors(IServiceProvider serviceProvider)
    {
        _addPreProcessors = new Lazy<List<IEntityAdditionPreProcessor>>(
            () => serviceProvider.GetServices<IEntityAdditionPreProcessor>().ToList());
        _modifyPreProcessors = new Lazy<List<IEntityModifyPreProcessor>>(
            () => serviceProvider.GetServices<IEntityModifyPreProcessor>().ToList());
    }

    public List<IEntityAdditionPreProcessor> AddPreProcessors => _addPreProcessors.Value;

    public List<IEntityModifyPreProcessor> ModifyPreProcessors => _modifyPreProcessors.Value;
}
