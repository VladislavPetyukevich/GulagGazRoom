using Interview.Domain.Questions;
using Interview.Domain.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public class QuestionTypeConfiguration : EntityTypeConfigurationBase<Question>
{
    protected override void ConfigureCore(EntityTypeBuilder<Question> builder)
    {
        builder.Property(question => question.Value).IsRequired().HasMaxLength(128);
        builder.Property(question => question.IsArchived).IsRequired().HasDefaultValue(false);
        /*
        builder.HasMany<Tag>().WithMany().UsingEntity<QuestionTag>(
            l => l.HasOne<Tag>().WithMany().HasForeignKey(e => e.TagId),
            r => r.HasOne<Question>().WithMany().HasForeignKey(e => e.QuestionId));
            */
    }
}
