using System;

namespace WizardWars
{
    public class Player
    {
        public int Health;
        public Collection Hand;
        public Collection Deck;
        public Collection Graveyard;
        public Collection Exile;
        public Collection ManaPool;
        public Collection Field;

        public Player()
        {
            Health = 20;

            Hand = new Collection(this);
            Deck = new Collection(this);
            Graveyard = new Collection(this);
            Exile = new Collection(this);
            ManaPool = new Collection(this);
            Field = new Collection(this);
        }

        public void DrawCards(int num)
        {
            Hand.AddCards(Deck.RemoveCards(num, Location.Top), Location.Bottom);
        }
    }
}
