using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace doronko_wanko_ap.Archipelago
{

    public class ArchipelagoData
    {
        public string Uri { get; internal set; }
        public string SlotName { get; internal set; }
        public string Password { get; internal set; }
        public int Index;

        public List<long> CheckedLocations;

        public GoalType Goal
        {
            get
            {
                return serverSettings.goal;
            }
        }

        /// <summary>
        /// seed for this archipelago data. Can be used when loading a file to verify the session the player is trying to
        /// load is valid to the room it's connecting to.
        /// </summary>
        public string seed { get; private set; }

        private Dictionary<string, object> slotData;

        private ServerSettings serverSettings;

        public bool NeedSlotData => slotData == null;

        public ArchipelagoData()
        {
            Uri = "localhost";
            SlotName = "Player1";
            CheckedLocations = new();
        }

        public ArchipelagoData(string uri, string slotName, string password)
        {
            Uri = uri;
            SlotName = slotName;
            Password = password;
            CheckedLocations = new();
        }


        /// <summary>
        /// assigns the slot data and seed to our data handler. any necessary setup using this data can be done here.
        /// </summary>
        /// <param name="roomSlotData">slot data of your slot from the room</param>
        /// <param name="roomSeed">seed name of this session</param>
        public void SetupSession(Dictionary<string, object> roomSlotData, string roomSeed)
        {
            slotData = roomSlotData;
            serverSettings = ((JObject)slotData["settings"]).ToObject<ServerSettings>();
            seed = roomSeed;
        }

        /// <summary>
        /// returns the object as a json string to be written to a file which you can then load
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public enum GoalType
    {
        Cake,
        Badge
    }

    public class ServerSettings
    {
        public readonly GoalType goal;

        [JsonConstructor]
        public ServerSettings(GoalType goal)
        {
            this.goal = goal;
        }

        public ServerSettings()
        {
            goal = GoalType.Badge;
        }
    }
}