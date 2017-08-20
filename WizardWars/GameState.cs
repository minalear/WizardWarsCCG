﻿using System;
using System.Collections.Generic;

namespace WizardWars
{
    public sealed class GameState
    {
        public Player PlayerOne, PlayerTwo;

        public List<Player> TurnOrder;
        public List<Phases> PhaseSequence;
        public Stack<StateAction> GameStack;
        public Stack<EffectAction> TargetedEffects; //Used for determining targets for multiple effects
        public StateAction CurrentAction;

        public int PriorityCounter;
        public int PhaseCounter;

        public List<Card> AllCards;

        public Player CurrentPriority { get { return TurnOrder[PriorityCounter]; } }
        public Player CurrentTurn { get { return TurnOrder[0]; } }
        public Phases CurrentPhase { get { return PhaseSequence[PhaseCounter]; } }
        public bool RequiresTarget { get { return TargetedEffects.Count > 0; } }

        public GameState()
        {
            PlayerOne = new Player(this);
            PlayerTwo = new Player(this);

            GameStack = new Stack<StateAction>();
            TargetedEffects = new Stack<EffectAction>();

            AllCards = new List<Card>();

            TurnOrder = new List<Player>() { PlayerOne, PlayerTwo };
            PhaseSequence = new List<Phases>() {
                Phases.Upkeep, Phases.Draw, Phases.Main,
                Phases.DeclareAttack, Phases.DeclareBlock, Phases.Combat,
                Phases.Main, Phases.Cleanup };

            validTargets = new List<Card>();
        }

        public void StartGame()
        {
            PriorityCounter = 0;
            PhaseCounter = 0;

            PlayerOne.DrawCards(7);
            PlayerTwo.DrawCards(7);

            AddStateAction(new PhaseAction(PhaseSequence[PhaseCounter]));
            PhaseChange?.Invoke(this, CurrentPhase);
            ContinueGame();
        }

        public void AddStateAction(StateAction action)
        {
            PriorityCounter = 0;
            NewStateAction?.Invoke(this, action);
            action.Init(this);

            //True if a player is reacting to a state action
            if (CurrentAction != null)
            {
                //Push both actions onto the stack and set the current action to null
                GameStack.Push(CurrentAction);
                CurrentAction = null;
            }

            GameStack.Push(action);
        }
        public void ContinueGame()
        {
            //Resolve action since all players have passed
            if (CurrentAction != null && TargetedEffects.Count == 0)
            {
                StateAction currentAction = CurrentAction;
                CurrentAction = null;

                currentAction.Resolve(this);
            }

            //If there are state actions to process
            if (GameStack.Count > 0 && TargetedEffects.Count == 0)
            {
                CurrentAction = GameStack.Pop();
                SetPriority(0); //Reset priority to 0
            }
            else if (TargetedEffects.Count > 0)
            {
                HighlightValidTargets(TargetedEffects.Peek().Effect);
                TargetedEffects.Peek().Caster.PromptPlayerTargetRequired(TargetedEffects.Peek());
            }
        }
        public void PassPriority()
        {
            //Check if an effect requires a target and prompt the proper player if so
            if (TargetedEffects.Count > 0)
            {
                HighlightValidTargets(TargetedEffects.Peek().Effect);
                TargetedEffects.Peek().Caster.PromptPlayerTargetRequired(TargetedEffects.Peek());
            }
            else
            {
                PriorityCounter++;
                if (PriorityCounter >= TurnOrder.Count)
                {
                    PriorityCounter = 0;
                    ContinueGame();
                }
                else if (CurrentAction != null)
                {
                    CurrentPriority.PromptPlayerStateAction(CurrentAction);
                }
            }
        }
        public void SetPriority(int index)
        {
            PriorityCounter = index;
            if (CurrentAction != null)
            {
                CurrentPriority.PromptPlayerStateAction(CurrentAction);
            }
        }

