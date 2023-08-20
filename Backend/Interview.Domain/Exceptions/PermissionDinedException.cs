namespace Interview.Infrastructure;

public class PermissionDinedException : Exception
{
    public PermissionDinedException()
    {
    }

    public PermissionDinedException(string message)
        : base(message)
    {
    }
}
