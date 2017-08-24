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
        public Stack<AbilityAction> TargetedAbilities; //Used for determining targets for multiple effects
        public StateAction CurrentAction;

        public int PriorityCounter;
        public int PhaseCounter;

        public List<Card> AllCards;

        public Player CurrentPriority { get { return TurnOrder[PriorityCounter]; } }
        public Player CurrentTurn { get { return TurnOrder[0]; } }
        public Phases CurrentPhase { get { return PhaseSequence[PhaseCounter]; } }
        public bool RequiresTarget { get { return TargetedAbilities.Count > 0; } }

        private Core.MainGame Game;

        public GameState(Core.MainGame game)
        {
            Game = game;

            PlayerOne = new Player(this);
            PlayerTwo = new Player(this);

            GameStack = new Stack<StateAction>();
            TargetedAbilities = new Stack<AbilityAction>();

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
            if (CurrentAction != null && TargetedAbilities.Count == 0)
            {
                StateAction currentAction = CurrentAction;
                CurrentAction = null;

                currentAction.Resolve(this);
            }

            //If there are state actions to process
            if (GameStack.Count > 0 && TargetedAbilities.Count == 0)
            {
                CurrentAction = GameStack.Pop();
                SetPriority(0); //Reset priority to 0
            }
            else if (TargetedAbilities.Count > 0)
            {
                //Prompt player if an ability requires a target
                HighlightValidTargets(TargetedAbilities.Peek());
                TargetedAbilities.Peek().Caster.PromptPlayerTargetRequired(TargetedAbilities.Peek());
            }
        }
        public void PassPriority()
        {
            //Check if an effect requires a target and prompt the proper player if so
            if (TargetedAbilities.Count > 0)
            {
                HighlightValidTargets(TargetedAbilities.Peek());
                TargetedAbilities.Peek().Caster.PromptPlayerTargetRequired(TargetedAbilities.Peek());
            }
            else
            {
                //Pass priority and pass the game if all players have passed, otherwise prompt the next player
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
            else if (action is AbilityAction)
            {
                AbilityAction effectAction = (AbilityAction)action;
                ResolveAbilityAction(effectAction.Caster, effectAction.Card, effectAction);
            }
            else if (action is PhaseAction)
            {
                PhaseAction phaseAction = (PhaseAction)action;
                ResolvePhaseAction(phaseAction.Phase);
            }

            UpdateGameState();
        }
        public void ResolveCardCastAction(Player caster, Card card)
        {
            //Trigger self-cast effects
            OnCast(card);

            //Add permanent cards to the battlefield and trigger ETB effects
            if (card.IsPermanent)
            {
                caster.Field.AddCard(card);
                OnChangeZones(card, Zones.Hand, Zones.Battlefield);
            }
            else
            {
                caster.Graveyard.AddCard(card);
                //OnTrigger(card, "entergraveyard");
            }
        }
        public void ResolveAbilityAction(Player caster, Card card, AbilityAction action)
        {
            action.Ability.Execute(this, card);
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
            if (card.IsSubType(SubTypes.Interrupt))
                return true;

            //If the card is not a interrupt card, they cannot cast it unless the only thing left on the stack is the PhaseAction
            if (CurrentAction != null && CurrentAction.GetType() != typeof(PhaseAction))
                return false;

            //Check if the current Phase is Main and that it is their turn
            return (CurrentTurn.ID == caster.ID && CurrentPhase == Phases.Main);
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

        public void HighlightValidTargets(AbilityAction action)
        {
            clearHighlights();
            validTargets.Clear();

            foreach (Player player in TurnOrder)
            {
                if (action.Ability.IsValidTarget(action.Card, player.PlayerCard))
                    player.PlayerCard.IsValidTarget = true;

                foreach (Card card in player.Field)
                {
                    if (action.Ability.IsValidTarget(action.Card, card))
                        card.IsValidTarget = true;
                }
            }
        }
        public void SubmitTarget(Card card)
        {
            AbilityAction action = TargetedAbilities.Peek();
            action.AddTarget(card);

            if (action.Ability.NumTargets == action.Targets.Count)
            {
                //Set the ability targets to the stored targets and POP IT
                action.Ability.Targets = action.Targets;
                TargetedAbilities.Pop();

                clearHighlights();
            }
            ContinueGame();
        }

        public void OnCast(Card source)
        {
            foreach (Ability ability in source.Abilities)
            {
                if (ability.Type == AbilityTypes.Cast)
                    AddStateAction(new AbilityAction(source, source.Controller, ability));
            }

            Ability triggeredAbility = null;
            if (source.IsTriggered(Triggers.Cast, source, out triggeredAbility))
            {
                AddStateAction(new AbilityAction(source, source.Controller, triggeredAbility));
            }
        }
        public void OnChangeZones(Card source, Zones origin, Zones destination)
        {
            Ability triggeredAbility = null;
            if (source.IsTriggered(Triggers.ChangesZone, source, out triggeredAbility) && triggeredAbility.IsValidZones(origin, destination))
            {
                AddStateAction(new AbilityAction(source, source.Controller, triggeredAbility));
            }
        }

        public void UpdateGameState()
        {
            //Reset all stats and UI specific states
            foreach (Player player in TurnOrder)
            {
                player.PlayerCard.IsValidTarget = false;

                for (int i = 0; i < player.Field.Count; i++)
                {
                    Card card = player.Field[i];
                    card.IsValidTarget = false;

                    card.BonusAttack = 0;
                    card.BonusHealth = 0;
                }
            }

            //Update all static effects
            foreach (Player player in TurnOrder)
            {
                for (int i = 0; i < player.Field.Count; i++)
                {
                    Card card = player.Field[i];
                    foreach (Ability ability in card.Abilities)
                    {
                        if (ability.Type == AbilityTypes.Static)
                            ability.Execute(this, card);
                    }
                }
            }

            //Check for destroyed permanents
            foreach (Player player in TurnOrder)
            {
                for (int i = 0; i < player.Field.Count; i++)
                {
                    Card card = player.Field[i];
                    if (card.IsPermanent && card.IsDestroyed())
                    {
                        player.Field.RemoveCardID(card.ID);
                        player.Graveyard.AddCard(card);
                        OnChangeZones(card, Zones.Battlefield, Zones.Graveyard);
                    }
                }
            }
        }

        public void CreateTokenCreature(CardInfo info, Player owner)
        {
            info.LoadCardArt();
            Card instance = info.CreateInstance(owner);
            owner.Field.AddCard(instance);
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
                card.IsValidTarget = false;
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
    public class AbilityAction : StateAction
    {
        public Card Card { get; set; }
        public Player Caster { get; set; }
        public Ability Ability { get; set; }
        public List<Card> Targets { get; set; }

        public AbilityAction(Card card, Player caster, Ability ability)
        {
            Card = card;
            Caster = caster;
            Ability = ability;
            Targets = new List<Card>();
        }

        public override void Init(GameState gameState)
        {
            if (Ability.TargetRequired)
                  gameState.TargetedAbilities.Push(this);
        }
        public override string ToString()
        {
            return string.Format("Player #{0}'s card ({1}) effect: {2}", Caster.ID + 1, Card, Ability);
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
