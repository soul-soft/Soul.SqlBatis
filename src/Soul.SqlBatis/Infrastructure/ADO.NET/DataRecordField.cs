using System;

namespace Soul.SqlBatis.Infrastructure
{
    internal class DataRecordField
    {
        public Type Type { get; }

        public string Name { get; }

        public int Ordinal { get; }

        public DataRecordField(Type type, string name, int ordinal)
        {
            Type = type;
            Name = name;
            Ordinal = ordinal;
        }

        public string Code
        {
            get
            {
                if (typeof(byte[]) == Type)
                {
                    return "ByteArray";
                }
                if (typeof(char[]) == Type)
                {
                    return "CharArray";
                }
                return ((int)Type.GetTypeCode(Type)).ToString();
            }
        }
    }
}
