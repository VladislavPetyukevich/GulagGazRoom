using Interview.Domain.Users.Roles;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public class RoleTypeConfiguration : EntityTypeConfigurationBase<Role>
{
    protected override void ConfigureCore(EntityTypeBuilder<Role> builder)
    {
        builder.Property(e => e.Name)
            .HasConversion(roleName => roleName.Name, name => RoleName.FromName(name, false))
            .HasMaxLength(64)
            .IsRequired();

        builder.HasData(new Role(RoleName.Admin), new Role(RoleName.User));
    }
}
