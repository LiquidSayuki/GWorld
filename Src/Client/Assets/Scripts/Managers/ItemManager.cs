﻿using Assets.Scripts.Models;
using Common.Data;
using Services;
using SkillBridge.Message;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    internal class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        internal void Init(List<NItemInfo> items)
        {
            this.Items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);

                Debug.LogFormat("ItemManager: Init[{0}], Count: {1}", item.Id, item.Count);
            }
            StatusService.Instance.RegisterStatusNotify(StatusType.Item, OnItemNotify);
        }

        // 收到statusService消息时，注册过的该方法就能触发
        bool OnItemNotify(NStatus status)
        {
            if (status.Action == StatusAction.Add)
            {
                this.AddItem(status.Id, status.Value);
            }
            else if (status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id, status.Value);
            }
            return true;
        }

        private void AddItem(int itemId, int count)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                item.Count += count;
            }
            else
            {
                item = new Item(itemId, count);
                this.Items.Add(itemId, item);
            }
            BagManager.Instance.AddItem(itemId, count);
        }

        private void RemoveItem(int itemId, int count)
        {
            if (!this.Items.ContainsKey(itemId))
            {
                return;
            }
            else
            {
                Item item = this.Items[itemId];
                if (item.Count < count)
                {
                    return;
                }
                item.Count -= count;
            }
            BagManager.Instance.RemoveItem(itemId, count);
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
