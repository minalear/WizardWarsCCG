import System
from WizardWars import *

card = CardInfo()
card.Name = "Lightning Helix"
card.ImagePath = "lightning_helix.png"
card.Cost = 2
card.RulesText = "Deal 3 damage to target creature or player.  You gain 3 life."
card.FlavorText = "Combat Medics can weave powerful destruction and restorative energies into one powerful spell."
card.Types = System.Array[Types]([Types.Spell])
card.SubTypes = System.Array[SubTypes]([SubTypes.Interrupt])