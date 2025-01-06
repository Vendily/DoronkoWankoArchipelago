using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace doronko_wanko_ap.Patches
{
    public class LocationPatches
    {
        public static void GeneratePatches()
        {
            ItemBoxManager.OnItemChargeUpdate.Subscribe(delegate ((int CurrentAmount, int TargetAmount) info)
            {
                if (info.TargetAmount > 0 && info.CurrentAmount == info.TargetAmount)
                {
                    Plugin.BepinLogger.LogDebug("Finished a Damage target: " + info.TargetAmount);
                }
            });
            AchievementEvents.OnNotificationRequired.Subscribe(delegate (Achievement Achievement)
            {
                Plugin.ArchipelagoClient.SendLocation(Plugin.ArchipelagoClient.LocationHandler.GetArchipelagoName(Achievement.Id));
                Plugin.ArchipelagoClient.GoalHandler.CheckGoalCompletion();
            });
        }
    }
}