        public void ResolveStateAction(StateAction action)
        {
            ActionResolved?.Invoke(this, action);

            if (action is CardCastAction)
            {
                CardCastAction castAction = (CardCastAction)action;
                ResolveCardCastAction(castAction.Caster, castAction.Card);
            }
            else if (action is EffectAction)
            {
                EffectAction effectAction = (EffectAction)action;
                ResolveEffectAction(effectAction.Caster, effectAction.Card, effectAction);
            }
            else if (action is PhaseAction)
            {
                PhaseAction phaseAction = (PhaseAction)action;
                ResolvePhaseAction(phaseAction.Phase);
            }
        }

        public bool HasPriority(Player player)
        {
            return (CurrentPriority.ID == player.ID);
        }
        public bool CanCastCard(Player caster, Card card)
        {
            //Players can only cast spells if they have priority
            if (!HasPriority(caster))
                return false;

            //Players can cast Interrupt cards anytime they have priority
            if (card.IsOfSubType(SubTypes.Interrupt))
                return true;

            //If the card is not a interrupt card, they cannot cast it unless the only thing left on the stack is the PhaseAction
            if (CurrentAction != null && CurrentAction.GetType() != typeof(PhaseAction))
                return false;

            //Check if the current Phase is Main and that it is their turn
            return (CurrentTurn.ID == caster.ID && CurrentPhase == Phases.Main);
        }
        public void ResolveCardCastAction(Player caster, Card card)
        {
            //Trigger self-cast effects
            OnTrigger(card, "cast");

            //Add permanent cards to the battlefield and trigger ETB effects
            if (card.IsPermanent)
            {
                caster.Field.AddCard(card);
                //card.EnteredBattlefield();
                OnTrigger(card, "enterbattlefield");
            }
            else
            {
                caster.Graveyard.AddCard(card);
                OnTrigger(card, "entergraveyard");
            }
        }
        public void ResolveEffectAction(Player caster, Card card, EffectAction action)
        {
            Effect effect = action.Effect;
            List<Card> targets = getAffectedTargets(caster, card, action);

            //Loop through each effect action and apply it
            for (int i = 0; i < effect.Actions.Length; i++)
            {
                foreach (Card target in targets)
                {
                    #region Spell Effects
                    /*string[] tokens = effect.Actions[i].Split('.');
                    if (tokens[0] == "tap")
                    {
                        //Tap each target
                        target.IsTapped = true;
                        OnTrigger(target, "tap");
                    }
                    else if (tokens[0] == "untap")
                    {
                        //Untap each target
                        target.IsTapped = false;
                        OnTrigger(target, "untap");
                    }
                    else if (tokens[0] == "destroy")
                    {
                        //Destroy all the god damn targets
                        target.Destroy();

                        //Send it to the graveyard if it is destroyed
                        if (target.IsDestroyed())
                            OnTrigger(target, "destroy");
                    }
                    else if (tokens[0] == "exile")
                    {
                        //Exile each target
                        target.Owner.Exile.AddCard(target);
                        OnTrigger(target, "exile");
                    }
                    else if (tokens[0] == "devote")
                    {
                        //Add target cards to owner's elysium field
                        target.Owner.Elysium.AddCard(target);
                        OnTrigger(target, "devote");
                    }
                    else if (tokens[0] == "damage")
                    {
                        //Damage target creature
                        int var = parseNumberVariable(caster, card, action, effect.Vars[i]);

                        if (target.Meta.IsType(Types.Player))
                            target.Controller.Damage(action.Card, var);
                        else
                            target.Damage(var);

                        OnTrigger(target, "damage");

                        //Send it to the graveyard if it is destroyed
                        if (target.IsDestroyed())
                            OnTrigger(target, "destroy");
                    }
                    else if (tokens[0] == "heal")
                    {
                        //Heal each target
                        int var = parseNumberVariable(caster, card, action, effect.Vars[i]);

                        if (target.Meta.IsType(Types.Player))
                            target.Controller.Heal(action.Card, var);
                        else
                            target.Heal(var);

                        OnTrigger(target, "heal");
                    }
                    else if (tokens[0] == "counter")
                    {

                    }
                    else if (tokens[0] == "draw")
                    {
                        if (target.Meta.IsType(Types.Player))
                        {
                            int num = parseNumberVariable(caster, card, action, effect.Vars[i]);
                            target.Controller.DrawCards(num);

                            OnTrigger(target, "draw");
                        }
                    }*/
                    #endregion
                }
            }

            UpdateGameState();
        }
        public void ResolvePhaseAction(Phases phase)
        {
            //Increment phase counter and change turns if it surpasses Cleanup
            PhaseCounter++;
            if (PhaseCounter >= PhaseSequence.Count)
            {
                PhaseCounter = 0;
                swapTurns();
            }

            //Add the new phase to the stack and trigger PhaseChange
            AddStateAction(new PhaseAction(CurrentPhase));
            PhaseChange?.Invoke(this, CurrentPhase);
        }
        public void CalculateCombat()
        {
            /*Player attacker = CurrentTurn;
            Player defender = GetOpponent(attacker);

            List<Card> attackingCreatures = new List<Card>();
            foreach (Card card in attacker.Field)
            {
                if (card.Attacking)
                    attackingCreatures.Add(card);
            }

            foreach (Card card in attackingCreatures)
            {
                if (!card.IsBlocked)
                    defender.Damage(card, card.Attack);
                else
                {
                    card.Damage(card.BlockerRef.Attack);
                    card.BlockerRef.Damage(card.Attack);
                }
            }

            //Check for destroyed creatures
            foreach (Card card in attacker.Field)
            {
                if (card.IsDestroyed())
                    OnTrigger(card, "destroy");
            }
            foreach (Card card in defender.Field)
            {
                if (card.IsDestroyed())
                    OnTrigger(card, "destroy");
            }

            UpdateGameState();*/
        }
        public Player GetOpponent(Player self)
        {
            foreach (Player player in TurnOrder)
            {
                if (player.ID != self.ID)
                    return player;
            }

            throw new InvalidProgramException("Couldn't find opponent.  Either players have matching IDs or there's only one player in the game.");
        }

