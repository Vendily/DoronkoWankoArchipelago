from dataclasses import dataclass

from Options import Choice, Toggle, PerGameCommonOptions

class Goal(Choice):
    """Choose the end goal.
    Cake: Reveal all the hidden paintings and interact with the cake
    Badge: Get all Badges"""
    display_name = "Goal"
    option_cake = 0
    option_badge = 1
    default = 1

class FillerDamageAmount(Choice):
    """The number of coins that will be in each filler damage item."""
    display_name = "Damage per Filler Item"
    option_10_damage = 0
    option_100_damage = 1
    option_250_damage = 2
    option_500_damage = 3
    default = 1

@dataclass
class DoronkoWankoOptions(PerGameCommonOptions):
    goal: Goal
    filler_damage_amount: FillerDamageAmount