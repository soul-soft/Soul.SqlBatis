using System;

namespace Soul.SqlBatis.Infrastructure
{
	public class DbExpressionException : Exception
	{
		public DbExpressionException(string message)
			: base(message)
		{

		}
	}
}
