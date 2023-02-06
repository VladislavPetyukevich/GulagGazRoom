using Interview.Domain.Questions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public class QuestionTypeConfiguration:  EntityTypeConfigurationBase<Question>
{
    
    protected override void ConfigureCore(EntityTypeBuilder<Question> builder)
    {
        builder.Property(question => question.Value).IsRequired().HasMaxLength(128);
    }
    
}