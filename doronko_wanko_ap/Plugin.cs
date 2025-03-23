using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using doronko_wanko_ap.Archipelago;
using doronko_wanko_ap.Patches;
using HarmonyLib;
using Newtonsoft.Json;

namespace doronko_wanko_ap
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.vendily.doronko_wanko_ap";
        public const string PluginName = "DORONKO WANKO Archipelago";
        public const string PluginVersion = "0.3.1";

        public const string ModDisplayInfo = $"{PluginName} v{PluginVersion}";
        private const string APDisplayInfo = $"Archipelago v{ArchipelagoClient.APVersion}";
        public static ManualLogSource BepinLogger;
        public static ArchipelagoClient ArchipelagoClient;

        private void Awake()
        {
            // Plugin startup logic
            BepinLogger = Logger;
            BepinLogger.LogInfo("Start Loading");
            try
            {
                ArchipelagoClient = new ArchipelagoClient();
                BepinLogger.LogInfo("Created Client");
            }
            catch (Exception ex)
            {

                BepinLogger.LogError(ex);
            }
            try
            {
                BepinLogger.LogInfo("Creating Patches");
                new Harmony(PluginGUID).PatchAll(GetType().Assembly);
            }
            catch (Exception ex)
            {
                BepinLogger.LogError(ex);
            }
            try
            {
                BepinLogger.LogInfo("Location Hooks");
                LocationPatches.GeneratePatches();
            }
            catch (Exception ex)
            {

                BepinLogger.LogError(ex);
            }
            BepinLogger.LogInfo($"{ModDisplayInfo} Loaded!");
        }

        public static void OnStartGame()
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