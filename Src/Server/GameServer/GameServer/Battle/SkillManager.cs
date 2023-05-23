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
    class SkillManager
    {
        //每个参与战斗的生物保有一个自己的Skill Manager
        private Creature Owner;

        public List<Skill> Skills { get; private set; } //逻辑执行Skill
        public List<NSkillInfo> Infos { get; private set; } //向客户端发送的网络Skill

        public SkillManager(Creature owner)
        {
            this.Owner = owner;
            this.Skills = new List<Skill>();
            this.Infos = new List<NSkillInfo>();
            this.InitSkills();
        }

        private void InitSkills()
        {
            this.Skills.Clear();
            this.Infos.Clear();

            if (!DataManager.Instance.Skills.ContainsKey(this.Owner.Define.TID)) return;

            foreach (var skillDefine in DataManager.Instance.Skills[this.Owner.Define.TID])
            {
                NSkillInfo info = new NSkillInfo();
                info.Id = skillDefine.Key;
                //TODO：将技能等级保存在数据库中
                //当前逻辑仅允许技能解锁和非解锁状态
                if(this.Owner.Info.Level >= skillDefine.Value.UnlockLevel)
                {
                    info.Level = 1;
                }
                else
                {
                    info.Level = 0;
                }
                this.Infos.Add(info); 
                Skill skill = new Skill(info, this.Owner);
                this.AddSkill(skill);
            }
        }

        public void AddSkill(Skill skill)
        {
            this.Skills.Add(skill);
        }
    }
}
