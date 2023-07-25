using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Linq
{
    public interface IModel
    {
        bool IsEntity(Type type);
        Entity GetEntity(Type type);
    }

    public class Model : IModel
    {
        private readonly List<Entity> _entities;

        public IReadOnlyCollection<Entity> Entities => _entities;

        public Model(IEnumerable<Entity> entities)
        {
            _entities = entities.ToList();
        }

        public Entity GetEntity(Type entityType)
        {
            return _entities.Where(a => a.EntityType == entityType).FirstOrDefault();
        }

        public bool IsEntity(Type entityType)
        {
            return _entities.Any(a => a.EntityType == entityType);
        }
    }
}
