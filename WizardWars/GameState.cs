using System;

namespace WizardWars
{
    public class GameState
    {
        public Player PlayerOne, PlayerTwo;

        public Player PlayerTarget;
        public Card CardTarget;

        public GameState()
        {
            PlayerOne = new Player();
            PlayerTwo = new Player();

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
                TriggerEffects(caster, card, Triggers.EnterBattlefield);

                //Add creature to the battlefield
                caster.Field.AddCard(card, Location.Bottom);
            }

            Console.WriteLine("Player 1 HP: {0}", PlayerOne.Health);
            Console.WriteLine("Player 2 HP: {0}", PlayerTwo.Health);
            Console.WriteLine();
        }
        public void TriggerEffects(Player caster, Card card, Triggers triggerType)
        {
            foreach (Effect effect in card.Effects)
            {
                if (effect.Trigger == triggerType)
                {
                    if (triggerType == Triggers.Cast)
                        triggerCastEffect(caster, card, effect);
                    else if (triggerType == Triggers.EnterBattlefield)
                        triggerETBEffect(caster, card, effect);
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
                    caster.DrawCards(num);
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
        private void triggerETBEffect(Player caster, Card card, Effect effect)
        {

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
                            return caster.Hand.Count;
                        }
                    }
                    else if (argSegments[1] == "Health")
                    {
                        return target.Health;
                    }
                    else if (argSegments[1] == "Creatures")
                    {
                        if (argSegments[2] == "OfType")
                        {
                            //X = number of creatures of the type
                            return target.Field.CountTypes((SubTypes)Enum.Parse(typeof(SubTypes), argSegments[3]));
                        }
                    }
                }
            }

            throw new ArgumentException(string.Format("Invalid syntax for variable: {0}", var));
        }
    }
}
