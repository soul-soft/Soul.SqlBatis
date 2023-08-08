using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityType
    {
        public Type Type { get; }

        private AttributeCollection _attributes;

        public IAttributeCollection Attributes => _attributes;


		public IReadOnlyCollection<EntityProperty> Properties { get; }

        public string Schema
        {
            get
            {
                var name = Attributes.Get<TableAttribute>()?.Schema;
                return name ?? string.Empty;
            }
        }

        public virtual string TableName
        {
            get
            {
                var name = Attributes.Get<TableAttribute>()?.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }
                return Type.Name;
            }

        }

        public EntityType(Type type)
        {
            Type = type;
            _attributes = new AttributeCollection(type.GetCustomAttributes());
            Properties = type.GetProperties()
                .Select(s => new EntityProperty(s))
                .ToList();
        }

        public virtual EntityProperty GetProperty(MemberInfo member)
        {
            return Properties.Where(a => a.Member == member).FirstOrDefault();
        }

        public void HasAnnotation(object annotation)
        {
			_attributes.Set(annotation);
        }
    }
}
