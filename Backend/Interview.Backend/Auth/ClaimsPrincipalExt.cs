using System.Security.Claims;

namespace Interview.Backend.Auth;

public static class ClaimsPrincipalExt
{
    public static void EnrichRoles(this ClaimsPrincipal self, User user)
    {
        var newRoles = user.Roles.Select(e => new Claim(ClaimTypes.Role, e.Name.Name));
        var claimIdentity = new ClaimsIdentity(newRoles);
        self.AddIdentity(claimIdentity);
    }

    public static User? ToUser(this ClaimsPrincipal self)
    {
        var id = self.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier);
        var email = self.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Email);
        var nickname = self.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Name);
        if(id == null || email == null || nickname == null)
            return null;
                    
        return new User(nickname.Value, email.Value, id.Value);
    }
}