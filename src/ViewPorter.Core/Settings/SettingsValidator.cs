using ViewPorter.Core.Profiles;

namespace ViewPorter.Core.Settings;

public sealed class SettingsValidator
{
    public SettingsDocument Normalize(SettingsDocument? document)
    {
        var source = document ?? new SettingsDocument();

        var profiles = source.Profiles
            .Where(IsValidProfile)
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
                    InnerMargin = 0
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
}
