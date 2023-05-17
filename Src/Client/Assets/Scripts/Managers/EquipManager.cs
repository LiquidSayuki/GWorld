using Assets.Scripts.Models;
using Common.Data;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipmentChangeHandler();
        public event OnEquipmentChangeHandler OnEquipmentChange;

        public Item[] Equipments = new Item[(int)EquipSlot.SlotMax];

        byte[] Data;

        unsafe public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }

        public bool Contains(int equipId)
        {
            for (int i = 0; i < this.Equipments.Length; i++)
            {
                if (Equipments[i] != null && Equipments[i].Id == equipId)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 将equip字节数据转化为装备
        /// </summary>
        /// <param name="data"></param>
        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = this.Data)
            {
                for (int i = 0; i < this.Equipments.Length; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId > 0)
                    {
                        Equipments[i] = ItemManager.Instance.Items[itemId];
                    }
                    else
                    {
                        Equipments[i] = null;
                    }
                }
            }
        }

        internal Item GetEquip(EquipSlot slot)
        {
            return Equipments[(int)slot];
        }

        unsafe public byte[] GetEquipData()
        {
            fixed (byte* pt = Data)
            {
                for (int i = 0; i < this.Equipments.Length; i++)
                {
                    int* itemId = (int*)(pt + i * sizeof(int));
                    if (Equipments[i] == null)
                    {
                        *itemId = 0;
                    }
                    else
                    {
                        *itemId = Equipments[i].Id;
                    }
                }
                return this.Data;
            }
        }

        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }

        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }

        //穿装备成功回调函数
        public void OnEquipItem(Item equip)
        {
            if (this.Equipments[(int)equip.EquipInfo.Slot] != null && this.Equipments[(int)equip.EquipInfo.Slot].Id == equip.Id)
            {
                return;
            }
            this.Equipments[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];

            if (OnEquipmentChange != null)
            {
                OnEquipmentChange();
            }
        }
        //脱装备成功回调函数
        public void OnUnequipItem(EquipSlot slot)
        {
            if (this.Equipments[(int)slot] != null)
            {
                this.Equipments[(int)slot] = null;
                if(OnEquipmentChange != null)
                {
                    OnEquipmentChange();
                }
            }
        }

        /// <summary>
        /// 得到当前所有正在装备的装备的Define信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<EquipDefine> GetEquipedDefines()
        {
            List<EquipDefine> equips = new List<EquipDefine>();
            for(int i = 0; i< (int)EquipSlot.SlotMax; i++)
            {
                if (Equipments[i] != null)
                {
                    equips.Add(Equipments[i].EquipInfo);
                }
            }
            return equips;
        }
    }
}
