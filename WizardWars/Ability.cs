using System.Collections.Generic;

namespace WizardWars
{
    public class Ability
    {
        public AbilityTypes Type;

        //Targeting Variables
        public int NumTargets = 1;
        public Card Target { get { return Targets[0]; } }
        public List<Card> Targets;
        public bool TargetRequired = false;

        //Trigger Ability Variables
        public Triggers Trigger;
        public Zones Origin, Destination;
        
        public virtual void Execute(GameState gameState, Card card) { }
        public virtual bool IsValidCard(Card source, Card card) { return false; }
        public virtual bool IsValidTarget(Card source, Card target) { return false; }

        public bool IsValidZones(Zones origin, Zones destination)
        {
            return ((Origin == origin || Origin == Zones.Any) && 
                    (Destination == destination || Destination == Zones.Any));
        }
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
        None,
        ChangesZone,
        Attack,
        Block,
        Cast,
        PhaseChange
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
