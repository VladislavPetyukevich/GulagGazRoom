using System.Linq.Expressions;
using Interview.Domain.Tags;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public abstract class TagLinkTypeConfigurationBase<T> : EntityTypeConfigurationBase<T>
    where T : TagLink
{
    protected void Configure(EntityTypeBuilder<T> builder, Expression<Func<Tag, IEnumerable<T>>> tagSelector)
    {
        builder.HasOne(e => e.Tag)
            .WithMany(tagSelector!)
            .HasForeignKey(e => e.TagId)
            .IsRequired();
        builder.Property(e => e.HexColor).IsRequired().HasMaxLength(6);
    }
}
