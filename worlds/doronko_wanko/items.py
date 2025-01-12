from BaseClasses import ItemClassification
from typing import TypedDict, List, Set, Dict

class ItemDict(TypedDict):
    name: str
    id: int
    classification: ItemClassification

base_id = 0x560000

doronko_wanko_items: List[ItemDict] = [
    {"name": "Living Room Fan", "id": base_id + 1, "classification": ItemClassification.progression},
    {"name": "Party Outfit", "id": base_id + 2, "classification": ItemClassification.filler},
    {"name": "Train Wheel", "id": base_id + 3, "classification": ItemClassification.progression},
    {"name": "Elephant Hat", "id": base_id + 4, "classification": ItemClassification.useful},
    {"name": "Nursery Box", "id": base_id + 5, "classification": ItemClassification.progression},
    {"name": "Whale Hat", "id": base_id + 6, "classification": ItemClassification.useful},
    {"name": "Nursery Fan", "id": base_id + 7, "classification": ItemClassification.progression},
    {"name": "Cannon Hat", "id": base_id + 8, "classification": ItemClassification.useful},
    {"name": "Paint Robot Cleaners", "id": base_id + 9, "classification": ItemClassification.useful},
    {"name": "Turret Gun", "id": base_id + 10, "classification": ItemClassification.useful},
    {"name": "Trophy", "id": base_id + 11, "classification": ItemClassification.useful},
    {"name": "Giant Gold Statue", "id": base_id + 12, "classification": ItemClassification.progression},
    {"name": "Gold Statue Hat", "id": base_id + 13, "classification": ItemClassification.filler},
    # Flag items
    {"name": "Mom Unlock", "id": base_id + 14, "classification": ItemClassification.progression},
    {"name": "Wine Button Unlock", "id": base_id + 15, "classification": ItemClassification.progression},
    {"name": "Train Unlock", "id": base_id + 16, "classification": ItemClassification.progression},
    # Filler Damage items
    {"name": "P$10 Damage", "id": base_id + 17, "classification": ItemClassification.filler},
    {"name": "P$100 Damage", "id": base_id + 18, "classification": ItemClassification.filler},
    {"name": "P$250 Damage", "id": base_id + 19, "classification": ItemClassification.filler},
    {"name": "P$500 Damage", "id": base_id + 20, "classification": ItemClassification.filler}
]

group_table: Dict[str, Set[str]] = {
    "Damage": {"P$10 Damage","P$100 Damage","P$250 Damage","P$500 Damage"}
}