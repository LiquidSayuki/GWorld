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
    internal class GuildService: Singleton<GuildService>
    {
        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinRes);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinReq);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest> (this.OnGuildLeave);
            //MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildRequest>(this.OnGuild); 
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);
        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }
        /// <summary>
        /// 收到创建工会的请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest message)
        {
            Character c = sender.Session.Character;
            Log.InfoFormat("OnGuildCreate : character[{0}-{1}] guild[{2}]", c.Info.Name, c.Id, message.guildName);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if(c.Guild != null)
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "不能加入多个工会";
                sender.SendResponse();
                return;
            }

            if (GuildManager.Instance.CheckNameExisted(message.guildName))
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "工会名称已经存在";
                sender.SendResponse();
                return;
            }

            GuildManager.Instance.CreateGuild(message.guildName, message.guildNotice, c);
            sender.Session.Response.guildCreate.guildInfo = c.Guild.GuildInfo(c);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();
        }
        /// <summary>
        /// 收到加入工会的请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinReq(NetConnection<NetSession> sender, GuildJoinRequest message)
        {
            Character c = sender.Session.Character;
            Log.InfoFormat("OnGuildJoin REQ : character[{0}-{1}] guild[{2}]", c.Info.Name, c.Id, message.Apply.GuildId);
            var guild = GuildManager.Instance.GetGuild(message.Apply.GuildId);

            
            if (guild == null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "工会不存在";
                sender.SendResponse();
                return;
            }

            message.Apply.characterId = c.Data.ID;
            message.Apply.Name = c.Data.Name;
            message.Apply.Class= c.Data.Class;
            message.Apply.Level= c.Data.Level;

            if (guild.JoinApply(message.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderId);
                if(leader != null) // 如果会长在线，向会长直接发送请求
                {
                    leader.Session.Response.guildJoinReq = message;
                    leader.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "请勿重复提交申请";
                sender.SendResponse();
            }
        }
        /// <summary>
        /// 收到加入工会的回应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinRes(NetConnection<NetSession> sender, GuildJoinResponse message)
        {
            Character c = sender.Session.Character;

            var guild = GuildManager.Instance.GetGuild(message.Apply.GuildId);
            if(message.Result == Result.Success)
            {
                guild.JoinApprove(message.Apply);
            }

            var requester = SessionManager.Instance.GetSession(message.Apply.characterId);
            if(requester != null) //如果在线，发送加入成功信息
            {
                requester.Session.Character.Guild = guild;

                requester.Session.Response.guildJoinRes = message;
                requester.Session.Response.guildJoinRes.Result = Result.Success;
                requester.Session.Response.guildJoinRes.Errormsg = "加入工会成功";
                requester.SendResponse();
            }
        }

        /// <summary>
        /// 收到拉取工会列表的请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest message)
        {
            Character c = sender.Session.Character;
            Log.InfoFormat("OnGuildList : character[{0}-{1}]", c.Info.Name, c.Id);

            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest message)
        {
            Character c = sender.Session.Character;

            sender.Session.Response.guildLeave = new GuildLeaveResponse();
            c.Guild.Leave(c.Id);
            sender.Session.Response.guildLeave.Result = Result.Success;
            DBService.Instance.Save();
            sender.SendResponse();
        }

        /// <summary>
        /// 处理工会管理请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildAdmin : character[{0}-{1}], Operation[{2}], to [{3}]", character.Name, character.Id, message.Command.ToString(), message.targetId.ToString());
            sender.Session.Response.guildAdmin = new GuildAdminResponse();
            if(character.Guild == null)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "你还没有加入工会";
                sender.SendResponse();
                return;
            }

            //TODO: 根据执行成功与否，返回不同的消息
            bool success = character.Guild.ExecuteAdmin(message.Command, message.targetId, character.Id);
           
            if (success)
            {
                var target = SessionManager.Instance.GetSession(message.targetId);
                //目标人员在线向他也发送工会修改信息
                if (target != null)
                {
                    target.Session.Response.guildAdmin = new GuildAdminResponse();
                    target.Session.Response.guildAdmin.Result = Result.Success;
                    target.Session.Response.guildAdmin.Command = message;
                    target.SendResponse();
                }
            }

            sender.Session.Response.guildAdmin.Result = success ? Result.Success : Result.Failed;
            sender.Session.Response.guildAdmin.Command = message;
            sender.SendResponse();
        }
    }
}
