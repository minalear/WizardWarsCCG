using System;
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
            if (card.Meta.IsSubType(SubTypes.Interrupt))
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
            foreach (Effect effect in card.Meta.Effects)
            {
                if (effect.HasTrigger("cast"))
                {
                    AddStateAction(new EffectAction(card, caster, effect));
                }
            }

            //Add creatures/relics to the battlefield and trigger ETB effects
            if (card.Meta.IsType(Types.Creature) || card.Meta.IsType(Types.Relic))
            {
                caster.Field.AddCard(card);
                foreach (Effect effect in card.Meta.Effects)
                {
                    if (effect.HasTrigger("enterbattlefield"))
                    {
                        AddStateAction(new EffectAction(card, caster, effect));
                    }
                }
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
                    string[] tokens = effect.Actions[i].Split('.');
                    if (tokens[0] == "tap")
                    {
                        //Tap each target
                        target.Tapped = true;
                    }
                    else if (tokens[0] == "untap")
                    {
                        //Untap each target
                        target.Tapped = false;
                    }
                    else if (tokens[0] == "destroy")
                    {
                        //Destroy all the god damn targets
                        target.Destroy();

                        //Send it to the graveyard if it is destroyed
                        if (target.IsDestroyed())
                        {
                            target.Zone.RemoveCardID(target.ID);
                            target.Owner.Graveyard.AddCard(target);

                            OnTrigger(target, "destroy");
                        }
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
                        target.Damage(var);

                        //Send it to the graveyard if it is destroyed
                        if (target.IsDestroyed())
                        {
                            target.Zone.RemoveCardID(target.ID);
                            target.Owner.Graveyard.AddCard(target);

                            OnTrigger(target, "destroy");
                        }
                    }
                    else if (tokens[0] == "heal")
                    {
                        //Heal each target
                        int var = parseNumberVariable(caster, card, action, effect.Vars[i]);
                        target.Heal(var);
                        OnTrigger(target, "heal");
                    }
                    else if (tokens[0] == "counter")
                    {

                    }
                    else if (tokens[0] == "draw")
                    {
                        int num = parseNumberVariable(caster, card, action, effect.Vars[i]);
                        /*if (tokens[1] == "self" || tokens[1] == "all")
                            PlayerOne.DrawCards(num);
                        if (tokens[1] == "opponent" || tokens[1] == "all")
                            PlayerTwo.DrawCards(num);*/
                        caster.DrawCards(num);
                    }
                    #endregion
                }
            }
        }
        public void ResolvePhase(Phases phase)
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
            Player attacker = CurrentTurn;
            Player defender = getOpponent();

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
                {
                    card.Zone.RemoveCardID(card.ID);
                    card.Owner.Graveyard.AddCard(card);

                    OnTrigger(card, "destroy");
                }
            }
            foreach (Card card in defender.Field)
            {
                if (card.IsDestroyed())
                {
                    card.Zone.RemoveCardID(card.ID);
                    card.Owner.Graveyard.AddCard(card);

                    OnTrigger(card, "destroy");
                }
            }
        }

        public void HighlightValidTargets(Effect effect)
        {
            clearHighlights();
            List<Card> validTargets = new List<Card>();

            //Assuming PlayerOne for testing
            foreach (string targetType in effect.ValidTargets)
            {
                string[] tokens = targetType.Split('.');

                if (tokens[0] == "player")
                {

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
            }
        }
        public void SubmitTargets(params Card[] cards)
        {
            TargetedEffects.Pop().Targets = cards;
            clearHighlights();
            ContinueGame();
        }

        public void OnTrigger(Card source, string trigger)
        {
            //Apply triggers to self
            Effect effect = null;
            if (source.IsTriggered(source, trigger, out effect))
            {
                AddStateAction(new EffectAction(source, source.Controller, effect));
            }


            //Apply triggers to other permanents 
            foreach (Card card in PlayerOne.Field)
            {
                if (card.IsTriggered(source, trigger, out effect))
                {
                    AddStateAction(new EffectAction(card, card.Controller, effect));
                }
            }
            foreach (Card card in PlayerTwo.Field)
            {
                if (card.IsTriggered(source, trigger, out effect))
                {
                    AddStateAction(new EffectAction(card, card.Controller, effect));
                }
            }
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
            foreach (Card card in AllCards)
                card.Highlighted = false;
        }
        private int parseNumberVariable(Player caster, Card card, EffectAction action, object var)
        {
            Type varType = var.GetType();
            if (varType == typeof(int) || varType == typeof(long))
                return Convert.ToInt32(var);
            else if (varType == typeof(string))
            {
                string syntax = (string)var;
                string[] argSegments = syntax.Split('.');

                //If the target is a player
                if (argSegments[0] == "self" || argSegments[0] == "player" || argSegments[0] == "opponent")
                {
                    //Player target = (argSegments[0] == "Self") ? caster : effectTarget.PlayerTarget;
                    Player target = PlayerTwo; //This is invalid, but until we convert Players into cards

                    //Variable calculated off of Hand information
                    if (argSegments[1] == "hand")
                    {
                        if (argSegments[2] == "count")
                        {
                            return target.Hand.Count;
                        }
                    }
                    else if (argSegments[1] == "health")
                    {
                        return target.Health;
                    }
                    else if (argSegments[1] == "creatures")
                    {
                        if (argSegments[2] == "oftype")
                        {
                            //X = number of creatures of the type
                            return target.Field.CountTypes((SubTypes)Enum.Parse(typeof(SubTypes), argSegments[3]));
                        }
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
                    if (tokens[0] == "players")
                    {

                    }
                    else if (tokens[0] == "creatures")
                    {
                        string typeRestriction = (tokens.Length > 2) ? tokens[2] : "any";

                        if (tokens[1] == "opponent" || tokens[1] == "all")
                        {
                            foreach (Card opponentCard in PlayerTwo.Field)
                            {
                                if (opponentCard.Meta.IsCreatureType(typeRestriction))
                                    targets.Add(opponentCard);
                            }
                        }
                        if (tokens[1] == "controlled" || tokens[1] == "all")
                        {
                            foreach (Card controlledCard in PlayerOne.Field)
                            {
                                if (controlledCard.Meta.IsCreatureType(typeRestriction))
                                    targets.Add(controlledCard);
                            }
                        }
                    }
                }
            }

            return targets;
        }
        private Player getOpponent()
        {
            foreach (Player player in TurnOrder)
            {
                if (player.ID != CurrentTurn.ID)
                    return player;
            }

            throw new InvalidProgramException("Couldn't find opponent.  Either players have matching IDs or there's only one player in the game.");
        }
        
        public event PhaseChangeEvent PhaseChange;
        public delegate void PhaseChangeEvent(object sender, Phases phase);
    }

    public class StateAction
    {
        public virtual void Init(GameState gameState) { }
        public virtual void Resolve(GameState gameState)
        {
            Console.WriteLine("({0}) action resolves.", ToString());
        }
    }
    public class CardCastAction : StateAction
    {
        public Card Card;
        public Player Caster;
        public object[] Args;

        public CardCastAction(Card card, Player caster, params object[] args)
        {
            Card = card;
            Caster = caster;
            Args = args;
        }

        public override void Resolve(GameState gameState)
        {
            gameState.ResolveCardCastAction(Caster, Card);
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
        public Card[] Targets { get; set; }

        public EffectAction(Card card, Player caster, Effect effect)
        {
            Card = card;
            Caster = caster;
            Effect = effect;
        }

        public override void Init(GameState gameState)
        {
            if (Effect.RequiresTarget())
                gameState.TargetedEffects.Push(this);
        }
        public override void Resolve(GameState gameState)
        {
            gameState.ResolveEffectAction(Caster, Card, this);
        }
        public override string ToString()
        {
            return string.Format("Player #{0}'s card ({1}) effect: {2}", Caster.ID + 1, Card, Effect);
        }
    }
    public class PhaseAction : StateAction
    {
        public Phases Phase;

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
                    card.Tapped = false;
            }
            else if (Phase == Phases.DeclareBlock)
            {
                foreach (Card card in gameState.CurrentTurn.Field)
                {
                    if (card.Attacking)
                        gameState.OnTrigger(card, "attack");
                }
            }
            else if (Phase == Phases.Combat)
            {
                gameState.CalculateCombat();
            }
        }
        public override void Resolve(GameState gameState)
        {
            gameState.ResolvePhase(Phase);
        }
        public override string ToString()
        {
            return Phase.ToString();
        }
    }

    public enum Phases { Upkeep, Draw, Main, DeclareAttack, DeclareBlock, Combat, Cleanup }
}
