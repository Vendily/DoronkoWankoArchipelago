using UniRx;
using TMPro;
using HarmonyLib;
using System;
using static DamageAmountManager;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace doronko_wanko_ap.Patches
{
    [HarmonyDebug]
    [HarmonyPatch(typeof(ItemBoxManager), "<Start>b__10_1")]
    public class ItemBoxManager_DamageOverflow_Patch
    {
        public static int overflowAmount { get; set; }

        static void CalcAndSetOverflowAmount(int totalAmount, int targetAmount)
        {
            int overflow = totalAmount - targetAmount;
            if (overflow > 0)
            {
                overflowAmount = overflow;
            }
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {

            var instructionsToInsert = new List<CodeInstruction>();

            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemBoxManager), "totalAmount")));
            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_0));
            instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ItemBoxManager_DamageOverflow_Patch), "CalcAndSetOverflowAmount", [typeof(int), typeof(int)])));

            var branch_pos = -1;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ble)
                {
                    branch_pos = i; break;
                }
            }
            if (branch_pos < 0)
            {
                Plugin.BepinLogger.LogError("Failed to find Ble_S call in Start Lambda Transpiler");
            }
            else
            {
                codes.InsertRange(branch_pos+1, instructionsToInsert);
            }

            return codes;
        }
    }


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
            
            if (ItemBoxManager_DamageOverflow_Patch.overflowAmount > 0)
            {
                int temp_overflow = ItemBoxManager_DamageOverflow_Patch.overflowAmount;
                ItemBoxManager_DamageOverflow_Patch.overflowAmount = 0;
                Plugin.BepinLogger.LogDebug($"Temp Overflow amount: {temp_overflow}; Overflow Amount: {ItemBoxManager_DamageOverflow_Patch.overflowAmount}");
                OnStackCreateOrDestory.OnNext((false, temp_overflow));
            }
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

            Plugin.ArchipelagoClient.ItemHandler.TryUpdate(Time.fixedDeltaTime);
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
