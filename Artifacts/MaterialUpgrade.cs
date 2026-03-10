using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace TheJazMaster.ArtifactPack.Artifacts;

internal sealed class MaterialUpgrade : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper, IPluginPackage<IModManifest> package)
	{
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, [ArtifactPool.Common], helper, package, out _);
	}

	public override void OnReceiveArtifact(State state)
	{
		state.ship.shieldMaxBase += 1;
		state.ship.hullMax += 2;
	}
}