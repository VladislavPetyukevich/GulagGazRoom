namespace Interview.Domain.Users;

public class User : Entity
{
    public User(string nickname)
    {
        Nickname = nickname;
    }
    
    private User() : this(string.Empty)
    {
    }

    public string Nickname { get; }
}