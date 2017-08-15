using System;
using Minalear;

namespace WizardWars.Core
{
    public class AI 
    {
        private GameState gameState;
        public Player Player { get; private set; }

        public AI(Player player)
        {
            gameState = player.GameState;
            Player = player;

            gameState.ActionResolved += GameState_ActionResolved;
            gameState.PhaseChange += GameState_PhaseChange;
        }

        public void Update(GameTime gameTime)
        {
            //Passes priority if it has priority
            if (gameState.HasPriority(Player))
            {
                gameState.PassPriority();
            }
        }

        private void GameState_ActionResolved(object sender, StateAction action)
        {
            //Disable actions for now
            return;

            if (!(action is PhaseAction)) return;
            if (gameState.CurrentTurn.ID == Player.ID && gameState.CurrentPhase == Phases.Main)
            {
                //Custom AI for test deck
                Player opponent = gameState.GetOpponent(Player);

                //Play every cantrip
                for (int i = 0; i < Player.Hand.Count; i++)
                {
                    Card card = Player.Hand[i];
                    if (gameState.CanCastCard(Player, card) && card.Name == "Cantrip")
                    {
                        Player.Hand.RemoveCardID(card.ID);
                        gameState.AddStateAction(new CardCastAction(card, Player));

                        i--;
                    }
                }

                //Wrath the board if will wrath
                Card wrath = null;
                bool willWrath = (Player.Hand.GetCard("Wrath of God", out wrath) && opponent.Field.CountTypes(Types.Creature) > Player.Field.CountTypes(Types.Creature));
                if (willWrath && gameState.CanCastCard(Player, wrath))
                {
                    Player.Hand.RemoveCardID(wrath.ID);
                    gameState.AddStateAction(new CardCastAction(wrath, Player));
                }

                //Play every creature
                for (int i = 0; i < Player.Hand.Count; i++)
                {
                    Card card = Player.Hand[i];
                    if (card.Meta.IsType(Types.Creature) && gameState.CanCastCard(Player, card))
                    {
                        Player.Hand.RemoveCardID(card.ID);
                        gameState.AddStateAction(new CardCastAction(card, Player));

                        i--;
                    }
                }

                if (gameState.GameStack.Count > 1)
                    gameState.ContinueGame();
                gameState.PassPriority();
            }
        }
        private void GameState_PhaseChange(object sender, Phases phase)
        {
            
        }
    }
}
