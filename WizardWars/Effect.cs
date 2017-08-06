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
    }
}
