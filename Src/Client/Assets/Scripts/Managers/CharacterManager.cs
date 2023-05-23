using Entities;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Creature> Characters = new Dictionary<int, Creature>();
        public UnityAction<Creature> OnCharacterEnter;
        public UnityAction<Creature> OnCharacterLeave;

        public CharacterManager(){}
        public void Dispose(){}
        public void Init(){}
        public void Clear()
        {
            this.Characters.Clear();
        }

        public void AddCharacter(SkillBridge.Message.NCharacterInfo cha)
        {
            Debug.LogFormat("CharacterManager:AddCharacter:ID:{0}Name:{1}MapID:{2} Entity:{3}", cha.Id, cha.Name, cha.mapId, cha.Entity.String());
            Creature character = new Creature(cha);
            this.Characters[cha.EntityId] = character;

            // 交给实体管理器
            EntityManager.Instance.AddEntity(character);
            // 事件调用，游戏物体管理器创建游戏物体
            if (OnCharacterEnter != null)
            {
                OnCharacterEnter(character);
            }
            //将创建好的玩家角色传递给User，方便调用
            if (cha.EntityId == User.Instance.CurrentCharacterInfo.EntityId)
            {
                User.Instance.CurrentCharacter = character;
            }
        }

        public void RemoveCharacter(int entityId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", entityId);
            if (this.Characters.ContainsKey(entityId))
            {
                EntityManager.Instance.RemoveEntity(this.Characters[entityId].Info.Entity);
                if (OnCharacterLeave != null)
                {
                    OnCharacterLeave(this.Characters[entityId]);
                }
                this.Characters.Remove(entityId);
            }
        }

        public Creature GetCharacter(int id)
        {
            Creature character;
            this.Characters.TryGetValue(id, out character);
            return character;
        }
    }
}
