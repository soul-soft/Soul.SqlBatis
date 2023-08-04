using System;

namespace Soul.SqlBatis
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewAttribute : Attribute
    {
        public string Name { get; }
        public string Scheme { get; }

        public ViewAttribute(string name, string scheme = null)
        {
            Name = name;
            Scheme = scheme;
        }
    }
}
