using System;

namespace Soul.SqlBatis.Metadata
{
    public interface IModel
    {
        string Format(string name);
        IEntityType FindEntityType(Type type);
    }
}
