namespace Interview.Domain.ServiceResults.Errors;

public class ServiceError : IEquatable<ServiceError>
{
    public string Message { get; }

    public ServiceError(string message)
    {
        Message = message;
    }

    public static ServiceError AccessDenied(string message = "") => new AccessDeniedServiceError(string.IsNullOrEmpty(message) ? "The user does not have enough permissions." : message);

    public static ServiceError NotFound(string message) => new NotFoundServiceError(message);

    public static ServiceError Error(string message) => new ServiceError(message);

    public virtual TRes Match<TRes>(Func<ServiceError, TRes> appError, Func<NotFoundServiceError, TRes> notFoundError, Func<AccessDeniedServiceError, TRes> accessDeniedError)
        => appError(this);

    public bool Equals(ServiceError? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Message == other.Message;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((ServiceError)obj);
    }

    public override int GetHashCode() => Message.GetHashCode();

    public override string ToString() => Message;
}
