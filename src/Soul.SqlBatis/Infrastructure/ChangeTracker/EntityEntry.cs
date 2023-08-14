﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityEntry : IEntityType
    {
        public object Entity { get; }
     
        internal IEntityType EntityType { get; }
       
        public EntityEntry(object entity, IEntityType entityType, IReadOnlyCollection<PropertyEntry> properties)
        {
            Entity = entity;
            EntityType = entityType;
            Properties = properties;
        }

        private EntityState _state = EntityState.Unchanged;

        public virtual EntityState State
        {
            get
            {
                if (_state == EntityState.Unchanged && Properties.Any(a => a.IsModified))
                {
                    _state = EntityState.Modified;
                    return EntityState.Modified;
                }
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public virtual IReadOnlyCollection<PropertyEntry> Properties { get; }

        public Type Type => EntityType.Type;

        public string Schema => EntityType.Schema;

        public string TableName => EntityType.TableName;

        public IReadOnlyCollection<object> Metadata => EntityType.Metadata;

        IReadOnlyCollection<IEntityProperty> IEntityType.Properties => EntityType.Properties;

        public IEntityProperty GetProperty(MemberInfo member)
        {
            return EntityType.GetProperty(member);
        }
    }

    public class EntityEntry<T> : EntityEntry
    {
        private readonly EntityEntry _entry;

        public new T Entity => (T)_entry.Entity;

        public override EntityState State { get => _entry.State; set => _entry.State = value; }

        public override IReadOnlyCollection<PropertyEntry> Properties => _entry.Properties;

        internal EntityEntry(EntityEntry entry)
            : base(entry.Entity, entry.EntityType, entry.Properties)
        {
            _entry = entry;
        }
    }
}
