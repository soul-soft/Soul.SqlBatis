using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    public class Model
    {
        private readonly List<EntityType> _entities;

        internal Model()
        {

        }

        public Model(IEnumerable<EntityType> entities)
        {
            _entities = entities.ToList();
        }

        public virtual EntityType GetEntityType(Type type)
        {
            return _entities.Where(a => a.Type == type).FirstOrDefault();
        }

        public virtual bool IsEntity(Type type)
        {
            return _entities.Any(a => a.Type == type);
        }
    }
}
