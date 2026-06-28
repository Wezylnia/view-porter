using System.IO;
using System.Text.Json;
using ViewPorter.Core.Settings;

namespace ViewPorter.App.Settings;

public sealed class JsonSettingsStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly SettingsValidator _validator;

    public JsonSettingsStore(SettingsValidator validator)
    {
        _validator = validator;
    }

    public async Task<SettingsDocument> LoadAsync(CancellationToken cancellationToken = default)
    {
        var path = SettingsPathProvider.GetSettingsPath();
        if (!File.Exists(path))
        {
            var defaults = _validator.Normalize(null);
            await SaveAsync(defaults, cancellationToken);
            return defaults;
        }

        try
        {
            await using var stream = File.OpenRead(path);
            var loaded = await JsonSerializer.DeserializeAsync<SettingsDocument>(stream, SerializerOptions, cancellationToken);
            return _validator.Normalize(loaded);
        }
        catch (JsonException)
        {
            QuarantineMalformedSettings(path);
            var defaults = _validator.Normalize(null);
            await SaveAsync(defaults, cancellationToken);
            return defaults;
        }
    }

    public async Task SaveAsync(SettingsDocument document, CancellationToken cancellationToken = default)
    {
        var normalized = _validator.Normalize(document);
        var path = SettingsPathProvider.GetSettingsPath();
        var tempPath = $"{path}.tmp";

        await using (var stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, normalized, SerializerOptions, cancellationToken);
        }

        File.Move(tempPath, path, overwrite: true);
    }

    private static void QuarantineMalformedSettings(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        var quarantinePath = $"{path}.corrupt-{DateTime.UtcNow:yyyyMMddHHmmss}";
        File.Move(path, quarantinePath, overwrite: true);
    }
}
