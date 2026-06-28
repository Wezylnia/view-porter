using ViewPorter.Core.Runtime;

namespace ViewPorter.Windows.Overlays;

public sealed class OverlayManager : IOverlayManager
{
    public Task ShowAsync(ViewportPlan plan, int borderColorArgb, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(plan);
        return Task.CompletedTask;
    }

    public Task HideAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
