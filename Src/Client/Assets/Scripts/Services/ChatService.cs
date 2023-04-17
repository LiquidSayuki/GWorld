using Assets.Scripts.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    internal class ChatService : Singleton<ChatService>, IDisposable
    {
        public ChatService() 
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(this.OnChat);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ChatResponse>(this.OnChat);
        }

        public void Init() { }

        private void OnChat(object sender, ChatResponse message)
        {
            if(message.Result == Result.Success)
            {
                if(message.worldMessages !=null) ChatManager.Instance.AddMessage(ChatChannel.World, message.worldMessages);
                if(message.teamMessages != null) ChatManager.Instance.AddMessage(ChatChannel.Team, message.teamMessages);
                if (message.privateMessages != null) ChatManager.Instance.AddMessage(ChatChannel.Private, message.privateMessages);
                if (message.systemMessages != null) ChatManager.Instance.AddMessage(ChatChannel.System, message.systemMessages);
                if (message.guildMessages != null) ChatManager.Instance.AddMessage(ChatChannel.Guild, message.guildMessages);
                if (message.localMessages != null) ChatManager.Instance.AddMessage(ChatChannel.Local, message.localMessages);
            }
            else
            {
                MessageBox.Show(message.Errormsg);
            }
        }

        internal void SendChat(ChatChannel sendChannel, string content, int toId, string toName)
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Message = new ChatMessage();
            message.Request.Chat.Message.Channel = sendChannel;
            message.Request.Chat.Message.Message = content;
            message.Request.Chat.Message.ToId = toId;
            message.Request.Chat.Message.ToName= toName;
            NetClient.Instance.SendMessage(message);
        }


    }
}
