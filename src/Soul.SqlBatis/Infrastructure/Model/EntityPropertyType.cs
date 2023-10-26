using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public interface IEntityPropertyType
    {
        bool IsKey { get; }
        bool IsIdentity { get; }
        bool IsNotMapped { get; }
        bool IsConcurrencyToken { get; }
        PropertyInfo Property { get; }
        string ColumnName { get; }
        string CSharpName { get; }
        IReadOnlyCollection<object> Metadata { get; }
    }

    internal class EntityPropertyType : IEntityPropertyType
    {
        public PropertyInfo Property { get; }

        private AttributeCollection _attributes;

        public IReadOnlyCollection<object> Metadata => _attributes.Metadata;

        public bool IsKey
        {
            get
            {
                return _attributes.Any(a => a is KeyAttribute);
            }
        }

        public bool IsIdentity
        {
            get
            {
                return _attributes.Any(a => a is IdentityAttribute);
            }
        }

        public bool IsNotMapped
        {
            get
            {
                return _attributes.Any(a => a is NotMappedAttribute);
            }
        }

        public bool IsConcurrencyToken
        {
            get
            {
                return _attributes.Any(a => a is ConcurrencyCheckAttribute);
            }
        }

        public string ColumnName
        {
            get
            {
                var columnName = _attributes.Get<ColumnAttribute>()?.Name;
                if (!string.IsNullOrEmpty(columnName))
                {
                    return columnName;
                }
                return Property.Name;
            }
        }

        public string CSharpName => Property.Name;

        public EntityPropertyType(MemberInfo member)
        {
            Property = member as PropertyInfo;
            _attributes = new AttributeCollection(member.GetCustomAttributes());
            if (string.Equals(Property.Name, "Id", System.StringComparison.OrdinalIgnoreCase))
            {
                SetAnnotation(new KeyAttribute());
                if (!_attributes.Any<ValueGeneratedNeverAttribute>())
                {
                    SetAnnotation(new IdentityAttribute());
                }
            }
        }

        public void SetAnnotation(object value)
        {
            _attributes.Set(value);
        }

        public void RemoveAnnotation<T>()
        {
            _attributes.Remove(typeof(T));
        }
    }
}
