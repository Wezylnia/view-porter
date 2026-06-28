using ViewPorter.Core.Runtime;

namespace ViewPorter.Windows.Overlays;

public interface IOverlayManager
{
    Task ShowAsync(ViewportPlan plan, int borderColorArgb, CancellationToken cancellationToken = default);

    Task HideAsync(CancellationToken cancellationToken = default);
}
