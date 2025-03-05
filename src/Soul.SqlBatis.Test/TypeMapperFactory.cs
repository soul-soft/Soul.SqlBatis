using System.Data;
using System.Reflection;
using System.Text.Json;

namespace Soul.SqlBatis.Test
{
    internal class TypeMapperFactory : ICustomTypeMapper
    {
        public static T GetJson<T>(IDataRecord record, int i)
        {
            return JsonSerializer.Deserialize<T>(record.GetString(i),new JsonSerializerOptions 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            })
            ?? throw new NullReferenceException();
        }

        public MethodInfo GetTypeMapper(EntityTypeMapperContext context)
        {
            if ("json".Equals(context.FieldTypeName, StringComparison.OrdinalIgnoreCase) && context.MemberType != typeof(string))
            {
                return GetType().GetMethod(nameof(GetJson))!.MakeGenericMethod(context.MemberType);
            }
            return null!;
        }
    }
}
