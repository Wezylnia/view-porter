namespace ViewPorter.Core.Monitors;

public sealed class MonitorCalibration
{
    public string MonitorKey { get; init; } = string.Empty;

    public int NativePixelWidth { get; init; }

    public int NativePixelHeight { get; init; }

    public double DiagonalInches { get; init; }

    public double AdjustmentFactor { get; init; } = 1.0d;

    public bool IsValid =>
        NativePixelWidth > 0 &&
        NativePixelHeight > 0 &&
        DiagonalInches > 0 &&
        AdjustmentFactor > 0;
}
