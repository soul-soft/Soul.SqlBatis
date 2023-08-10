using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityProperty
    {
        public MemberInfo Member { get; }

        private AttributeCollection _attributes;

        public IAttributeCollection Attributes => _attributes;

		public bool IsKey
        {
            get
            {
                return Attributes.Any(a => a is KeyAttribute);
            }
        }

		public bool IsIdentity
		{
			get
			{
				return Attributes.Any(a => a is IdentityAttribute);
			}
		}

		public bool IsNotMapped
        {
            get
            {
                return Attributes.Any(a => a is NotMappedAttribute);
            }
        }

        

        public string ColumnName
        {
            get
            {
                var columnName = Attributes.Get<ColumnAttribute>()?.Name;
                if (!string.IsNullOrEmpty(columnName))
                {
                    return columnName;
                }
                return Member.Name;
            }
        }

        public EntityProperty(MemberInfo member)
        {
            Member = member;
            _attributes = new AttributeCollection(member.GetCustomAttributes());
            if (string.Equals(Member.Name, "Id", System.StringComparison.OrdinalIgnoreCase))
            {
                HasAnnotation(new KeyAttribute());
                HasAnnotation(new IdentityAttribute());
            }
        }

		internal void HasAnnotation(object value)
        {
			_attributes.Set(value);
        }

        internal void RemoveAnnotation<T>()
        {
            _attributes.Remove(typeof(T));
        }
    }
}
