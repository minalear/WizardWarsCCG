using System;

namespace WizardWars
{
    public class Ability
    {
        public AbilityTypes Type;
        public virtual void Execute(GameState gameState) { }
    }

    public enum AbilityTypes
    {
        Cast,
        Triggered,
        Activated,
        Static
    }
    public enum Triggers
    {
        ChangesZone
    }
    public enum Zones
    {
        Default,
        Any,
        Battlefield,
        Hand,
        Deck,
        Graveyard,
        Elysium,
        Exile,
        Temple
    }
}
