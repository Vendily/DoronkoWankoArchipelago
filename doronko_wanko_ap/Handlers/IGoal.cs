using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doronko_wanko_ap.Handlers
{
    public interface IGoal
    {
        string Name { get; }

        public bool CheckCompletion();
    }

    public class CakeGoal : IGoal
    {
        public string Name => "Cake";

        public bool CheckCompletion()
        {
            if (AchievementManager.Instance.AllAchievements.TryGetValue("GameClear", out var achievement)) 
            {
                return achievement.IsAcquired;
            }
            return false;
        }
    }

    public class BadgeGoal : IGoal
    {
        public string Name => "Badge";

        public bool CheckCompletion()
        {
            foreach (string ach_id in AchievementKeys.Keys)
            {
                if (AchievementManager.Instance.AllAchievements.TryGetValue(ach_id, out var achievement))
                {
                    if(!achievement.IsAcquired) return false;
                }
            }
            return true;
        }
    }
}
