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

        public Collection AllCards;

        public Player CurrentPriority { get { return TurnOrder[PriorityCounter]; } }
        public Player CurrentTurn { get { return TurnOrder[0]; } }
        public Phases CurrentPhase { get { return PhaseSequence[PhaseCounter]; } }

        public GameState()
        {
            PlayerOne = new Player(this);
            PlayerTwo = new Player(this);

            GameStack = new Stack<StateAction>();
            TargetedEffects = new Stack<EffectAction>();

            AllCards = new Collection();

            TurnOrder = new List<Player>() { PlayerOne, PlayerTwo };
            PhaseSequence = new List<Phases>() { Phases.Upkeep, Phases.Draw, Phases.Main, Phases.Battle, Phases.Main, Phases.Cleanup };
        }

        public void StartGame()
        {
            PriorityCounter = 0;
            PhaseCounter = 0;

            PlayerOne.DrawCards(7);
            PlayerTwo.DrawCards(7);

            AddStateAction(new PhaseAction(PhaseSequence[PhaseCounter]));
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
                CurrentAction.Resolve(this);
                CurrentAction = null;
            }

            if (TargetedEffects.Count == 0)
            {
                //If there are state actions to process
                if (GameStack.Count > 0)
                {
                    CurrentAction = GameStack.Pop();
                    SetPriority(0); //Reset priority to 0
                }
                //Progress phases/turn
                else
                {
                    PhaseCounter++;
                    if (PhaseCounter >= PhaseSequence.Count)
                    {
                        PhaseCounter = 0;
                        swapTurns();
                    }

                    AddStateAction(new PhaseAction(PhaseSequence[PhaseCounter]));
                    ContinueGame();
                }
            }
        }
        public void PassPriority()
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

            //Add creatures to the battlefield and trigger ETB effects
            if (card.Meta.IsType(Types.Creature))
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

            ContinueGame();
        }
        public void ResolveEffectAction(Player caster, Card card, Effect effect)
        {

        }

        public void HighlightValidTargets(Effect effect)
        {
            clearHighlights();

            foreach (string targetType in effect.ValidTargets)
            {
                string[] tokens = targetType.Split('.');
            }
        }
        public void SetTargets(params Card[] cards)
        {
            TargetedEffects.Pop().Targets = cards;
            ContinueGame();
        }

        private void swapTurns()
        {
            //This shifts the whole list over one and adds the original first to the end
            //Provides multiplayer support
            Player first = TurnOrder[0];
            TurnOrder.RemoveAt(0);
            TurnOrder.Add(first);
        }
        private void clearHighlights()
        {
            foreach (Card card in AllCards)
                card.Highlighted = false;
        }
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

        public override void Resolve(GameState gameState)
        {
            if (Phase == Phases.Draw)
                gameState.CurrentTurn.DrawCards(1);
        }
        public override string ToString()
        {
            return Phase.ToString();
        }
    }

    public enum Phases { Upkeep, Draw, Main, Battle, Cleanup }
}