        public void HighlightValidTargets(Effect effect)
        {
            clearHighlights();
            validTargets.Clear();

            //Assuming PlayerOne for testing
            /*foreach (string targetType in effect.ValidTargets)
            {
                string[] tokens = targetType.Split('.');

                if (tokens[0] == "player")
                {
                    if (tokens[1] == "any" || tokens[1] == "opponent")
                        validTargets.Add(PlayerTwo.PlayerCard);
                    if (tokens[1] == "any" || tokens[1] == "self")
                        validTargets.Add(PlayerOne.PlayerCard);
                }
                else if (tokens[0] == "creature")
                {
                    string typeRestriction = (tokens.Length > 2) ? tokens[2] : "any";

                    if (tokens[1] == "any" || tokens[1] == "opponent")
                    {
                        foreach (Card card in PlayerTwo.Field)
                        {
                            if (card.Meta.IsCreatureType(typeRestriction))
                                validTargets.Add(card);
                        }
                    }
                    if (tokens[1] == "any" || tokens[1] == "controlled")
                    {
                        foreach (Card card in PlayerOne.Field)
                        {
                            if (card.Meta.IsCreatureType(typeRestriction))
                                validTargets.Add(card);
                        }
                    }
                }
            }

            //Highlight targets
            foreach (Card card in validTargets)
            {
                card.Highlighted = true;
            }*/
        }
        public void SubmitTarget(Card card)
        {
            if (isValidTarget(TargetedEffects.Peek().Effect, card))
            {
                TargetedEffects.Pop().AddTarget(card);
                clearHighlights();
                ContinueGame();
            }
        }

