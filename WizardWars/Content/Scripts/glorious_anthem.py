from System import *
from WizardWars import *

class BuffCreatures(Ability):
    def __init__(self):
        self.Type = AbilityTypes.Static
    def Execute(self, gameState, card):
        for permanent in card.Controller.Field:
            if permanent.IsOfType(Types.Creature):
                permanent.BonusAttack += 1
                permanent.BonusHealth += 1
            if permanent.IsOfSubType(SubTypes.Soldier):
                permanent.BonusAttack += 1

card = CardInfo()
card.Name = "Glorious Anthem"
card.ImagePath = "glorious_anthem.png"
card.Cost = 4
card.RulesText = "Creatures you control gain +1/+1.  Soldier creatures gain an additional +1/+0."
card.FlavorText = "Knights of Astoria follow one simple creed; Anything for the homeland."
card.SetTypes(Types.Relic)
card.SetAbilities(BuffCreatures())