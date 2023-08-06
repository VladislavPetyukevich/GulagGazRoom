using Interview.Domain.Repository;
using Interview.Domain.Users.Roles;

namespace Interview.Domain.Events;

public class AppEvent : Entity
{
    public required string Type { get; set; }

    public required List<Role> Roles { get; set; }
}
