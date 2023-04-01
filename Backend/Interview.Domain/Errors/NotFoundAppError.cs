namespace Interview.Domain.Errors
{
    public sealed class NotFoundAppError : AppError
    {
        public NotFoundAppError(string message)
            : base(message)
        {
        }

        public override TRes Match<TRes>(Func<AppError, TRes> appError, Func<NotFoundAppError, TRes> notFoundError)
            => notFoundError(this);
    }
}
