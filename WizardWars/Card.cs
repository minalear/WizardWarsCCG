using System;

namespace WizardWars
{
    public class Card
    {
        public string Name;
        public int Cost;

        public string RulesText;
        public string FlavorText;

        public int Attack;
        public int Defense;

        public Types[] Types;
        public SubTypes[] SubTypes;

        public Effect[] Effects;

        public Player Owner, Controller;

        public Card()
        {
            Types = new Types[0];
            SubTypes = new SubTypes[0];
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, Cost);
        }
    }

    public enum Types
    {
        Spell,
        Creature
    }
    public enum SubTypes
    {
        Human,
        Monk,
        Grunkan
    }
}