        public void OnTrigger(Card source, string trigger)
        {
            //Apply triggers to self
            /*List<Effect> effects = null;
            if (source.IsTriggered(source, trigger, out effects))
            {
                foreach (Effect effect in effects)
                    AddStateAction(new EffectAction(source, source.Controller, effect));
            }
            
            //Apply triggers to other permanents 
            foreach (Player player in TurnOrder)
            {
                foreach (Card card in player.Field)
                {
                    if (card.ID != source.ID && card.IsTriggered(source, trigger, out effects))
                    {
                        foreach (Effect effect in effects)
                            AddStateAction(new EffectAction(card, card.Controller, effect));
                    }
                }
            }*/
        }
        public void UpdateGameState()
        {
            /*foreach (Player player in TurnOrder)
            {
                for (int i = 0; i < player.Field.Count; i++)
                {
                    Card card = player.Field[i];
                    if (card.IsDestroyed())
                    {
                        card.Zone.RemoveCardID(card.ID);
                        card.Owner.Graveyard.AddCard(card);

                        i--;
                    }
                }
            }*/
        }

        private void swapTurns()
        {
            //This shifts the whole list over one and adds the original first to the end
            //Provides multiplayer support
            Player first = TurnOrder[0];
            TurnOrder.RemoveAt(0);
            TurnOrder.Add(first);

            SetPriority(0);
        }
        private void clearHighlights()
        {
            /*foreach (Card card in AllCards)
                card.Highlighted = false;*/
        }
        private int parseNumberVariable(Player caster, Card card, EffectAction action, object var)
        {
            Type varType = var.GetType();
            if (varType == typeof(int) || varType == typeof(long))
                return Convert.ToInt32(var);
            else if (varType == typeof(string))
            {
                string syntax = (string)var;
                string[] tokens = syntax.Split('.');

                Player target = PlayerOne;
                if (tokens[0] == "player")
                {
                    if (tokens[1] == "target")
                        target = action.Targets[0].Controller;
                    else if (tokens[1] == "self")
                        target = card.Controller;
                    else if (tokens[1] == "opponent")
                        target = GetOpponent(card.Controller);
                }

                if (tokens[2] == "hand" && tokens[3] == "count")
                {
                    return target.Hand.Count;
                }
                else if (tokens[2] == "health")
                {
                    return target.PlayerCard.CurrentHealth;
                }
                else if (tokens[2] == "creatures")
                {
                    if (tokens[3] == "oftype")
                    {
                        //X = number of creatures of the type
                        return target.Field.CountTypes((SubTypes)Enum.Parse(typeof(SubTypes), tokens[4], true));
                    }
                    else if (tokens[3] == "count")
                    {
                        return target.Field.CountTypes(Types.Creature);
                    }
                }
            }

            throw new ArgumentException(string.Format("Invalid syntax for variable: {0}", var));
        }
        private List<Card> getAffectedTargets(Player caster, Card card, EffectAction action)
        {
            Effect effect = action.Effect;
            List<Card> targets = new List<Card>();

            //Add specifically targeted
            if (effect.RequiresTarget())
                targets.AddRange(action.Targets);

            //Add everything else under "affects"
            if (effect.Affects != null && effect.Affects.Length > 0)
            {
                foreach (string group in effect.Affects)
                {
                    string[] tokens = group.Split('.');

                    //PLAYERS
                    if (tokens[0] == "player")
                    {
                        if (tokens[1] == "self" || tokens[1] == "all")
                            targets.Add(caster.PlayerCard);
                        if (tokens[1] == "opponent" || tokens[1] == "all")
                            targets.Add(GetOpponent(caster).PlayerCard);
                    }
                    else if (tokens[0] == "creatures")
                    {
                        string typeRestriction = (tokens.Length > 2) ? tokens[2] : "any";

                        if (tokens[1] == "opponent" || tokens[1] == "all")
                        {
                            foreach (Card opponentCard in PlayerTwo.Field)
                            {
                                /*if (opponentCard.Meta.IsCreatureType(typeRestriction))
                                    targets.Add(opponentCard);*/
                            }
                        }
                        if (tokens[1] == "controlled" || tokens[1] == "all")
                        {
                            foreach (Card controlledCard in PlayerOne.Field)
                            {
                                /*if (controlledCard.Meta.IsCreatureType(typeRestriction))
                                    targets.Add(controlledCard);*/
                            }
                        }
                    }
                }
            }

            return targets;
        }
        private bool isValidTarget(Effect effect, Card card)
        {
            //validTargets was set earlier when validTargets were highlighted
            foreach (Card validTarget in validTargets)
            {
                if (validTarget.ID == card.ID)
                    return true;
            }

            return false;
        }

