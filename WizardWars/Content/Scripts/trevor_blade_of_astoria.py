from System import *
from WizardWars import *

class CreateToken(Ability):
    def __init__(self):
        self.Type = AbilityTypes.Triggered
        self.Trigger = Triggers.ChangesZone
        self.Origin = Zones.Any
        self.Destination = Zones.Battlefield
    def IsValidCard(self, source, card):
        return (source.ID == card.ID)
    def Execute(self, gameState, card):
        token = CardInfo("Wobbuffet", ".png", 0)
        token.SetTypes(Types.Creature)
        token.SetSubTypes(SubTypes.Human)
        token.RulesText = "Changeling (This card is every creature type at all times.)"
        token.Attack = 2
        token.Health =2
        gameState.CreateTokenCreature(token, card.Controller)

card = CardInfo()
card.Name = "Trevor, Blade of Astoria"
card.ImagePath = ".png"
card.Cost = 6
card.Attack = 4
card.Health = 4
card.RulesText = "When Trevor, Blade of Astoria enters the battlefield, create a 2/2 blue Shapeshifter creature token named Wobbuffet."
card.FlavorText = "Considered even a legend at a young age, Trevor's blade is the guiding beacon for Astoria."
card.SetKeywords("Double strike", "Vigilance", "Haste")
card.SetTypes(Types.Hero, Types.Creature)
card.SetSubTypes(SubTypes.Human, SubTypes.Soldier)
card.SetAbilities(CreateToken())