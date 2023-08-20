using System.Linq.Expressions;
using Interview.Domain.RoomParticipants;
using Interview.Domain.Users.Roles;

namespace Interview.Domain.Events.Service.FindPage;

public class AppEventItem<TParticipantType>
{
    public required Guid Id { get; set; }

    public required string Type { get; set; }

    public required ICollection<RoleNameType> Roles { get; set; }

    public required ICollection<TParticipantType> ParticipantTypes { get; set; }
}

public sealed class AppEventItemParticipantType : AppEventItem<RoomParticipantType>
{
    public AppEventItem ToAppEventItem()
    {
        return new AppEventItem
        {
            Id = Id,
            Type = Type,
            Roles = Roles,
            ParticipantTypes = ParticipantTypes.Select(e => e.Name).ToList(),
        };
    }
}

public sealed class AppEventItem : AppEventItem<string>
{
}
