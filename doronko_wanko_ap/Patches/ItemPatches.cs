using System.Collections.Generic;
using System.Reflection;
using System;
using doronko_wanko_ap.Handlers;
using UniRx;
using UnityEngine;
using System.Linq;
using HarmonyLib;

namespace doronko_wanko_ap.Patches
{
    [HarmonyPatch(typeof(ItemBoxManager), "Start")]
    public class ItemBoxManager_Start_Patch
    {
        public static void Postfix(ItemBoxManager __instance)
        {
            Plugin.ArchipelagoClient.ItemHandler.ItemBoxManager = __instance;
        }

    }

    [HarmonyPatch(typeof(Mommy))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(DustStainerParams), typeof(AreaDetector), typeof(GameObject), typeof(GameObject), typeof(GameObject), typeof(MommyAnimator) })]
    public class Mommy_ctor_Patch
    {
        public static void Postfix(Mommy __instance, Dictionary<Area, GameObject> ___area2model)
        {
            Plugin.ArchipelagoClient.ItemHandler.Mommy = __instance;
            try
            {
                foreach (var (area2, gameObject2) in ___area2model.Select(x => (x.Key, x.Value)))
                {
                    gameObject2.SetActive(false);
                }
            }
            catch (Exception e)
            {
                Plugin.BepinLogger.LogError(e);
            }
        }

    }

    [HarmonyPatch(typeof(Mommy), "ChangeArea")]
    public class Mommy_ChangeArea_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            if (!Plugin.ArchipelagoClient.ItemHandler.FlagItems[0])
            {
                __result = false;
                return false;
            }
            return true;
        }

    }

    [HarmonyPatch(typeof(Train), "Start")]
    public class Train_Start_Patch
    {

        public static void Postfix(Train __instance)
        {
            Plugin.ArchipelagoClient.ItemHandler.Train = __instance;
            __instance.gameObject.SetActive(false);
        }

    }

    [HarmonyPatch(typeof(TemperaturePanel), "Start")]
    public class TemperaturePanel_Start_Patch
    {

        public static void Postfix(TemperaturePanel __instance)
        {
            __instance.gameObject.SetActive(false);
            Plugin.ArchipelagoClient.ItemHandler.WineButtons.Add(__instance);
        }

    }
}