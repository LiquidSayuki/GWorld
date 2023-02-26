using Assets.Scripts.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    class BagManager :Singleton<BagManager>
    {
        public int Unlocked;
        public BagItem[] Items;
        NBagInfo Info;


        unsafe public void Init(NBagInfo info)
        {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            Items  = new BagItem[this.Unlocked];
            if (info.Items != null && info.Items.Length >= this.Unlocked)
            {
                Analyse(info.Items);
            }
            else
            {
                // 对于没有背包数据的玩家，建立一个存储的字节数组
                Info.Items = new byte[sizeof(BagItem) * this.Unlocked];
                Reset();
                Debug.LogFormat("BagInit: {0}", Info.Items);
            }
        }

        //背包整理
        public void Reset()
        {
            int i = 0;
            foreach(var kv in ItemManager.Instance.Items)
            {
                if(kv.Value.Count <= kv.Value.Define.StackLimit)
                {
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)kv.Value.Count;
                }
                else
                {
                    // 如果道具数量超过堆叠上限
                    int count = kv.Value.Count;
                    while (count > kv.Value.Define.StackLimit)
                    {
                        //拆分一组满道具
                        this.Items[i].ItemId = (ushort)kv.Key;
                        this.Items[i].Count = (ushort)kv.Value.Define.StackLimit;
                        i++;
                        count -= kv.Value.Define.StackLimit;
                    }
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)count;
                }
                i++;
            }
        }

        unsafe void Analyse(byte[] data)
        {
            // 想要使用指针必须是fixed当中
            // pt是指向Data的指针
            fixed (byte* pt = data)
            {
                for (int i =0; i< this.Unlocked; i++)
                {
                    //一个bagitem的指针
                    //pt为初始位置，第i个物体*一个物体所占空间为当前物体的起始字节在字节数组中的位置（偏移）
                    //将每一个物体的初始字节在字节数组中的位置的指针，存储在数组中
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    Items[i] = *item;
                }
            }
        }

        unsafe public NBagInfo GetBagInfo()
        {
            fixed (byte* pt = Info.Items)
            {
                for (int i = 0; i< this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
            }
            return this.Info;
        }
    }
}
