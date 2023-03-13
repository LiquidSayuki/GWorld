using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public class EquipDefine
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public EquipSlot Slot { get; set; }
        public string Category { get; set; }
        public float STR { get; set; }
        public float INT { get; set; }
        public float AGI { get; set; }
        public float HP { get; set; }
        public float MP { get; set; }
    }
}
