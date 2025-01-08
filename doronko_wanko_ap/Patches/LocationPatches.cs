using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace doronko_wanko_ap.Patches
{
    public class LocationPatches
    {
        private static FieldInfo t_Amount;
        private static FieldInfo c_Amount;
        private static FieldInfo u_count;
        private static MethodInfo getTargetAmount;

        public static void GeneratePatches()
        {
            On.ItemBoxManager.ItemUnlock += ItemBoxManager_ItemUnlock;
            On.DamageAmountManager.FixedUpdate += GameLoop_Update;
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

            t_Amount = typeof(ItemBoxManager).GetField("totalAmount", BindingFlags.Instance | BindingFlags.NonPublic);
            c_Amount = typeof(ItemBoxManager).GetField("currentAmount", BindingFlags.Instance | BindingFlags.NonPublic);
            u_count = typeof(ItemBoxManager).GetField("unlockCount", BindingFlags.Instance | BindingFlags.NonPublic);
            getTargetAmount = typeof(ItemBoxManager).GetMethod("GetTargetAmount", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private static void ItemBoxManager_ItemUnlock(On.ItemBoxManager.orig_ItemUnlock orig, ItemBoxManager self)
        {
            ItemBoxManager.OnItemUnlock.OnNext(value: true);
            int get_TargetAmount = (int)getTargetAmount.Invoke(self, [0]);
            Plugin.BepinLogger.LogDebug($"Unlocked item at target {get_TargetAmount}");

            t_Amount.SetValue(self, (int)t_Amount.GetValue(self) - get_TargetAmount);
            c_Amount.SetValue(self, (int)c_Amount.GetValue(self) - get_TargetAmount);
            u_count.SetValue(self, (int)u_count.GetValue(self) + 1);
            string damage_id = Plugin.ArchipelagoClient.LocationHandler.GetDamageGameName((int)u_count.GetValue(self) - 1);
            Plugin.ArchipelagoClient.SendLocation(Plugin.ArchipelagoClient.LocationHandler.GetArchipelagoName(damage_id));
            ItemBoxManager.OnItemChargeUpdate.OnNext(((int)c_Amount.GetValue(self), (int)getTargetAmount.Invoke(self, [0])));
        }

        private static void GameLoop_Update(On.DamageAmountManager.orig_FixedUpdate orig, DamageAmountManager self)
        {
            Plugin.ArchipelagoClient.ItemHandler.Update();
            orig(self);
        }
    }
}
