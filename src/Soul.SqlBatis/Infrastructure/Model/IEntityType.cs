using System;
using System.Collections.Generic;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public interface IEntityType
    {
        Type Type { get; }
        string Schema { get; }
        string TableName { get; }
        IReadOnlyCollection<object> Metadata { get; }
        IReadOnlyCollection<IEntityPropertyType> Properties { get; }
        IEntityPropertyType GetProperty(MemberInfo member);
        void HasAnnotation(object annotation);
    }
}
