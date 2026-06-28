namespace ViewPorter.Windows.Foreground;

public interface IForegroundWindowWatcher : IDisposable
{
    event EventHandler? ForegroundWindowChanged;

    void StartWatching();

    void StopWatching();
}
