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

        private void GameState_ActionResolved(object sender, StateAction action) { }
        private void GameState_PhaseChange(object sender, Phases phase) { }
    }
}
