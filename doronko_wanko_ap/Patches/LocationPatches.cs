using UniRx;
using TMPro;
using HarmonyLib;
using System;

namespace doronko_wanko_ap.Patches
{
    /*
     * For some reason, when I add a prefix here, it causes Damage filler items to be added endlessly...
     * Shame, this way would have been easy to implement
    [HarmonyPatch(typeof(ItemBoxManager), "Start")]
    public class ItemBoxManager_DamageOverflow_Patch
    {
        public static int overflowAmount { get; set; }

        public static void Prefix(ItemBoxManager __instance, int ___totalAmount)
        {
            overflowAmount = 0;
            DamageAmountManager.OnStackCreateOrDestory.Where(((bool IsCreate, int Amount) info) => !info.IsCreate).Subscribe(delegate ((bool IsCreate, int Amount) info)
            {
                int targetAmount = Traverse.Create(__instance).Method("GetTargetAmount", new Type[] { typeof(int) }).GetValue<int>();
                Plugin.BepinLogger.LogDebug($"Target: {targetAmount}, Total: {___totalAmount}, Stack: {info.Amount}");
                if ((___totalAmount + info.Amount) > targetAmount)
                {
                    overflowAmount = (___totalAmount + info.Amount) - targetAmount;
                    Plugin.BepinLogger.LogDebug($"Overflow Amount: {overflowAmount}");
                }
            });
        }

    }*/

    [HarmonyPatch(typeof(ItemBoxManager), "ItemUnlock")]
    public class ItemBoxManager_ItemUnlock_Patch
    {

        public static bool Prefix(ItemBoxManager __instance, ref int ___totalAmount, ref int ___currentAmount, ref int ___unlockCount)
        {
            ItemBoxManager.OnItemUnlock.OnNext(value: true);
            Traverse getTargetAmount = Traverse.Create(__instance).Method("GetTargetAmount", new Type[] {  typeof(int) });
            int targetAmount = getTargetAmount.GetValue<int>(0);
            Plugin.BepinLogger.LogDebug($"Unlocked item at target {targetAmount}");

            ___totalAmount-= targetAmount;
            ___currentAmount-= targetAmount;
            ___unlockCount++;

            string damage_id = Plugin.ArchipelagoClient.LocationHandler.GetDamageGameName(___unlockCount - 1);
            Plugin.ArchipelagoClient.LocationHandler.damageIndex = ___unlockCount;
            Plugin.ArchipelagoClient.SendLocation(Plugin.ArchipelagoClient.LocationHandler.GetArchipelagoName(damage_id));
            ItemBoxManager.OnItemChargeUpdate.OnNext((___currentAmount, getTargetAmount.GetValue<int>(0)));
            /*
            if (ItemBoxManager_DamageOverflow_Patch.overflowAmount > 0)
            {
                int temp_overflow = ItemBoxManager_DamageOverflow_Patch.overflowAmount;
                ItemBoxManager_DamageOverflow_Patch.overflowAmount = 0;
                Plugin.BepinLogger.LogDebug($"Temp Overflow amount: {temp_overflow}; Overflow Amount: {ItemBoxManager_DamageOverflow_Patch.overflowAmount}");
            }*/
            return false; // The original should not be run
        }

    }

    [HarmonyPatch(typeof(ItemBoxUINotifier), "Start")]
    public class ItemBoxUINotifier_Start_Patch
    {

        public static void Postfix(ItemBoxUINotifier __instance, TextMeshProUGUI ___nextAmount)
        {
            ItemBoxManager.OnItemChargeUpdate.Subscribe(delegate ((int CurrentAmount, int TargetAmount) info)
            {
                int damage_idx = Plugin.ArchipelagoClient.LocationHandler.damageIndex;
                if (damage_idx <= 12) // only 13 gifts
                {
                    // add 1 to visually look the same as AP World
                    ___nextAmount.text = $"(Damage Gift {damage_idx + 1}) {___nextAmount.text}";
                }
            });
        }

    }

    [HarmonyPatch(typeof(DamageAmountManager), "FixedUpdate")]
    public class DamageAmountManager_FixedUpdate_Hook
    {

        public static void Prefix()
        {
            Plugin.ArchipelagoClient.ItemHandler.Update();
        }

    }
    public class LocationPatches
    {

        public static void GeneratePatches()
        {
            AchievementEvents.OnNotificationRequired.Subscribe(delegate (Achievement Achievement)
            {
                Plugin.ArchipelagoClient.SendLocation(Plugin.ArchipelagoClient.LocationHandler.GetArchipelagoName(Achievement.Id));
                Plugin.ArchipelagoClient.GoalHandler.CheckGoalCompletion();
            });
            DamageAmountManager.OnTotalAmountUpdate.Subscribe(delegate ((int CurrentValue, int AppendValue) info)
            {
                int totalAmount = info.CurrentValue + info.AppendValue;
                Plugin.BepinLogger.LogDebug($"OnTotalAmountUpdate totalAmount: {totalAmount}");
                if (totalAmount >= 20000000)
                {
                    AchievementManager.Instance.IncrementCount("TotalAmountLv4");
                }
                if (totalAmount >= 100000)
                {
                    AchievementManager.Instance.IncrementCount("TotalAmountLv3");
                }
                if (totalAmount >= 50000)
                {
                    AchievementManager.Instance.IncrementCount("TotalAmountLv2");
                }
                if (totalAmount >= 10000)
                {
                    AchievementManager.Instance.IncrementCount("TotalAmountLv1");
                }
            });
        }
    }
}
