from typing import List, TypedDict, Set, Dict

from typing_extensions import NotRequired,Optional

class LocationInfo(TypedDict):
    name: str
    id: int
    inGameId: NotRequired[str]
    badge: bool
    region: Optional[str]

base_id = 0x561000
# Yes, the badges are titled that way in English, slap a [sic] next to all of them
doronko_wanko_locations: List[LocationInfo] = [
    # Damage Deliveries
    {"name":"Damage Gift 1 (P$1,200)","id":base_id + 1,"inGameId":"Damage0","badge": False},
    {"name":"Damage Gift 2 (P$5,000)","id":base_id + 2,"inGameId":"Damage1","badge": False},
    {"name":"Damage Gift 3 (P$7,500)","id":base_id + 3,"inGameId":"Damage2","badge": False},
    {"name":"Damage Gift 4 (P$10,000) 1","id":base_id + 4,"inGameId":"Damage3","badge": False},
    {"name":"Damage Gift 5 (P$10,000) 2","id":base_id + 5,"inGameId":"Damage4","badge": False},
    {"name":"Damage Gift 6 (P$12,500) 1","id":base_id + 6,"inGameId":"Damage5","badge": False},
    {"name":"Damage Gift 7 (P$12,500) 2","id":base_id + 7,"inGameId":"Damage6","badge": False},
    {"name":"Damage Gift 8 (P$12,500) 3","id":base_id + 8,"inGameId":"Damage7","badge": False},
    {"name":"Damage Gift 9 (P$12,500) 4","id":base_id + 9,"inGameId":"Damage8","badge": False},
    {"name":"Damage Gift 10 (P$12,500) 5","id":base_id + 10,"inGameId":"Damage9","badge": False},
    {"name":"Damage Gift 11 (P$12,500) 6","id":base_id + 11,"inGameId":"Damage10","badge": False},
    {"name":"Damage Gift 12 (P$12,500) 7","id":base_id + 12,"inGameId":"Damage11","badge": False},
    {"name":"Damage Gift 13 (P$20,000,000)","id":base_id + 13,"inGameId":"Damage12","badge": False, "region":"Gold Statue"},
    # Memory Badges
    {"name":"Shake Your Body","id":base_id + 14,"inGameId":"ShakeYourBody","badge": True},
    {"name":"Splaash","id":base_id + 15,"inGameId":"100thSplash","badge": True},
    {"name":"Splaaash","id":base_id + 16,"inGameId":"1000thSplash","badge": True},
    {"name":"Feel the floor!","id":base_id + 17,"inGameId":"FeelTheFloor","badge": True},
    {"name":"I like the floor!","id":base_id + 18,"inGameId":"ILikeTheFloor","badge": True},
    {"name":"I love the floor!!","id":base_id + 19,"inGameId":"ILoveTheFloor","badge": True},
    {"name":"Also small stuffs, too!","id":base_id + 20,"inGameId":"FirstMess","badge": True},
    {"name":"Messy room!","id":base_id + 21,"inGameId":"MessyRoom","badge": True},
    {"name":"Catastrophe!","id":base_id + 22,"inGameId":"CatastropheCatastrophe","badge": True},
    {"name":"I'm an artist!","id":base_id + 23,"inGameId":"Artist","badge": True},
    {"name":"Did it well!","id":base_id + 24,"inGameId":"DidWell","badge": True},
    {"name":"Total Damage: P$10,000","id":base_id + 25,"inGameId":"TotalAmountLv1","badge": True},
    {"name":"Total Damage: P$50,000","id":base_id + 26,"inGameId":"TotalAmountLv2","badge": True},
    {"name":"Total Damage: P$100,000","id":base_id + 27,"inGameId":"TotalAmountLv3","badge": True},
    {"name":"Total Damage: P$20,000,000!!","id":base_id + 28,"inGameId":"TotalAmountLv4","badge": True,"region":"Gold Statue"},
    {"name":"Muddy road","id":base_id + 29,"inGameId":"MuddyRoad","badge": True},
    {"name":"My house is museum","id":base_id + 30,"inGameId":"MyMuseum","badge": True, "region":"12 Paintings"},
    {"name":"Hero landing!","id":base_id + 31,"inGameId":"HeroLanding","badge": True},
    {"name":"Visited all rooms!","id":base_id + 32,"inGameId":"VisitedAllRooms","badge": True, "region":"12 Paintings"},
    {"name":"Congratulations!","id":base_id + 33,"inGameId":"GameClear","badge": True, "region":"12 Paintings"},
    # Kitchen Badges
    {"name":"Something to eat?", "id": base_id + 34, "inGameId":"SometingEat", "badge": True},
    {"name":"Kitchen invasion","id":base_id + 35,"inGameId":"KitchenInvasion","badge": True},
    {"name":"Invading kitchen...","id":base_id + 36,"inGameId":"InvadingKitchen","badge": True},
    {"name":"Kitchen invasion completed","id":base_id + 37,"inGameId":"KitchenInvasionCompleted","badge": True},
    {"name":"Exploding tomato","id":base_id + 38,"inGameId":"ExplodingTomato","badge": True},
    {"name":"Edible mud?","id":base_id + 39,"inGameId":"EdibleMud","badge": True},
    {"name":"I'm not tasty!","id":base_id + 40,"inGameId":"Tasty","badge": True},
    {"name":"Clogged pipe","id":base_id + 41,"inGameId":"CloggedPipe","badge": True},
    {"name":"Small blackhole?","id":base_id + 42,"inGameId":"SmallBlackhole","badge": True},
    {"name":"You tired, mom?","id":base_id + 43,"inGameId":"YouTiredMom","badge": True,"region":"Mom"},
    {"name":"Chores after dinner","id":base_id + 44,"inGameId":"ChoresDinner","badge": True},
    # Living Room Badges
    {"name":"Living room","id":base_id + 45,"inGameId":"LivingRoom","badge": True},
    {"name":"Make the living room dirty!","id":base_id + 46,"inGameId":"MakeTheLivingRoomDirty","badge": True},
    {"name":"Muddy muddy living room","id":base_id + 47,"inGameId":"MuddyMuddyLivingRoom","badge": True},
    {"name":"Dominated living room!","id":base_id + 48,"inGameId":"DominatedLivingRoom","badge": True},
    {"name":"My Family!","id":base_id + 49,"inGameId":"MyFamily","badge": True},
    {"name":"Don't be stuck to TV!","id":base_id + 50,"inGameId":"DontTv","badge": True},
    {"name":"I hate caffeine...","id":base_id + 51,"inGameId":"IHate","badge": True},
    {"name":"Pome Speaker","id":base_id + 52,"inGameId":"PomeSpeaker","badge": True},
    {"name":"little sour...","id":base_id + 53,"inGameId":"littleSour","badge": True},
    # Any Robot Cleaner can trigger this
    {"name":"My hardworking co-workers","id":base_id + 54,"inGameId":"MyHardworkingCoworkers","badge": True},
    # Any Fan can trigger this
    {"name":"Fan to play!","id":base_id + 55,"inGameId":"ItsFanny","badge": True},
    {"name":"Welcoming Pome","id":base_id + 56,"inGameId":"SwipingAll","badge": True},
    {"name":"Dancing on the desk!","id":base_id + 57,"inGameId":"OnlyDesk","badge": True},
    {"name":"SHINING POME","id":base_id + 58,"inGameId":"BigPome","badge": True,"region":"12 Paintings"},
    # Nursery Badges
    {"name":"Big reversal!","id":base_id + 59,"inGameId":"BigReversal","badge": True},
    {"name":"Better than bears","id":base_id + 60,"inGameId":"BetterThanBears","badge": True},
    {"name":"Shark vs. Pomeranian","id":base_id + 61,"inGameId":"SharkVsPomeranian","badge": True},
    {"name":"Pomeceratops","id":base_id + 62,"inGameId":"Pomeceratops","badge": True},
    {"name":"My fluffy fellows","id":base_id + 63,"inGameId":"MyFluffyFellows","badge": True},
    {"name":"Colourful balloons!","id":base_id + 64,"inGameId":"ColorfulBalloons","badge": True},
    {"name":"Attack+10","id":base_id + 65,"inGameId":"Attack10","badge": True},
    {"name":"My siblings room!!","id":base_id + 66,"inGameId":"BrotherRoom","badge": True},
    {"name":"More colorful!","id":base_id + 67,"inGameId":"MoreColorful","badge": True},
    {"name":"Much colorful!","id":base_id + 68,"inGameId":"MuchColorful","badge": True},
    {"name":"The most colorful!","id":base_id + 69,"inGameId":"TheMostColorful","badge": True},
    # Any Train Wheel can trigger this
    {"name":"What's this wheel used for?","id":base_id + 70,"inGameId":"WhatsFor","badge": True},
    {"name":"Two wheels left!","id":base_id + 71,"inGameId":"TwoWheelsLeft","badge": True,"region":"Train"},
    {"name":"One wheel left!","id":base_id + 72,"inGameId":"OneWheelLeft","badge": True,"region":"Train"},
    {"name":"All abroad!!","id":base_id + 73,"inGameId":"AllAbroad","badge": True,"region":"Fixed Train"},
    {"name":"Top of my house","id":base_id + 74,"inGameId":"TopOfMyHouse","badge": True},
    {"name":"Lot's of footprints","id":base_id + 75,"inGameId":"Footprints","badge": True,"region":"Fixed Train"},
    {"name":"Mom?","id":base_id + 76,"inGameId":"MomB","badge": True,"region":"Mom"},
    {"name":"Opened!","id":base_id + 77,"inGameId":"Opened","badge": True, "region":"Fixed Train"},
    # Basement Living Room
    {"name":"Pome is in business!","id":base_id + 78,"inGameId":"PomeBuisness","badge": True},
    {"name":"Good at painting!","id":base_id + 79,"inGameId":"GoodAtPaintng","badge": True},
    {"name":"Champagne Time!","id":base_id + 80,"inGameId":"TakeALookOnMe","badge": True},
    {"name":"Relaxing room!","id":base_id + 81,"inGameId":"RelaxingRoom","badge": True},
    {"name":"Muddy basement!","id":base_id + 82,"inGameId":"MuddyBasement","badge": True},
    {"name":"Flooded basement","id":base_id + 83,"inGameId":"FloodedBasement","badge": True},
    {"name":"I dominated basement!","id":base_id + 84,"inGameId":"IDominatedBasement","badge": True},
    {"name":"Pome delivery","id":base_id + 85,"inGameId":"PomeDelivery","badge": True},
    {"name":"Good night, mom!","id":base_id + 86,"inGameId":"GoodNightMom","badge": True,"region":"Mom"},
    {"name":"What's this circle...","id":base_id + 87,"inGameId":"WhatsThisCircle","badge": True},
    # Wine Cellar
    {"name":"Lots of vintage wines","id":base_id + 88,"inGameId":"LotsOfVintageWines","badge": True},
    {"name":"A vintage pome","id":base_id + 89,"inGameId":"AVintagePome","badge": True},
    {"name":"'99 POME","id":base_id + 90,"inGameId":"99POME","badge": True},
    {"name":"I dominated wine cellar!","id":base_id + 91,"inGameId":"IDominatedWineCeller","badge": True},
    # Either temperature panel can trigger this
    {"name":"DO NOT PRESS","id":base_id + 92,"inGameId":"DONOTPRESS","badge": True},
    {"name":"Temperature Control is important","id":base_id + 93,"inGameId":"TotalDamage1000","badge": True},
    {"name":"The Wine Deluge","id":base_id + 94,"inGameId":"TotalDamage10000","badge": True, "region":"Fixed Train"},
    {"name":"Hidden aisle","id":base_id + 95,"inGameId":"HiddenAisle","badge": True, "region":"Fixed Train"},
]

group_table: Dict[str, Set[str]] = {
    "Damage": {"Damage Gift 1 (P$1,200)","Damage Gift 2 (P$5,000)","Damage Gift 3 (P$7,500)",
               "Damage Gift 4 (P$10,000) 1","Damage Gift 5 (P$10,000) 2",
               "Damage Gift 6 (P$12,500) 1","Damage Gift 7 (P$12,500) 2","Damage Gift 8 (P$12,500) 3","Damage Gift 9 (P$12,500) 4",
               "Damage Gift 10 (P$12,500) 5","Damage Gift 11 (P$12,500) 6","Damage Gift 12 (P$12,500) 7",
               "Damage Gift 13 (P$20,000,000)"}
}