using Minalear;

namespace WizardWars
{
    public class Card
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

        public Player Owner, Controller;

        public Texture2D Art;

        public Card()
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
        //Races
        Human,
        Grunkan,

        //Roles
        Monk,
        Pirate
    }
}
