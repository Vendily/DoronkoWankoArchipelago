using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net.Helpers;
using doronko_wanko_ap.Archipelago;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Collections.Specialized.BitVector32;

namespace doronko_wanko_ap.Handlers
{
    public class ItemHandler
    {
        private const long BASE_ID = 0x560000;
        private readonly Queue<QueuedItem> _itemQueue = new();
        private static readonly object itemLock = new();
        private readonly int[] DAMAGE_AMOUNTS = [10, 100, 250, 500];
        public ItemBoxManager ItemBoxManager { get; set; }
        public Mommy Mommy { get; set; }
        public List<TemperaturePanel> WineButtons { get; set; }
        public Train Train { get; set; }
        public readonly bool[] FlagItems = [false, false, false];


        public ItemHandler()
        {
            WineButtons = new();
        }


        private bool CanReceiveItem()
        {
            return ItemBoxManager != null && SceneManager.GetActiveScene().name == "Main";
        }


        /// <summary>
        /// we received an item so reward it here
        /// </summary>
        /// <param name="helper">item helper which we can grab our item from</param>
        public void OnItemReceived(ReceivedItemsHelper helper)
        {
            lock (itemLock)
            {
                long id = helper.PeekItem().ItemId;
                string name = helper.PeekItem().ItemName;
                string player = Plugin.ArchipelagoClient.GetPlayerNameFromSlot(helper.PeekItem().Player);
                int index = helper.Index;
                helper.DequeueItem();

                _itemQueue.Enqueue(new QueuedItem(id, name, player, index));
            }
        }

        public void Update()
        {
            lock (itemLock)
            {
                if (_itemQueue.Count > 0 && CanReceiveItem())
                    ProcessItem(_itemQueue.Dequeue());
            }
        }

        private void ProcessItem(QueuedItem queuedItem)
        {
            if (queuedItem.index < ArchipelagoClient.ServerData.Index) return;

            ArchipelagoClient.ServerData.Index++;

            Plugin.BepinLogger.LogInfo($"Received {queuedItem.name} ({queuedItem.id}) from {queuedItem.player}");
            FieldInfo itemDataList = typeof(ItemBoxManager).GetField("itemDataList", BindingFlags.Instance | BindingFlags.NonPublic);
            ItemBoxData[] itemBoxDatas = (ItemBoxData[])itemDataList.GetValue(ItemBoxManager);
            long item_id = queuedItem.id - BASE_ID;
            if (item_id < ((int)ArchipelagoItemID.FlagStart))
            {
                // This is an item box item
                item_id -= 1; // 0-indexed
                FieldInfo itembox = typeof(ItemBox).GetField("item", BindingFlags.Instance | BindingFlags.NonPublic);
                //((GameObject)itembox.GetValue(itemBoxDatas[item_id].itemInstance))?.SetActive(value: true);
                itemBoxDatas[item_id].itemInstance.DropBox();
            }
            else if (item_id < ((int)ArchipelagoItemID.FillerStart))
            {
                switch (item_id)
                {
                    case (int)ArchipelagoItemID.MomUnlock:
                        UnlockMommy();
                        break;
                    case (int)ArchipelagoItemID.WineButtonUnlock:
                        UnlockWineButton();
                        break;
                    case (int)ArchipelagoItemID.TrainUnlock:
                        UnlockTrain();
                        break;
                }
            }
            else
            {
                // This is a damage item
                item_id -= (int)ArchipelagoItemID.FillerStart;
                int damage = DAMAGE_AMOUNTS[item_id];
                DamageAmountManager.OnAmountCount.OnNext((DamageAmountManager.DamageCategory.Floor, damage));
            }
        }

        private void UnlockMommy()
        {
            bool old_flag = FlagItems[0];
            FlagItems[0] = true;
            if (!old_flag)
            {
                try
                {
                    typeof(Mommy).GetField("currentArea", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Mommy, Area.BASEMENT);
                    typeof(Mommy).GetMethod("ChangeArea", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Mommy, [Area.KITCHEN]);
                }
                catch (Exception ex)
                {
                    Plugin.BepinLogger.LogError(ex);
                }
                
            }
        }
        private void UnlockWineButton()
        {
            bool old_flag = FlagItems[1];
            FlagItems[1] = true;
            if (!old_flag)
            {
                try
                {
                    foreach (TemperaturePanel b in WineButtons)
                    {
                        b.gameObject.SetActive(true);
                    }
                }
                catch (Exception ex)
                {
                    Plugin.BepinLogger.LogError(ex);
                }
            }
        }

        private void UnlockTrain()
        {
            bool old_flag = FlagItems[2];
            FlagItems[2] = true;
            if (!old_flag)
            {
                try
                {
                    Train.gameObject.SetActive(true);
                }
                catch (Exception ex)
                {
                    Plugin.BepinLogger.LogError(ex);
                }
            }
        }
    }
    public readonly struct QueuedItem
    {
        public readonly long id;
        public readonly string name;
        public readonly string player;
        public readonly int index;

        public QueuedItem(long id, string name, string player, int index)
        {
            this.id = id;
            this.name = name;
            this.player = player;
            this.index = index;
        }
    }
}
