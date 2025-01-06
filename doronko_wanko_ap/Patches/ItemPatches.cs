using System.Reflection;
using doronko_wanko_ap.Archipelago;
using doronko_wanko_ap.Handlers;
using UnityEngine;

namespace doronko_wanko_ap.Patches
{
    public class ItemPatches
    {

        public static void GeneratePatches()
        {
            On.DamageAmountManager.FixedUpdate += GameLoop_Update;
            On.ItemBoxManager.ItemUnlock += ItemBoxManager_ItemUnlock;
            On.ItemBoxManager.Start += ItemBoxManager_Start;
        }

        private static void ItemBoxManager_Start(On.ItemBoxManager.orig_Start orig, ItemBoxManager self)
        {
            orig(self);
            ItemHandler.ItemBoxManager = self;
        }

        private static void ItemBoxManager_ItemUnlock(On.ItemBoxManager.orig_ItemUnlock orig, ItemBoxManager self)
        {
            ItemBoxManager.OnItemUnlock.OnNext(value: true);
            FieldInfo t_Amount = typeof(ItemBoxManager).GetField("totalAmount", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo c_Amount = typeof(ItemBoxManager).GetField("currentAmount", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo u_count = typeof(ItemBoxManager).GetField("unlockCount", BindingFlags.Instance | BindingFlags.NonPublic);
            
            int getTargetAmount = (int)typeof(ItemBoxManager).GetMethod("GetTargetAmount", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(self, null);
            Plugin.BepinLogger.LogInfo($"Unlocked item at target {getTargetAmount}");

            t_Amount.SetValue(self, (int)t_Amount.GetValue(self) - getTargetAmount);
            c_Amount.SetValue(self, (int)c_Amount.GetValue(self) - getTargetAmount);
            u_count.SetValue(self,(int)u_count.GetValue(self) + 1);
            string damage_id = Plugin.ArchipelagoClient.LocationHandler.GetDamageGameName((int)u_count.GetValue(self) - 1);
            Plugin.ArchipelagoClient.SendLocation(Plugin.ArchipelagoClient.LocationHandler.GetArchipelagoName(damage_id));
            ItemBoxManager.OnItemChargeUpdate.OnNext(((int)c_Amount.GetValue(self), (int)typeof(ItemBoxManager).GetMethod("GetTargetAmount", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(self, null)));
        }

        private static void GameLoop_Update(On.DamageAmountManager.orig_FixedUpdate orig, DamageAmountManager self)
        {
            Plugin.ArchipelagoClient.ItemHandler.Update();
            orig(self);
        }
        
    }
}
