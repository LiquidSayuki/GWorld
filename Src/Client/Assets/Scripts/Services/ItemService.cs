using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using UnityEngine;

namespace Services
{
    class ItemService : Singleton<ItemService>, IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);
        }


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);
        }        

        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
            MessageBox.Show("购买结果" + message.Result + "\n" + message.Errormsg, "确定");
        }

        public void SendBuyItem(int shopId, int shopItemId)
        {
            Debug.LogFormat("SendBuyItem: shopId: [{0}], ItemId[{1}]", shopId, shopItemId);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopitemId = shopItemId;
            message.Request.itemBuy.shopId = shopId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnItemEquip(object sender, ItemEquipResponse message)
        {
            if (message.Result == Result.Success)
            {
                if (pendingEquip != null)
                {
                    if (this.isEquip)
                    {
                        EquipManager.Instance.OnEquipItem(pendingEquip);
                    }
                    else
                    {
                        EquipManager.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot);
                    }
                    pendingEquip = null;
                }
            }
        }


        Item pendingEquip = null;
        bool isEquip;
        public bool SendEquipItem(Item item, bool operation)
        {
            if (pendingEquip != null)
            {
                return false;
            }
            Debug.LogFormat("SendEquipItem: ItemId: [{0}], Operation[{1}]", item.Id, operation);

            pendingEquip = item;
            this.isEquip = operation;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequest();
            message.Request.itemEquip.Slot = (int)item.EquipInfo.Slot;
            message.Request.itemEquip.itemId = item.Id;
            message.Request.itemEquip.isEquip = operation;
            NetClient.Instance.SendMessage(message);
            return true;
        }
    }
}
