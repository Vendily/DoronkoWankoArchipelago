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
            AchievementEvents.OnNotificationRequired.Subscribe(delegate (Achievement Achievement)
            {
                Plugin.ArchipelagoClient.SendLocation(Plugin.ArchipelagoClient.LocationHandler.GetArchipelagoName(Achievement.Id));
            });
        }
    }
}
