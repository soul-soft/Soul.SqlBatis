using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Soul.SqlBatis.Infrastructure
{
    public interface IModel
    {
        IEntityType GetEntityType(Type type);
    }

    public class Model : IModel
    {
        private readonly ConcurrentDictionary<Type, IEntityType> _entities = new ConcurrentDictionary<Type, IEntityType>();

        public Model(IEnumerable<IEntityType> entities)
        {
            foreach (var item in entities)
            {
                _entities.TryAdd(item.Type, item);
            }
        }

        public virtual IEntityType GetEntityType(Type type)
        {
            return _entities.GetOrAdd(type, key =>
            {
                return new EntityType(key);
            });
        }
    }
}
