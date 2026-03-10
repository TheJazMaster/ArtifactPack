using System.Collections.Generic;

namespace TheJazMaster.ArtifactPack.Actions;

class AShowCards : CardAction {

    required public List<int> uuids;
    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        return new CustomShowCards {
            cardIds = uuids,
            message = ModEntry.Instance.Localizations.Localize(["artifact", "TormentNexus", "showCards"])
        };
    }
}