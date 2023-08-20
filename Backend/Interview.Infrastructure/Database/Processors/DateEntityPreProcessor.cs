using Interview.Domain.Events.ChangeEntityProcessors;
using Interview.Domain.Repository;

namespace Interview.Infrastructure.Database.Processors;

public class DateEntityPreProcessor : AbstractPreProcessor
{
    protected override void AddEntityHandler(Entity entity, CancellationToken cancellationToken)
    {
        entity.UpdateCreateDate(DateTime.UtcNow);
    }

    protected override void ModifyOriginalEntityHandler(Entity entity, CancellationToken cancellationToken)
    {
        entity.UpdateUpdateDate(DateTime.UtcNow);
    }
}
