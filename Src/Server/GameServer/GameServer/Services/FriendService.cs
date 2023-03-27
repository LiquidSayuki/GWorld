using Common;
using GameServer.Entities;
using GameServer.Managers;
using log4net;
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
    internal class FriendService:Singleton<FriendService>
    {
        public FriendService() 
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddReq);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddRes);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);
        }

        public void Init(){}

        /// <summary>
        ///  收到添加好友请求后，验证请求合法性，并将添加好友请求转发给目标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnFriendAddReq(NetConnection<NetSession> sender, FriendAddRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddRequest : From[{0}:{1}] to [{2}:{3}]", message.FromId, message.FromName, message.ToId, message.ToName);

            // 如果没有传入ID，则认为是通过名字查找
            if (message.ToId == 0)
            {
                foreach (var cha in CharacterManager.Instance.Characters)
                {
                    if (cha.Value.Data.Name == message.ToName)
                    {
                        message.ToId = cha.Key;
                        break;
                    }
                }
            }

            NetConnection<NetSession> friend = null;
            if (message.ToId > 0)
            {
                if (character.FriendManager.GetFriendInfo(message.ToId) != null)
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "已经是好友了";
                    sender.SendResponse();
                    return;
                }
                // 取得对方玩家的Session，以便转发请求
                friend = SessionManager.Instance.GetSession(message.ToId);
            }

            if(friend == null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "角色不存在或不在线";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("ForwardRequest: From [{0}:{1}] to [{2}:{3}]", message.FromId, message.FromName, message.ToId, message.ToName);
            friend.Session.Response.friendAddReq = message;
            friend.SendResponse();
        }


        /// <summary>
        /// 收到好友请求回复时，向双方返回处理结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnFriendAddRes(NetConnection<NetSession> sender, FriendAddResponse message)
        {
            Character character = sender.Session.Character;
             Log.InfoFormat("OnFriendAddResPonse: From [{0}:{1}] to [{2}:{3}] : Result[{4}]", message.Request.ToId, message.Request.ToName,message.Request.FromId, message.Request.FromName, message.Result);
            sender.Session.Response.friendAddRes = message;
            var requester = SessionManager.Instance.GetSession(message.Request.FromId);
            if(requester == null)
            {
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "对方已经下线";
            }
            else
            {
                if(message.Result == Result.Success)
                {
                    // 将AB互相添加进对方好友列表中
                    character.FriendManager.AddFriend(requester.Session.Character);
                    requester.Session.Character.FriendManager.AddFriend(character);
                    DBService.Instance.Save();
                    // 结果发送给请求方
                    requester.Session.Response.friendAddRes = message;
                    requester.Session.Response.friendAddRes.Result = Result.Success;
                    requester.Session.Response.friendAddRes.Errormsg = "添加好友成功";
                }
                else
                {
                    requester.Session.Response.friendAddRes = message;
                    requester.Session.Response.friendAddRes.Result = Result.Failed;
                    requester.Session.Response.friendAddRes.Errormsg = "对方拒绝了您的好友请求";
                }
                requester.SendResponse();
            }
            //sender.Session.Response.friendAddRes.Result = Result.Success;
            //sender.Session.Response.friendAddRes.Errormsg = message.Request.FromName + " 成为您的好友";
            sender.SendResponse();
        }

        /// <summary>
        /// 收到删除好友的请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnRemoveFriend: From [{0}:{1}] to [{2}]", character.Id,character.Info.Name, message.friendId);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = message.Id;

            //移除自己好友列表的对方
            if (character.FriendManager.RemoveFriendByID(message.Id))
            {
                sender.Session.Response.friendRemove.Result = Result.Success;

                //移除对方好友列表中的自己
                var friend = SessionManager.Instance.GetSession(message.friendId);
                if(friend != null)
                { // 对方在线
                    friend.Session.Character.FriendManager.RemoveFriendByID(character.Id);
                }
                else
                {// 对方不在线
                    this.RemoveFriend(message.friendId, character.Id);
                }
            }
            else
            {
                sender.Session.Response.friendRemove.Result = Result.Failed;
            }

            DBService.Instance.Save();

            sender.SendResponse();
        }

        /// <summary>
        /// 直接通过数据库去操作好友列表
        /// 用于一方不在线，没有好友manager以及session的情况
        /// </summary>
        /// <param name="chaId"></param>
        /// <param name="friendId"></param>
        private void RemoveFriend(int chaId, int friendId)
        {
            var removeItem = DBService.Instance.Entities.CharacterFriends.FirstOrDefault(v => v.CharacterID == chaId && v.FriendID == friendId);
            if (removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
        }

    }
}
