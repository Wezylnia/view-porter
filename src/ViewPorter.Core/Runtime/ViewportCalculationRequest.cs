using ViewPorter.Core.Monitors;
using ViewPorter.Core.Profiles;

namespace ViewPorter.Core.Runtime;

public sealed class ViewportCalculationRequest
{
    public required MonitorDescriptor Monitor { get; init; }

    public required ViewportProfile Profile { get; init; }

    public MonitorCalibration? Calibration { get; init; }
}
