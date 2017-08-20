﻿using System;
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
        public int CurrentAttack, CurrentHealth;
        public int Damage;

        public int Cost;

        public Player Owner, Controller;
        public List<Effect> Effects;

        public List<Counter> Counters;

        public bool IsPermanent;
        public bool IsTapped;
        public bool IsFaceDown;
        public bool IsSummoningSick;

        public List<string> Keywords;

        public CardInfo MetaInfo;

        public Collection Zone;


        public Card(CardInfo card, Player owner)
        {
            ID = _nextValidID++;
            MetaInfo = card;

            Owner = owner;
            Controller = owner;

            Effects = new List<Effect>();
            Counters = new List<Counter>();

            IsPermanent = (IsOfType(Types.Creature) || IsOfType(Types.Relic));
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

        public void DealDamage(Card source, int number)
        {

        }

        public bool IsOfType(Types type)
        {
            return MetaInfo.IsType(type);
        }
        public bool IsOfSubType(SubTypes subType)
        {
            return MetaInfo.IsSubType(subType);
        }

        public void ApplyStaticEffects(List<Effect> effects)
        {

        }


        public bool Equals(Card other)
        {
            return (ID == other.ID);
        }
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, ID);
        }
    }
}
