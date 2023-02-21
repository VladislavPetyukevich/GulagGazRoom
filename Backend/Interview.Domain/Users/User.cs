using Interview.Domain.Users.Roles;

namespace Interview.Domain.Users;

public class User : Entity
{
    public User(string nickname, string email, string twitchIdentity)
    {
        Nickname = nickname;
        Email = email;
        TwitchIdentity = twitchIdentity;
    }

    private User()
        : this(string.Empty, string.Empty, string.Empty)
    {
    }

    public string Nickname { get; internal set; }

    public string Email { get; internal set; }

    public string TwitchIdentity { get; private set; }

    public List<Role> Roles { get; private set; } = new List<Role>();
}
