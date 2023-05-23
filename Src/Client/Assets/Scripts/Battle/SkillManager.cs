using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battle
{
    public class SkillManager
    {
        //每个参与战斗的生物保有一个自己的Skill Manager
        Creature Owner;

        public List<Skill> Skills { get; private set; }

        public SkillManager(Creature owner)
        {
            this.Owner = owner;
            this.Skills = new List<Skill>();
            this.InitSkills();
        }

        private void InitSkills()
        {
            this.Skills.Clear();
            foreach(var skillInfo in this.Owner.Info.Skills)
            {
                Skill skill = new Skill(skillInfo, this.Owner);
                this.AddSkill(skill);
            }
        }

        public void AddSkill(Skill skill)
        {
            this.Skills.Add(skill);
        }

        public Skill GetSkill(int skillId)
        {
            for(int i = 0; i< this.Skills.Count; i++)
            {
                if (this.Skills[i].Define.ID == skillId)
                {
                    return this.Skills[i];
                }
            }
            return null;
        }

        internal void OnUpdate(float delta)
        {
            for(int i = 0; i< this.Skills.Count; i++)
            {
                this.Skills[i].OnUpdate(delta);
            }
        }
    }
}
