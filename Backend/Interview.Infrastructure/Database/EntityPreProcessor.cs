using Interview.Domain.Repository;
using Interview.Domain.Users;

namespace Interview.Infrastructure.Database;

public interface IEntityPreProcessor
{
    public void Notify(Entity entity);
}

public interface IEntityAdditionPreProcessor : IEntityPreProcessor
{
}

public interface IEntityModifyPreProcessor : IEntityPreProcessor
{
}

public class OwnerAdditionEntityPreProcessor : IEntityAdditionPreProcessor
{
    private readonly ICurrentUserAccessor? _currentUserAccessor;

    public OwnerAdditionEntityPreProcessor(ICurrentUserAccessor? currentUserAccessor)
    {
        _currentUserAccessor = currentUserAccessor;
    }

    public void Notify(Entity entity)
    {
        entity.CreatedById = _currentUserAccessor?.UserId;
    }
}

public class AccessEntityModifyPreProcessor : IEntityModifyPreProcessor
{
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public AccessEntityModifyPreProcessor(ICurrentUserAccessor currentUserAccessor)
    {
        _currentUserAccessor = currentUserAccessor;
    }

    public void Notify(Entity entity)
    {
        if (!_currentUserAccessor.IsAdmin() && !_currentUserAccessor.HasId(entity.CreatedById))
        {
            throw new UnauthorizedAccessException($"{typeof(Entity)} forbidden");
        }
    }
}
