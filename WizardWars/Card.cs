﻿using System;
using System.Collections.Generic;
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

        public bool IsTapped { get; set; }
        public bool IsFaceDown { get; set; }
        public bool Attacking { get; set; }
        public bool IsBlocked { get; set; }
        public Card BlockerRef { get; set; }
        public bool IsSummoningSick { get; set; }

        public bool Blocking { get; set; }
        public bool IsManaDrained { get; set; }

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
        public bool IsTriggered(Card source, string trigger, out List<Effect> effects)
        {
            string ownerArg = (source.Controller.ID == Controller.ID) ? "controlled" : "opponent";

            bool triggered = false;
            effects = new List<Effect>();

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
                            effects.Add(cardEffect);
                            triggered = true;
                        }
                        else if (tokens.Length != 1 && (tokens[1] == ownerArg || tokens[1] == "any"))
                        {
                            effects.Add(cardEffect);
                            triggered = true;
                        }
                    }
                }
            }
            
            return triggered;
        }
        public bool CanAttack()
        {
            if (!Meta.IsType(Types.Creature)) return false;
            if (IsSummoningSick || !Meta.HasKeyword(CardInfo.HASTE)) return false;
            if (IsTapped) return false;
            if (Meta.HasKeyword(CardInfo.DEFENDER)) return false;

            return true;
        }

        public void EnteredBattlefield()
        {
            if (Meta.IsType(Types.Creature))
            {
                if (!Meta.HasKeyword(CardInfo.HASTE))
                    IsSummoningSick = true;
            }
        }

        public int GetManaValue()
        {
            //Facedown cards are always worth 1 mana
            return (IsFaceDown) ? 1 : Meta.ManaValue;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Meta.Name, ID);
        }
    }
}
