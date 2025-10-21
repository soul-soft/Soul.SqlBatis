using System;
using System.Collections.Generic;

namespace Soul.SqlBatis.Metadata
{

    public interface IEntityType: IAnnotatable
    {
        string Name { get; }
        IModel Model { get; }
        Type TypeInfo { get; }
        IKey PrimaryKey { get; }
        IProperty FindProperty(string name);
        IEnumerable<IProperty> GetProperties();
    }

    public static class IEntityTypeExtensions
    {
        public static string GetTableName(this IEntityType entityType)
        {
            return entityType.FindAnnotation(RelationalAnnotationNames.TableName) ?? entityType.Name;
        }
    }
}