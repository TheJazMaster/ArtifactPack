using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace TheJazMaster.ArtifactPack.Artifacts;

internal sealed class FlashDrive : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper, IPluginPackage<IModManifest> package)
	{
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, [ArtifactPool.Common], helper, package, out _);
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		if (combat.turn == 1)
		{
			combat.QueueImmediate(new ADrawCard
			{
				count = 2,
				artifactPulse = Key()
			});
		}
	}
}