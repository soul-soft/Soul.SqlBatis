using System;

namespace Soul.SqlBatis.Exceptions
{
	public class ModelException : Exception
	{
		public ModelException(string message)
			: base(message)
		{

		}
	}
}
