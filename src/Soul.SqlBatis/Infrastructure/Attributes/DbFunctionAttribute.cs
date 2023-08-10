using System;

namespace Soul.SqlBatis
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DbFunctionAttribute : Attribute
    {
        public string Name { get; set; }
        public string Scheme { get; set; }
    }
}
