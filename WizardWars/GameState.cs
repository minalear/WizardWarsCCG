using System;

namespace WizardWars
{
    public class GameState
    {
        public Player PlayerOne, PlayerTwo;

        public Player PlayerTarget;
        public Card CardTarget;

        private bool hasTarget;
        private Card stagedCard;
        private Effect currentEffect;
        private Triggers currentTriggerType;
        private int currentEffectIndex;

        public GameState()
        {
            PlayerOne = new Player();
            PlayerTwo = new Player();

            //Temporary
            PlayerTarget = PlayerOne;
        }

        public bool StageCard(Player caster, Card card)
        {
            stagedCard = card;
            hasTarget = false;

            //Check costs of the card
            if (caster.Mana >= card.Cost)
            {
                caster.Mana -= card.Cost;
                PlayCard(caster, card);

                return true;
            }
            else
            {
                Console.WriteLine("Not enough mana!");

                return false;
            }
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

            Console.WriteLine("Player 1: {0} - {1}", PlayerOne.Health, PlayerOne.Mana);
            Console.WriteLine("Player 2: {0} - {1}", PlayerTwo.Health, PlayerTwo.Mana);
            Console.WriteLine();
        }
        public void TriggerEffects(Player caster, Card card, Triggers triggerType)
        {
            for (int i = 0; i < card.Effects.Length; i++)
            {
                Effect effect = card.Effects[i];
                if (effect.Trigger == triggerType)
                {
                    currentEffect = effect;

                    if (!requiresTarget(effect) || hasTarget)
                        triggerEffect(caster, card, effect);
                    else
                    {
                        currentEffectIndex = i;
                        currentTriggerType = triggerType;

                        //Highlight Targets
                    }
                }
            }
        }
        public void TriggerEffects(Player caster, Card card, Triggers triggerType, int index)
        {
            for (int i = index; i < card.Effects.Length; i++)
            {
                Effect effect = card.Effects[i];
                if (effect.Trigger == triggerType)
                {
                    currentEffect = effect;

                    if (!requiresTarget(effect) || hasTarget)
                        triggerEffect(caster, card, effect);
                    else
                    {
                        currentEffectIndex = i;
                        currentTriggerType = triggerType;

                        //Highlight Targets
                    }
                }
            }
        }

        public void SubmitTarget(Player caster, Card currentTarget)
        {
            if (isValidTarget(currentTarget, currentEffect))
            {
                hasTarget = true;
                CardTarget = currentTarget;

                TriggerEffects(caster, stagedCard, currentTriggerType, currentEffectIndex);
            }
            else
            {
                Console.WriteLine("Invalid Target!");
            }
        }
        public bool SubmitTarget(Player caster, Player target)
        {
            return true;
        }    

        private bool requiresTarget(Effect effect)
        {
            if (effect.ValidTargets[0] == Targets.Self && effect.ValidTargets.Length == 1)
                return false;

            return true;
        }
        private void triggerEffect(Player caster, Card card, Effect effect)
        {
            //Cast self spell (caster is target)
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

            hasTarget = false; //Reset for future effects
        }
        private void triggerTargetedEffect(Player caster, Card card, Effect effect, Card target)
        {
            if (effect.Actions[0] == Actions.Damage)
            {
                int num = parseNumberVariable(caster, card, effect.Vars[0]);
                target.Defense -= num;
            }

            hasTarget = false; //Reset for future effects
        }
        private void triggerTargetedEffect(Player caster, Card card, Effect effect, Player target)
        {
            hasTarget = false; //Reset for future effects
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
        private bool isValidTarget(Card currentTarget, Effect effect)
        {
            foreach (Targets validTarget in effect.ValidTargets)
            {
                if (validTarget == Targets.Creature && currentTarget.IsType(Types.Creature))
                    return true;
            }

            return false;
        }
    }
}
