using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace doronko_wanko_ap.Handlers
{
    public class LocationHandler
    {
        private readonly string DATA_PATH = Path.Combine(Environment.CurrentDirectory, "BepInEx", "Plugins", "doronko_wanko_ap", "data");
        private readonly Dictionary<string, string> locations;
        public int damageIndex { get; set; }
        public string GetDamageGameName(int index)
        {
            return $"Damage{index}";
        }

        public string GetArchipelagoName(string inGameName)
        {
            return locations[inGameName];
        }

        public LocationHandler()
        {
            locations = LoadLocationsFromFile();
            List<int> templist = new List<int>();
            damageIndex = 0;
        }

        private Dictionary<string, string> LoadLocationsFromFile()
        {
            string locationsPath = Path.Combine(DATA_PATH, "locations.json");

            if (!File.Exists(locationsPath))
            {
                throw new FileNotFoundException("Failed to load location data", locationsPath);
            }

            string json = File.ReadAllText(locationsPath);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
    }
}
