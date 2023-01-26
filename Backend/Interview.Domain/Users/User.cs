namespace Interview.Domain.Users;

public class User : Entity
{
    public string Nickname { get; private set; }

    public User(string nickname)
    {
        Nickname = nickname;
    }
}