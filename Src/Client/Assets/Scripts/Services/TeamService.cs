using Assets.Scripts.Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;

namespace Services
{
    internal class TeamService : Singleton<TeamService>, IDisposable
    {
        internal void Init()
        {
        }

        public TeamService()
        {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        /// <summary>
        /// 向好友发送加入组队请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="friendId"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal void SendTeamInviteRequest(int friendId, string friendName)
        {
            Debug.Log("Send Team Invite REQUEST");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteReq = new TeamInviteRequest();
            message.Request.teamInviteReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.teamInviteReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.teamInviteReq.ToId = friendId;
            message.Request.teamInviteReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        private void OnTeamInviteRequest(object sender, TeamInviteRequest message)
        {
            var confirm = MessageBox.Show(string.Format("[{0}]  邀请你加入队伍", message.FromName), "组队请求", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendTeamInviteResponse(true, message);
            };
            confirm.OnNo = () =>
            {
                this.SendTeamInviteResponse(false, message);
            };
        }

        /// <summary>
        /// 回应他人组队请求
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="request"></param>
        internal void SendTeamInviteResponse(bool accept, TeamInviteRequest request)
        {
            Debug.Log("Send Team Invite RESPONSE");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInivteRes = new TeamInviteResponse();
            message.Request.teamInivteRes.Result = accept ? Result.Success : Result.Failed;
            message.Request.teamInivteRes.Errormsg = accept ? "对方接受了您的组队请求" : "对方拒绝了您的组队请求";
            message.Request.teamInivteRes.Request = request;
            NetClient.Instance.SendMessage(message);
        }
        private void OnTeamInviteResponse(object sender, TeamInviteResponse message)
        {
            if (message.Result == Result.Success)
            {
                MessageBox.Show(message.Request.ToName + "加入了队伍", "组队成功");
            }
            else
            {
                MessageBox.Show(message.Errormsg, "组队失败");
            }
        }

        private void OnTeamInfo(object sender, TeamInfoResponse message)
        {
            Debug.Log("On Teaminfo Received");
            TeamManager.Instance.UpdateTeamInfo(message.Team);
        }

        internal void SendTeamLeaveRequest(int id)
        {
            Debug.Log("Send Team Leave Request");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamLeave = new TeamLeaveRequest();
            message.Request.teamLeave.TeamId = id;
            message.Request.teamLeave.CharacterId = User.Instance.CurrentCharacter.Id;
            NetClient.Instance.SendMessage(message);
        }
        private void OnTeamLeave(object sender, TeamLeaveResponse message)
        {
            if (message.Result == Result.Success)
            {
                if (message.CharacterId == User.Instance.TeamInfo.Leader)
                {
                    MessageBox.Show("队长离开，队伍解散", "退出队伍");
                }
                else
                {
                    MessageBox.Show("退出成功", "退出队伍");
                }
                TeamManager.Instance.UpdateTeamInfo(null);
            }
            else
            {
                MessageBox.Show("退出失败", "退出队伍", MessageBoxType.Error);
            }
        }
    }
}
