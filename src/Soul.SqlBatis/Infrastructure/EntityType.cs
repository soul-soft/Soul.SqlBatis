using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityType
    {
        private List<Property> _properties;

        public IReadOnlyCollection<Property> Properties => _properties;

        public EntityType(List<Property> properties)
        {
            _properties = properties;
        }

        public Property GetProperty(MemberInfo member)
        {
            return _properties.Where(a => a.Member == member).FirstOrDefault();
        }
    }
}
