using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    /// <summary>
    /// Character
    /// 玩家角色类
    /// </summary>
    class Character : CharacterBase,IPostResponser
    {

        public TCharacter Data;
        public ItemManager ItemManager;
        public StatusManager StatusManager;
        public QuestManager QuestManager;
        public FriendManager FriendManager;


        public Character(CharacterType type, TCharacter cha) :
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ), new Core.Vector3Int(100, 0, 0))
        {

            this.Data = cha;
            // cha 数据库character
            // ID 是唯一的DB ID
            this.Id = cha.ID;

            this.Info = new NCharacterInfo();
            this.Info.Id = cha.ID;
            this.Info.EntityId = this.entityId; // Entity ID
            this.Info.ConfigId = cha.TID; //Config ID
            this.Info.Type = type;
            this.Info.Name = cha.Name;
            this.Info.Level = 10;//cha.Level;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Entity = this.EntityData;
            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Unlocked = this.Data.Bag.Unlocked;
            this.Info.Bag.Items = this.Data.Bag.Items;
            this.Info.Equips = this.Data.Equips;


            this.Define = DataManager.Instance.Characters[this.Info.ConfigId];



            this.ItemManager = new ItemManager(this);
            this.ItemManager.GetItemInfos(this.Info.Items);

            this.StatusManager = new StatusManager(this);

            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(this.Info.Quests);

            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(this.Info.Friends);
        }

        public long Gold
        {
            get { return this.Data.Gold; }

            set
            {
                if (this.Data.Gold == value)
                    return;
                this.StatusManager.AddGoldChange((int)(value - this.Data.Gold));
                this.Data.Gold = value;
            }
        }

        /// <summary>
        /// 对角色状态的更新
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            this.FriendManager.PostProcess(message);
            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProcess(message);
            }
        }

        // 角色离开时
        public void Clear()
        {
            this.FriendManager.UpdateFriendInfo(this.Info, 0);
        }
    }
}
