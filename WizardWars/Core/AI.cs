using System;

namespace WizardWars.Core
{
    public class AI 
    {
        public Player Player { get; private set; }

        public AI(Player player)
        {
            Player = player;
        }

        public void Update()
        {
            //Passes priority if it has priority
            if (Player.GameState.HasPriority(Player))
            {
                Console.WriteLine("Player #{0}: Passing on Action ({1})", Player.ID + 1, Player.GameState.CurrentAction);
                Player.GameState.PassPriority();
            }
        }
    }
}
