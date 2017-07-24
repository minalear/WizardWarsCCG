using System;

namespace WizardWars
{
    public class BoardState
    {
        public Player PlayerOne, PlayerTwo;
        public Collection DeckOne, DeckTwo;
        public Collection FieldOne, FieldTwo;
        public Collection HandOne, HandTwo;

        public Player PlayerTarget;
        public Card CardTarget;

        public BoardState()
        {
            PlayerOne = new Player();
            PlayerTwo = new Player();

            DeckOne = new Collection(PlayerOne);
            FieldOne = new Collection(PlayerOne);
            HandOne = new Collection(PlayerOne);

            //Temporary
            PlayerTarget = PlayerOne;
        }

        public void PlayCard(Player caster, Card card)
        {
            //Trigger cast effects
            TriggerEffects(caster, card, Triggers.Cast);

            Types mainType = card.Types[0];
            if (mainType == Types.Creature)
            {
                //Trigger ETB effects
                //Add creature to the battlefield
            }
        }
        public void TriggerEffects(Player caster, Card card, Triggers triggerType)
        {
            foreach (Effect effect in card.Effects)
            {
                if (effect.Trigger == triggerType)
                {
                    if (triggerType == Triggers.Cast)
                        triggerCastEffect(caster, card, effect);
                }
            }
        }

        private void triggerCastEffect(Player caster, Card card, Effect effect)
        {
            //Determine if it has targets
            if (/*effect.ValidTargets[0] == Targets.Self*/true)
            {
                //Affects caster
                if (effect.Actions[0] == Actions.Draw)
                {
                    int num = parseNumberVariable(caster, card, effect.Vars[0]);
                    HandOne.AddCards(DeckOne.RemoveCards(num, Location.Top), Location.Bottom);
                }
                else if (effect.Actions[0] == Actions.Heal)
                {
                    int num = parseNumberVariable(caster, card, effect.Vars[0]);
                    caster.Health += num;
                }
                else if (effect.Actions[0] == Actions.Damage)
                {
                    int num = parseNumberVariable(caster, card, effect.Vars[0]);
                    caster.Health -= num;
                }
            }
        }
        private int parseNumberVariable(Player caster, Card card, object var)
        {
            Type varType = var.GetType();
            if (varType == typeof(int) || varType == typeof(long))
                return Convert.ToInt32(var);
            else if (varType == typeof(string))
            {
                string syntax = (string)var;
                string[] argSegments = syntax.Split('.');

                //If the target is a playerS
                if (argSegments[0] == "Self" || argSegments[0] == "Player" || argSegments[0] == "Opponent")
                {
                    Player target = (argSegments[0] == "Self") ? caster : PlayerTarget;

                    //Variable calculated off of Hand information
                    if (argSegments[1] == "Hand")
                    {
                        if (argSegments[2] == "Count")
                        {
                            return HandOne.RawList.Count;
                        }
                    }
                    else if (argSegments[1] == "Health")
                    {
                        return target.Health;
                    }
                }
            }

            throw new ArgumentException(string.Format("Invalid syntax for variable: {0}", var));
        }
    }
}
