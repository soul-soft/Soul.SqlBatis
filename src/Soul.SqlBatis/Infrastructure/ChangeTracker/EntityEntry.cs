using System;
using System.Collections.Generic;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityEntry : IEntityType
    {
        public object Entity { get; }

        internal IEntityType EntityType { get; }

        public EntityEntry(object entity, IEntityType entityType, IReadOnlyCollection<EntityPropertyEntry> properties)
        {
            Entity = entity;
            EntityType = entityType;
            Properties = properties;
        }


        public virtual EntityState State { get; internal set; }

        public virtual IReadOnlyCollection<EntityPropertyEntry> Properties { get; }

        public Type Type => EntityType.Type;

        public string Schema => EntityType.Schema;

        public string TableName => EntityType.TableName;

        public IReadOnlyCollection<object> Metadata => EntityType.Metadata;

        IReadOnlyCollection<IEntityPropertyType> IEntityType.Properties => EntityType.Properties;

        public IEntityPropertyType GetProperty(MemberInfo member)
        {
            return EntityType.GetProperty(member);
        }

        public void HasAnnotation(object annotation)
        {
            EntityType.HasAnnotation(annotation);
        }
    }

    public class EntityEntry<T> : EntityEntry
    {
        private readonly EntityEntry _entry;

        public new T Entity => (T)_entry.Entity;

        public override EntityState State { get => _entry.State; internal set => _entry.State = value; }

        public override IReadOnlyCollection<EntityPropertyEntry> Properties => _entry.Properties;

        internal EntityEntry(EntityEntry entry)
            : base(entry.Entity, entry.EntityType, entry.Properties)
        {
            _entry = entry;
        }
    }
}
