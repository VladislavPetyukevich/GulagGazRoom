using System.Linq.Expressions;
using Interview.Domain.Users.Roles;

namespace Interview.Domain.Events.Service.FindPage;

public sealed class AppEventItem
{
    public required Guid Id { get; set; }

    public required string Type { get; set; }

    public required ICollection<RoleNameType> Roles { get; set; }

    public required ICollection<string> ParticipantTypes { get; set; }
}
