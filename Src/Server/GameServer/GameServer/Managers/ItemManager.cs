using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;


namespace GameServer.Managers
{
    class ItemManager
    {
        Character Owner;
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public ItemManager(Character owner)
        {
            this.Owner = owner;

            foreach(var item in this.Owner.Data.Items)
            {
                this.Items.Add(item.ItemID, new Item(item));
            }
        }

        public bool UseItem(int itemID, int count =1)
        {
            Log.InfoFormat("[{0}]UseItem[{1}:{2}]", this.Owner.Data.ID, itemID, count);
            Item item = null;
            if(this.Items.TryGetValue(itemID, out item))
            {
                if (item.Count < count)
                {
                    return false;
                }

                //TODO: 增加使用逻辑
                item.Remove(count);
                return true;
            }
            return false;
        }

        public bool HasItem(int itemID)
        {
            Item item = null;
            if(this.Items.TryGetValue(itemID,out item))
                return item.Count > 0;
            return false;
        }

        public Item GetItem(int itemID)
        {
            Item item = null;
            this.Items.TryGetValue(itemID, out item);
            Log.InfoFormat("[{0}] GetItem[{1}:{2}]",this.Owner.Data.ID, itemID,item);
            return item;
        }

        public bool AddItem(int itemID, int count)
        {
            Item item = null;
            if(this.Items.TryGetValue(itemID,out item))
            {
                item.Add(count);
            }
            else
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.TCharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemID;
                dbItem.ItemCount = count;
                Owner.Data.Items.Add(dbItem);
                item = new Item(dbItem);
                this.Items.Add(itemID, item);
            }
            Log.InfoFormat("[{0}] AddItem[{1}] addCount:{2}", this.Owner.Data.ID, item, count);
            DBService.Instance.Save();
            return true;
        }

        public bool RemoveItem(int itemID, int count) 
        {
            if (!this.Items.ContainsKey(itemID))
            {
                return false;
            }
            Item item = this.Items[itemID];
            if (item.Count < count)
            {
                return false ;
            }
            item.Remove(count);
            Log.InfoFormat("[{0}]RemoveItem[{1}] removeCount:{2}", this.Owner.Data.ID, item, count);
            DBService.Instance.Save();
            return true;
        }

        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach(var item in Items)
            {
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }

    }
}
