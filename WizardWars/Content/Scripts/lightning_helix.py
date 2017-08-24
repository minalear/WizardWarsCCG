import System
from WizardWars import *

class DealDamage(Ability):
	def __init__(self):
		self.Type = AbilityTypes.Cast
		self.TargetRequired = True
	def IsValidTarget(self, source, target):
		return target.IsType(Types.Player) or target.IsType(Types.Creature)
	def Execute(self, gameState, source):
		self.Target.Damage(source, 3)
		source.Controller.Heal(source, 3)

card = CardInfo()
card.Name = "Lightning Helix"
card.ImagePath = "lightning_helix.png"
card.Cost = 2
card.RulesText = "Deal 3 damage to target creature or player.  You gain 3 life."
card.FlavorText = "Combat Medics can weave powerful destruction and restorative energies into one powerful spell."
card.SetTypes(Types.Spell)
card.SetSubTypes(SubTypes.Interrupt)
card.SetAbilities(DealDamage())