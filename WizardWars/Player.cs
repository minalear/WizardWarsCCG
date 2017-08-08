using System;

namespace WizardWars
{
    public class Player
    {
        private static int _nextValidID = 0;

        public int ID { get; private set; }
        public GameState GameState;

        public int Health;
        public int Mana;

        public Collection AllCards;

        public Collection Hand;
        public Collection Deck;
        public Collection Graveyard;
        public Collection Exile;
        public Collection Elysium;
        public Collection Field;

        public bool CanDrawCards = true;

        public Player(GameState gameState)
        {
            ID = _nextValidID++;
            GameState = gameState;

            Health = 20;
            Mana = 0;

            AllCards = new Collection(this);

            Hand = new Collection(this);
            Deck = new Collection(this);
            Graveyard = new Collection(this);
            Exile = new Collection(this);
            Elysium = new Collection(this);
            Field = new Collection(this);
        }

        public void DrawCards(int num)
        {
            if (CanDrawCards)
            {
                if (Deck.Count > 0)
                    Hand.AddCards(Deck.RemoveCards(num, Location.Top), Location.Bottom);
            }
        }

        public virtual void PromptPlayerStateAction(StateAction action)
        {
            //PRETEND THIS IS AI PASSING
            if (ID != 1) //PlayerTwo ID
            {
                Console.WriteLine("Current Stack: {0}", action);
            }
        }
        public virtual void PromptPlayerTargetRequired(EffectAction action)
        {
            if (ID != 1)
            {
                Console.WriteLine("Select target for {0}", action);
            }
        }

        public override string ToString()
        {
            return string.Format("Player #{0}", ID + 1);
        }
    }
}
