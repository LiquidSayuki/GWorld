using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        public MapService()
        {
           MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapCharacterEnterRequest>(this.OnMapCharacterEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);

        }





        public void Init()
        {
            MapManager.Instance.Init();
        }

        private void OnMapCharacterEnter(NetConnection<NetSession> sender, MapCharacterEnterRequest message)
        {
            return;
        }

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest message)
        {
            Character character = sender.Session.Character;
            
            MapManager.Instance[character.Info.mapId].UpdateEntity(message.entitySync);

            Log.InfoFormat("同步收到 sender 信息 : character: ID : {0}, User ID: {1}",sender.Session.Character.Id ,sender.Session.User.ID);
            Log.InfoFormat("同步收到 request信息 : character: ID : {0} EntityID: {1}", character.Id, message.entitySync.Id);
        }

        public void SendEntityUpdate(NetConnection<NetSession> conn, NEntitySync entity)
        {
            
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapEntitySync = new MapEntitySyncResponse();
            message.Response.mapEntitySync.entitySyncs.Add(entity);

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);

            Log.InfoFormat("同步发出 Session信息 :  CharacterID : {0}, UserID: {1}", conn.Session.Character.Id, conn.Session.User.ID);
            Log.InfoFormat("同步发出 Entity信息: Entity: ID : {0} - {1}", entity.Id, entity.Entity.Id);
        }

    }
}
