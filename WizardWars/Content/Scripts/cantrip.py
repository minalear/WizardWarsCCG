﻿import System
from WizardWars import *

class DrawCard(Ability):
	def __init__(self):
		self.Type = AbilityTypes.Cast
	def Execute(self, gameState, card):
		card.Controller.DrawCards(1)

card = CardInfo("Cantrip", "cantrip.png", 1)
card.RulesText = "Draw a card."
card.FlavorText = "Even the most powerful mages still recall their first spells."
card.SetTypes(Types.Spell)
card.SetSubTypes(SubTypes.Interrupt)
card.SetAbilities(DrawCard())