using Interview.Domain.Questions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public class QuestionTagTypeConfiguration : TagLinkTypeConfigurationBase<QuestionTag>
{
    protected override void ConfigureCore(EntityTypeBuilder<QuestionTag> builder)
    {
        builder.HasIndex(e => e.QuestionId);
        builder.HasOne(e => e.Question)
            .WithMany(e => e.Tags)
            .HasForeignKey(e => e.QuestionId)
            .IsRequired();
        Configure(builder, e => e.QuestionTags);
    }
}
