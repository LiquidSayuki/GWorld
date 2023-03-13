using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;

using Common;
using Common.Data;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using GameServer.Services;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        // MainBody Satart

        public int ID
        {
            get { return this.Define.ID; }
        }
        internal MapDefine Define;

        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        SpawnManager SpawnManager = new SpawnManager();
        public MonsterManager MonsterManager = new MonsterManager();

        internal Map(MapDefine define)
        {
            this.Define = define;
            this.SpawnManager.Init(this);
            this.MonsterManager.Init(this);
        }

        internal void Update()
        {
            SpawnManager.Update();
        }

        #region 角色进入、离开地图
        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            // 向新进入玩家通知所有已经在地图内的其他玩家和怪物
            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;

            // 添加所有玩家进入回复
            foreach(var kv in this.MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                // 向其他玩家广播
                if (kv.Value.character != character)
                {
                    this.AddCharacterEnterMap(kv.Value.connection, character.Info);
                }
            }
            // 添加所有怪物进入回复
            foreach (var kv in this.MonsterManager.Monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }
            conn.SendResponse();
        }

        //Enter
        private void AddCharacterEnterMap(NetConnection<NetSession> connection, NCharacterInfo info)
        {
            // 相比旧的地图进入，多进行一次验证，减少response被强制new一下，导致消息清空等可能问题
           if (connection.Session.Response.mapCharacterEnter == null)
            {
                connection.Session.Response.mapCharacterEnter= new MapCharacterEnterResponse();
                connection.Session.Response.mapCharacterEnter.mapId= this.Define.ID;
            }

            connection.Session.Response.mapCharacterEnter.Characters.Add(info);
            connection.SendResponse();
        }

/*        
         void SendCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character);

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }
*/

        // Leave
        internal void CharacterLeave(Character character)
        {
            Log.InfoFormat("Map.CharacterLeave{0}", character.Id);

            // 需要向每一个玩家广播玩家离开的消息
            foreach(var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection, character);
            }
            this.MapCharacters.Remove(character.Id);
        }

        void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            conn.Session.Response.mapCharacterLeave= new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.characterId = character.Id;
            conn.SendResponse();
        }

        #endregion


        public void UpdateEntity(NEntitySync sync)
        {
            foreach(var kv in this.MapCharacters)
            {
                if (kv.Value.character.entityId == sync.Id)
                {
                    kv.Value.character.Position = sync.Entity.Position;
                    kv.Value.character.Direction = sync.Entity.Direction;
                    kv.Value.character.Speed = sync.Entity.Speed;
                }
                else
                {
                    MapService.Instance.SendEntityUpdate(kv.Value.connection, sync);
                }
            }
        }

        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter: Map{0}, monsterId{1}", this.Define.ID, monster.Id);
            // 向所有玩家发送怪物进入信息
            foreach(var kv in this.MapCharacters)
            {
                this.AddCharacterEnterMap(kv.Value.connection, monster.Info);
            }
        }
    }
}
