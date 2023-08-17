using Interview.Domain.Users;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Interview.Infrastructure.Database;

public interface IPooledDbContextInterceptor<in TContext>
    where TContext : DbContext
{
    void OnCreate(TContext dbContext);

    void OnReturn(TContext dbContext);

    IServiceProvider GetServiceProvider();
}

public sealed class DefaultAppDbContextPooledDbContextInterceptor : IPooledDbContextInterceptor<AppDbContext>
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultAppDbContextPooledDbContextInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void OnCreate(AppDbContext dbContext)
    {
        dbContext.Interceptor = this;
    }

    public void OnReturn(AppDbContext dbContext)
    {
        dbContext.Interceptor = null!;
    }

    public IServiceProvider GetServiceProvider()
    {
        return _serviceProvider;
    }
}
