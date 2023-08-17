using Interview.Domain.Users;
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
        dbContext.LazyCurrentUserAccessor = new LazyCurrentUserAccessor(_serviceProvider);
    }

    public void OnReturn(AppDbContext dbContext)
    {
        dbContext.LazyCurrentUserAccessor = null!;
    }
}

public sealed class LazyCurrentUserAccessor : ICurrentUserAccessor
{
    private readonly Lazy<ICurrentUserAccessor> _root;

    public LazyCurrentUserAccessor(IServiceProvider serviceProvider)
    {
        _root = new Lazy<ICurrentUserAccessor>(
            () => serviceProvider.GetRequiredService<ICurrentUserAccessor>());
    }

    public Guid? UserId => _root.Value.UserId;

    public User? UserDetailed => _root.Value.UserDetailed;
}
