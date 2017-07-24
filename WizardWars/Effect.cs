using System;

namespace WizardWars
{
    public class Effect
    {
        public Triggers Trigger;
        public string Prompt;

        public Targets[] ValidTargets;
        public Actions[] Actions;

        public object[] Vars;
    }

    public enum Actions
    {
        Draw,
        Damage,
        Destroy,
        Exile,
        Sacrifice,
        Heal
    }

    public enum Targets
    {
        Player,
        Creature
    }

    public enum Triggers
    {
        Cast,
        Attack,
        EnterBattlefield,
        Death
    }
}
