using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class ChatManager:Singleton<ChatManager>
    {
        public List<ChatMessage> System = new List<ChatMessage> ();
        public List<ChatMessage> World = new List<ChatMessage>();
        public Dictionary<int,List<ChatMessage>> Local = new Dictionary<int, List<ChatMessage>>();
        public Dictionary<int, List<ChatMessage>> Team = new Dictionary<int, List<ChatMessage>>();
        public Dictionary<int, List<ChatMessage>> Guild = new Dictionary<int, List<ChatMessage>>();

        public void Init() { }

        public void AddMessage(Character from, ChatMessage message)
        {
            message.FromId = from.Id;
            message.FromName = from.Name;
            message.Time = TimeUtil.timestamp;
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    this.AddLocalMessage(from.Info.mapId, message);
                    break;
                case ChatChannel.World:
                    this.AddWorldMessage(message);
                    break;
                case ChatChannel.System:
                    this.AddSystemMessage(message);
                    break;
                case ChatChannel.Team:
                    this.AddTeamMessage(from.team.Id, message);
                    break;
                case ChatChannel.Guild:
                    this.AddGuildMessage(from.Guild.Id, message);
                    break;
            }
        }
        private void AddLocalMessage(int mapId, ChatMessage message)
        {
            if (!this.Local.TryGetValue(mapId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Local[mapId] = messages;
            }
            messages.Add(message);
        }

        private void AddWorldMessage(ChatMessage message)
        {
            this.World.Add(message);
        }

        private void AddSystemMessage(ChatMessage message)
        {
            this.System.Add(message);
        }
        private void AddTeamMessage(int teamId, ChatMessage message)
        {
            if (!this.Team.TryGetValue(teamId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Local[teamId] = messages;
            }
            messages.Add(message);
        }
        private void AddGuildMessage(int guildId, ChatMessage message)
        {
            if (!this.Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Local[guildId] = messages;
            }
            messages.Add(message);
        }

        public int GetLocalMessages(int mapId, int localIdx, List<ChatMessage> result)
        {
            if (!this.Local.TryGetValue(mapId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessages(localIdx, result, messages);
        }

        internal int GetWorldMessages(int worldIdx, List<ChatMessage> result)
        {
            return GetNewMessages(worldIdx, result, this.World);
        }

        internal int GetSystemMessages(int systemIdx, List<ChatMessage> result)
        {
            return GetNewMessages(systemIdx, result, this.System);
        }

        public int GetTeamMessages(int teamId, int teamIdx, List<ChatMessage> result)
        {
            if (!this.Local.TryGetValue(teamId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessages(teamIdx, result, messages);
        }

        public int GetGuildMessages(int guildId, int idx, List<ChatMessage> result)
        {
            if(!this.Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessages(idx, result, messages);
        }


        /// <summary>
        /// 获取更新的消息
        /// </summary>
        /// <param name="idx">当前消息位置</param>
        /// <param name="result">玩家的消息数组</param>
        /// <param name="messages">服务器的消息数组</param>
        /// <returns>本次拉取后最新的消息idx位置</returns>
        private int GetNewMessages(int idx, List<ChatMessage> result, List<ChatMessage> messages)
        {
            if(idx == 0) //玩家首次来拉取聊天消息
            {
                if(messages.Count > GameDefine.MaxChatRecordNums) //如果聊天消息总数大于设定值
                {
                    idx = messages.Count - GameDefine.MaxChatRecordNums; //将idx指向总数-设定值的位置，这样玩家就只能拉取到最近的若干条消息
                }
            }

            //从idx开始，拉取消息直到最新为止
            for(; idx < messages.Count; idx++)
            {
                result.Add(messages[idx]);
            }
            return idx;
        }
    }
}
