using System.Collections.Generic;

namespace Soul.SqlBatis.Metadata
{
    public class Key: IKey
    {
        public Key(IReadOnlyList<IProperty> properties)
        {
            Properties = properties;
        }

        public IReadOnlyList<IProperty> Properties { get; }
    }
}
