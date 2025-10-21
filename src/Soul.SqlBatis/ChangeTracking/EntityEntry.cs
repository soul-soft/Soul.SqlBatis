using Soul.SqlBatis.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.ChangeTracking
{
    public class EntityEntry : IEntityEntry
    {
        private EntityState _state;
        private readonly object _entity;
        private readonly Dictionary<string, object> _originalValues;
        private readonly List<PropertyEntry> _properties = new List<PropertyEntry>();

        internal EntityEntry(object entity, IEntityType metadata)
        {
            _entity = entity;
            Metadata = metadata;
            _originalValues = CreateOriginalValues(entity, metadata);
        }

        public object Entity => _entity;

        public EntityState State
        {
            get
            {
                DetectChanges();
                return _state;
            }
            set
            {
                SetEntityState(value);
            }
        }

        private void DetectChanges()
        {
            if (IsPersisted() && _state == EntityState.Unchanged)
            {
                foreach (var item in Properties)
                {
                    if (item.IsModified)
                    {
                        _state = EntityState.Modified;
                        return;
                    }
                }
            }
        }


        public IEntityType Metadata { get; private set; }


        public IReadOnlyList<PropertyEntry> Properties => _properties;


        public object GetOriginalValue(IProperty property)
        {
            if (_originalValues.TryGetValue(property.Name, out object value))
            {
                return value;
            }
            return DBNull.Value;
        }

        public object GetCurrentValue(IProperty property)
        {
            return property.PropertyInfo.GetValue(_entity);
        }

        public void SetCurrentValue(IProperty property, object value)
        {
            property.PropertyInfo.SetValue(_entity, Convert.ChangeType(value, property.PropertyInfo.PropertyType));
        }

        internal void AddProperty(IProperty property)
        {
            _properties.Add(new PropertyEntry(this, property));
        }

        private void SetEntityState(EntityState state)
        {
            _state = state;
            SetPropertyModifiedFlags(state);
        }

        private void SetPropertyModifiedFlags(EntityState state)
        {
            if (state == EntityState.Unchanged)
            {
                foreach (var item in Metadata.GetProperties())
                {
                    if (Metadata.PrimaryKey != null && Metadata.PrimaryKey.Properties.Contains(item))
                    {
                        continue;
                    }
                    var currentValue = GetCurrentValue(item);
                    _originalValues[item.Name] = currentValue;
                }
            }
            else if (state == EntityState.Modified)
            {
                foreach (var item in Metadata.GetProperties())
                {
                    if (Metadata.PrimaryKey != null && Metadata.PrimaryKey.Properties.Contains(item))
                    {
                        continue;
                    }
                    _originalValues[item.Name] = DBNull.Value;
                }
            }
        }

        private static Dictionary<string, object> CreateOriginalValues(object entity, IEntityType entityType)
        {
            var mapper = ObjectMapper.GetOrCreateMapper(entityType.TypeInfo);
            var values = mapper(entity);
            return values;
        }

        public bool IsPersisted()
        {
            var primaryKey = Metadata.PrimaryKey;
            if (primaryKey != null)
            {
                foreach (var item in primaryKey.Properties)
                {
                    var value = GetOriginalValue(item);

                    if (value == null)
                    {
                        return false;
                    }
                    else if (value is int i32 && i32 <= 0)
                    {
                        return false;
                    }
                    else if (value is long i64 && i64 <= 0)
                    {
                        return false;
                    }
                    else if (value is string str && str == string.Empty)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
