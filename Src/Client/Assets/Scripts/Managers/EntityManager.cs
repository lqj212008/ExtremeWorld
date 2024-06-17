using Entities;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
	interface IEntityNotify
	{
		void OnEntityRemoved();
		void OnEntityChanged(Entity entity);
		void OnEntityEvent(EntityEvent @event);
	}
	class EntityManager : Singleton<EntityManager> 
	{
		Dictionary<int,Entity> entities = new Dictionary<int,Entity>();

		Dictionary<int,IEntityNotify> notifiers = new Dictionary<int,IEntityNotify>();

		public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
		{
			this.notifiers[entityId] = notify;
		}

		public void AddEntity(Entity entity)
		{
			entities[entity.entityId] = entity;
		}
		public void RemoveEntity(NEntity entity)
		{
			this.entities.Remove(entity.Id);
			if(notifiers.ContainsKey(entity.Id))
			{
				notifiers[entity.Id].OnEntityRemoved();
				notifiers.Remove(entity.Id);
			}
		}

        internal void OnEntitySyncs(NEntitySync entitySync)
        {
            Entity entity = null;
			entities.TryGetValue(entitySync.Id, out entity);
			if(entity != null)
			{
				if(entitySync.Entity != null)
				{
					entity.EntityData = entitySync.Entity;
				}
				if (notifiers.ContainsKey(entitySync.Id))
				{
					notifiers[entity.entityId].OnEntityChanged(entity);
					notifiers[entity.entityId].OnEntityEvent(entitySync.Event);
				}
			}
        }
    }
}

