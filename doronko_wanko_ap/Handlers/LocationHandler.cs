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
        private readonly List<string> damagelist;

        public string GetDamageGameName(int index)
        {
            return damagelist[index];
        }

        public string GetArchipelagoName(string inGameName)
        {
            return locations[inGameName];
        }

        public LocationHandler()
        {
            locations = LoadLocationsFromFile();
            List<int> templist = new List<int>();
            foreach ( string k in locations.Keys)
            {
                if (!k.StartsWith("Damage")) continue;
                templist.Add(int.Parse(k.Substring("Damage".Length)));
            }
            templist.Sort();
            damagelist = new List<string>();
            foreach (int x in templist)
            {
                damagelist.Add("Damage" + x);
            }
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
