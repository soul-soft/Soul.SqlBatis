using System.Collections.Generic;

namespace Soul.SqlBatis.Metadata
{
    public interface IKey
    {
        IReadOnlyList<IProperty> Properties { get; }
    }
}
