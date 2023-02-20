using Common;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class EntityManager: Singleton<EntityManager>
    {
        private int idx = 0;
        // 建立起地图与角色群的映射关系
        public List<Entity> AllEnitities = new List<Entity>();
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        public void AddEntity(int mapId, Entity entity)
        {
            AllEnitities.Add(entity);
            entity.EntityData.Id = ++this.idx;

            List<Entity> entities = null;
            if (!MapEntities.TryGetValue(mapId, out entities))
            {
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
            }
            // ?
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId ,Entity entity)
        {
            this.AllEnitities.Remove(entity);
            this.MapEntities[mapId].Remove(entity);
        }
    }
}
