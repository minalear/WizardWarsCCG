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

        public bool HasTarget(Targets target)
        {
            foreach (Targets test in ValidTargets)
            {
                if (target == test)
                    return true;
            }

            return false;
        }
        public override string ToString()
        {
            return Actions[0].ToString();
        }
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
        Self,
        Player,
        Opponent,
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
