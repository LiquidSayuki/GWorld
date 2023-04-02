using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    internal class TeamService: Singleton<TeamService>
    {

       public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteReq);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteRes);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeave);
        }
        public void Init() { TeamManager.Instance.Init(); }



        /// <summary>
        /// 收到组队请求，处理组队请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnTeamInviteReq(NetConnection<NetSession> sender, TeamInviteRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("Team Invite Request: From[{0}-{1}] to[{2}-{3}]", message.FromId, message.FromName, message.ToId, message.ToName);

            NetConnection<NetSession> target = SessionManager.Instance.GetSession(message.ToId);
            if(target == null)
            {
                sender.Session.Response.teamInivteRes = new TeamInviteResponse();
                sender.Session.Response.teamInivteRes.Result = Result.Failed;
                sender.Session.Response.teamInivteRes.Errormsg = "对方不在线";
                sender.SendResponse();
                return;
            }

            if(target.Session.Character.team != null)
            {
                sender.Session.Response.teamInivteRes = new TeamInviteResponse();
                sender.Session.Response.teamInivteRes.Result = Result.Failed;
                sender.Session.Response.teamInivteRes.Errormsg = "对方已在其他队伍中";
                sender.SendResponse();
                return;
            }

            //执行转发
            target.Session.Response.teamInviteReq = message;
            target.SendResponse();
            return;
        }

        /// <summary>
        /// 收到组队回应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnTeamInviteRes(NetConnection<NetSession> sender, TeamInviteResponse message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("Team Invite Response: From[{0}-{1}] to[{2}-{3}]", message.Request.ToId, message.Request.ToName, message.Request.FromId, message.Request.FromName);

            sender.Session.Response.teamInivteRes = message;
            if(message.Result == Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(message.Request.FromId);
                if(requester == null)
                {
                    sender.Session.Response.teamInivteRes.Result = Result.Failed;
                    sender.Session.Response.teamInivteRes.Errormsg = "对方已下线";
                }
                else
                {
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, character);

                    //让回复方收到回复时显示的入队人信息正常一些
                    sender.Session.Response.teamInivteRes.Request.ToName = "您";

                    requester.Session.Response.teamInivteRes = message;
                    requester.SendResponse();
                }
            }
            sender.SendResponse();
        }

        private void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest message)
        {
            
            Character character = sender.Session.Character;
            Log.InfoFormat("Team Leave [{0}-{1}]", character.Id, character.Info.Name);
            //队长退出,队伍解散，对所有人发送消息
            if (character.team.Leader.Id == character.Id)
            {
                foreach(Character c in character.team.Members)
                {
                    Log.InfoFormat("LeaderLeave [{0}-{1}] kicked off from team", c.Id, c.Info.Name);
                    var member = SessionManager.Instance.GetSession(c.Id);
                    member.Session.Response.teamLeave = new TeamLeaveResponse();
                    member.Session.Response.teamLeave.Result = Result.Success;
                    member.Session.Response.teamLeave.CharacterId = character.Id;
                    member.SendResponse();
                }
            }

            character.team.Leave(character);
            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            sender.Session.Response.teamLeave.Result = Result.Success;
            sender.Session.Response.teamLeave.Errormsg = null;
            sender.Session.Response.teamLeave.CharacterId = character.Id;
            sender.SendResponse();
        }

    }
}
