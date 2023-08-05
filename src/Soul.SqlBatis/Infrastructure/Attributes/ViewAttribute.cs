using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ViewAttribute : TableAttribute
	{
		public ViewAttribute(string name) : base(name)
		{

		}
	}
}
