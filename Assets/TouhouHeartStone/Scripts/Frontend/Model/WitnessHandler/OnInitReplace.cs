﻿using IGensoukyo.Utilities;

namespace TouhouHeartstone.Frontend.Model.Witness
{
    public class OnInitReplace : WitnessHandler
    {
        public override string Name => "onInitReplace";

        public override bool HandleWitness(EventWitness witness, DeckController deck, GenericAction callback)
        {
            int player = witness.getVar<int>("playerIndex");
            int[] originalRID = witness.getVar<int[]>("originCardsRID");
            int[] cardsRID = witness.getVar<int[]>("replacedCardsRID");
            int[] cardsDID = witness.getVar<int[]>("replacedCardsDID");

            DebugUtils.NullCheck(cardsRID, "replacedCardsRID");
            DebugUtils.NullCheck(originalRID, "originCardsRID");
            if (cardsDID == null)
                cardsDID = new int[cardsRID.Length];
            
            deck.SetInitReplace(player, CardID.ToCardIDs(originalRID), CardID.ToCardIDs(cardsDID, cardsRID), callback);

            return false;
        }
    }
}
