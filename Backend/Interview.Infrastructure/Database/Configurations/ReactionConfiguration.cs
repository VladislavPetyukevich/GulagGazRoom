using Interview.Domain.Reactions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Internal;

namespace Interview.Infrastructure.Database.Configurations;

public class ReactionConfiguration : EntityTypeConfigurationBase<Reaction>
{
    private readonly ISystemClock _clock;

    public ReactionConfiguration(ISystemClock clock)
    {
        _clock = clock;
    }

    protected override void ConfigureCore(EntityTypeBuilder<Reaction> builder)
    {
        builder.Property(question => question.Type)
            .IsRequired()
            .HasConversion(e => e.Name, e => ReactionType.FromName(e, false));

        builder.HasIndex(e => e.Type);

        var entities = ReactionType.List.Where(e => e != ReactionType.Unknown)
            .Select(rt => new Reaction
            {
                Id = rt.Id,
                Type = rt,
            })
            .ToList();

        foreach (var entity in entities)
        {
            entity.UpdateCreateDate(_clock.UtcNow.Date);
        }

        builder.HasData(entities);
    }
}
