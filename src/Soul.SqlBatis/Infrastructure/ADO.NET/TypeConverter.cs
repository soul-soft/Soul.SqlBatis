using System;
using System.Reflection;

namespace Soul.SqlBatis
{
	internal class TypeConverter
	{
		public object Target { get; set; }

		public Type MemberType { get; set; }
		
		public Type ColumnType { get; set; }

		public MethodInfo Method { get; set; }
	}
}
