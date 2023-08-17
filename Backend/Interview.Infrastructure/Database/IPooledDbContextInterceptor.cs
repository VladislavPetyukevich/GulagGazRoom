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
        dbContext.LazyCurrentUserAccessor = () => _serviceProvider.GetService<ICurrentUserAccessor>();
    }

    public void OnReturn(AppDbContext dbContext)
    {
        dbContext.LazyCurrentUserAccessor = null!;
    }
}
