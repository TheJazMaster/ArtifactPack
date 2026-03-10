using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheJazMaster.ArtifactPack.Artifacts;

namespace TheJazMaster.ArtifactPack;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null!;

    internal Harmony Harmony { get; }

	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

	internal IPartEntry MegaThrustersWing { get; }

    internal List<IModSettingsApi.IModSetting> SettingsEntries = [];
	internal ModSettings ModSettings = new();


    internal static IReadOnlyList<Type> ArtifactTypes { get; } = [
		typeof(LifePreserver),
		typeof(FlashDrive),
		typeof(MaterialUpgrade),
		typeof(DodgeEngine),
		typeof(CardRefund),
		typeof(Incinerator),
		typeof(MegaThrusters),
		typeof(BrainDrain),
		typeof(TormentNexus),
	];
    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);
		ModSettings = helper.Storage.LoadJson<ModSettings>(helper.Storage.GetMainStorageFile("json"));

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"I18n/en.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);

        MegaThrustersWing = helper.Content.Ships.RegisterPart("wing_mega", new() {
            Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Parts/wing_mega.png")).Sprite
        });

        foreach (Type type in ArtifactTypes) {
			AccessTools.DeclaredMethod(type, nameof(IRegisterableArtifact.Register))?.Invoke(null, [helper, package]);
		}

		SetUpModSettings(helper);
        Harmony.PatchAll();
    }

	private void SetUpModSettings(IModHelper helper) {
		if (helper.ModRegistry.GetApi<IModSettingsApi>("Nickel.ModSettings") is { } settingsApi) {
			settingsApi.RegisterModSettings(settingsApi.MakeList(SettingsEntries)
				.SubscribeToOnMenuClose(_ => {
                    helper.Storage.SaveJson(helper.Storage.GetMainStorageFile("json"), ModSettings);
                }));
		}
	}
}

class ModSettings {
	public readonly Dictionary<string, bool> artifactsDisabled = [];
}