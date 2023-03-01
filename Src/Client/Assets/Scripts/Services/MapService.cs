using System;
using Network;
using UnityEngine;

using Common.Data;
using SkillBridge.Message;
using Models;
using Managers;
// using UnityEditor.VersionControl;
using Assets.Scripts.Managers;
using Entities;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {

        public int CurrentMapId = 0;
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);

            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync); 

        }



        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);

            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        public void Init()
        {

        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("MapService: OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                // 确认地图内角色列表中，玩家所属的角色
                // 首次进入地图没有EntityID，所以为空也应当判断
                if (User.Instance.CurrentCharacter == null || User.Instance.CurrentCharacter.Id == cha.Id)
                {
                    User.Instance.CurrentCharacter = cha;
                }
                // 对于所有角色，都需要交给角色管理器
                CharacterManager.Instance.AddCharacter(cha);
            }
            //当前角色切换地图
            if (CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("MapService: OnMapCharacterLeave: charID:{0}", response.characterId);
            // 销毁离开的玩家，如果离开游戏的是自身，销毁所有玩家
            if (response.characterId != User.Instance.CurrentCharacter.Id)
            {
                CharacterManager.Instance.RemoveCharacter(response.characterId);
            }
            else
            {
                CharacterManager.Instance.Clear();
            }
        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
            }
            else
                Debug.LogErrorFormat("EnterMap: Map {0} not existed", mapId);
        }

        // 地图同步请求
        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity)
        {
            // Debug.LogFormat("MapService-SendMapEntitySync: ID{0}, Position{1}, DIR:{2}, Speed:{3}", entity.Id, entity.Position.String(), entity.Direction.String(), entity.Speed);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();

            NEntitySync nEntitySync = new NEntitySync()
            {
                Id = entity.Id,
                Event = entityEvent,
                Entity = entity
            };

            message.Request.mapEntitySync.entitySync = nEntitySync;
            Debug.LogFormat("MapService-SendMapEntitySync: nEntitySync ID:{0}", nEntitySync.Id);
            NetClient.Instance.SendMessage(message);
        }

        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("MapEntityUpdateResponse : Entitiys {0}", response.entitySyncs.Count);
            sb.AppendLine();

            foreach (var entity in response.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);
                sb.AppendFormat("[{0} event: {1} entity :{2} ]", entity.Id, entity.Event, entity.Entity.String());
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
        }

        internal void SendMapTeleport(int TeleporterId)
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = TeleporterId;
            NetClient.Instance.SendMessage(message);

            Debug.LogFormat("Mapservice - SendMapTeleport 发送出传送请求");
        }
    }
}
