using Assets.Scripts.Managers;
using Common.Battle;
using Common.Data;
using Entities;
using SkillBridge.Message;

namespace Battle
{
    public class Skill
    {
        public NSkillInfo Info;
        public SkillDefine Define;
        public Creature Owner;
        public bool IsCasting = false;
        private float castTime = 0;

        private float cd;
        public float CD
        {
            get; private set;
        }



        public Skill(NSkillInfo skillInfo, Creature owner)
        {
            this.Info = skillInfo;
            this.Owner = owner;
            // 根据职业读取配置表获得技能
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
            this.CD = 0;
        }

        public SkillResult CanCast(Creature target)
        {
/*            if(this.Define.CastTarget == TargetType.Target) //敌方目标技能
            {
                if(target == null || target == this.Owner)//目标为空，目标为自身
                {
                    return SkillResult.InvalidTarget;
                }
            }
            if(this.Define.CastTarget == TargetType.Position && BattleManager.Instance.CurrentPosition == null)
            {
                return SkillResult.InvalidTarget;
            }

            if(this.Owner.Attributes.MP < this.Define.MPCost)//魔法不足
            {
                return SkillResult.OutOfMp;
            }
            if(this.CD > 0)//冷却中
            {
                return SkillResult.Cooldown;
            }*/
            return SkillResult.Ok;
        }

        public  void BeginCast()
        {
            this.IsCasting = true;
            this.castTime = 0;
            this.CD = this.Define.CD;

            this.Owner.PlayAnim(this.Define.SkillAnim);
        }

        internal void OnUpdate(float delta)
        {
            if (this.IsCasting)
            {

            }
            UpdateCD(delta);
        }
        private void UpdateCD(float delta)
        {
            if(this.CD> 0)
            {
                this.CD -= delta;
            }
            if(this.CD < 0)
            {
                this.CD = 0;
            }
        }
    }
}
