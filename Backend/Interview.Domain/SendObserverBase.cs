namespace Interview.Domain;

public abstract class SendObserverBase<T> : IObserver<T>
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly List<Task> _activeSendActions;

    public SendObserverBase()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _activeSendActions = new List<Task>();
    }

    public void OnCompleted()
    {
        _cancellationTokenSource.Cancel();
        
        Task.WhenAll(_activeSendActions).ConfigureAwait(false).GetAwaiter().GetResult();
        _activeSendActions.Clear();
        
        _cancellationTokenSource.Dispose();
    }

    public void OnError(Exception error)
    {
        Console.WriteLine("{0}: {1}", GetType().Name, error);
    }

    public void OnNext(T value)
    {
        var activeActions = _activeSendActions.Where(task => task.Status != TaskStatus.Running).ToList();
        foreach (var activeAction in activeActions)
        {
            activeAction.Dispose();
            _activeSendActions.Remove(activeAction);
        }
        
        var newSendAction = SendAsync(value, _cancellationTokenSource.Token);
        _activeSendActions.Add(newSendAction);
    }

    protected abstract Task SendAsync(T value, CancellationToken cancellationToken);
}