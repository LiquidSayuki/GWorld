using Common;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class FriendManager
    {
        Character Owner;
        List<NFriendInfo> friends = new List<NFriendInfo>();
        bool friendChanged = false;

        public FriendManager(Character owner)
        {
            this.Owner = owner;
            this.InitFriends();
        }

        private void InitFriends()
        {
            this.friends.Clear();
            // 数据库中的CharacterFriend表，每一个都是TCharacterFriend
            foreach (var i in this.Owner.Data.Friends)
            {
                this.friends.Add(GetFriendInfo(i));
            }
        }

        /// <summary>
        /// 获取好友的在线状态
        /// </summary>
        /// <param name="friend"></param>
        /// <returns></returns>
        private NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo friendInfo = new NFriendInfo();
            var character = CharacterManager.Instance.GetCharacter(friend.FriendID);
            // 好友的玩家数据 TCharacter 转化NCharacter
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;
            
            if(character == null)
            {//不在线
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;
            }
            else
            {//在线
                friendInfo.friendInfo = character.GetBasicInfo();
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;

                if(friend.Level != character.Info.Level)
                {
                    friend.Level = character.Info.Level;
                }

                character.FriendManager.UpdateFriendInfo(this.Owner.Info, 1);
                friendInfo.Status = 1;
            }
           // Log.InfoFormat("Get Friend Info: Player:[{0}{1}]", this.Owner.Id, this.Owner.Info.Name);
            return friendInfo;
        }

        /// <summary>
        /// 通过好友的角色id，得到好友的Info
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public NFriendInfo GetFriendInfo(int friendId)
        {
            foreach (var f in this.friends)
            {
                if( f.friendInfo.Id == friendId)
                {
                    return f;
                }
            }
            return null;
        }

        internal void GetFriendInfos(List<NFriendInfo> list)
        {
            foreach (var f in this.friends)
            {
                list.Add(f);
            }
        }

        // 添加/移除好友
        internal void AddFriend(Character friend)
        {
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendID = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level,
            };
            this.Owner.Data.Friends.Add(tf);
            friendChanged = true;
        }
        internal bool RemoveFriendByID(int id)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.Id == id);
            if (removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }
        internal bool RemoveFriendByFriendID(int friendId)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.FriendID == friendId);
            if (removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        /// <summary>
        /// 更新好友在线状态
        /// </summary>
        /// <param name="friendInfo"></param>
        /// <param name="status"></param>
        internal void UpdateFriendInfo(NCharacterInfo friendInfo, int status)
        {
            foreach (var i in this.friends)
            {
                if(i.friendInfo.Id == friendInfo.Id)
                {
                    i.Status= status;
                    break;
                }
            }
            this.friendChanged = true;
        }
        /// <summary>
        /// 显式的向他人通知自己的在线离线状态
        /// </summary>
        public void OfflineNotify()
        {
            foreach(var friendInfo in this.friends)
            {
                //寻找自己所有好友，如果好友在线，更新对方的好友管理器的，自己的在线信息
                var friend = CharacterManager.Instance.GetCharacter(friendInfo.friendInfo.Id);
                if(friend != null)
                {
                    friend.FriendManager.UpdateFriendInfo(this.Owner.Info, 0);
                }
            }
        }

        /// <summary>
        /// 将最新的好友信息添加到当前Session的Response中，等待发送
        /// </summary>
        /// <param name="message">当前用户的Session的Response</param>
        internal void PostProcess(NetMessageResponse message)
        {
            if (this.friendChanged)
            {
                this.InitFriends();
                if(message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.friends);
                }
                friendChanged = false;
            }
        }
    }
}
