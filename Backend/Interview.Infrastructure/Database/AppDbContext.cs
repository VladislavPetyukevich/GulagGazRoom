using Interview.Domain.Questions;
using Interview.Domain.Rooms;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }

    private AppDbContext()
    {
    }

    public DbSet<User> Users { get; private set; } = null!;

    public DbSet<Question> Questions { get; private set; } = null!;

    public DbSet<Room> Rooms { get; private set; } = null!;

    public DbSet<Role> Roles { get; private set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
