using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using doronko_wanko_ap.Archipelago;
using Newtonsoft.Json;

namespace doronko_wanko_ap.Patches
{
    public class ConnectionPatches
    {
        public static void GeneratePatches()
        {
            On.TitleScreen.LoadMainScene += TitleScreen_LoadMainScene;
            On.TitleScreen.LoadOpeningScene += TitleScreen_LoadOpeningScene;
        }

        private static void TitleScreen_LoadOpeningScene(On.TitleScreen.orig_LoadOpeningScene orig, TitleScreen self)
        {
            orig(self);
            OnStartGame();
        }

        private static void TitleScreen_LoadMainScene(On.TitleScreen.orig_LoadMainScene orig, TitleScreen self)
        {
            orig(self);
            OnStartGame();
        }

        private static void OnStartGame()
        {
            string locationsPath = Path.Combine(Environment.CurrentDirectory, "AP.json");

            if (!File.Exists(locationsPath))
            {
                Plugin.BepinLogger.LogError($"Failed to load connection data {locationsPath}");
                throw new FileNotFoundException("Failed to load connection data", locationsPath);
            }

            string json = File.ReadAllText(locationsPath);
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            ArchipelagoClient.ServerData.Uri = data["Url"];
            ArchipelagoClient.ServerData.SlotName = data["SlotName"];
            ArchipelagoClient.ServerData.Password = data["Password"];
            Plugin.ArchipelagoClient.Connect();
        }
    }
}
