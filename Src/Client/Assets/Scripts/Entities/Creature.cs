using Assets.Scripts.Managers;
using Battle;
using Common.Battle;
using Common.Data;
using SkillBridge.Message;
using System.Collections.Generic;
using UnityEngine;


namespace Entities
{
    public class Creature : Entity
    {
        public NCharacterInfo Info;
        public Common.Data.CharacterDefine Define;
        public Attributes Attributes;
        public SkillManager SkillManager;

        public Skill CasteringSkill = null;

        public int Id
        {
            get { return this.Info.Id; }
        }
        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }
        public bool IsPlayer
        {
            get { return this.Info.Type == CharacterType.Player; }
        }
        public bool IsCurrentPlayer
        {
            get
            {
                if (!IsPlayer) return false;
                return this.Info.Id == Models.User.Instance.CurrentCharacterInfo.Id;
            }
        }
        private bool battleState;
        public bool BattleStats
        {
            get { return battleState; }
            set
            {
                if(battleState != value)
                {
                    battleState = value;
                    this.SetStandby(value);
                }
            }
        }

        /// <summary>
        /// 构造函数在这里
        /// </summary>
        /// <param name="info"></param>
        public Creature(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.ConfigId];

            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define, this.Info.Level, this.GetEquips(), this.Info.attrDynamic);

            this.SkillManager = new SkillManager(this);
        }

        /// <summary>
        /// 获取当前Creature的装备。
        /// 默认对于怪物，NPC等生物，获取空装备
        /// 请在有装备的子类中重载此函数
        /// </summary>
        /// <returns></returns>
        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        public void CastSkill(int skillId, Creature target, NVector3 position)
        {
            this.SetStandby(true);
            var skill = this.SkillManager.GetSkill(skillId);
            skill.BeginCast();
        }

        public void SetStandby(bool b)
        {
            if(this.Controller != null)
            {
                this.Controller.SetStandby(b);
            }
        }
        public void PlayAnim(string name)
        {
            if(this.Controller != null)
            {
                this.Controller.PlayAnim(name);
            }
        }

        public void MoveForward()
        {
            // Debug.LogFormat("MoveForward");
            this.speed = this.Define.Speed;
        }

        public void MoveBack()
        {
            // Debug.LogFormat("MoveBack");
            this.speed = -this.Define.Speed;
        }

        public void Stop()
        {
            // Debug.LogFormat("Stop");
            this.speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            // Debug.LogFormat("SetDirection:{0}", direction);
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            // Debug.LogFormat("SetPosition:{0}", position);
            this.position = position;
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            this.SkillManager.OnUpdate(delta);
        }
    }
}
