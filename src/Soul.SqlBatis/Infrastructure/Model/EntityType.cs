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

        private List<Property> _properties;

        public Type Type => _type;

        public IReadOnlyCollection<Property> Properties => _properties;

        public string TableName => _type.Name;

		public object ColumnNames { get; internal set; }

		public EntityType(Type type)
        {
            _type = type;
            _properties = GetProperties();
        }

        public Property GetProperty(MemberInfo member)
        {
            return _properties.Where(a => a.Member == member).FirstOrDefault();
        }

        private List<Property> GetProperties()
        {
            return _type.GetProperties()
                .Where(a => !a.CustomAttributes.Any(s => s.AttributeType == typeof(NotMappedAttribute)))
                .Select(s => new Property(s))
                .ToList();
        }
    }
}
