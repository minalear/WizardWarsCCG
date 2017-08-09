using System;
using Minalear;

namespace WizardWars
{
    public class CardInfo
    {
        public string Name;
        public string ImagePath;
        public int Cost;

        public string RulesText;
        public string FlavorText;
        public string[] Keywords;

        public int Attack;
        public int Defense;

        public Types[] Types;
        public SubTypes[] SubTypes;

        public Effect[] Effects;

        public Texture2D Art;
        public bool ArtLoaded { get; private set; }

        public CardInfo()
        {
            Types = new Types[0];
            SubTypes = new SubTypes[0];
            Effects = new Effect[0];
        }

        public void LoadCardArt()
        {
            if (!ArtLoaded)
            {
                Art = Texture2D.LoadFromSource(ImagePath);
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

        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, Cost);
        }
    }

    public enum Types
    {
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

        //Roles
        Monk,
        Pirate,
        Rebel
    }
}
