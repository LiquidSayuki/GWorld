using Assets.Scripts.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    internal class GuildService : Singleton<GuildService>, IDisposable
    {
        public UnityAction<bool> OnGuildCreateResult;
        public UnityAction OnGuildUpdate;

        public GuildService() 
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinRes);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinReq);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild); //工会信息请求
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

        public void Dispose()
        {

        }

        internal void Init() { }

        /// <summary>
        /// 发送工会创建请求
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="guildNotice"></param>
        internal void SendGuildCreate(string guildName, string guildNotice)
        {
            Debug.Log("Send Guild Create");
            NetMessage message= new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.guildName = guildName;
            message.Request.guildCreate.guildNotice = guildNotice;
            NetClient.Instance.SendMessage(message);
        }
        private void OnGuildCreate(object sender, GuildCreateResponse message)
        {
            Debug.Log("On Guild Create");
            if (OnGuildCreateResult != null)
            {
                //创建工会UI收到true则关闭
                this.OnGuildCreateResult(message.Result == Result.Success);
            }
            if(message.Result == Result.Success)
            {
                GuildManager.Instance.Init(message.guildInfo);
                MessageBox.Show(string.Format("{0}创建成功", message.guildInfo.guildName), "工会");
            }
            else
            {
                //TODO: 告知用户失败原因
                MessageBox.Show(string.Format("工会创建失败"), "工会");
            }
        }
        /// <summary>
        /// 发送工会加入申请
        /// </summary>
        /// <param name="guildId"></param>
        public void SendGuildJoinRequest(int guildId)
        {
            Debug.Log("Send Guild Join Request");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(message);
        }
        private void OnGuildJoinRes(object sender, GuildJoinResponse message)
        {
            Debug.Log("Guild Join Response Received!");
            if (message.Result == Result.Success)
            {
                MessageBox.Show(" 加入公会成功", "工会");
            }
            else
            {
                MessageBox.Show( "加入工会失败","工会");
            }
        }
        /// <summary>
        /// 回复工会加入申请（实时的）
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="request"></param>
        public void SendGuildJoinRes(bool accept, GuildJoinRequest request)
        {
            Debug.Log("Send Guild Join Response");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = request.Apply;
            // 此处才是实际拒绝和接受请求的位置
            message.Request.guildJoinRes.Apply.Result = accept? ApplyResult.Accept: ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 回复工会加入申请（列表的）
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="apply"></param>
        public void SendGuildJoinApply(bool accept, NGuildApplyInfo apply)
        {
            Debug.Log("Send Guild Join Response!");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }
        private void OnGuildJoinReq(object sender, GuildJoinRequest message)
        {
            var confirm = MessageBox.Show(string.Format("[{0}]  请求加入工会", message.Apply.Name), "工会申请", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendGuildJoinRes(true, message);
            };
            confirm.OnNo = () =>
            {
                this.SendGuildJoinRes(false, message);
            };
        }

        internal void SendGuildLeave()
        {
            Debug.Log("Send Guild Leave Request");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }
        private void OnGuildLeave(object sender, GuildLeaveResponse message)
        {
            Debug.Log("Guild Leave Response Received!");
            if (message.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show(" 退出公会成功", "工会");
            }
            else
            {
                MessageBox.Show("退出工会失败", "工会",MessageBoxType.Error);
            }
        }

        private void OnGuild(object sender, GuildResponse message)
        {
            GuildManager.Instance.Init(message.guildInfo);
            if (this.OnGuildUpdate != null)
            {
                this.OnGuildUpdate.Invoke();
            }
        }

        /// <summary>
        /// 拉取工会列表
        /// </summary>
        internal void SendGuildListRequest()
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }
        private void OnGuildList(object sender, GuildListResponse message)
        {
            GuildManager.Instance.allGuilds = message.Guilds;
            if(this.OnGuildUpdate != null)
            {
                this.OnGuildUpdate.Invoke();
            }
        }

        /// <summary>
        /// 发送工会管理指令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="id"></param>
        internal void SendAdminCommond(GuildAdminCommand command, int id)
        {
            Debug.Log("Send Guild Admin Commond!");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = command;
            message.Request.guildAdmin.targetId = id;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildAdmin(object sender, GuildAdminResponse message)
        {
            Debug.Log("Guild Admin Commond response received!");
            MessageBox.Show(string.Format("{0},{1},{2}", message.Command.ToString(), message.Result, message.Errormsg));
        }
    }
}
