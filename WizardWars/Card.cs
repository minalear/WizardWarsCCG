using System;

namespace WizardWars
{
    public class Card : IEquatable<Card>
    {
        private static int _nextValidID = 0;

        public int ID { get; private set; }
        public CardInfo Meta { get; private set; }

        public int Attack, Defense;
        public Player Owner, Controller;

        public Card(CardInfo card)
        {
            ID = _nextValidID++;
            Meta = card;
            Attack = card.Attack;
            Defense = card.Defense;
        }

        public bool Equals(Card other)
        {
            return (ID == other.ID);
        }
    }
}
