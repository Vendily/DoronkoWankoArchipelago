from BaseClasses import CollectionState
from Utils import visualize_regions
from worlds.generic.Rules import set_rule, CollectionRule


def create_rules(self):
    multiworld = self.multiworld
    player = self.player
    options = self.options

    set_rule(multiworld.get_location("12/12 Paintings", player),
             lambda state: can_get_all_paintings(options.logic == "glitched",state,player))

    if options.logic == "standard":
         set_rule(multiworld.get_location("Top of my house", player),
                lambda state: state.has("Nursery Box",player))

    set_rule(multiworld.get_location("Fan to play!", player),
             lambda state: (state.has("Living Room Fan",player) or state.has("Nursery Fan",player)))

    set_rule(multiworld.get_location("DO NOT PRESS", player),
             lambda state: state.has("Wine Button Unlock",player))

    set_rule(multiworld.get_location("Temperature Control is important", player),
             lambda state: state.has("Wine Button Unlock", player))

    set_rule(multiworld.get_location("The Wine Deluge", player),
             lambda state: state.has("Wine Button Unlock", player))

    #visualize_regions(self.multiworld.get_region("Menu", player), "doronko_wanko.puml")


def can_get_all_paintings(is_glitched,state,player):
    wall_paintings = glitched_logic_check(is_glitched,state,lambda s: (s.has("Living Room Fan",player) and s.has("Nursery Fan",player)),lambda s: s.has("Turret Gun",player))
    nursery_painting = glitched_logic_check(is_glitched,state, lambda s: s.has("Nursery Box",player), lambda s: True)
    train_painting = (state.has("Train Unlock",player) and state.has("Train Wheel",player))
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

def glitched_logic_check(is_glitched: bool,state:CollectionState,normal_rule: CollectionRule, glitched_rule: CollectionRule) -> bool:
    if is_glitched:
        return glitched_rule(state)
    return normal_rule(state)