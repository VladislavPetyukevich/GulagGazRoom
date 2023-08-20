using Interview.Domain.Repository;
using Interview.Domain.Users;

namespace Interview.Infrastructure.Database.Processors;

public class AccessEntityActionPreProcessor : AbstractPreProcessor
{
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public AccessEntityActionPreProcessor(ICurrentUserAccessor currentUserAccessor)
    {
        _currentUserAccessor = currentUserAccessor;
    }

    protected override void AddEntityHandler(Entity entity, CancellationToken cancellationToken)
    {
    }

    protected override void ModifyOriginalEntityHandler(Entity entity, CancellationToken cancellationToken)
    {
        if (!_currentUserAccessor.IsAdmin() && !_currentUserAccessor.HasId(entity.CreatedById))
        {
            throw new PermissionDinedException("The user does not have the rights to perform the action");
        }
    }
}
