using Assets.Scripts.Managers;
using Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

namespace Assets.Scripts.Services
{
    internal class BattleService : Singleton<BattleService>, IDisposable
    {
        public BattleService() 
        {
            MessageDistributer.Instance.Subscribe<SkillCastResponse>(this.OnSkillCast);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<SkillCastResponse>(this.OnSkillCast);
        }

        public void SendSkillCast(int skillId, int casterId , int targetId, NVector3 position)
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.skillCast = new SkillCastRequest();
            message.Request.skillCast.castInfo = new NSkillCastInfo();
            message.Request.skillCast.castInfo.skillId = skillId;
            message.Request.skillCast.castInfo.casterId = casterId;
            message.Request.skillCast.castInfo.targetId = targetId;
            message.Request.skillCast.castInfo.Position = position;
            NetClient.Instance.SendMessage(message);
        }

        private void OnSkillCast(object sender, SkillCastResponse message)
        {
            Debug.LogFormat("SkillCast Response Get");
            if(message.Result == Result.Success)
            {
                Creature caster = EntityManager.Instance.GetEntity(message.castInfo.casterId) as Creature;
                if(caster != null)
                {
                    Creature target = EntityManager.Instance.GetEntity(message.castInfo.casterId) as Creature;
                    caster.CastSkill(message.castInfo.skillId, target, message.castInfo.Position);
                }
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(message.Errormsg);
            }
        }
    }
}
