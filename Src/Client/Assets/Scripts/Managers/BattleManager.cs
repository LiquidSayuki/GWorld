using Assets.Scripts.Services;
using Battle;
using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public delegate void TargetChangeHandler(Creature creature);
    public class BattleManager: Singleton<BattleManager> 
    {
        public event TargetChangeHandler OnTargetChanged;

        private Creature currentTarget;
        public Creature CurrentTarget
        {
            get { return currentTarget; }
            set
            {
                this.SetTarget(value);
            }
        }


        private NVector3 currentPosition;
        public NVector3 CurrentPosition
        {
            get { return currentPosition; }
            set
            {
                this.SetPosition(value);
            }
        }
        public void Init()
        {

        }

        private void SetTarget(Creature value)
        {
            this.currentTarget = value;
            if(this.OnTargetChanged != null && this.currentTarget != null)
            {
                this.OnTargetChanged(this.currentTarget);
            }
            Debug.LogFormat("BattleMamager SetTarget[{0}]", value.Name);
        }
        private void SetPosition(NVector3 value)
        {
            this.currentPosition = value;
            Debug.LogFormat("BattleMamager SetPosition[{0}]", value);
        }
        public void CastSkill(Skill skill)
        {
            int target = currentTarget != null ? currentTarget.entityId : 0;
            BattleService.Instance.SendSkillCast(skill.Define.ID, skill.Owner.entityId, target, currentPosition);
        }
    }
}
