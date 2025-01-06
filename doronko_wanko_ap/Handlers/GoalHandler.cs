using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using doronko_wanko_ap.Archipelago;

namespace doronko_wanko_ap.Handlers
{
    public class GoalHandler
    {
        private readonly Dictionary<GoalType, IGoal> _goals = new()
    {
        { GoalType.Cake, new CakeGoal() },
        { GoalType.Badge, new BadgeGoal() },
    };
        private IGoal CurrentGoal => _goals.TryGetValue(ArchipelagoClient.ServerData.Goal, out IGoal goal)
        ? goal
        : throw new System.Exception($"Invalid goal type: {ArchipelagoClient.ServerData.Goal}");

        public void CheckGoalCompletion()
        {
            bool complete = CurrentGoal.CheckCompletion();

            Plugin.BepinLogger.LogDebug($"Goal status ({CurrentGoal.Name}): {(complete ? "Complete" : "Incomplete")}");

            if (complete)
                Plugin.ArchipelagoClient.SendGoalCompletion();
        }
    }
}
