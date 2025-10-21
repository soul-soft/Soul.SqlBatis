using System.Reflection;

namespace Soul.SqlBatis.Metadata
{

    public interface IProperty : IAnnotatable
    {
        string Name { get; }
        PropertyInfo PropertyInfo { get; }
        IEntityType EntityType { get; }
        ValueGenerated ValueGenerated { get; }
    }


    public static class IPropertyExtensions
    {
        public static string GetColumnName(this IProperty property)
        {
            var column = property.FindAnnotation(RelationalAnnotationNames.ColumnName);
            return column ?? property.Name;
        }
    }
}
