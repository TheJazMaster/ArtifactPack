using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace TheJazMaster.ArtifactPack.Artifacts;

internal sealed class Incinerator : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper, IPluginPackage<IModManifest> package)
	{
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, [ArtifactPool.Common], helper, package, out _);
	}

	public override void OnReceiveArtifact(State state)
	{
		state.GetCurrentQueue().QueueImmediate(new AAddCard
		{
			card = new TrashFumes {
				temporaryOverride = false
			},
			amount = 1
		});
		state.GetCurrentQueue().QueueImmediate(new ARemoveCard {
			allowCancel = true
		});
		state.GetCurrentQueue().QueueImmediate(new ARemoveCard {
			allowCancel = true
		});
	}

    public override List<Tooltip>? GetExtraTooltips() => [
		new TTCard {
			card = new TrashFumes {
				temporaryOverride = false
			}
		}
	];
}