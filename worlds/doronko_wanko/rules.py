from Utils import visualize_regions
from worlds.generic.Rules import set_rule

def create_rules(self):
    multiworld = self.multiworld
    player = self.player
    options = self.options

    set_rule(multiworld.get_location("12/12 Paintings", player),
             lambda state: can_get_all_paintings(state,player))

    set_rule(multiworld.get_location("Top of my house", player),
             lambda state: state.has("Nursery Box",player))

    set_rule(multiworld.get_location("Fan to play!", player),
             lambda state: (state.has("Living Room Fan",player) or state.has("Nursery Fan",player)))

    #visualize_regions(self.multiworld.get_region("Menu", player), "doronko_wanko.puml")


def can_get_all_paintings(state,player):
    wall_paintings = (state.has("Living Room Fan",player) and state.has("Nursery Fan",player))
    nursery_painting = state.has("Nursery Box",player)
    train_painting = state.has("Train Wheel",player)
    return wall_paintings and nursery_painting and train_painting

def can_get_all_badges(state,player):
    return (state.can_reach_location("Visited all rooms!",player)
            and state.can_reach_location("Congratulations!",player)
            and state.can_reach_location("SHINING POME",player)
            and state.can_reach_location("All abroad!!",player)
            and state.can_reach_location("Opened!",player)
            and state.can_reach_location("The Wine Deluge",player)
            and state.can_reach_location("Hidden aisle",player)
            and state.can_reach_location("Top of my house",player)
            and state.can_reach_location("Total Damage: P$20,000,000!!",player))