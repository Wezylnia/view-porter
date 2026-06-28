using ViewPorter.Core.Geometry;

namespace ViewPorter.Core.Profiles;

public sealed class ViewportProfile
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    public string Name { get; init; } = "Default";

    public MonitorSelection MonitorSelection { get; init; } = new();

    public SizingMode SizingMode { get; init; } = SizingMode.AspectFit;

    public AspectRatio AspectRatio { get; init; } = AspectRatio.Ratio16By9;

    public PixelSize PixelSize { get; init; } = new(1920, 1080);

    public double? TargetDiagonalInches { get; init; }

    public int BorderColorArgb { get; init; } = unchecked((int)0xFF000000);

    public bool MoveForegroundWindow { get; init; } = true;

    public int InnerMargin { get; init; }
}
