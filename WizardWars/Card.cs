using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardWars
{
    public class Card
    {
        public string Name;
        public int Cost;
        public string RulesText;
        public string FlavorText;

        public string[] Types;
        public string[] SubTypes;

        public Card()
        {
            Types = new string[0];
            SubTypes = new string[0];
        }
    }
}
