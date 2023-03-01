using Assets.Scripts.Managers;
using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Models
{
    public class Item
    {
        public int Id;
        public int Count;
        public ItemDefine Define;

        //可以重载自己的构造函数
        public Item(NItemInfo item):
            this(item.Id, item.Count){ }

        public Item(int id, int count)
        {
            this.Id = id;
            this.Count = count;
            this.Define = DataManager.Instance.Items[this.Id];
        }

        public override string ToString()
        {
            return string.Format("ID:{0}, Count{1}", this.Id, this.Count);
        }
    }
}
