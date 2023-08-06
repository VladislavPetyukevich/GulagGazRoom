namespace Interview.Domain.ServiceResults.Errors
{
    public class AccessDeniedServiceError : ServiceError
    {
        public AccessDeniedServiceError(string message)
            : base(message)
        {
        }

        public override TRes Match<TRes>(Func<ServiceError, TRes> appError, Func<NotFoundServiceError, TRes> notFoundError, Func<AccessDeniedServiceError, TRes> accessDeniedError)
            => accessDeniedError(this);
    }
}
