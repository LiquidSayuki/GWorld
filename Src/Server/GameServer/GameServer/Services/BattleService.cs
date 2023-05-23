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
    class BattleService: Singleton<BattleService>
    {
        public BattleService() 
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<SkillCastRequest>(this.OnSkillCastRequest) ;
        }
        public void Init() { }
        private void OnSkillCastRequest(NetConnection<NetSession> sender, SkillCastRequest message)
        {
            Character cha = sender.Session.Character;
            Log.InfoFormat("");
            sender.Session.Response.skillCast = new SkillCastResponse();
            sender.Session.Response.skillCast.Result = Result.Success;
            sender.Session.Response.skillCast.castInfo = message.castInfo;

            MapManager.Instance[cha.Info.mapId].BroadcastBattleResponse(sender.Session.Response);
        }
    }
}
