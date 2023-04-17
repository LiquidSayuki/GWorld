using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    internal class ChatService:Singleton<ChatService>
    {
        public ChatService() 
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ChatRequest>(this.OnChat);
        }
        public void Init()
        {
            ChatManager.Instance.Init();
        }
        private void OnChat(NetConnection<NetSession> sender, ChatRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("Chat From{0}-{1}-Channel{2}", character.Id, character.Name, message.Message.Channel);

            //单独处理私聊
            if(message.Message.Channel == ChatChannel.Private)
            {
                var chatTo = SessionManager.Instance.GetSession(message.Message.ToId);
                if(chatTo == null)
                {
                    sender.Session.Response.Chat = new ChatResponse();
                    sender.Session.Response.Chat.Result = Result.Failed;
                    sender.Session.Response.Chat.Errormsg = "对方不在线";
                    sender.Session.Response.Chat.privateMessages.Add(message.Message);
                    sender.SendResponse();
                    return;
                }
                else
                {
                    if(chatTo.Session.Response.Chat == null)
                    {
                        chatTo.Session.Response.Chat = new ChatResponse();
                    }
                    message.Message.FromId = character.Id;
                    message.Message.FromName = character.Name;
                    chatTo.Session.Response.Chat.Result = Result.Success;
                    chatTo.Session.Response.Chat.privateMessages.Add(message.Message);
                    chatTo.SendResponse();

                    if(sender.Session.Response.Chat == null)
                    {
                        sender.Session.Response.Chat = new ChatResponse();
                    }
                    sender.Session.Response.Chat.Result = Result.Success;
                    sender.Session.Response.Chat.privateMessages.Add(message.Message);
                    sender.SendResponse();
                }
            }
            else//公共聊天交付给管理器
            {
                sender.Session.Response.Chat = new ChatResponse();
                sender.Session.Response.Chat.Result = Result.Success;
                ChatManager.Instance.AddMessage(character, message.Message);
                sender.SendResponse();
            }
        }
    }


}
