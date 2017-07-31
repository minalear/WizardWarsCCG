﻿using Minalear;

namespace WizardWars
{
    public class CardInfo
    {
        public string Name;
        public string ImagePath;
        public int Cost;

        public string RulesText;
        public string FlavorText;

        public int Attack;
        public int Defense;

        public Types[] Types;
        public SubTypes[] SubTypes;

        public Effect[] Effects;

        public Texture2D Art;

        public CardInfo()
        {
            Types = new Types[0];
            SubTypes = new SubTypes[0];
            Effects = new Effect[0];
        }

        public void LoadCardArt()
        {
            Art = Texture2D.LoadFromSource(ImagePath);
        }
        public void UnloadCardArt()
        {
            Art.Dispose();
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

        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, Cost);
        }
    }

    public enum Types
    {
        Spell,
        Creature,
        Hero
    }
    public enum SubTypes
    {
        //Races
        Human,
        Grunkan,

        //Roles
        Monk,
        Pirate
    }
}