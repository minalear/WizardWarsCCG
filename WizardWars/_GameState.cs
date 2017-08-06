using System;
using System.Collections.Generic;

namespace WizardWars
{
    public class _GameState
    {
        public Player PlayerOne, PlayerTwo;
        public bool IsCasting { get; private set; }
        public Card StagedCard { get { return stagedCard; } }

        private Card stagedCard;
        private Triggers stagedTriggers;
        private Effect stagedEffect;
        private int stagedEffectIndex;

        private Target effectTarget;
        private bool hasTarget;

        public _GameState()
        {
            PlayerOne = new Player();
            PlayerTwo = new Player();
        }
        
        public bool StageCard(Player caster, Card card)
        {
            stagedCard = card;

            //Check costs of the card
            if (caster.Mana >= card.Meta.Cost)
            {
                IsCasting = true;
                caster.Mana -= card.Meta.Cost;
                PlayCard(caster, card);

                return true;
            }

            Console.WriteLine("Not enough mana!");
            return false; //Cannot cast card
        }
        public void SubmitTarget(Player caster, Target target)
        {
            if (IsCasting)
            {
                bool isValid = isValidTarget(stagedEffect, target);
                if (!isValid)
                {
                    //Invalid targets will autocancel the spell
                    InvalidTarget?.Invoke(this, EventArgs.Empty);
                    CancelCard(caster);
                }
                else
                {
                    effectTarget = target;
                    hasTarget = true;

                    TriggerEffects(caster, stagedCard, stagedTriggers, stagedEffectIndex);
                }
            }
        }
        public void CancelCard(Player caster)
        {
            IsCasting = false;
            hasTarget = false;
            stagedEffectIndex = 0;

            //Refund mana
            caster.Mana += stagedCard.Meta.Cost;
        }

        public void PlayCard(Player caster, Card card)
        {
            //Here we would add the card to the stack and wait for a response from the opponent
            TriggerEffects(caster, card, Triggers.Cast); //Trigger on-cast triggers

            if (card.Meta.Types[0] == Types.Creature || card.Meta.Types[0] == Types.Hero)
            {
                //Trigger ETB effects
                TriggerEffects(caster, card, Triggers.EnterBattlefield);

                //Add creature to the field
                caster.Field.AddCard(card);
            }

            #if DEBUG
            Console.WriteLine("Player 1: {0} - {1}", PlayerOne.Health, PlayerOne.Mana);
            Console.WriteLine("Player 2: {0} - {1}", PlayerTwo.Health, PlayerTwo.Mana);
            Console.WriteLine();
            #endif
        }
        public void TriggerEffects(Player caster, Card card, Triggers trigger, int index = 0)
        {
            //Loop through all effects, starting at the provided index
            for (int i = index; i < card.Meta.Effects.Length; i++)
            {
                Effect effect = card.Meta.Effects[i];

                //Trigger only the effects that match the given type
                if (effect.Trigger == trigger)
                {
                    if (effect.Targeted && !hasTarget)
                    {
                        stagedTriggers = trigger;
                        stagedEffect = effect;
                        stagedEffectIndex = i;

                        Console.WriteLine(effect.Prompt);
                        return;
                    }
                    else if (effect.Targeted && hasTarget)
                    {
                        applyTargetedEffect(caster, card, effect, effectTarget);
                        hasTarget = false; //Reset target
                    }
                    else
                    {
                        //These effects can be Self casted spells or things that affect multiple cards (ie Wrath of God)
                        List<Target> affectedCreatures = getAllTargets(caster, effect);
                        foreach (Target target in affectedCreatures)
                        {
                            applyTargetedEffect(caster, card, effect, target);
                        }
                    }
                }
            }

            IsCasting = false; //We're done casting the spell
        }

