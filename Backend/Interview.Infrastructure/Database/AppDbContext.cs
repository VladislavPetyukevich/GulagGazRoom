using Interview.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Database;

public sealed class AppDbContext : DbContext
{
    public DbSet<User> Users { get; private set; } = null!;

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}