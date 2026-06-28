using ViewPorter.Core.Geometry;
using ViewPorter.Core.Monitors;
using ViewPorter.Core.Profiles;

namespace ViewPorter.Core.Runtime;

public sealed class ViewportCalculator
{
    public ViewportCalculationResult Calculate(ViewportCalculationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Monitor);
        ArgumentNullException.ThrowIfNull(request.Profile);

        if (!request.Profile.AspectRatio.IsValid)
        {
            return ViewportCalculationResult.Failure("Aspect ratio must be greater than zero.");
        }

        var workingBounds = request.Monitor.Bounds.Deflate(Math.Max(0, request.Profile.InnerMargin));
        if (workingBounds.IsEmpty)
        {
            return ViewportCalculationResult.Failure("Monitor bounds are too small after applying margins.");
        }

        var targetSize = request.Profile.SizingMode switch
        {
            SizingMode.PixelSize => CalculatePixelSize(request.Profile, workingBounds),
            SizingMode.AspectFit => CalculateAspectFitSize(request.Profile, workingBounds),
            SizingMode.PhysicalDiagonal => CalculatePhysicalSize(request.Profile, request.Calibration, workingBounds),
            _ => ViewportCalculationResult.Failure("Unsupported sizing mode.")
        };

        if (!targetSize.IsSuccess || targetSize.Plan is null)
        {
            return targetSize;
        }

        return targetSize;
    }

    private static ViewportCalculationResult CalculatePixelSize(ViewportProfile profile, PixelRect bounds)
    {
        if (profile.PixelSize.IsEmpty)
        {
            return ViewportCalculationResult.Failure("Pixel width and height must be greater than zero.");
        }

        var size = new PixelSize(
            Math.Min(profile.PixelSize.Width, bounds.Width),
            Math.Min(profile.PixelSize.Height, bounds.Height));

        return CreatePlan(bounds, size);
    }

    private static ViewportCalculationResult CalculateAspectFitSize(ViewportProfile profile, PixelRect bounds)
    {
        var widthRatio = bounds.Width / (double)profile.AspectRatio.Width;
        var heightRatio = bounds.Height / (double)profile.AspectRatio.Height;
        var scale = Math.Min(widthRatio, heightRatio);

        if (scale <= 0)
        {
            return ViewportCalculationResult.Failure("Aspect-fit viewport cannot be calculated for the monitor bounds.");
        }

        var size = new PixelSize(
            Math.Max(1, (int)Math.Round(profile.AspectRatio.Width * scale, MidpointRounding.AwayFromZero)),
            Math.Max(1, (int)Math.Round(profile.AspectRatio.Height * scale, MidpointRounding.AwayFromZero)));

        size = new PixelSize(Math.Min(size.Width, bounds.Width), Math.Min(size.Height, bounds.Height));
        return CreatePlan(bounds, size);
    }

    private static ViewportCalculationResult CalculatePhysicalSize(
        ViewportProfile profile,
        MonitorCalibration? calibration,
        PixelRect bounds)
    {
        if (calibration is null || !calibration.IsValid)
        {
            return ViewportCalculationResult.Failure("A valid monitor calibration is required for physical sizing.");
        }

        if (profile.TargetDiagonalInches is null || profile.TargetDiagonalInches <= 0)
        {
            return ViewportCalculationResult.Failure("Target diagonal inches must be greater than zero.");
        }

        var nativeDiagonalPixels = Math.Sqrt(
            (calibration.NativePixelWidth * calibration.NativePixelWidth) +
            (calibration.NativePixelHeight * calibration.NativePixelHeight));
        var pixelsPerInch = (nativeDiagonalPixels / calibration.DiagonalInches) * calibration.AdjustmentFactor;

        var ratioMagnitude = Math.Sqrt(
            (profile.AspectRatio.Width * profile.AspectRatio.Width) +
            (profile.AspectRatio.Height * profile.AspectRatio.Height));

        var targetWidthInches = profile.TargetDiagonalInches.Value * profile.AspectRatio.Width / ratioMagnitude;
        var targetHeightInches = profile.TargetDiagonalInches.Value * profile.AspectRatio.Height / ratioMagnitude;

        var size = new PixelSize(
            Math.Max(1, (int)Math.Round(targetWidthInches * pixelsPerInch, MidpointRounding.AwayFromZero)),
            Math.Max(1, (int)Math.Round(targetHeightInches * pixelsPerInch, MidpointRounding.AwayFromZero)));

        size = new PixelSize(Math.Min(size.Width, bounds.Width), Math.Min(size.Height, bounds.Height));
        return CreatePlan(bounds, size);
    }

    private static ViewportCalculationResult CreatePlan(PixelRect monitorBounds, PixelSize viewportSize)
    {
        var viewport = monitorBounds.Center(viewportSize).ClampInside(monitorBounds);

        var plan = new ViewportPlan
        {
            Viewport = viewport,
            TopBorder = new PixelRect(monitorBounds.Left, monitorBounds.Top, monitorBounds.Width, viewport.Top - monitorBounds.Top),
            BottomBorder = new PixelRect(monitorBounds.Left, viewport.Bottom, monitorBounds.Width, monitorBounds.Bottom - viewport.Bottom),
            LeftBorder = new PixelRect(monitorBounds.Left, viewport.Top, viewport.Left - monitorBounds.Left, viewport.Height),
            RightBorder = new PixelRect(viewport.Right, viewport.Top, monitorBounds.Right - viewport.Right, viewport.Height)
        };

        return ViewportCalculationResult.Success(plan);
    }
}