        private void applySelfEffect(Player caster, Card card, Effect effect)
        {
            //Caster is target of all actions for Self spells
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
        private void applyTargetedEffect(Player caster, Card card, Effect effect, Target target)
        {
            if (target.CurrentMode == Target.Current.Opponent || target.CurrentMode == Target.Current.Self)
            {
                //A player is the current target
                if (effect.Actions[0] == Actions.Draw)
                {
                    int num = parseNumberVariable(caster, card, effect.Vars[0]);
                    target.PlayerTarget.DrawCards(num);
                }
                else if (effect.Actions[0] == Actions.Heal)
                {
                    int num = parseNumberVariable(caster, card, effect.Vars[0]);
                    target.PlayerTarget.Health += num;
                }
                else if (effect.Actions[0] == Actions.Damage)
                {
                    int num = parseNumberVariable(caster, card, effect.Vars[0]);
                    target.PlayerTarget.Health -= num;
                }
            }
            else
            {
                //A creature is the current target
                if (effect.Actions[0] == Actions.Damage)
                {
                    int num = parseNumberVariable(caster, card, effect.Vars[0]);
                    target.CardTarget.Damage(num);

                    //Check is dead and remove from battlefield
                    if (target.CardTarget.IsDestroyed() && caster.Field.HasCardID(target.CardTarget.ID))
                    {
                        caster.Graveyard.AddCard(caster.Field.RemoveCardID(target.CardTarget.ID));
                    }
                }
                else if (effect.Actions[0] == Actions.Destroy)
                {
                    TriggerEffects(caster, target.CardTarget, Triggers.Death);
                    target.CardTarget.Destroy();

                    //Check is dead and remove from battlefield
                    if (target.CardTarget.IsDestroyed() && caster.Field.HasCardID(target.CardTarget.ID))
                    {
                        caster.Graveyard.AddCard(caster.Field.RemoveCardID(target.CardTarget.ID));
                    }
                }
            }
        }

        private bool isValidTarget(Effect effect, Target target)
        {
            //Submitted target is a card (assuming field card for now)
            if (target.CurrentMode == Target.Current.Card)
            {
                return effect.HasTarget(Targets.Creature);
            }
            else if (target.CurrentMode == Target.Current.Self)
            {
                return (effect.HasTarget(Targets.Self) || effect.HasTarget(Targets.Player));
            }
            else if (target.CurrentMode == Target.Current.Opponent)
            {
                return (effect.HasTarget(Targets.Opponent) || effect.HasTarget(Targets.Player));
            }

            return false;
        }
        private List<Target> getAllTargets(Player caster, Effect effect)
        {
            List<Target> targets = new List<Target>();
            foreach (object target in effect.Affects)
            {
                targets.AddRange(parseTargets(caster, target));
            }

            return targets;
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

                //If the target is a player
                if (argSegments[0] == "Self" || argSegments[0] == "Player" || argSegments[0] == "Opponent")
                {
                    Player target = (argSegments[0] == "Self") ? caster : effectTarget.PlayerTarget;

                    //Variable calculated off of Hand information
                    if (argSegments[1] == "Hand")
                    {
                        if (argSegments[2] == "Count")
                        {
                            return target.Hand.Count;
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
        private List<Target> parseTargets(Player caster, object var)
        {
            List<Target> targets = new List<Target>();

            string str = (string)var;
            string[] split = str.Split('.');

            if (split[0] == "self")
                targets.Add(new Target(caster, true));
            if (split[0] == "creatures")
            {
                if (split[1] == "all" && split.Length == 2)
                {
                    //Target all creatures
                    foreach (Card card in PlayerOne.Field)
                    {
                        if (card.Meta.IsType(Types.Creature))
                            targets.Add(new Target(card, Target.Zones.Field));
                    }
                    foreach (Card card in PlayerTwo.Field)
                    {
                        if (card.Meta.IsType(Types.Creature))
                            targets.Add(new Target(card, Target.Zones.Field));
                    }
                }
                else if (split[1] == "all" && split[2] == "OfType")
                {
                    SubTypes creatureType = (SubTypes)Enum.Parse(typeof(SubTypes), split[3]);

                    //Target all creatures of a type
                    foreach (Card card in PlayerOne.Field)
                    {
                        if (card.Meta.IsType(Types.Creature) && card.Meta.IsSubType(creatureType))
                            targets.Add(new Target(card, Target.Zones.Field));
                    }
                    foreach (Card card in PlayerTwo.Field)
                    {
                        if (card.Meta.IsType(Types.Creature) && card.Meta.IsSubType(creatureType))
                            targets.Add(new Target(card, Target.Zones.Field));
                    }
                }
            }

            return targets;
        }

        public delegate void InvalidTargetEvent(object sender, EventArgs e);
        public event InvalidTargetEvent InvalidTarget;
    }

    public class Target
    {
        public Card CardTarget;
        public Player PlayerTarget;

        public Current CurrentMode = Current.Card;
        public Zones CurrentZone = Zones.Field;

        public Target(Card target, Zones zone)
        {
            CardTarget = target;
            CurrentMode = Current.Card;
        }
        public Target(Player target, bool self)
        {
            PlayerTarget = target;
            CurrentMode = (self) ? Current.Self : Current.Opponent;
        }

        public enum Current
        {
            Opponent, Self, Card
        }
        public enum Zones
        {
            Hand, Elysium, Field, Deck, Graveyard
        }
    }
}
