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
        public StateAction CurrentAction;

        public int PriorityCounter;
        public int PhaseCounter;

        public Player CurrentPriority { get { return TurnOrder[PriorityCounter]; } }
        public Player CurrentTurn { get { return TurnOrder[0]; } }
        public Phases CurrentPhase { get { return PhaseSequence[PhaseCounter]; } }

        public GameState()
        {
            PlayerOne = new Player(this);
            PlayerTwo = new Player(this);

            GameStack = new Stack<StateAction>();

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
        }

        public void AddStateAction(StateAction action)
        {
            PriorityCounter = 0;

            //True if a player is reacting to a state action
            if (CurrentAction != null)
            {
                //Push both actions onto the stack and set the current action to null
                GameStack.Push(CurrentAction);
                CurrentAction = null;
            }

            GameStack.Push(action);
            ContinueGame();
        }
        public void ContinueGame()
        {
            //Resolve action since all players have passed
            if (CurrentAction != null)
            {
                CurrentAction.Resolve(this);
                CurrentAction = null;
            }

            //If there are state actions to process
            if (GameStack.Count > 0)
            {
                CurrentAction = GameStack.Pop();
                PriorityCounter = -1; //Pass Priority will set this to 0
                PassPriority();
            }
            //Progress phases/turn
            else
            {
                PhaseCounter++;
                if (PhaseCounter >= PhaseSequence.Count)
                {
                    PhaseCounter = 0;
                    swampTurns();
                }

                AddStateAction(new PhaseAction(PhaseSequence[PhaseCounter]));
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

        private void swampTurns()
        {
            //This shifts the whole list over one and adds the original first to the end
            //Provides multiplayer support
            Player first = TurnOrder[0];
            TurnOrder.RemoveAt(0);
            TurnOrder.Add(first);
        }
    }

    public class StateAction
    {
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

        public override string ToString()
        {
            return string.Format("Player #{0} cast card ({1})", Caster.ID + 1, Card.ToString());
        }
    }
    public class PhaseAction : StateAction
    {
        public Phases Phase;

        public PhaseAction(Phases phase)
        {
            Phase = phase;
        }

        public override string ToString()
        {
            return Phase.ToString();
        }
    }

    public enum Phases { Upkeep, Draw, Main, Battle, Cleanup }
}
