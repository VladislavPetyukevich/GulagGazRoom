using Interview.Domain;
using Interview.Domain.Events;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations
{
    public class AppEventConfiguration : EntityTypeConfigurationBase<AppEvent>
    {
        protected override void ConfigureCore(EntityTypeBuilder<AppEvent> builder)
        {
            builder.Property(e => e.Type).IsRequired().HasMaxLength(128);
            builder.HasMany(e => e.Roles).WithMany();
            builder.HasIndex(e => e.Type).IsUnique();
        }
    }
}
