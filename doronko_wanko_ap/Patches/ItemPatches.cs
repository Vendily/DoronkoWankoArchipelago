using doronko_wanko_ap.Handlers;
using UniRx;

namespace doronko_wanko_ap.Patches
{
    public class ItemPatches
    {

        public static void GeneratePatches()
        {
            On.ItemBoxManager.Start += ItemBoxManager_Start;
        }

        private static void ItemBoxManager_Start(On.ItemBoxManager.orig_Start orig, ItemBoxManager self)
        {
            orig(self);
            ItemHandler.ItemBoxManager = self;
        }
        
    }
}