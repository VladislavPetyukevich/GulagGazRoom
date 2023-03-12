using Interview.Domain.Users.Roles;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Internal;

namespace Interview.Infrastructure.Database.Configurations;

public class RoleTypeConfiguration : EntityTypeConfigurationBase<Role>
{
    private readonly ISystemClock _clock;

    public RoleTypeConfiguration(ISystemClock clock)
    {
        _clock = clock;
    }

    protected override void ConfigureCore(EntityTypeBuilder<Role> builder)
    {
        builder.Property(e => e.Name)
            .HasConversion(roleName => roleName.Name, name => RoleName.FromName(name, false))
            .HasMaxLength(64)
            .IsRequired();

        var roles = new[]
        {
            new Role(RoleName.Admin),
            new Role(RoleName.User),
        };
        foreach (var role in roles)
        {
            role.UpdateCreateDate(_clock.UtcNow.Date);
        }

        builder.HasData(roles);
    }
}
