using Interview.Domain.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;
/*
public class QuestionTagTypeConfiguration : EntityTypeConfigurationBase<QuestionTag>
{
    protected override void ConfigureCore(EntityTypeBuilder<QuestionTag> builder)
    {
        builder.HasOne(e => e.Tag).WithMany().HasForeignKey(e => e.TagId).IsRequired();
        builder.HasOne(e => e.Question).WithMany().HasForeignKey(e => e.QuestionId).IsRequired();
        builder.Property(e => e.HexColor).IsRequired().HasMaxLength(6);
    }
}
*/
