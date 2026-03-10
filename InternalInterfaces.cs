using System;
using System.Collections.Generic;
using Nanoray.PluginManager;
using Nickel;
using TheJazMaster.ArtifactPack;
using static TheJazMaster.ArtifactPack.IModSettingsApi;

internal interface IRegisterableCard
{
    static abstract void Register(IModHelper helper);
}

internal interface IRegisterableArtifact
{
    static ModSettings ModSettings => ModEntry.Instance.ModSettings;
    static List<IModSetting> SettingsEntries => ModEntry.Instance.SettingsEntries;
	
	static IArtifactEntry Register(Type type, ArtifactPool[] pools, IModHelper helper, IPluginPackage<IModManifest> package, out string name, Deck? deck = null, bool unremovable = false) {
		name = type.Name;
		var entry = helper.Content.Artifacts.RegisterArtifact(name, new()
		{
			ArtifactType = type,
			Meta = new()
			{
				owner = deck ?? Deck.colorless,
				pools = pools,
				unremovable = unremovable
			},
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/{name}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", name, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", name, "description"]).Localize
		});
		MakeSetting(helper, entry);
        entry.Amend(new()
        {
            CanBeOffered = new() {
                Value = (state) =>
                {
                    return !ModEntry.Instance.ModSettings.artifactsDisabled.GetValueOrDefault(entry.UniqueName);
                }
            }
        });
        return entry;
	}
	static IArtifactEntry Register(Type type, ArtifactPool[] pools, IModHelper helper, IPluginPackage<IModManifest> package, out string name, out Spr activeSpr, out Spr inactiveSpr, Deck? deck = null, bool unremovable = false) {
		name = type.Name;
		activeSpr = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/{name}.png")).Sprite;
		inactiveSpr = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/{name}Disabled.png")).Sprite;
		var entry = helper.Content.Artifacts.RegisterArtifact(name, new()
		{
			ArtifactType = type,
			Meta = new()
			{
				owner = deck ?? Deck.colorless,
				pools = pools,
				unremovable = unremovable
			},
			Sprite = activeSpr,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", name, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", name, "description"]).Localize,
		});
        MakeSetting(helper, entry);
        entry.Amend(new()
        {
            CanBeOffered = new() {
                Value = (state) =>
                {
                    return !ModEntry.Instance.ModSettings.artifactsDisabled.GetValueOrDefault(entry.UniqueName);
                }
            }
        });
        return entry;
    }
	static abstract void Register(IModHelper helper, IPluginPackage<IModManifest> package);

    public static void MakeSetting(IModHelper helper, IArtifactEntry entry) {
		if (helper.ModRegistry.GetApi<IModSettingsApi>("Nickel.ModSettings") is { } settingsApi) {
			SettingsEntries.Add(settingsApi.MakeCheckbox(
				() => entry.Configuration.Name?.Invoke(DB.currentLocale.locale) ?? "???",
				() => !ModSettings.artifactsDisabled.GetValueOrDefault(entry.UniqueName),
                (_, _, to) => ModSettings.artifactsDisabled[entry.UniqueName] = !to));
		}
    }
}

internal interface IPostDBInitHook
{
    void PostDBInit();
}