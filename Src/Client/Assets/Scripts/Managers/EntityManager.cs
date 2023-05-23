using Entities;
using SkillBridge.Message;
using System.Collections.Generic;

namespace Assets.Scripts.Managers
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
        void OnEntityChanged(Entity entity);
        void OnEntityEvent(EntityEvent @event);
    }

    internal class EntityManager : Singleton<EntityManager>
    {
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        // 使用一个接口
        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        public void RegisterEntityNotify(int entityId, IEntityNotify notify)
        {
            this.notifiers[entityId] = notify;
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }

        public void RemoveEntity(NEntity nEntity)
        {
            this.entities.Remove(nEntity.Id);
            if (notifiers.ContainsKey(nEntity.Id))
            {
                // 调用接口的方法
                // 由于EntityController继承了接口，必须注册这个方法
                // 可以使用这个方式来进行通知
                notifiers[nEntity.Id].OnEntityRemoved();
                notifiers.Remove(nEntity.Id);
            }

        }

        internal void OnEntitySync(NEntitySync sync)
        {
            Entity entity = null;
            entities.TryGetValue(sync.Id, out entity);
            if (entity != null)
            {
                if (sync.Entity != null)
                {
                    entity.EntityData = sync.Entity;
                }
                if (notifiers.ContainsKey(sync.Id))
                {
                    notifiers[entity.entityId].OnEntityChanged(entity);
                    notifiers[entity.entityId].OnEntityEvent(sync.Event);
                }
            }
        }

        public Entity GetEntity(int entityId)
        {
            Entity entity = null;
            entities.TryGetValue(entityId, out entity); 
            return entity;
        }
    }
}
