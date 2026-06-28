using ViewPorter.Core.Geometry;

namespace ViewPorter.Windows.Windows;

public interface IWindowManager
{
    Task<bool> MoveForegroundWindowIntoViewportAsync(PixelRect viewport, CancellationToken cancellationToken = default);

    Task RestoreChangedWindowsAsync(CancellationToken cancellationToken = default);
}
