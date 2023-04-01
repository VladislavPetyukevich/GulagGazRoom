namespace Interview.Domain.Errors
{
    public class AppError : IEquatable<AppError>
    {
        public string Message { get; }

        public AppError(string message)
        {
            Message = message;
        }

        public static AppError NotFound(string message) => new NotFoundAppError(message);

        public static AppError Error(string message) => new AppError(message);

        public virtual TRes Match<TRes>(Func<AppError, TRes> appError, Func<NotFoundAppError, TRes> notFoundError)
            => appError(this);

        public bool Equals(AppError? other)
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

            return Equals((AppError)obj);
        }

        public override int GetHashCode() => Message.GetHashCode();

        public override string ToString() => Message;
    }
}
