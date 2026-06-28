using ViewPorter.Core.Geometry;
using ViewPorter.Core.Monitors;
using ViewPorter.Core.Profiles;
using ViewPorter.Core.Runtime;

namespace ViewPorter.Core.Tests;

public sealed class ViewportCalculatorTests
{
    private readonly ViewportCalculator _calculator = new();

    [Fact]
    public void AspectFit_UsesLargestCenteredRectangle()
    {
        var result = _calculator.Calculate(
            new ViewportCalculationRequest
            {
                Monitor = CreateMonitor(new PixelRect(0, 0, 3840, 2160)),
                Profile = new ViewportProfile
                {
                    Name = "Centered 4:3",
                    SizingMode = SizingMode.AspectFit,
                    AspectRatio = AspectRatio.Ratio4By3
                }
            });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Plan);
        Assert.Equal(new PixelRect(480, 0, 2880, 2160), result.Plan!.Viewport);
        Assert.Equal(new PixelRect(0, 0, 480, 2160), result.Plan.LeftBorder);
        Assert.Equal(new PixelRect(3360, 0, 480, 2160), result.Plan.RightBorder);
    }

    [Fact]
    public void PixelSize_ClampsToMonitorBounds()
    {
        var result = _calculator.Calculate(
            new ViewportCalculationRequest
            {
                Monitor = CreateMonitor(new PixelRect(0, 0, 2560, 1440)),
                Profile = new ViewportProfile
                {
                    Name = "Too Large",
                    SizingMode = SizingMode.PixelSize,
                    PixelSize = new PixelSize(3000, 2000),
                    AspectRatio = AspectRatio.Ratio16By9
                }
            });

        Assert.True(result.IsSuccess);
        Assert.Equal(new PixelRect(0, 0, 2560, 1440), result.Plan!.Viewport);
    }

    [Fact]
    public void PhysicalDiagonal_RequiresCalibration()
    {
        var result = _calculator.Calculate(
            new ViewportCalculationRequest
            {
                Monitor = CreateMonitor(new PixelRect(0, 0, 3840, 2160)),
                Profile = new ViewportProfile
                {
                    Name = "24.5 inch",
                    SizingMode = SizingMode.PhysicalDiagonal,
                    AspectRatio = AspectRatio.Ratio16By9,
                    TargetDiagonalInches = 24.5d
                }
            });

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Contains("calibration", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void PhysicalDiagonal_CalculatesViewportFromCalibration()
    {
        var result = _calculator.Calculate(
            new ViewportCalculationRequest
            {
                Monitor = CreateMonitor(new PixelRect(0, 0, 3840, 2160)),
                Calibration = new MonitorCalibration
                {
                    MonitorKey = "display-1",
                    NativePixelWidth = 3840,
                    NativePixelHeight = 2160,
                    DiagonalInches = 27,
                    AdjustmentFactor = 1.0d
                },
                Profile = new ViewportProfile
                {
                    Name = "24.5 inch",
                    SizingMode = SizingMode.PhysicalDiagonal,
                    AspectRatio = AspectRatio.Ratio16By9,
                    TargetDiagonalInches = 24.5d
                }
            });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Plan);
        Assert.True(result.Plan!.Viewport.Width < 3840);
        Assert.True(result.Plan.Viewport.Height < 2160);
    }

    private static MonitorDescriptor CreateMonitor(PixelRect bounds) =>
        new()
        {
            MonitorKey = "display-1",
            DeviceName = @"\\.\DISPLAY1",
            FriendlyName = "Display 1",
            Bounds = bounds,
            WorkingArea = bounds,
            IsPrimary = true
        };
}
