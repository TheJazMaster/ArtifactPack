using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace TheJazMaster.ArtifactPack.Artifacts;

internal sealed class LifePreserver : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper, IPluginPackage<IModManifest> package)
	{
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, [ArtifactPool.Common], helper, package, out _);
	}

	public override void OnReceiveArtifact(State state)
	{
		state.GetCurrentQueue().QueueImmediate(new ACardSelect
		{
			browseAction = new CardSelectAddBuoyantForever(),
			browseSource = CardBrowse.Source.Deck,
			filterBuoyant = false,
			filterTemporary = false
		});
	}

    public override List<Tooltip>? GetExtraTooltips() => [
        new TTGlossary("cardtrait.buoyant")
    ];
}