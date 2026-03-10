using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FSPRO;
using Nanoray.PluginManager;
using Nickel;
using TheJazMaster.ArtifactPack.Actions;

namespace TheJazMaster.ArtifactPack.Artifacts;

internal sealed class TormentNexus : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper, IPluginPackage<IModManifest> package)
	{
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, [ArtifactPool.Boss], helper, package, out _, unremovable: true);
	}

	public override void OnReceiveArtifact(State state)
	{
        List<int> uuids = TransformPile(state, state.deck);
        if (state.route is Combat c) {
            uuids.AddRange(TransformPile(state, c.hand));
            uuids.AddRange(TransformPile(state, c.discard));
            uuids.AddRange(TransformPile(state, c.exhausted));
        }
        if (uuids.Count > 0)
            state.GetCurrentQueue().QueueImmediate(new AShowCards
            {
                uuids = uuids
            });
    }

    public static List<int> TransformPile(State state, List<Card> pile) {
        List<int> ret = [];
        for (int i = pile.Count - 1; i >= 0; i--)
        {
            Card card = pile[i];
            CardMeta meta = card.GetMeta();
            if (meta.deck == Deck.colorless && meta.dontOffer)
            {
                pile.RemoveAt(i);
                Card? reward = CardReward.GetOffering(state, 1, state.characters.Random(state.rngCardOfferingsMidcombat).deckType, BattleType.Normal, overrideUpgradeChances: true).FirstOrDefault();
                if (reward != null) {
                    state.SendCardToDeck(reward);
                    ret.Add(reward.uuid);
                    Audio.Play(Event.CardHandling);
                }
            }
        }
        return ret;
    }
}