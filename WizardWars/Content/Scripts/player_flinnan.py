import System
from WizardWars import *

class DrawCard(Ability):
    def __init__(self):
        self.Name = "Draw a card."
        self.Type = AbilityTypes.Activated
        self.Cost = 2
    def Execute(self, gameState, card):
        card.Controller.DrawCards(1)
class AshnodValue(Ability):
    def __init__(self):
        self.Name = "Sacrifice a creature, add 2 mana to your mana pool."
        self.Prompt = "Sacrifice a creature."
        self.Type = AbilityTypes.Activated
        self.TargetRequired = True
    def IsValidTarget(self, source, target):
        return target.IsType(Types.Creature) and target.Controller.ID == source.Controller.ID
    def Execute(self, gameState, card):
        self.Target.Destroy()
        card.Controller.Mana += 2

card = CardInfo("Flinnan, the Blue Mage", "player_flinnan.png", 0)
card.SetTypes(Types.Player)
card.SetAbilities(DrawCard(), AshnodValue())