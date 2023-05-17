using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Creature : Entity
    {
        public int Id { get; set; }
        // 此 ID 为数据库ID
        public string Name { get { return this.Info.Name; }}
        public NCharacterInfo Info;
        public CharacterDefine Define;

        public Creature(Vector3Int pos, Vector3Int dir):base(pos,dir)
        {

        }
        public Creature(CharacterType type, int configId, int level, Vector3Int pos, Vector3Int dir) :
           base(pos, dir)
        {
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Level = level;
            // 配置表ID
            this.Info.ConfigId = configId;
            this.Info.Entity = this.EntityData;
            // 实体ID
            this.Info.EntityId = this.entityId;
            this.Define = DataManager.Instance.Characters[this.Info.ConfigId];
            this.Info.Name = this.Define.Name;
        }
    }
}
