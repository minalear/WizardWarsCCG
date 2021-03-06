﻿using System;
using Minalear;
using Microsoft.Scripting.Hosting;

namespace WizardWars
{
    public class CardInfo
    {
        public static Texture2D CardBack;

        public string Name;
        public string ImagePath;
        public int Cost;
        public int ManaValue = 1;

        public string RulesText;
        public string FlavorText;
        public string[] Keywords;

        public int Attack;
        public int Health;

        public Types[] Types;
        public SubTypes[] SubTypes;
        
        public Ability[] Abilities;

        public Texture2D Art;
        public bool ArtLoaded { get; private set; }

        public CardInfo()
        {
            Types = new Types[0];
            SubTypes = new SubTypes[0];
            Abilities = new Ability[0];
        }
        public CardInfo(string name, string image, int cost)
        {
            Name = name;
            ImagePath = image;
            Cost = cost;

            Types = new Types[0];
            SubTypes = new SubTypes[0];
            Abilities = new Ability[0];
        }

        public void LoadCardArt()
        {
            if (!ArtLoaded)
            {
                Art = CardFactory.CreateCardTexture(this);
                ArtLoaded = true;
            }
        }
        public void UnloadCardArt()
        {
            if (ArtLoaded)
            {
                Art.Dispose();
                ArtLoaded = false;
            }
        }

        public bool IsType(Types type)
        {
            foreach (Types cardType in Types)
            {
                if (type == cardType)
                    return true;
            }

            return false;
        }
        public bool IsSubType(SubTypes subType)
        {
            foreach (SubTypes cardSubType in SubTypes)
            {
                if (subType == cardSubType)
                    return true;
            }

            return false;
        }

        public bool IsCreatureType(string literal)
        {
            if (!IsType(WizardWars.Types.Creature)) return false;
            if (literal == "any") return true;
            if (literal == "nonhero") return !IsType(WizardWars.Types.Hero);

            return IsSubType((SubTypes)Enum.Parse(typeof(SubTypes), literal, true));
        }
        public bool HasKeyword(string literal)
        {
            foreach (string keyword in Keywords)
            {
                if (literal == keyword)
                    return true;
            }

            return false;
        }

        public Card CreateInstance(Player owner)
        {
            return new Card(this, owner);
        }

        public void SetKeywords(params string[] keywords)
        {
            Keywords = keywords;
        }
        public void SetTypes(params Types[] types)
        {
            Types = types;
        }
        public void SetSubTypes(params SubTypes[] subTypes)
        {
            SubTypes = subTypes;
        }
        public void SetAbilities(params Ability[] abilities)
        {
            Abilities = abilities;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, Cost);
        }

        public const string HASTE = "Shock";
        public const string FLASH = "Interrupt";
        public const string DEFENDER = "Passive";
    }

    public enum Types
    {
        Player,
        Spell,
        Creature,
        Hero,
        Relic,
        Temple
    }
    public enum SubTypes
    {
        Interrupt,

        //Races
        Human,
        Grunkan,
        Spirit,
        Dragon,

        //Roles
        Monk,
        Pirate,
        Rebel,
        Soldier
    }
}
