using System;
using BepInEx;
using BepInEx.Logging;
using doronko_wanko_ap.Archipelago;
using doronko_wanko_ap.Patches;

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
                BepinLogger.LogInfo("Connection Patches");
                ConnectionPatches.GeneratePatches();
            }
            catch (Exception ex)
            {

                BepinLogger.LogError(ex);
            }
            try
            {
                BepinLogger.LogInfo("Item Patches");
                ItemPatches.GeneratePatches();
            }
            catch (Exception ex)
            {

                BepinLogger.LogError(ex);
            }
            try
            {
                BepinLogger.LogInfo("Location Patches");
                LocationPatches.GeneratePatches();
            }
            catch (Exception ex)
            {

                BepinLogger.LogError(ex);
            }
            BepinLogger.LogInfo($"{ModDisplayInfo} Loaded!");
        }
    }
}