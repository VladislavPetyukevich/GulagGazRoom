using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public sealed class UserTypeConfiguration : EntityTypeConfigurationBase<User>
{
    protected override void ConfigureCore(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.Nickname).IsRequired().HasMaxLength(128);
        builder.HasMany(user => user.Roles).WithMany();
    }
}
