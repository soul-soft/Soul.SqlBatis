using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    internal class EntityType : IEntityType
    {
        public Type Type { get; }

        private AttributeCollection _attributes;

        public IReadOnlyCollection<IEntityPropertyType> Properties { get; }

        public EntityType(Type type)
        {
            Type = type;
            _attributes = new AttributeCollection(type.GetCustomAttributes());
            Properties = type.GetProperties()
                .Select(s => new EntityPropertyType(s))
                .ToList();
        }

        public IReadOnlyCollection<object> Metadata => _attributes.Metadata;

        public string Schema
        {
            get
            {
                var name = _attributes.Get<TableAttribute>()?.Schema;
                return name ?? string.Empty;
            }
        }

        public virtual string TableName
        {
            get
            {
                var name = _attributes.Get<TableAttribute>()?.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }
                return Type.Name;
            }

        }

        public virtual IEntityPropertyType GetProperty(MemberInfo member)
        {
            return Properties.Where(a => a.Property.Name == member.Name).FirstOrDefault();
        }

        public void HasAnnotation(object annotation)
        {
            _attributes.Set(annotation);
        }
    }
}
