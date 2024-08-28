using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.ChangeTracking
{
    public interface IEntityEntry : IEntityType
    {
        bool IsChanged { get; }

        EntityState State { get; set; }

        object Entity { get; }

        IReadOnlyList<IMemberEntry> Members { get; }
    }

    public interface IMemberEntry : IEntityProperty
    {
        bool IsChanged { get; }

        object OriginalValue { get; }

        object CurrentValue { get; }

        void SetValue(object value);
    }

    public class EntityEntry : IEntityEntry
    {
        private readonly object _entity;
        private readonly IEntityType _entityType;
        private readonly bool _ignoreNullMembers;
        internal EntityState StateSnapshot = EntityState.Detached;
        private readonly List<MemberEntry> _members;

        internal EntityEntry(object entity, bool ignoreNullMembers, IEntityType entityType, IEnumerable<MemberEntry> members)
        {
            _entity = entity;
            _entityType = entityType;
            _members = members.ToList();
            _ignoreNullMembers = ignoreNullMembers;
        }

        public bool IsChanged => _members.Any(p => p.IsChanged);

        public EntityState State
        {
            get
            {
                if (StateSnapshot == EntityState.Unchanged && IsChanged)
                {
                    StateSnapshot = EntityState.Modified;
                }
                else if (StateSnapshot == EntityState.Modified && !IsChanged)
                {
                    StateSnapshot = EntityState.Unchanged;
                }
                return StateSnapshot;
            }
            set
            {
                if ((value == EntityState.Deleted || value == EntityState.Modified) && Members.Where(a => a.IsKey()).All(a => a.CurrentValue == null))
                {
                    throw new InvalidOperationException("Primary key values cannot be null when the entity is being deleted or modified.");
                }

                if (value == EntityState.Modified && StateSnapshot == EntityState.Detached)
                {
                    ResetMemberValue(); // 重置默认值
                }

                StateSnapshot = value;
            }
        }

        private void ResetMemberValue()
        {
            foreach (var item in _members)
            {
                if (item.IsKey())
                {
                    continue;
                }
                if (!_ignoreNullMembers)
                {
                    item.OriginalValue = DBNull.Value;
                }
                else
                {
                    item.OriginalValue = null;
                }
            }

        }

        public IEntityProperty GetProperty(string memberName)
        {
            return _entityType.GetProperty(memberName);
        }

        public IReadOnlyList<IEntityProperty> GetProperties()
        {
            return _entityType.GetProperties();
        }

        public object Entity => _entity;

        public IReadOnlyList<IMemberEntry> Members => _members;

        public Type DeclaringType => _entityType.DeclaringType;

        public string TableName => _entityType.TableName;

        internal static bool ValueEquals(object currentValue, object originalValue)
        {
            // 如果引用相同，立即返回 true
            if (ReferenceEquals(currentValue, originalValue))
            {
                return true;
            }

            // 如果任一对象为 null，立即返回 false
            if (currentValue == null || originalValue == null)
            {
                return false;
            }

            // 使用 Equals 方法进行快速类型相等性比较
            if (currentValue.Equals(originalValue))
            {
                return true;
            }

            // 特殊处理字符串
            if (currentValue is string currentString && originalValue is string originalString)
            {
                return string.Equals(currentString, originalString, StringComparison.Ordinal);
            }

            // 特殊处理数值类型
            if (IsNumericType(currentValue) && IsNumericType(originalValue))
            {
                return Convert.ToDouble(currentValue) == Convert.ToDouble(originalValue);
            }

            // 特殊处理集合类型
            if (currentValue is IEnumerable currentEnumerable && originalValue is IEnumerable originalEnumerable)
            {
                return EnumerablesEqual(currentEnumerable, originalEnumerable);
            }

            // 默认使用 Equals 方法
            return currentValue.Equals(originalValue);
        }

        private static bool IsNumericType(object obj)
        {
            return obj is byte || obj is sbyte ||
                   obj is short || obj is ushort ||
                   obj is int || obj is uint ||
                   obj is long || obj is ulong ||
                   obj is float || obj is double ||
                   obj is decimal;
        }

        private static bool EnumerablesEqual(IEnumerable first, IEnumerable second)
        {
            var firstEnumerator = first.GetEnumerator();
            var secondEnumerator = second.GetEnumerator();

            while (firstEnumerator.MoveNext())
            {
                if (!(secondEnumerator.MoveNext() && ValueEquals(firstEnumerator.Current, secondEnumerator.Current)))
                {
                    return false;
                }
            }

            return !secondEnumerator.MoveNext();
        }
    }

    public class EntityEntry<T> : IEntityEntry
    {
        private readonly IEntityEntry _entityEntry;

        internal EntityEntry(EntityEntry entityEntry)
        {
            _entityEntry = entityEntry;
        }

        public bool IsChanged => _entityEntry.IsChanged;

        public EntityState State { get => _entityEntry.State; set => _entityEntry.State = value; }

        public object Entity => _entityEntry.Entity;

        public IReadOnlyList<IMemberEntry> Members => _entityEntry.Members;

        public Type DeclaringType => _entityEntry.DeclaringType;

        public string TableName => _entityEntry.TableName;

        public IReadOnlyList<IEntityProperty> GetProperties()
        {
            return _entityEntry.GetProperties();
        }

        public IEntityProperty GetProperty(string memberName)
        {
            return _entityEntry.GetProperty(memberName);
        }
    }

    public class MemberEntry : IMemberEntry
    {
        private readonly IEntityProperty _property;

        private readonly object _entity;

        public object OriginalValue { get; internal set; }


        internal MemberEntry(object entity, object originalValue, IEntityProperty property)
        {
            _entity = entity;
            _property = property;
            OriginalValue = originalValue;
        }

        public object CurrentValue
        {
            get
            {
                return Property.GetValue(_entity);
            }
        }

        public PropertyInfo Property => _property.Property;

        public string ColumnName => _property.ColumnName;

        public bool IsChanged => !EntityEntry.ValueEquals(CurrentValue, OriginalValue);

        public bool IsIdentity()
        {
            return _property.IsIdentity();
        }

        public bool IsNotMapped()
        {
            return _property.IsNotMapped();
        }

        public bool IsKey()
        {
            return _property.IsKey();
        }

        public void SetValue(object value)
        {
            var propertyType = Nullable.GetUnderlyingType(Property.PropertyType) ?? Property.PropertyType;

            if (value != null && value.GetType() != propertyType)
            {
                value = Convert.ChangeType(value, propertyType);
            }
            Property.SetValue(_entity, value);
        }
    }

    internal class EntityEntrySnapshot : IEntityEntry
    {
        private readonly IEntityEntry _entityEntry;

        public bool IsChanged { get; }

        public EntityState State { get; set; }

        public object Entity => _entityEntry.Entity;

        public IReadOnlyList<IMemberEntry> Members { get; }

        public Type DeclaringType => _entityEntry.DeclaringType;

        public string TableName => _entityEntry.TableName;

        public EntityEntrySnapshot(IEntityEntry entityEntry, bool isChanged, EntityState state, IReadOnlyList<MemberEntrySnapshot> members)
        {
            _entityEntry = entityEntry;
            IsChanged = isChanged;
            State = state;
            Members = members;
        }

        public IReadOnlyList<IEntityProperty> GetProperties()
        {
            return _entityEntry.GetProperties();
        }

        public IEntityProperty GetProperty(string memberName)
        {
            return _entityEntry.GetProperty(memberName);
        }
    }

    internal class MemberEntrySnapshot : IMemberEntry
    {
        private readonly IMemberEntry _memberEntry;

        public bool IsChanged { get; }

        public object OriginalValue { get; }

        public object CurrentValue { get; }

        public PropertyInfo Property => _memberEntry.Property;

        public string ColumnName => _memberEntry.ColumnName;

        public MemberEntrySnapshot(IMemberEntry memberEntry, bool isChanged, object originalValue, object currentValue)
        {
            _memberEntry = memberEntry;
            IsChanged = isChanged;
            OriginalValue = originalValue;
            CurrentValue = currentValue;
        }

        public bool IsIdentity()
        {
            return _memberEntry.IsIdentity();
        }

        public bool IsKey()
        {
            return _memberEntry.IsKey();
        }

        public bool IsNotMapped()
        {
            return _memberEntry.IsNotMapped();
        }

        public void SetValue(object value)
        {
            _memberEntry.SetValue(value);
        }
    }
}
