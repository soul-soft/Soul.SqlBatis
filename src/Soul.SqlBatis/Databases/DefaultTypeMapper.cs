using System;
using System.Data;

namespace Soul.SqlBatis.Databases
{
    public static class DefaultTypeMapper
    {
        public static T EnumValueMapper<T>(IDataRecord record, int index)
        {
            var value = record.GetInt32(index);
            if (value is T result)
            {
                return result;
            }
            return (T)Enum.Parse(typeof(T), value.ToString());
        }
    }
}
