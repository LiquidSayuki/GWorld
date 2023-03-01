using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    class NetSession : INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }

        internal void Disconnected()
        {
            if (this.Character != null)
            {
                UserService.Instance.CharacterLeave(this.Character);
            }
        }

        NetMessage response;
        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = new NetMessage();
                }
                if (response.Response == null)
                {
                    response.Response = new NetMessageResponse();
                }
                return response.Response;
            }
        }

        public byte[] GetResponse()
        {
            if (response != null)
            {
                if (this.Character != null && this.Character.StatusManager.HasStatus)
                {
                    // 从状态管理器得到所有待发送的状态
                    this.Character.StatusManager.ApplyResponse(Response);
                }
                //在发送消息时调用
                byte[] data = PackageHandler.PackMessage(response);
                // 打包发送过response后，清空旧response
                response = null;
                return data;
            }
            return null;
        }
    }
}
