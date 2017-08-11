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
        public bool Highlighted { get; set; }

        public bool Tapped { get; set; }
        public bool Attacking { get; set; }
        public bool IsBlocked { get; set; }
        public Card BlockerRef { get; set; }

        public bool Blocking { get; set; }

        public Collection Zone { get; set; }

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

        public void Damage(int amount)
        {
            Defense -= amount;
        }
        public void Heal(int amount)
        {
            //Can't heal more than max defense
            Defense = OpenTK.MathHelper.Clamp(Defense + amount, Defense + amount, Meta.Defense);
        }
        public void Destroy()
        {
            Defense = 0;
        }
        public bool IsDestroyed()
        {
            return (Defense <= 0);
        }
        public bool IsTriggered(Card source, string trigger, out Effect effect)
        {
            string ownerArg = (source.Controller.ID == Controller.ID) ? "controlled" : "opponent";

            foreach (Effect cardEffect in Meta.Effects)
            {
                foreach (string effectTrigger in cardEffect.Triggers)
                {
                    string[] tokens = effectTrigger.Split('.');
                    if (tokens[0] == trigger)
                    {
                        //If the source is also this
                        if ((tokens.Length == 1 || tokens[1] == "self") && ID == source.ID)
                        {
                            effect = cardEffect;
                            return true;
                        }
                        else if (tokens.Length != 1 && (tokens[1] == ownerArg || tokens[1] == "any"))
                        {
                            effect = cardEffect;
                            return true;
                        }
                    }
                }
            }

            effect = null;
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Meta.Name, ID);
        }
    }
}
