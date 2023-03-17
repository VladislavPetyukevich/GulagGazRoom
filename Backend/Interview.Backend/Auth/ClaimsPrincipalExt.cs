using System.Security.Claims;

namespace Interview.Backend.Auth;

public static class ClaimsPrincipalExt
{
    private const string UserId = "USER_ID";
    
    public static void EnrichRolesWithId(this ClaimsPrincipal self, User user)
    {
        var newRoles = user.Roles.Select(e => new Claim(ClaimTypes.Role, e.Name.Name));

        var claimIdentity = new ClaimsIdentity(newRoles);

        claimIdentity.AddClaim(new Claim(UserId, user.Id.ToString()));

        self.AddIdentity(claimIdentity);
    }

    public static User? ToUser(this ClaimsPrincipal self)
    {
        var twitchId = self.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier);
        var nickname = self.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Name);
        if (twitchId == null || nickname == null)
        {
            return null;
        }

        var id = self.Claims.FirstOrDefault(e => e.Type == UserId);
        var user = new User(nickname.Value, twitchId.Value);
        if (id != null && Guid.TryParse(id.Value, out var typedId))
        {
            user.Id = typedId;
        }

        return user;
    }
}
