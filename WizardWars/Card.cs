using System;
using System.Collections.Generic;
using Minalear;

namespace WizardWars
{
    public class Card : IEquatable<Card>
    {
        private static int _nextValidID = 0;

        public int ID { get; private set; }

        public Texture2D Art { get { return MetaInfo.Art; } }

        public string Name;
        public int BaseAttack, BaseHealth;
        public int BonusAttack, BonusHealth;
        public int CurrentAttack { get { return BaseAttack + BonusAttack; } }
        public int CurrentHealth { get { return BaseHealth + BonusHealth; } }
        public int DamageAmount;

        public int Cost;
        public int ManaValue = 1;

        public Player Owner, Controller;
        public List<Ability> Abilities;

        public List<Counter> Counters;

        public bool IsPermanent;
        public bool IsTapped;
        public bool IsFaceDown;
        public bool IsSummoningSick;
        public bool IsManaDrained;

        public bool IsValidTarget;

        public List<string> Keywords;

        public CardInfo MetaInfo;

        public Collection Zone;
        
        public Card(CardInfo card, Player owner)
        {
            ID = _nextValidID++;
            MetaInfo = card;

            Owner = owner;
            Controller = owner;

            Cost = MetaInfo.Cost;

            Abilities = new List<Ability>(card.Abilities);
            Counters = new List<Counter>();

            IsPermanent = (IsType(Types.Creature) || IsType(Types.Relic));
            UpdateStats();
        }
        
        public void UpdateStats()
        {
            Name = MetaInfo.Name;

            BaseAttack = MetaInfo.Attack;
            BaseHealth = MetaInfo.Health;
            BonusAttack = 0;
            BonusHealth = 0;
        }
        public void Destroy()
        {
            //Replace this with a more direct method
            DamageAmount = CurrentHealth;
        }

        public void Damage(Card source, int number)
        {
            DamageAmount += number;
        }
        public void Heal(Card source, int number) { }

        public bool IsType(Types type)
        {
            return MetaInfo.IsType(type);
        }
        public bool IsSubType(SubTypes subType)
        {
            return MetaInfo.IsSubType(subType);
        }
        public bool IsDestroyed()
        {
            return DamageAmount >= CurrentHealth;
        }

        public bool IsTriggered(Triggers trigger, Card source, out Ability triggeredAbility)
        {
            foreach (Ability ability in Abilities)
            {
                if (ability.Type == AbilityTypes.Triggered && ability.Trigger == trigger)
                {
                    if (ability.IsValidCard(this, source))
                    {
                        triggeredAbility = ability;
                        return true;
                    }
                }
            }

            triggeredAbility = null;
            return false;
        }

        public bool Equals(Card other)
        {
            return (ID == other.ID);
        }
        public override string ToString()
        {
            if (IsType(Types.Creature))
                return string.Format("{0} ({1}) ({2}/{3})", Name, ID, CurrentAttack, CurrentHealth);
            return string.Format("{0} ({1})", Name, ID);
        }
    }
}
