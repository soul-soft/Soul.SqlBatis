﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    public class Model
    {
        private readonly List<EntityType> _entities;
      
        public Model(IEnumerable<EntityType> entities)
        {
            _entities = entities.ToList();
        }
        
        public IReadOnlyCollection<EntityType> Entities => _entities;

        public EntityType GetEntityType(Type type)
        {
            return _entities.Where(a => a.Type == type).FirstOrDefault();
        }

        public bool IsEntity(Type type)
        {
            return _entities.Any(a => a.Type == type);
        }
    }
}
