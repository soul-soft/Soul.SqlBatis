using System;

namespace Soul.SqlBatis.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DatabaseGeneratedAttribute : Attribute
    {
        public DatabaseGeneratedOption Option { get; }

        public DatabaseGeneratedAttribute(DatabaseGeneratedOption option)
        {
            Option = option;
        }
    }
}
