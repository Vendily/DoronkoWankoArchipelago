using System.IO;
using System.Text;
using doronko_wanko_ap.Archipelago;
using HarmonyLib;
using UnityEngine;

namespace doronko_wanko_ap.Patches
{
    [HarmonyPatch(typeof(TitleScreen), "ShowMainMenu")]
    public class TitleScreen_OpeningScene_Patch
    {
        public static void Prefix()
        {
            Plugin.OnStartGame();
        }

    }

    [HarmonyPatch(typeof(AchievementSaver))]
    [HarmonyPatch("SaveDataPath", MethodType.Getter)]
    public class AchievementSaver_SaveDataPath_Patch
    {
        private static string cached_filename = null;
        public static void Postfix(ref string __result)
        {
            if (ArchipelagoClient.Authenticated)
            {
                if (AchievementSaver_SaveDataPath_Patch.cached_filename == null)
                {
                    StringBuilder filename_builder = new StringBuilder();
                    filename_builder.Append(ArchipelagoClient.ServerData.seed);
                    filename_builder.Append("_");
                    filename_builder.Append(ArchipelagoClient.ServerData.SlotName);
                    filename_builder.Append("_Achievements.dat");
                    AchievementSaver_SaveDataPath_Patch.cached_filename = filename_builder.ToString();
                    Plugin.BepinLogger.LogDebug("Save File: " + AchievementSaver_SaveDataPath_Patch.cached_filename);
                }
                __result = Path.Combine(Application.persistentDataPath, "Saves", AchievementSaver_SaveDataPath_Patch.cached_filename);
            }
        }
    }
}
