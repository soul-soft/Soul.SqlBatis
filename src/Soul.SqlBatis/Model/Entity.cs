using System;

namespace Soul.SqlBatis.Linq
{
    public class Entity
    {
        public Type EntityType { get; }

        public string GetColumnName(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
