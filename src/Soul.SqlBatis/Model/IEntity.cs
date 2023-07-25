using System;

namespace Soul.SqlBatis.Linq
{
    public interface IEntity
    {
        string GetColumnName(string propertyName);
    }
}
