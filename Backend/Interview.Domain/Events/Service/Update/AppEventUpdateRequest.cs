using Interview.Domain.Users.Roles;

namespace Interview.Domain.Events.Service.Update;

public class AppEventUpdateRequest
{
    public string? Type { get; set; }

    public ICollection<RoleNameType>? Roles { get; set; }
}
