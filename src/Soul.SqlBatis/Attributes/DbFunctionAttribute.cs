using System;

namespace Soul.SqlBatis
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DbFunctionAttribute : Attribute
    {
        public string Name { get; set; }
        public string Scheme { get; set; }
        public string Format { get; set; }
    }
}
