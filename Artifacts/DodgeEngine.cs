using System;
using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace TheJazMaster.ArtifactPack.Artifacts;

internal sealed class DodgeEngine : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper, IPluginPackage<IModManifest> package)
	{
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, [ArtifactPool.Common], helper, package, out _);
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		if (combat.turn == 4)
		{
			combat.QueueImmediate(new AStatus
			{
				status = Status.evade,
				statusAmount = 3,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

    public override int? GetDisplayNumber(State s)
    {
        if (s.route is Combat c) {
            return (c.turn > 4) ? null : c.turn;
        }
        return null;
    }

	public override List<Tooltip>? GetExtraTooltips() => StatusMeta.GetTooltips(Status.evade, 3);
}