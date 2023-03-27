using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class MonsterManager
    {
        private Map map;

        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();
        internal void Init(Map map)
        {
            this.map = map;
        }

        internal Monster Create(int spawnMonId, int spawnLevel, NVector3 position, NVector3 direction)
        {
            Monster monster = new Monster(spawnMonId, spawnLevel, position, direction);
            EntityManager.Instance.AddEntity(this.map.ID, monster);
            // 怪物没有DBid 所以使用了entity id
            monster.Id = monster.entityId;
            monster.Info.EntityId = monster.entityId;
            monster.Info.mapId = this.map.ID;
            Monsters[monster.Id] = monster;

            this.map.MonsterEnter(monster);
            return monster;
        }
    }
}
