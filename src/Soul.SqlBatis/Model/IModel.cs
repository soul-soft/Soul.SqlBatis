using System;

namespace Soul.SqlBatis.Linq
{
    public interface IModel
    {
        bool IsEntity(Type type);
        IEntity GetEntity(Type type);
    }
}
