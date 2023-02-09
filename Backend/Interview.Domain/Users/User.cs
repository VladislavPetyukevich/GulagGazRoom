namespace Interview.Domain.Users;

public class User : Entity
{
    public User(string nickname)
    {
        Nickname = nickname;
    }

    public string Nickname { get; }
}