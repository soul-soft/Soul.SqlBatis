using System;

namespace Soul.SqlBatis.Infrastructure
{
	public class ModelException : Exception
	{
		public ModelException(string message)
			: base(message)
		{

		}
	}
}
