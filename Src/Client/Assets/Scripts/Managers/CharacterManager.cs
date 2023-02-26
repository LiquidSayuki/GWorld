﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Entities;
using SkillBridge.Message;

namespace Assets.Scripts.Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();
        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<Character> OnCharacterLeave;

        public CharacterManager()
        {

        }

        public void Dispose()
        {
        }

        public void Init()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }

        public void AddCharacter(SkillBridge.Message.NCharacterInfo cha)
        {
            Debug.LogFormat("CharacterManager:AddCharacter:ID:{0}Name:{1}MapID:{2} Entity:{3}", cha.Id, cha.Name, cha.mapId, cha.Entity.String());
            Character character = new Character(cha);
            this.Characters[cha.Id] = character;

            EntityManager.Instance.AddEntity(character);

            if(OnCharacterEnter!=null)
            {
                OnCharacterEnter(character);
            }
        }


        public void RemoveCharacter(int characterId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", characterId);
            if (this.Characters.ContainsKey(characterId))
            {
                EntityManager.Instance.RemoveEntity(this.Characters[characterId].Info.Entity);
                if(OnCharacterLeave != null)
                {
                    OnCharacterLeave(this.Characters[characterId]);
                }
                this.Characters.Remove(characterId);
            }
            

        }
    }
}
