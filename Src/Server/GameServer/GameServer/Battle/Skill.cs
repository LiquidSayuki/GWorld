using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle
{
    class Skill
    {
        public NSkillInfo Info;
        public SkillDefine Define;
        Creature Owner;

        public Skill(NSkillInfo skillInfo, Creature owner)
        {
            this.Info = skillInfo;
            this.Owner = owner;
            // 根据职业读取配置表获得技能
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
        }
    }
}
