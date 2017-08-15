using System;
using Newtonsoft.Json;

namespace WizardWars
{
    public class Effect
    {
        public string[] Triggers;
        public string Prompt;
        public object[] Vars;
        public string[] ValidTargets;
        public string[] Affects;
        public object Cost;
        public string[] Actions;
        public object NumTargets = 1;

        [JsonIgnore()]
        public CardInfo Card;

        public bool HasTrigger(string trigger)
        {
            foreach (string t in Triggers)
            {
                if (t == trigger) return true;
            }
            return false;
        }
        public bool RequiresTarget()
        {
            return (ValidTargets != null);
        }

        public override string ToString()
        {
            return string.Format("Action: ({0})", Actions[0]);
        }
    }
}
