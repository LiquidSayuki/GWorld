using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    internal class ChatManager:Singleton<ChatManager>
    {
        public enum LocalChannel
        {
            All = 0,
            Local = 1,
            World = 2,
            Team = 3,
            Guild = 4,
            Private = 5,
        }
        public Action OnChat { get; internal set; }

        public string PrivateName = "";
        public int PrivateID = 0;

        public LocalChannel displayChannel;
        public LocalChannel sendChannel;

        public ChatChannel SendChannel
        {
            get
            {
                switch (sendChannel)
                {
                    case LocalChannel.Local: return ChatChannel.Local;
                    case LocalChannel.World: return ChatChannel.World;
                    case LocalChannel.Private: return ChatChannel.Private;
                    case LocalChannel.Team: return ChatChannel.Team;
                    case LocalChannel.Guild: return ChatChannel.Guild;
                }
                return ChatChannel.Local;
            }
        }
        /// <summary>
        /// 本地显示的6种频道与服务器聊天频道的对应
        /// </summary>
        private ChatChannel[] ChannelFilter = new ChatChannel[6]
        {
            /*
             ChatChannel的枚举值，数组是1 2 4 8 16 32
            也就是 0001 0010 0100 1000这样的模式
            所以此处才可以 | 这些枚举值
            或者说就像linux的 777 一样确定哪些频道通过过滤
            或者的或者，就可以实现完全自定义的通信频道复选
             */
            ChatChannel.Local | ChatChannel.World | ChatChannel.Team |ChatChannel.Guild | ChatChannel.Private | ChatChannel.System,
            ChatChannel.Local,
            ChatChannel.World, 
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private,
        };
        /// <summary>
        /// 本地6个对应频道的对应消息的列表
        /// </summary>
        public List<ChatMessage>[] Messages = new List<ChatMessage>[6]
        {
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
        };

        public void Init() 
        { 
            foreach (var i in this.Messages)
            {
                i.Clear();
            }
        }

        internal void StartPrivateChat(int targetId, string targetName)
        {
            this.PrivateID = targetId;
            this.PrivateName= targetName;

            this.sendChannel = LocalChannel.Private;
            if(this.OnChat != null)
            {
                this.OnChat();
            }
        }

        internal void SendChat(string content, int toId = 0, string toName = "")
        {
            /*            this.Messages.Add(new ChatMessage()
                        {
                            Channel = ChatChannel.Local,
                            Message = content,
                            FromId = User.Instance.CurrentCharacter.Id,
                            FromName = User.Instance.CurrentCharacter.Name,
                        });*/
            ChatService.Instance.SendChat(this.SendChannel, content, toId, toName);
        }

        internal void AddMessage(ChatChannel channel, List<ChatMessage> messages)
        {
            for(int i = 0; i < 6; i++)
            {
                if ((this.ChannelFilter[i] & channel) == channel) // 此处就是刚才 111111 的应用
                {
                    this.Messages[i].AddRange(messages);
                }
            }
            if(this.OnChat != null)
            {
                this.OnChat();
            }
        }
        public void AddSystemMessage(string message, string from = "")
        {
            this.Messages[(int)LocalChannel.All].Add(new ChatMessage
            {
                Channel = ChatChannel.System,
                Message = message,
                FromName = from,
            });
            if (this.OnChat != null)
            {
                this.OnChat();
            }
        }

        /// <summary>
        /// 获取当前显示频道 所有的聊天消息文本
        /// </summary>
        /// <returns></returns>
        public string GetCurrentMessages()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var message in this.Messages[(int)displayChannel])
            {
                sb.AppendLine(FormatMessage(message));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 改变当前的发送频道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool SetSendChannel(LocalChannel channel)
        {
            if (channel == LocalChannel.Team)
            {
                if (User.Instance.TeamInfo == null)
                {
                    this.AddSystemMessage("你没有加入任何队伍");
                    return false;
                }
            }
            if (channel == LocalChannel.Guild)
            {
                if(User.Instance.CurrentCharacterInfo.Guild == null)
                {
                    this.AddSystemMessage("你没有加入任何工会");
                    return false;
                }
            }
            this.sendChannel = channel;
            Debug.LogFormat("SetChannel:{0}", this.sendChannel);
            return true;
        }

        /// <summary>
        /// 对不同频道格式化对应聊天消息文本
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string FormatMessage(ChatMessage message)
        {
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    return string.Format("[本地]{0}{1}",FormatFromPlayer(message), message.Message);
                case ChatChannel.World:
                    return string.Format("<color=cyan> [世界]{0}{1} </color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.System:
                    return string.Format("<color=red> [系统]{0} </color>", message.Message);
                case ChatChannel.Private:
                    return string.Format("<color=magenta> [私聊]{0}{1} </color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Team:
                    return string.Format("<color=green> [队伍]{0}{1} </color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Guild:
                    return string.Format("<color=blue> [工会]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            }
            return "";
        }

        /// <summary>
        /// 制作聊天消息抬头连接（发送人）
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string FormatFromPlayer(ChatMessage message)
        {
            if(message.FromId == User.Instance.CurrentCharacterInfo.Id)
            {
                return "<a name=\"\" class = \"player\"> [你]</a>";
            }
            else
            {
                return string.Format("<a name=\"c:{0}:{1}\" class = \"player\"> [{1}]</a>", message.FromId,message.FromName);
            }
        }
    }
}
