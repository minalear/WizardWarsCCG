using System;

namespace WizardWars
{
    public class CastingController
    {
        private Cost currentCost, paidCost;
        private Card stagedCard;
        private Ability stagedAbility;

        private CastingState castingState = CastingState.None;

        public GameState GameState { get; private set; }
        public Player Player { get; private set; }
        public CastingState CastingState { get { return castingState; } }
        public int RemainingManaCost { get { return currentCost.ManaValue - paidCost.ManaValue; } }

        public CastingController(GameState gameState, Player player)
        {
            GameState = GameState;
            Player = player;

            currentCost = Cost.Zero;
            paidCost = Cost.Zero;
        }

        public void StageCard(Card card)
        {
            stagedCard = card;
            castingState = CastingState.CardCast;

            currentCost = new Cost(card);

            if (costPaid())
                finishCasting();
        }
        public void StageAbility(Card card, Ability ability)
        {
            stagedCard = card;
            stagedAbility = ability;
            castingState = CastingState.Ability;

            currentCost = new Cost(ability);

            if (costPaid())
                finishCasting();
        }

        public void PayMana(int amount)
        {
            if (castingState == CastingState.None) return;
            paidCost.ManaValue += amount;

            if (costPaid())
                finishCasting();
        }

        public void Cancel()
        {
            int refund = paidCost.ManaValue;
            if (castingState == CastingState.CardCast)
                Canceled?.Invoke(new CastingEventArgs(stagedCard, refund));
            else if (castingState == CastingState.Ability)
                Canceled?.Invoke(new CastingEventArgs(stagedCard, stagedAbility, refund));

            cleanup();
        }

        private void finishCasting()
        {
            int refund = paidCost.ManaValue - currentCost.ManaValue;
            if (castingState == CastingState.CardCast)
                CostPaid?.Invoke(new CastingEventArgs(stagedCard, refund));
            else if (castingState == CastingState.Ability)
                CostPaid?.Invoke(new CastingEventArgs(stagedCard, stagedAbility, refund));

            cleanup(); //Cleanup
        }
        private bool costPaid()
        {
            if (castingState == CastingState.None) return false;

            return (paidCost.ManaValue >= currentCost.ManaValue);
        }
        private void cleanup()
        {
            castingState = CastingState.None;
            paidCost = Cost.Zero;
            currentCost = Cost.Zero;
        }
        
        public event Action<CastingEventArgs> CostPaid, Canceled;
    }

    public class Cost
    {
        public int ManaValue { get; set; }

        public Cost(int value)
        {
            ManaValue = value;
        }
        public Cost(Card card)
        {
            ManaValue = card.Cost;
        }
        public Cost(Ability ability)
        {
            ManaValue = ability.Cost;
        }

        private static Cost _zero = new Cost(0);
        public static Cost Zero { get { return _zero; } }
    }
    public class CastingEventArgs : EventArgs
    {
        public Card Card { get; private set; }
        public Ability Ability { get; private set; }
        public int Refund { get; private set; }

        public CastingEventArgs(Card card, int manaRefund)
        {
            Card = card;
            Refund = manaRefund;
        }
        public CastingEventArgs(Card card, Ability ability, int manaRefund)
        {
            Card = card;
            Ability = ability;
            Refund = manaRefund;
        }
    }

    public enum CastingState { None, Ability, CardCast }
}
