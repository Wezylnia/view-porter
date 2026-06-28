using ViewPorter.Core.Profiles;
using ViewPorter.Core.Settings;

namespace ViewPorter.Core.Tests;

public sealed class SettingsValidatorTests
{
    private readonly SettingsValidator _validator = new();

    [Fact]
    public void Normalize_CreatesDefaultProfile_WhenInputIsMissing()
    {
        var document = _validator.Normalize(null);

        Assert.Equal(SettingsDocument.CurrentSchemaVersion, document.SchemaVersion);
        Assert.Single(document.Profiles);
        Assert.Equal(document.Profiles[0].Id, document.SelectedProfileId);
    }

    [Fact]
    public void Normalize_DropsInvalidProfiles()
    {
        var document = _validator.Normalize(
            new SettingsDocument
            {
                Profiles =
                [
                    new ViewportProfile
                    {
                        Id = string.Empty,
                        Name = "Broken",
                        AspectRatio = AspectRatio.Ratio16By9
                    },
                    new ViewportProfile
                    {
                        Id = "valid",
                        Name = "Valid",
                        AspectRatio = AspectRatio.Ratio16By9
                    }
                ],
                SelectedProfileId = "valid"
            });

        Assert.Single(document.Profiles);
        Assert.Equal("valid", document.SelectedProfileId);
    }
}
