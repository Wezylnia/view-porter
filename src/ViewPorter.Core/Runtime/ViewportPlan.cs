using ViewPorter.Core.Geometry;

namespace ViewPorter.Core.Runtime;

public sealed class ViewportPlan
{
    public required PixelRect Viewport { get; init; }

    public required PixelRect TopBorder { get; init; }

    public required PixelRect BottomBorder { get; init; }

    public required PixelRect LeftBorder { get; init; }

    public required PixelRect RightBorder { get; init; }
}
