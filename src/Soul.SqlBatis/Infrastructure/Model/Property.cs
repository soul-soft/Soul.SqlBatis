using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public class Property
    {
        public MemberInfo Member { get; }

        public string ColumnName => Member.Name;

        public Property(MemberInfo member)
        {
            Member = member;
        }
    }
}