        private List<Card> validTargets;
        
        public event PhaseChangeEvent PhaseChange;
        public event StateActionResolved ActionResolved;
        public event NewStateActionEvent NewStateAction;

        public delegate void PhaseChangeEvent(object sender, Phases phase);
        public delegate void StateActionResolved(object sender, StateAction action);
        public delegate void NewStateActionEvent(object sender, StateAction action);
    }

    public class StateAction
    {
        public virtual void Init(GameState gameState) { }
        public virtual void Resolve(GameState gameState)
        {
            gameState.ResolveStateAction(this);
        }
    }
    public class CardCastAction : StateAction
    {
        public Card Card;
        public Player Caster;

        public CardCastAction(Card card, Player caster)
        {
            Card = card;
            Caster = caster;
        }
        
        public override string ToString()
        {
            return string.Format("Player #{0} cast card ({1})", Caster.ID + 1, Card.ToString());
        }
    }
    public class EffectAction : StateAction
    {
        public Card Card { get; set; }
        public Player Caster { get; set; }
        public Effect Effect { get; set; }
        public List<Card> Targets { get; set; }

        public EffectAction(Card card, Player caster, Effect effect)
        {
            Card = card;
            Caster = caster;
            Effect = effect;
            Targets = new List<Card>();
        }

        public override void Init(GameState gameState)
        {
            if (Effect.RequiresTarget())
                gameState.TargetedEffects.Push(this);
        }
        public override string ToString()
        {
            return string.Format("Player #{0}'s card ({1}) effect: {2}", Caster.ID + 1, Card, Effect);
        }

        public void AddTarget(Card card)
        {
            Targets.Add(card);
        }
    }
    public class PhaseAction : StateAction
    {
        public Phases Phase { get; set; }

        public PhaseAction(Phases phase)
        {
            Phase = phase;
        }

        public override void Init(GameState gameState)
        {
            if (Phase == Phases.Draw)
                gameState.CurrentTurn.DrawCards(1);
            else if (Phase == Phases.Upkeep)
            {
                foreach (Card card in gameState.CurrentTurn.Field)
                {
                    card.IsTapped = false;
                    card.IsSummoningSick = false;
                }
                foreach (Card card in gameState.CurrentTurn.Elysium)
                {
                    card.IsTapped = false;
                    //card.IsManaDrained = false;
                }
            }
            else if (Phase == Phases.DeclareBlock)
            {
                foreach (Card card in gameState.CurrentTurn.Field)
                {
                    /*if (card.Attacking)
                        gameState.OnTrigger(card, "attack");*/
                }
            }
            else if (Phase == Phases.Combat)
            {
                gameState.CalculateCombat();
            }
            else if (Phase == Phases.Cleanup)
            {
                gameState.CurrentTurn.NumberOfTimesDevoted = 0;
            }
        }
        public override void Resolve(GameState gameState)
        {
            //Clear mana from both player's mana base
            foreach (Player player in gameState.TurnOrder)
            {
                player.Mana = 0;

                //Ensure all tapped lands are considered drained between phases
                foreach (Card card in player.Elysium)
                {
                    /*if (card.IsTapped)
                        card.IsManaDrained = true;*/
                }
            }

            base.Resolve(gameState);
        }
        public override string ToString()
        {
            return Phase.ToString();
        }
    }

    public enum Phases { Upkeep, Draw, Main, DeclareAttack, DeclareBlock, Combat, Cleanup }
}
