using System.Collections.Generic;
using System.Linq;

namespace TheJazMaster.ArtifactPack.Actions;

class AExhaustCheapest : CardAction {

    public override void Begin(G g, State s, Combat c)
    {
        var viable = (from card in c.hand group card by card.GetDataWithOverrides(s).cost into cardGroup orderby cardGroup.Key ascending select cardGroup.ToList()).ToList();
        if (viable.Count > 0) {
            Card card = viable.Count == 1 ? viable[0][0] : viable[0].Random(s.rngActions);
            card.shakeNoAnim = 1;
            c.QueueImmediate(new AExhaustOtherCard {
                uuid = card.uuid
            });
        }
    }
}