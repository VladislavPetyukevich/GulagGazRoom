namespace Interview.Domain.Reactions;

public interface IReactionSource : IObservable<Reaction>, IDisposable
{
}
