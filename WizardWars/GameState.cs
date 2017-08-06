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
            PlayerOne = new Player();
            PlayerTwo = new Player();

            TurnOrder = new Player[]{ PlayerOne, PlayerTwo };
            turnCounter = 0;
            phaseCounter = 0;
        }

        public void StageCard(Card card)
        {
            StagedCard = card;
            card.Owner.RequestMana(card.Cost);
        }
        public void CostPaid()
        {

        }

        public void Continue()
        {

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
