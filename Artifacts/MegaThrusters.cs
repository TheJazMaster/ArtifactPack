using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace TheJazMaster.ArtifactPack.Artifacts;

internal sealed class MegaThrusters : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper, IPluginPackage<IModManifest> package)
	{
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, [ArtifactPool.Boss], helper, package, out _, unremovable: true);
	}

	public override void OnReceiveArtifact(State state)
	{
		Part part = new()
        {
			type = PType.wing,
			skin = ModEntry.Instance.MegaThrustersWing.UniqueName
		};

		state.GetCurrentQueue().QueueImmediate(new AShipUpgrades {
			actions = [
				new AInsertPart {
					targetPlayer = true,
					x = 0,
					part = part
				}
			]
		});
	}
    public override void OnCombatStart(State state, Combat combat)
    {
        combat.Queue(new AStatus
        {
            status = Status.ace,
            statusAmount = 1,
            targetPlayer = true,
            artifactPulse = Key()
        });
    }

    public override List<Tooltip>? GetExtraTooltips() => [
        .. StatusMeta.GetTooltips(Status.ace, 1),
		.. StatusMeta.GetTooltips(Status.evade, 1)
    ];
}