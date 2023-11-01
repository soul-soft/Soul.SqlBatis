using System.Collections.Generic;
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
}
