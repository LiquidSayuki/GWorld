using Common;
using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Team team;
        public int TeamUpdateTS; //时间戳

        public Guild Guild;
        public double GuildUpdateTS;

        public Chat chat;

        public Character(CharacterType type, TCharacter cha) :
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ), new Core.Vector3Int(100, 0, 0))
        {

            this.Data = cha;
            // cha 数据库 character
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

            this.Guild = GuildManager.Instance.GetGuild(this.Data.GuildId);

            this.chat = new Chat(this);
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
        /// 对角色状态的更新,唤醒一些管理器，要求他们把最新信息附带在message中等待发送
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            //Friend
            this.FriendManager.PostProcess(message);

            //Team
            if(this.team != null)
            {
                // 通过时间戳判断队伍信息是否相比之前有更新过
                Log.InfoFormat("CharacterTS [{0}] teamTS[{1}] Result:[{2}]", TeamUpdateTS, team.timestamp, TeamUpdateTS < team.timestamp);
                if (TeamUpdateTS < this.team.timestamp)
                {         
                    TeamUpdateTS = team.timestamp;
                    this.team.PostProcess(message);
                }
            }

            // Guild
            if(this.Guild != null)
            {
                if(this.Info.Guild == null) // 角色登陆时没有工会，后来加入了工会
                {
                    this.Info.Guild = this.Guild.GuildInfo(this);
                    if(message.mapCharacterEnter != null)
                    {
                        GuildUpdateTS = Guild.timestamp;
                    }
                }

                if(GuildUpdateTS < this.Guild.timestamp && message.mapCharacterEnter == null)
                {
                    GuildUpdateTS = Guild.timestamp;
                    this.Guild.PostProcess(this, message);
                }
            }

            //Chat
            if(this.chat != null)
            {
                this.chat.PostProcess(message);
            }

            //Status
            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProcess(message);
            }
        }

        // 角色离开时
        public void Clear()
        {
            this.FriendManager.OfflineNotify();
        }

        /// <summary>
        /// 复制一份自身的基本信息出去
        /// </summary>
        /// <returns>角色的简略信息</returns>
        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo
            {
                Id = this.Id,
                Name = this.Info.Name,
                Class = this.Info.Class,
                Level = this.Info.Level,
            };
        }
    }
}
