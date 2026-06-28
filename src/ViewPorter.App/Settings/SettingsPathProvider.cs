using System.IO;

namespace ViewPorter.App.Settings;

public static class SettingsPathProvider
{
    public static string GetSettingsPath()
    {
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ViewPorter");

        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "settings.json");
    }
}
