using Interview.Domain.Users.Roles;

namespace Interview.Domain.Users;

public class User : Entity
{
    public User(string nickname, string twitchIdentity)
    {
        Nickname = nickname;
        TwitchIdentity = twitchIdentity;
    }

    private User()
        : this(string.Empty, string.Empty)
    {
    }

    public string Nickname { get; internal set; }

    public string TwitchIdentity { get; private set; }

    public List<Role> Roles { get; private set; } = new List<Role>();
}
