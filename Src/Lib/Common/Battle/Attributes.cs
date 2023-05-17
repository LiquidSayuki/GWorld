using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common.Battle
{
    public class Attributes
    {
        AttributeData Initial = new AttributeData();
        AttributeData Growth = new AttributeData();
        AttributeData Equip = new AttributeData();
        AttributeData Basic = new AttributeData();
        AttributeData Buff = new AttributeData();
        public AttributeData Final = new AttributeData();

        int Level;

        private NAttributeDynamic dynamic;

        public float HP
        {
            get { return dynamic.Hp; }
            set
            {
                dynamic.Hp = (int)Math.Min(MaxHP, value);
            }
        }

        public float MP
        {
            get { return dynamic.Mp; }
            set
            {
                dynamic.Mp = (int)Math.Min(MaxMP, value);
            }
        }

        public float MaxHP { get { return this.Final.MaxHP; } }
        public float MaxMP { get { return this.Final.MaxMP; } }
        public float STR { get { return this.Final.STR; } }
        public float INT { get { return this.Final.INT; } }
        public float DEX { get { return this.Final.DEX; } }
        public float AD { get { return this.Final.AD; } }
        public float AP { get { return this.Final.AP; } }
        public float DEF { get { return this.Final.DEF; } }
        public float MDEF { get { return this.Final.MDEF; } }
        public float SPD { get { return this.Final.SPD; } }
        public float CRI { get { return this.Final.CRI; } }

        /// <summary>
        /// 初始化角色属性
        /// </summary>
        /// <param name="define">角色</param>
        /// <param name="level">等级</param>
        /// <param name="equips">装备</param>
        /// <param name="dynamicAttr">当前动态HPMP</param>
        public void Init(CharacterDefine define, int level, List<EquipDefine> equips, NAttributeDynamic dynamicAttr)
        {
            this.dynamic = dynamicAttr;
            this.LoadInitAttribute(this.Initial, define);
            this.LoadGrowthAttribute(this.Growth, define);
            this.LoadEquipAttributes(this.Equip, equips);
            this.Level = level;
            this.InitBasicAttributes();
            this.InitSecindaryAttributes();

            this.InitFinalAttributes();
            if(this.dynamic == null)
            {
                this.dynamic = new NAttributeDynamic();
            }
            this.HP = dynamicAttr.Hp;
            this.MP = dynamicAttr.Mp;
        }

        //载入角色0级属性
        private void LoadInitAttribute(AttributeData data, CharacterDefine define)
        {
            data.MaxHP = define.MaxHP;
            data.MaxMP = define.MaxMP;
            data.STR = define.STR;
            data.INT = define.INT;
            data.DEX = define.DEX;
            data.AP = define.AP;
            data.AD = define.AD;
            data.DEF = define.DEF;
            data.MDEF = define.MDEF;
            data.SPD = define.SPD;
            data.CRI = define.CRI;
        }
        //载入角色属性成长
        private void LoadGrowthAttribute(AttributeData data, CharacterDefine define)
        {
            data.STR = define.GrowthSTR;
            data.INT = define.GrowthINT;
            data.DEX = define.GrowthDEX;
        }
        //载入装备属性
        private void LoadEquipAttributes(AttributeData data, List<EquipDefine> equips)
        {
            data.Reset();
            foreach(EquipDefine equip in equips)
            {
                data.MaxHP += equip.MaxHP;
                data.MaxMP += equip.MaxMP;
                data.STR += equip.STR;
                data.INT += equip.INT;
                data.DEX += equip.DEX;
                data.AP += equip.AP;
                data.AD += equip.AD;
                data.DEF += equip.DEF;
                data.MDEF += equip.MDEF;
                data.SPD += equip.SPD;
                data.CRI += equip.CRI;
            }
        }

        //计算角色基础属性
        private void InitBasicAttributes()
        {
            for(int i = 0; i< (int)AttributeType.MAX; i++)
            {
                this.Basic.Data[i] = this.Initial.Data[i];

                //对于力量敏捷智力，计算成长值
                if(i == (int)AttributeType.STR || i == (int)AttributeType.INT || i == (int)AttributeType.DEX)
                {
                    this.Basic.Data[i] += this.Growth.Data[i] * (this.Level - 1);
                    //力敏智影响生命魔法，所以需要先一步计算装备增幅
                    this.Basic.Data[i] += this.Equip.Data[i];
                }
            }
        }
        //角色二级属性（生命，魔法，攻击，防御，速度，暴击）
        private void InitSecindaryAttributes()
        {
            this.Basic.MaxHP = this.Basic.STR * 17 + this.Initial.MaxHP + this.Equip.MaxHP;
            this.Basic.MaxMP = this.Basic.INT * 12 + this.Initial.MaxMP + this.Equip.MaxMP;

            this.Basic.AD  = this.Basic.STR * 5 + this.Initial.AD + this.Equip.AD;
            this.Basic.AP = this.Basic.INT * 5 + this.Initial.AP + this.Equip.AP;
            this.Basic.DEF = this.Basic.STR * 2 + this.Basic.DEX * 1 + this.Initial.DEF+ this.Equip.DEF;
            this.Basic.MDEF = this.Basic.INT * 2 + this.Basic.DEX *1 + this.Initial.MDEF + this.Equip.DEF;
            this.Basic.SPD = this.Basic.DEX * 0.2f + this.Initial.SPD + this.Equip.SPD;
            this.Basic.CRI = this.Basic.DEX * 0.002f + this.Initial.CRI + this.Equip.CRI;
        }

        private void InitFinalAttributes()
        {
            for(int i = 0; i < (int)AttributeType.MAX; i++)
            {
                this.Final.Data[i] = this.Basic.Data[i] + this.Buff.Data[i];
            }
        }
    }
}
