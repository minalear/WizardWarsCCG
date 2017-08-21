import System
from WizardWars import *

card = CardInfo()
card.Name = "Cantrip"
card.ImagePath = "cantrip.png"
card.Cost = 1
card.RulesText = "Draw a card."
card.FlavorText = "Even the most powerful mages still recall their first spells."
card.Types = System.Array[Types]([Types.Spell])
card.SubTypes = System.Array[SubTypes]([SubTypes.Interrupt])