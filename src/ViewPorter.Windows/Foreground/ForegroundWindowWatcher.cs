namespace ViewPorter.Windows.Foreground;

public sealed class ForegroundWindowWatcher : IForegroundWindowWatcher
{
    public event EventHandler? ForegroundWindowChanged;

    public void StartWatching()
    {
    }

    public void StopWatching()
    {
    }

    public void Dispose()
    {
        StopWatching();
    }

    internal void RaiseForegroundWindowChanged() => ForegroundWindowChanged?.Invoke(this, EventArgs.Empty);
}
