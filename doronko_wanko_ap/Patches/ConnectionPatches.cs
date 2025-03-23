using System;
using System.Collections.Generic;
using System.IO;
using doronko_wanko_ap.Archipelago;
using HarmonyLib;
using Newtonsoft.Json;

namespace doronko_wanko_ap.Patches
{
    [HarmonyPatch(typeof(TitleScreen), "LoadOpeningScene")]
    public class TitleScreen_OpeningScene_Patch
    {

        [HarmonyPostfix]
        public static void Postfix()
        {
            Plugin.OnStartGame();
        }

    }

    [HarmonyPatch(typeof(TitleScreen), "LoadMainScene")]
    public class TitleScreen_MainScene_Patch
    {

        [HarmonyPostfix]
        public static void Postfix()
        {
            Plugin.OnStartGame();
        }

    }
}
