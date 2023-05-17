using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public enum ItemFunction
    {
        none=0,
        RecoverHP=1,
        RecoverMP=2,
        AddBuff=3,
        AddExp=4,
        AddMoney=5,
        AddItem=6,
        AddSkillPoint = 7,
    }

    public class ItemDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public string Category { get; set; }
        public bool CanUse { get; set; }
        public float UseCD { get; set; }
        public int Price { get; set; }
        public int SellPrice { get; set; }
        public int StackLimit { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public ItemFunction Function { get; set; }
        public int Param { get; set; }
        public int Level { get; set; }
        public CharacterClass LimitClass { get; set; }
        public List<int> Params { get; set; }
    }
}
