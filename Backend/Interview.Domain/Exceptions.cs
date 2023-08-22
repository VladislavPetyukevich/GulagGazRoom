using Interview.Domain.Repository;

namespace Interview.Domain;

public class UserException : Exception
{
    public UserException(string message)
        : base(message)
    {
    }

    public UserException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

public class NotFoundException : UserException
{
    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public static NotFoundException Create<T>(Guid id)
        where T : Entity
    {
        return new NotFoundException($"Not found '{typeof(T).Name}' by Id '{id}'");
    }
}

public class AccessDeniedException : UserException
{
    public AccessDeniedException(string message)
        : base(message)
    {
    }

    public AccessDeniedException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
