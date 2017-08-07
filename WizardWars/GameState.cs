using System;
using System.Collections.Generic;

namespace WizardWars
{
    public sealed class GameState
    {
        public Player PlayerOne, PlayerTwo;

        public Player PlayerPriority;
        public Player PlayerTurn;
        public Phases CurrentPhase { get { return GamePhaseSequence[phaseCounter]; } }

        public Card StagedCard;

        public GameState()
        {
            PlayerOne = new Player(this);
            PlayerTwo = new Player(this);

            TurnOrder = new Player[]{ PlayerOne, PlayerTwo };
            turnCounter = 0;
            phaseCounter = 0;
        }

        public bool CanCastCard(Player caster, Card card)
        {
            if (caster.HasPriority)
            {
                if (card.Meta.IsSubType(SubTypes.Interrupt))
                    return true;
                if (CurrentPhase == Phases.Main)
                    return true;
            }

            return false;
        }
        public void StageCard(Card card)
        {
            StagedCard = card;
            card.Owner.RequestCastingCost(card, card.Cost);
        }
        private int phaseCounter = 0;
        private int turnCounter = 0;

        private Phases[] GamePhaseSequence = {
            Phases.Upkeep, Phases.Draw, Phases.Main, Phases.Battle, Phases.Main, Phases.Cleanup
        };
        private Player[] TurnOrder;
    }

    public enum Phases { Upkeep, Draw, Main, Battle, Cleanup }
}
