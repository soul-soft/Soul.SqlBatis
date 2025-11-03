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
        private readonly DbContext _context;
        private readonly IPropertyAccessor _propertyAccessor;
        private readonly Dictionary<string, object> _originalValues;
        private readonly List<PropertyEntry> _properties = new List<PropertyEntry>();

        internal EntityEntry(DbContext context, object entity, IEntityType metadata)
        {
            _entity = entity;
            _context = context;
            Metadata = metadata;
            _propertyAccessor = EmitProxyGenerator.CreateProxy(entity);
            _originalValues = CreateOriginalValues(entity, metadata);
            foreach (var item in metadata.GetProperties())
            {
                AddProperty(item);
            }
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
            if (_state == EntityState.Unchanged)
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

        internal void AddProperty(IProperty property)
        {
            _properties.Add(new PropertyEntry(this, property));
        }


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
            return _propertyAccessor.GetPropertyValue(property.Name);
        }

        public void SetCurrentValue(IProperty property, object value)
        {
            var propertyValue = Convert.ChangeType(value, property.PropertyInfo.PropertyType);
            _propertyAccessor.SetPropertyValue(property.Name, propertyValue);
        }

        public void SetOriginalValue(IProperty property, object value)
        {
            _originalValues[property.Name] = value;
        }

        private void SetEntityState(EntityState state)
        {
            _state = state;
            SetPropertyModifiedFlags(state);
        }

        private void SetPropertyModifiedFlags(EntityState state)
        {
            switch (state)
            {
                case EntityState.Detached:
                    _context.ChangeTracker.UnTrack(Entity);
                    break;
                case EntityState.Unchanged:
                    foreach (var item in Metadata.GetProperties())
                    {
                        if (Metadata.PrimaryKey != null && Metadata.PrimaryKey.Properties.Contains(item))
                        {
                            continue;
                        }
                        var currentValue = GetCurrentValue(item);
                        _originalValues[item.Name] = currentValue;
                    }
                    _context.ChangeTracker.Track(this);
                    break;
                case EntityState.Deleted:
                    if (!IsPersisted())
                    {
                        throw new NotSupportedException("The entity must be persisted before it can be updated.");
                    }
                    _context.ChangeTracker.Track(this);
                    break;
                case EntityState.Modified:
                    if (!IsPersisted())
                    {
                        throw new NotSupportedException("The entity must be persisted before it can be updated.");
                    }
                    if (!_context.ChangeTracker.HasEntry(Entity))
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
                    _context.ChangeTracker.Track(this);
                    break;
                case EntityState.Added:
                    var property = Metadata.PrimaryKey.GetIdentity();
                    if (property != null)
                    {
                        var value = GetIdentityValue();
                        SetIdentityValue(property, value);
                    }
                    _context.ChangeTracker.Track(this);
                    break;
                default:
                    break;
            }
        }

        private int GetIdentityValue()
        {
            var total = _context.ChangeTracker.Entities()
                .Where(f => f._state == EntityState.Added)
                .Where(a => a.Metadata.TypeInfo == Metadata.TypeInfo)
                .Count();
            return total + 1;
        }

        private void SetIdentityValue(IProperty property, int lastId)
        {
            _context.ChangeTracker.UnTrack(Entity);
            SetCurrentValue(property, lastId);
            SetOriginalValue(property, lastId);
            _context.ChangeTracker.Track(this);
        }

        internal void SetIdentityValue(int lastId)
        {
            var property = Metadata.PrimaryKey.GetIdentity();
            if (property != null)
            { 
                _context.ChangeTracker.UnTrack(Entity);
                SetCurrentValue(property, lastId);
                SetOriginalValue(property, lastId);
                _context.ChangeTracker.Track(this);
            }
        }

        private Dictionary<string, object> CreateOriginalValues(object entity, IEntityType entityType)
        {
            var values = new Dictionary<string, object>();
            foreach (var item in entityType.GetProperties())
            {
                var value = _propertyAccessor.GetPropertyValue(item.Name);
                values.Add(item.Name, value);
            }
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
