from System import *
from WizardWars import *

class CreateToken(Ability):
    def __init__(self):
        self.Type = AbilityTypes.Triggered
        self.Trigger = Triggers.ChangesZone
        self.Origin = Zones.Any
        self.Destination = Zones.Battlefield
    def Execute(self, gameState):
        token = CardInfo("Wobbuffet", ".png", 0)
        token.SetTypes(Types.Creature)
        token.SetSubTypes(SubTypes.Human)
        token.RulesText = "Changeling (This card is every creature type at all times.)"
        token.Attack = 1
        token.Health = 1
        gameState.CreateTokenCreature(token)

card = CardInfo()
card.Name = "Trevor, Blade of Astoria"
card.ImagePath = ".png"
card.Cost = 6
card.RulesText = "When Trevor, Blade of Astoria enters the battlefield, create a 2/2 blue Shapeshifter creature token named Wobbuffet."
card.FlavorText = "Considered even a legend at a young age, Trevor's blade is the guiding beacon for Astoria."
card.SetKeywords("Double strike", "Vigilance", "Haste")
card.SetTypes(Types.Hero, Types.Creature)
card.SetSubTypes(SubTypes.Human)
card.Ability = CreateToken()