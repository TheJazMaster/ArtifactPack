using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using TheJazMaster.ArtifactPack.Actions;

namespace TheJazMaster.ArtifactPack.Artifacts;

[HarmonyPatch]
internal sealed class BrainDrain : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper, IPluginPackage<IModManifest> package)
	{
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, [ArtifactPool.Boss], helper, package, out _, unremovable: true);
	}

	public override void OnReceiveArtifact(State state)
	{
        state.ship.baseDraw += 2;
    }

    public override void OnRemoveArtifact(State state)
    {
        state.ship.baseDraw -= 2;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AEndTurn), nameof(AEndTurn.Begin))]
    public static void AEndTurn_Begin_Postfix(G g, State s, Combat c, AEndTurn __instance)
    {
        var artifact = s.EnumerateAllArtifacts().OfType<BrainDrain>().FirstOrDefault();
        if (artifact == null) return;

        c.QueueImmediate(new AExhaustCheapest
        {
            artifactPulse = artifact.Key()
        });
    }
}