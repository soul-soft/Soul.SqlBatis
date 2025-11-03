using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Metadata
{
    public class Key: IKey
    {
        public Key(IReadOnlyList<IProperty> properties)
        {
            Properties = properties;
        }

        public IReadOnlyList<IProperty> Properties { get; }

        public IProperty GetIdentity()
        {
            return Properties.Where(a => a.ValueGenerated == ValueGenerated.OnAdd).FirstOrDefault();
        }
    }
}
