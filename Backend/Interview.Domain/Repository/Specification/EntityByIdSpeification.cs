using Interview.Domain.Repository;
using Interview.Domain.Users;
using NSpecifications;

namespace Interview.Domain.RoomReviews.Specification;

public sealed class EntityByIdSpeification<T> : Spec<T>
    where T : Entity
{
    public EntityByIdSpeification(Guid id)
        : base(e => e.Id == id)
    {
    }
}
