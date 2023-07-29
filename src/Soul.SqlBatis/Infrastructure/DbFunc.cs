using System;
using System.Collections.Generic;

namespace Soul.SqlBatis
{
	public static class DbFunc
	{
		public static bool Like(string column, string value)
		{
			throw new NotImplementedException();
		}

		public static bool In<T1, T2>(T1 column, IEnumerable<T2> values)
		{
			throw new NotImplementedException();
		}

		public static bool IsNull<T>(T column)
		{
			throw new NotImplementedException();
		}

		public static bool IsNotNull<T>(T column)
		{
			throw new NotImplementedException();
		}

        public static T Raw<T>(DbSql syntax)
        {
            throw new NotImplementedException();
        }
    }
}
