using System;

namespace WizardWars
{
    public abstract class Effect
    {
        public abstract void ApplyEffect(BoardState state);
    }

    public class DrawEffect : Effect
    {
        public override void ApplyEffect(BoardState state)
        {

        }
    }
}
