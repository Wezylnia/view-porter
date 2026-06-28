using ViewPorter.App.Settings;
using ViewPorter.Core.Settings;

namespace ViewPorter.App.Profiles;

public sealed class ProfileStore
{
    private readonly JsonSettingsStore _settingsStore;

    public ProfileStore(JsonSettingsStore settingsStore)
    {
        _settingsStore = settingsStore;
    }

    public Task<SettingsDocument> LoadAsync(CancellationToken cancellationToken = default) =>
        _settingsStore.LoadAsync(cancellationToken);

    public Task SaveAsync(SettingsDocument document, CancellationToken cancellationToken = default) =>
        _settingsStore.SaveAsync(document, cancellationToken);
}
