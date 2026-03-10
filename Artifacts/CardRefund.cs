using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace TheJazMaster.ArtifactPack.Artifacts;

[HarmonyPatch]
internal sealed class CardRefund : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper, IPluginPackage<IModManifest> package)
	{
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, [ArtifactPool.Common], helper, package, out _);
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Combat), nameof(Combat.SendCardToExhaust))]
	private static void Combat_SendCardToExhaust_Postfix(State s, Combat __instance, Card card) {
		foreach (CardRefund cr in s.EnumerateAllArtifacts().OfType<CardRefund>()) {
            __instance.Queue(new AStatus
            {
                status = Status.drawNextTurn,
                statusAmount = 1,
                targetPlayer = true,
                artifactPulse = cr.Key()
            });
        }
	}
}