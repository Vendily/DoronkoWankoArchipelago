using System.Collections.Generic;
using System.Reflection;
using System;
using doronko_wanko_ap.Handlers;
using UniRx;
using UnityEngine;
using System.Linq;

namespace doronko_wanko_ap.Patches
{
    public class ItemPatches
    {

        public static void GeneratePatches()
        {
            On.ItemBoxManager.Start += ItemBoxManager_Start;
            On.Mommy.ctor += Mommy_ctor;
            On.Mommy.ChangeArea += Mommy_ChangeArea;
            On.Train.Start += Train_Start;
            On.TemperaturePanel.Start += TemperaturePanel_Start;
        }

        private static void ItemBoxManager_Start(On.ItemBoxManager.orig_Start orig, ItemBoxManager self)
        {
            orig(self);
            Plugin.ArchipelagoClient.ItemHandler.ItemBoxManager = self;
        }

        private static void Mommy_ctor(On.Mommy.orig_ctor orig, Mommy self, DustStainerParams stainerParams, AreaDetector avoidedTarget, GameObject basementModel, GameObject nurseryModel, GameObject kitchenModel, MommyAnimator animator)
        {
            orig(self, stainerParams, avoidedTarget, basementModel, nurseryModel, kitchenModel, animator);
            Plugin.ArchipelagoClient.ItemHandler.Mommy = self;
            try
            {
                Dictionary<Area, GameObject> area2model = (Dictionary<Area, GameObject>)typeof(Mommy).GetField("area2model", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
                foreach (var (area2, gameObject2) in area2model.Select(x => (x.Key, x.Value)))
                {
                    gameObject2.SetActive(false);
                }
            }
            catch (Exception e)
            {
                Plugin.BepinLogger.LogError(e);
            }
        }

        private static bool Mommy_ChangeArea(On.Mommy.orig_ChangeArea orig, Mommy self, Area nextArea)
        {
            if (!Plugin.ArchipelagoClient.ItemHandler.FlagItems[0]) return false;
            return orig(self, nextArea);
        }

        private static void TemperaturePanel_Start(On.TemperaturePanel.orig_Start orig, TemperaturePanel self)
        {
            orig(self);
            self.gameObject.SetActive(false);
            Plugin.ArchipelagoClient.ItemHandler.WineButtons.Add(self);
        }

        private static void Train_Start(On.Train.orig_Start orig, Train self)
        {
            orig(self);
            Plugin.ArchipelagoClient.ItemHandler.Train = self;
            self.gameObject.SetActive(false);
        }

    }
}