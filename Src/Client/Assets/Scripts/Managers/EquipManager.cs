using Assets.Scripts.Models;
using Services;
using SkillBridge.Message;

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

        public void OnUnEquipItem(EquipSlot slot)
        {
            if (this.Equipments[(int)slot] != null)
            {
                this.Equipments[(int)slot] = null;
                if (OnEquipmentChange != null)
                {
                    OnEquipmentChange();
                }
            }
        }

    }
}
