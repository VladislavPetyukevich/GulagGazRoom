using Interview.Domain.Events.ChangeEntityProcessors;
using Interview.Domain.Repository;
using Interview.Domain.Users;

namespace Interview.Infrastructure.Database.Processors;

public class DateEntityPreProcessor : AbstractPreProcessor
{
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public DateEntityPreProcessor(ICurrentUserAccessor currentUserAccessor)
    {
        _currentUserAccessor = currentUserAccessor;
    }

    protected override void AddEntityHandler(Entity entity, CancellationToken cancellationToken = default)
    {
        entity.UpdateCreateDate(DateTime.UtcNow);
        entity.CreatedById = _currentUserAccessor.UserId;
    }

    protected override void ModifyOriginalEntityHandler(Entity entity, CancellationToken cancellationToken = default)
    {
        entity.UpdateUpdateDate(DateTime.UtcNow);
    }
}
