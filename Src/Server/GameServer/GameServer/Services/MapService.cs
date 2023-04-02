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

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport) ;
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
            if (character != null)
            {
                MapManager.Instance[character.Info.mapId].UpdateEntity(message.entitySync);
            }
            //Log.InfoFormat("同步收到 sender 信息 : character: ID : {0}, User ID: {1}",sender.Session.Character.Id ,sender.Session.User.ID);
            //Log.InfoFormat("同步收到 request信息 : character: ID : {0} EntityID: {1}", character.Id, message.entitySync.Id);
        }

        public void SendEntityUpdate(NetConnection<NetSession> conn, NEntitySync entity)
        {
            conn.Session.Response.mapEntitySync = new MapEntitySyncResponse() ;
            conn.Session.Response.mapEntitySync.entitySyncs.Add(entity);
            conn.SendResponse();

            // Log.InfoFormat("同步发出 Session信息 :  CharacterID : {0}, UserID: {1}", conn.Session.Character.Id, conn.Session.User.ID);
            // Log.InfoFormat("同步发出 Entity信息: Entity: ID : {0} - {1}", entity.Id, entity.Entity.Id);
        }


        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("MapService-OnMapTelepor - Character ID :{0}:{1} - TeleporterID: {2}",character.Id,character.Data, request.teleporterId);

            if (!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))
            {
                Log.WarningFormat("Source Teleporter ID : {0} Does not exist", request.teleporterId);
                return;
            }
            TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId];
            if(source.LinkTo == 0 || !DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))
            {
                Log.WarningFormat("Target Teleporter ID : {0} Does not exist", source.LinkTo);
            }

            TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo];

            MapManager.Instance[source.MapID].CharacterLeave(character);
            character.Position= target.Position;
            character.Direction= target.Direction;
            MapManager.Instance[target.MapID].CharacterEnter(sender, character);
        }

    }
}
