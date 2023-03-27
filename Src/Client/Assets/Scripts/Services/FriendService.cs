﻿using Assets.Scripts.Managers;
using Models;
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
    public class FriendService : Singleton<FriendService>, IDisposable
    {
        public UnityAction OnFriendUpdate;

        public void Init()
        {

        }

        public FriendService()
        {
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Subscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }

        /// <summary>
        /// 向他人发送好友请求
        /// </summary>
        /// <param name="friendId"></param>
        /// <param name="friendName"></param>
        internal void SendFriendAddRequest(int friendId, string friendName)
        {
            Debug.Log("Send Friend Add Request");
            NetMessage message = new NetMessage(); 
            message.Request = new NetMessageRequest();
            message.Request.friendAddReq = new FriendAddRequest();
            message.Request.friendAddReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.friendAddReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.friendAddReq.ToId = friendId;
            message.Request.friendAddReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }
        // 收到他人回复
        private void OnFriendAddResponse(object sender, FriendAddResponse message)
        {
            if(message.Result == Result.Success)
            {
                MessageBox.Show(message.Request.ToName + " 接受了您的请求", "添加好友成功");
            }
            else
            {
                MessageBox.Show(message.Errormsg, "添加好友失败");
            }
        }

        /// <summary>
        /// 收到他人好友请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnFriendAddRequest(object sender, FriendAddRequest message)
        {
            var confirm = MessageBox.Show(string.Format("[{0}]  请求添加你为好友", message.FromName), "好友请求", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendFriendAddResponse(true, message);
            };
            confirm.OnNo = () =>
            {
                this.SendFriendAddResponse(false, message);
            };
        }
        // 回复他人好友请求
        internal void SendFriendAddResponse(bool accept, FriendAddRequest request)
        {
            Debug.Log("Send Friend Add Response");
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.friendAddRes = new FriendAddResponse();
            message.Response.friendAddRes.Result = accept ? Result.Success : Result.Failed;
            message.Response.friendAddRes.Errormsg = accept ? "对方同意了你的好友请求" : "对方拒绝了你的好友请求";
            message.Response.friendAddRes.Request= request;
            NetClient.Instance.SendMessage(message);
        }

        internal void SendFriendRemoveRequest(int id, int friendId)
        {
            Debug.Log("Send Friend Remove Request");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendRemove = new FriendRemoveRequest();
            message.Request.friendRemove.Id = id;
            message.Request.friendRemove.friendId= friendId;
            NetClient.Instance.SendMessage(message);
        }


        private void OnFriendRemove(object sender, FriendRemoveResponse message)
        {
            if(message.Result == Result.Success)
            {
                MessageBox.Show("删除成功", "删除好友");
            }
            else
            {
                MessageBox.Show("删除失败", "删除好友",MessageBoxType.Error);
            }
        }


        private void OnFriendList(object sender, FriendListResponse message)
        {
            Debug.Log("On Friend List Change");
            FriendManager.Instance.allFriends = message.Friends;
            // UI订阅了好友列表通知，就可以发送给UI了
            if (this.OnFriendUpdate != null)
            {
                this.OnFriendUpdate();
            }
        }













    }
}
