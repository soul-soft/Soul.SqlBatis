using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityType
    {
        private readonly Type _type;

        public Type Type => _type;

        public virtual IReadOnlyCollection<EntityTypeProperty> Properties
        {
            get
            {
                return _type.GetProperties()
                  .Where(a => !a.CustomAttributes.Any(s => s.AttributeType == typeof(NotMappedAttribute)))
                  .Select(s => new EntityTypeProperty(s))
                  .ToList();
            }
        }

        private string _scheme;

        public string Scheme
        {
            get
            {
                if (!string.IsNullOrEmpty(_scheme))
                    return _scheme;
                var tableAttribute = _type.GetCustomAttribute<TableAttribute>();
                if (tableAttribute != null)
                {
                    return tableAttribute.Schema ?? string.Empty;
                }
                return string.Empty;
            }
            internal set
            {
                _scheme = value;
            }
        }

        private string _tableName;

        public virtual string TableName
        {
            get
            {
                if (!string.IsNullOrEmpty(_tableName))
                    return _tableName;
                var tableAttribute = _type.GetCustomAttribute<TableAttribute>();
                if (tableAttribute != null)
                {
                    return tableAttribute.Name;
                }
                return _type.Name;
            }
            internal set
            {
                _tableName = value;
            }
        }

        public EntityType(Type type)
        {
            _type = type;
        }

        public virtual EntityTypeProperty GetProperty(MemberInfo member)
        {
            return Properties.Where(a => a.Member == member).FirstOrDefault();
        }


    }
}
