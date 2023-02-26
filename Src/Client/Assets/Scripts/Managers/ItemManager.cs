using Assets.Scripts.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using Common.Data;

namespace Assets.Scripts.Managers
{
    internal class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item> ();
        internal void Init(List<NItemInfo> items)
        {
            this.Items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);

                Debug.LogFormat("ItemManager: Init[{0}], Count: {1}", item.Id, item.Count);
            }
        }

        public ItemDefine GetItem(int itemId)
        {
            return null;
        }

        public bool UseItem(int itemId)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;
        }
    }
}
