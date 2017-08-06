using System;

namespace WizardWars
{
    public class Player
    {
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
        public bool HasPriority = false;

        public Player()
        {
            Health = 20;
            Mana = 100;

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
                Hand.AddCards(Deck.RemoveCards(num, Location.Top), Location.Bottom);
            }
        }
        public virtual void PromptPlayer(string prompt) { }
        public virtual void RequestCastingCost(Card card, int cost)
        {
            PromptPayCastingCost?.Invoke(this, new CastingCostEventArgs(card, cost));
        }

        public virtual void PromptResponseToCast(Card card)
        {

        }


        public event PromptPayCastingCostEvent PromptPayCastingCost;
        public delegate void PromptPayCastingCostEvent(object sender, CastingCostEventArgs e);
    }

    public class CastingCostEventArgs : EventArgs
    {
        public CastingCostEventArgs(Card card, int cost)
        {
            Card = card;
            Cost = cost;
        }

        public Card Card { get; private set; }
        public int Cost { get; private set; }
    }
}
