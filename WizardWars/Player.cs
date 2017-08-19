using System;

namespace WizardWars
{
    public class Player
    {
        private static int _nextValidID = 0;

        public int ID { get; private set; }
        public GameState GameState;

        public int Heatlh { get { return PlayerCard.Defense; } }
        public int Mana;

        public Card PlayerCard;

        public Collection AllCards;

        public Collection Hand;
        public Collection Deck;
        public Collection Graveyard;
        public Collection Exile;
        public Collection Elysium;
        public Collection Field;

        public bool CanDrawCards = true;
        public int NumberOfTimesDevoted = 0;
        public const int MAX_DEVOTION_PER_TURN = 1;

        public Player(GameState gameState)
        {
            ID = _nextValidID++;
            GameState = gameState;

            Mana = 0;

            Hand = new Collection("Hand", this);
            Deck = new Collection("Deck", this);
            Graveyard = new Collection("Graveyard", this);
            Exile = new Collection("Exile", this);
            Elysium = new Collection("Elysium", this);
            Field = new Collection("Battlefield", this);
        }

        public void DrawCards(int num)
        {
            if (CanDrawCards)
            {
                if (Deck.Count > 0)
                {
                    //TODO: Check for invalid number of cards
                    num = OpenTK.MathHelper.Clamp(num, 0, Deck.Count);
                    Hand.AddCards(Deck.RemoveCards(num, Location.Top), Location.Bottom);
                }
            }
        }
        public void Damage(Card source, int num)
        {
            PlayerCard.Damage(num);
            HealthChanged?.Invoke(this, PlayerCard.Defense);

            //Eventually implement a fail state
            if (PlayerCard.IsDestroyed()) { }
        }
        public void Heal(Card source, int num)
        {
            PlayerCard.Heal(num);
            HealthChanged?.Invoke(this, PlayerCard.Defense);

            //Eventually implement a fail state
            if (PlayerCard.IsDestroyed()) { }
        }

        public bool TryDevoteCard(Card card)
        {
            //If it's your turn, main phase, you haven't already devoted, and there isn't anything on the stack
            if (GameState.CurrentTurn.ID == ID && NumberOfTimesDevoted < MAX_DEVOTION_PER_TURN && GameState.CurrentPhase == Phases.Main && GameState.GameStack.Count == 0)
            {
                Elysium.AddCard(card);
                NumberOfTimesDevoted++;

                return true;
            }

            return false;
        }
        public bool TryTurnElysiumCardUp(Card card)
        {
            if (card.IsTapped || card.IsManaDrained) return false;
            card.IsFaceDown = false;

            return true;
        }

        public virtual void PromptPlayerStateAction(StateAction action) { }
        public virtual void PromptPlayerTargetRequired(EffectAction action) { }
        public virtual void PromptPlayerPayCastingCost(Card card, int manaCost) { }

        public delegate void PlayerHealthChanged(object sender, int num);
        public event PlayerHealthChanged HealthChanged;

        public override string ToString()
        {
            return string.Format("Player #{0}", ID + 1);
        }
    }
}
