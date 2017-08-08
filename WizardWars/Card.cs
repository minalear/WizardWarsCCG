using System;
using Minalear;

namespace WizardWars
{
    public class Card : IEquatable<Card>
    {
        private static int _nextValidID = 0;

        public int ID { get; private set; }

        public string Name { get { return Meta.Name; } }
        public int Cost { get { return Meta.Cost; } }
        public Texture2D Art { get { return Meta.Art; } }
        public CardInfo Meta { get; private set; }
        public bool Tapped { get; set; }
        public bool Highlighted { get; set; }

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

        public void Damage(int damage)
        {
            Defense -= damage;
        }
        public void Destroy()
        {
            Defense = 0;
        }
        public bool IsDestroyed()
        {
            return (Defense <= 0);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Meta.Name, ID);
        }
    }
}
