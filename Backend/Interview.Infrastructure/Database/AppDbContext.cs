using Interview.Domain.Questions;
using Interview.Domain.Rooms;
using Interview.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Database;

public sealed class AppDbContext : DbContext
{
    public DbSet<User> Users { get; private set; } = null!;

    public DbSet<Question> Questions { get; private set; } = null!;

    public DbSet<Room> Rooms { get; private set; } = null!;

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}