using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class UICharEquip : UIWindow
    {
        public GameObject itemPrefab;
        public GameObject itemEquipedPrefab;
        public Transform itemListRoot;

        public Transform[] slots;


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

        private void InitAllEquipItems()
        {
            foreach (var kv in ItemManager.Instance.Items)
            {
                if (kv.Value.Define.Type == SkillBridge.Message.ItemType.Equipment)
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
            foreach(var item in slots)
            {
                if (item.childCount > 0)
                {
                    Destroy(item.GetChild(0).gameObject);
                }
            }
        }

        private void ClearAllEquipList()
        {
            foreach(var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
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


    }
}
