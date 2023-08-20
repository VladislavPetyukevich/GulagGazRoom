using Interview.Domain.Questions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public class QuestionTagTypeConfiguration : EntityTypeConfigurationBase<QuestionTag>
{
    protected override void ConfigureCore(EntityTypeBuilder<QuestionTag> builder)
    {
        builder.Property(question => question.Value).IsRequired().HasMaxLength(128);
        builder.HasIndex(e => e.Value).IsUnique();
    }
}
