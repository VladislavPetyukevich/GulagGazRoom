using Interview.Domain.Questions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public class QuestionTagTypeConfiguration : EntityTypeConfigurationBase<QuestionTag>
{
    protected override void ConfigureCore(EntityTypeBuilder<QuestionTag> builder)
    {
        builder.HasIndex(e => e.QuestionId);
        builder.HasOne(e => e.Question)
            .WithMany(e => e.Tags)
            .HasForeignKey(e => e.QuestionId)
            .IsRequired();
        builder.HasOne(e => e.Tag)
            .WithMany(e => e.QuestionTags)
            .HasForeignKey(e => e.TagId)
            .IsRequired();
        builder.Property(e => e.HexColor).IsRequired().HasMaxLength(6);
    }
}
