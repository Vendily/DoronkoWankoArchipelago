using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using TMPro;
using UniRx;
using UnityEngine;

namespace doronko_wanko_ap.Patches
{
    [HarmonyDebug]
    [HarmonyPatch(typeof(ItemBoxManager), "<Start>b__10_1")]
    public class ItemBoxManager_DamageOverflow_Patch
    {
        public static int overflowAmount { get; set; }

        static void CalcAndSetOverflowAmount(int totalAmount, int targetAmount)
        {
            Plugin.BepinLogger.LogDebug($"Total: {totalAmount}, Target: {targetAmount}");
            int overflow = totalAmount - targetAmount;
            if (overflow > 0)
            {
                overflowAmount = overflow;
                Plugin.BepinLogger.LogDebug($"Saved Overflow: {overflowAmount}");
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
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
                Plugin.BepinLogger.LogError("Failed to find Ble call in Start Lambda Transpiler");
            }
            else
            {
                var instructionsToInsert = new List<CodeInstruction>();

                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemBoxManager), "totalAmount")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ItemBoxManager_DamageOverflow_Patch), "CalcAndSetOverflowAmount", [typeof(int), typeof(int)])));
                codes.InsertRange(branch_pos+1, instructionsToInsert);
            }
            return codes;
        }
    }
    [HarmonyPatch]
    [HarmonyDebug]
    public class ItemBoxManager_ItemUnlock_Patch
    {
        public static bool sentLocation { get; set; }
        public static MethodBase TargetMethod()
        {
            var targetMethod = typeof(ItemBoxManager).GetMethod("ItemUnlock", BindingFlags.Instance | BindingFlags.NonPublic);
            var stateMachineAttr = targetMethod.GetCustomAttribute<AsyncStateMachineAttribute>();
            var moveNextMethod = stateMachineAttr.StateMachineType.GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance);

            return moveNextMethod;
        }
        static void DamageOverflowAndResetSendLocation()
        {
            if (ItemBoxManager_DamageOverflow_Patch.overflowAmount > 0)
            {
                int temp_overflow = ItemBoxManager_DamageOverflow_Patch.overflowAmount;
                ItemBoxManager_DamageOverflow_Patch.overflowAmount = 0;
                Plugin.BepinLogger.LogDebug($"Temp Overflow amount: {temp_overflow}; Overflow Amount: {ItemBoxManager_DamageOverflow_Patch.overflowAmount}");
                DamageAmountManager.OnStackCreateOrDestory.OnNext((false, temp_overflow));
            }
            sentLocation = false;
        }
        static void SendLocation(int unlockCount)
        {
            if (!sentLocation)
            {
                Plugin.BepinLogger.LogInfo($"Unlock Count = {unlockCount}");
                string damage_id = Plugin.ArchipelagoClient.LocationHandler.GetDamageGameName(unlockCount);
                Plugin.ArchipelagoClient.LocationHandler.damageIndex = unlockCount + 1;
                Plugin.ArchipelagoClient.SendLocation(Plugin.ArchipelagoClient.LocationHandler.GetArchipelagoName(damage_id));
                sentLocation = true;
            }
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // remove drop box instruction
            // itemDataList[unlockCount].itemInstance.DropBox();
            var op_start = -1;
            var op_end = -1;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("Void GetResult()"))
                {
                    op_start = i;
                }
                if (op_start >= 0 && codes[i].opcode == OpCodes.Callvirt)
                {
                    op_end = i;
                    break;
                }
                   
            }
            if (op_start > -1 && op_end > -1)
            {
                codes.RemoveRange(op_start + 1, op_end - op_start);
                // ItemBoxManager_ItemUnlock_Patch.SendLocation(unlockCount)
                var instructionsToInsert = new List<CodeInstruction>();
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_1));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_1));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemBoxManager), "unlockCount")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ItemBoxManager_ItemUnlock_Patch), "SendLocation", [typeof(int)])));
                codes.InsertRange(op_start + 1, instructionsToInsert);
            }
            // Add damage overflow damage
            int op_pos = -1;
            for (var i = codes.Count-1; i > 0 ; i--)
            {
                if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand.ToString().Contains("Void OnNext"))
                {
                    op_pos = i;
                    break;
                }
            }
            if (op_pos < 0)
            {
                Plugin.BepinLogger.LogError("Failed to find callvirt System.Void UniRx.Subject`1<System.ValueTuple`2<System.Int32,System.Int32>>::OnNext(T) in ItemUnlock() Transpiler");
            }
            else
            {
                var instructionsToInsert = new List<CodeInstruction>();
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ItemBoxManager_ItemUnlock_Patch), "DamageOverflowAndResetSendLocation")));
                codes.InsertRange(op_pos + 1, instructionsToInsert);
            }
            return codes;
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
        public static void Prefix(DamageAmountManager __instance)
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
