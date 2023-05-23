using Common.Data;
using Entities;
using SkillBridge.Message;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        SkillBridge.Message.NUserInfo userInfo;
        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }

        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            this.userInfo = info;
        }


        public MapDefine CurrentMapData { get; set; }

        //角色信息
        public SkillBridge.Message.NCharacterInfo CurrentCharacterInfo { get; set; }
        //角色实体
        public Creature CurrentCharacter { get; set; }

        public GameObject CurrentCharacterObject { get; set; }

        public NTeamInfo TeamInfo { get; set; }

        public void AddGold(int gold)
        {
            this.CurrentCharacterInfo.Gold += gold;
        }

    }
}
