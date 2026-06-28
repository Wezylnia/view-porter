using ViewPorter.Core.Geometry;

namespace ViewPorter.Windows.Windows;

public sealed class WindowManager : IWindowManager
{
    public Task<bool> MoveForegroundWindowIntoViewportAsync(PixelRect viewport, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task RestoreChangedWindowsAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
