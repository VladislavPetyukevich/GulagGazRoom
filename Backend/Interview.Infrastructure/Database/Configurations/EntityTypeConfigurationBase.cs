using Interview.Domain;
using Interview.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public abstract class EntityTypeConfigurationBase<T> : IEntityTypeConfiguration<T>
    where T : Entity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        ConfigureCore(builder);
    }

    protected abstract void ConfigureCore(EntityTypeBuilder<T> builder);
}
