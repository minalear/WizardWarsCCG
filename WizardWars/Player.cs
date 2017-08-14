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
            if (PlayerCard.IsDestroyed())
            {
                Console.WriteLine("{0} has died!", this);
            }
        }

        public bool TryDevoteCard(Card card)
        {
            if (GameState.CurrentTurn.ID == ID && NumberOfTimesDevoted < MAX_DEVOTION_PER_TURN && GameState.CurrentPhase == Phases.Main)
            {
                Elysium.AddCard(card);
                NumberOfTimesDevoted++;

                return true;
            }

            return false;
        }

        public virtual void PromptPlayerStateAction(StateAction action)
        {
            //PRETEND THIS IS AI PASSING
            if (ID != 1) //PlayerTwo ID
            {
                Console.WriteLine("Current Stack: {0}", action);
            }
        }
        public virtual void PromptPlayerTargetRequired(EffectAction action)
        {
            if (ID != 1)
            {
                Console.WriteLine("Select target for {0}", action);
            }
        }
        public virtual void PromptPlayerPayCastingCost(Card card, int manaCost) { }

        public override string ToString()
        {
            return string.Format("Player #{0}", ID + 1);
        }
    }
}
