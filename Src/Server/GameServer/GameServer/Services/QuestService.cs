using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class QuestService : Singleton<QuestService>
    {
        public QuestService() 
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAcceptRequest>(this.OnQuestAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(this.OnQuestSubmit);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestListRequest>(this.OnQuestList);
        }
        public void Init(){}

        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("QuestSubmit [Character: {0}] [QuestID{1}]", character.Id, message.QuestId);


            sender.Session.Response.questSubmit = new QuestSubmitResponse();

            Result result = character.QuestManager.SubmitQuest(sender, message.QuestId);

            sender.Session.Response.questSubmit.Result = result;
            sender.SendResponse();
        }

        private void OnQuestAccept(NetConnection<NetSession> sender, QuestAcceptRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("QuestAccept [Character: {0}] [QuestID{1}]", character.Id, message.QuestId);


            sender.Session.Response.questAccept = new QuestAcceptResponse();

            Result result = character.QuestManager.AcceptQuest(sender, message.QuestId);

            sender.Session.Response.questAccept.Result = result;
            sender.SendResponse();
        }

        private void OnQuestList(NetConnection<NetSession> sender, QuestListRequest message)
        {
            throw new NotImplementedException();
        }
    }
}
