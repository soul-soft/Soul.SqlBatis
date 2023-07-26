using System;
using System.Collections.Generic;
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

        public EntityType(Type type)
        {
            _type = type;
        }

        public Property GetProperty(MemberInfo member)
        {
            return _properties.Where(a => a.Member == member).FirstOrDefault();
        }
    }
}
