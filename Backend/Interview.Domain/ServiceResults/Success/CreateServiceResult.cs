namespace Interview.Domain.ServiceResults.Success;

public sealed class CreateServiceResult<T> : ServiceResult<T>
{
    public CreateServiceResult(T value)
        : base(value)
    {
    }

    public override TRes Match<TRes>(Func<OkServiceResult<T>, TRes> ok, Func<CreateServiceResult<T>, TRes> create)
        => create(this);
}
