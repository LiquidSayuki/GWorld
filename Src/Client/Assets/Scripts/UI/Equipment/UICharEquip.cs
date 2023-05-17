using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Common.Battle;
using Models;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    class UICharEquip : UIWindow
    {
        public Text money;

        public GameObject itemPrefab;
        public GameObject itemEquipedPrefab;
        public Transform itemListRoot;
        public Transform[] slots;

        public Text HP;
        public Slider HPBar;
        public Text MP;
        public Slider MPBar;

        public Text[] attrs;

        void Start()
        {
            RefreshUI();
            EquipManager.Instance.OnEquipmentChange += RefreshUI;
        }

        private void OnDestry()
        {
            EquipManager.Instance.OnEquipmentChange -= RefreshUI;
        }
        void RefreshUI()
        {
            ClearAllEquipList();
            InitAllEquipItems();
            ClearEquipedList();
            InitEquipedItems();
            if(this.money != null)
            {
                this.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
            }
            InitAttributes();
        }

        private void InitEquipedItems()
        {
            for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
            {
                var item = EquipManager.Instance.Equipments[i];
                if (item != null)
                {
                    GameObject go = Instantiate(itemEquipedPrefab, slots[i]);
                    UIEquipItem ui = go.GetComponent<UIEquipItem>();
                    ui.SetEquipItem(i, item, this, true);
                }
            }
        }
        /// <summary>
        /// 初始化所有装备列表
        /// </summary>
        private void InitAllEquipItems()
        {
            foreach (var kv in ItemManager.Instance.Items)
            {
                if (kv.Value.Define.Type == SkillBridge.Message.ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacterInfo.Class)
                {
                    // 已经装备的装备不显示在装备栏中
                    if (EquipManager.Instance.Contains(kv.Key))
                    {
                        continue;
                    }
                    GameObject go = Instantiate(itemPrefab, itemListRoot);
                    UIEquipItem ui = go.GetComponent<UIEquipItem>();
                    ui.SetEquipItem(kv.Key, kv.Value, this, false);
                }
            }
        }

        private void ClearEquipedList()
        {
            foreach (var item in slots)
            {
                if (item.childCount > 0)
                {
                    Destroy(item.GetChild(0).gameObject);
                }
            }
        }

        private void ClearAllEquipList()
        {
            foreach (var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
            {
                Destroy(item.gameObject);
            }
        }

        public void DoEquip(Item item)
        {
            EquipManager.Instance.EquipItem(item);
        }
        public void UnEquip(Item item)
        {
            EquipManager.Instance.UnEquipItem(item);
        }

        private void InitAttributes()
        {
            Attributes charattr = User.Instance.CurrentCharacter.Attributes;
            this.HP.text = string.Format("{0}/{1}",charattr.HP, (int)charattr.MaxHP);
            this.MP.text = string.Format("{0}/{1}", charattr.MP, (int)charattr.MaxMP);
            this.HPBar.maxValue = charattr.MaxHP;
            this.MPBar.maxValue = charattr.MaxMP;
            this.HPBar.value = charattr.HP;
            this.MPBar.value = charattr.MP;

            for(int i = (int)AttributeType.STR; i < (int)AttributeType.MAX; i++)
            {
                if(i == (int)AttributeType.CRI)
                {
                    this.attrs[i - 2].text = string.Format("{0:f2}%", charattr.Final.Data[i] * 100);
                }
                else
                {
                    this.attrs[i - 2].text = (((int)charattr.Final.Data[i]).ToString());
                }
            }
        }
    }
}
