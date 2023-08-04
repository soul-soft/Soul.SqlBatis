using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityTypeProperty
    {
        public MemberInfo Member { get; }

        private string _columnName;

        public string ColumnName
        {
            get
            {
                return _columnName;
            }
            set
            {
                _columnName = value;
            }
        }

        public EntityTypeProperty(MemberInfo member)
        {
            Member = member;
        }
    }
}
