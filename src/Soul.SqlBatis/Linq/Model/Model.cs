using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    public class Model
    {
        private readonly List<EntityType> _entities;
      
        public Model(IEnumerable<EntityType> entities)
        {
            _entities.AddRange(entities);
        }
        
        public IReadOnlyCollection<EntityType> Entities => _entities;

        public EntityType GetEntity(Type type)
        {
            return _entities.Where(a => a.Type == type).FirstOrDefault();
        }

        public bool IsEntity(Type type)
        {
            return _entities.Any(a => a.Type == type);
        }
    }
}
