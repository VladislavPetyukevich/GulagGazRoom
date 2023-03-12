using Interview.Domain.Reactions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public class ReactionConfiguration : EntityTypeConfigurationBase<Reaction>
{
    protected override void ConfigureCore(EntityTypeBuilder<Reaction> builder)
    {
        builder.Property(question => question.Type)
            .IsRequired()
            .HasConversion(e => e.Name, e => ReactionType.FromName(e, false));

        builder.HasOne(e => e.FromUser)
            .WithOne()
            .HasForeignKey<Reaction>(e => e.FromUserId)
            .IsRequired();

        builder.HasOne(e => e.ToUser)
            .WithOne()
            .HasForeignKey<Reaction>(e => e.ToUserId)
            .IsRequired();

        builder.HasOne(e => e.Room)
            .WithOne()
            .HasForeignKey<Reaction>(e => e.RoomId)
            .IsRequired();
    }
}
