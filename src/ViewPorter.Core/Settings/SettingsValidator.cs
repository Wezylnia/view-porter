using ViewPorter.Core.Profiles;

namespace ViewPorter.Core.Settings;

public sealed class SettingsValidator
{
    private const int DefaultViewportMargin = 160;

    public SettingsDocument Normalize(SettingsDocument? document)
    {
        var source = document ?? new SettingsDocument();

        var profiles = source.Profiles
            .Where(IsValidProfile)
            .Select(NormalizeProfile)
            .ToArray();

        if (profiles.Length == 0)
        {
            profiles =
            [
                new ViewportProfile
                {
                    Id = "default-16x9",
                    Name = "Centered 16:9",
                    SizingMode = SizingMode.AspectFit,
                    AspectRatio = AspectRatio.Ratio16By9,
                    PixelSize = new Geometry.PixelSize(1920, 1080),
                    InnerMargin = DefaultViewportMargin
                }
            ];
        }

        var selectedProfileId = profiles.Any(profile => profile.Id == source.SelectedProfileId)
            ? source.SelectedProfileId
            : profiles[0].Id;

        return new SettingsDocument
        {
            SchemaVersion = SettingsDocument.CurrentSchemaVersion,
            SelectedProfileId = selectedProfileId,
            AutomationEnabled = source.AutomationEnabled,
            Profiles = profiles,
            MonitorCalibrations = source.MonitorCalibrations.Where(calibration => calibration.IsValid).ToArray()
        };
    }

    private static bool IsValidProfile(ViewportProfile profile) =>
        !string.IsNullOrWhiteSpace(profile.Id) &&
        !string.IsNullOrWhiteSpace(profile.Name) &&
        profile.AspectRatio.IsValid &&
        profile.InnerMargin >= 0;

    private static ViewportProfile NormalizeProfile(ViewportProfile profile)
    {
        if (profile.Id == "default-16x9" &&
            profile.SizingMode == SizingMode.AspectFit &&
            profile.AspectRatio.Equals(AspectRatio.Ratio16By9) &&
            profile.InnerMargin == 0)
        {
            return new ViewportProfile
            {
                Id = profile.Id,
                Name = profile.Name,
                MonitorSelection = profile.MonitorSelection,
                SizingMode = profile.SizingMode,
                AspectRatio = profile.AspectRatio,
                PixelSize = profile.PixelSize,
                TargetDiagonalInches = profile.TargetDiagonalInches,
                BorderColorArgb = profile.BorderColorArgb,
                MoveForegroundWindow = profile.MoveForegroundWindow,
                InnerMargin = DefaultViewportMargin
            };
        }

        return profile;
    }
}
