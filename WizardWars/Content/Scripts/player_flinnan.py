import System
from WizardWars import *

class DrawCard(Ability):
	def __init__(self):
		self.Type = AbilityTypes.Activated
	def Execute(self, gameState, card):
		card.Controller.DrawCards(1)

card = CardInfo("Flinnan, the Blue Mage", "player_flinnan.png", 0)
card.SetTypes(Types.Player)
card.SetAbilities(DrawCard())