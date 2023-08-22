namespace Interview.Domain.Users.Records;

public class PermissionItem
{
    public PermissionItem(Guid id, string type, bool activate)
    {
        Id = id;
        Type = type;
        Activate = activate;
    }

    public Guid Id { get; set; }

    public string Type { get; set; }

    public bool Activate { get; set; }
}
