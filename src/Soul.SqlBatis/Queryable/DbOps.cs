using System;
using System.Collections.Generic;

namespace Soul.SqlBatis
{
    public static class DbOps
    {
        public static bool Contains(string column, string value)
        {
            throw new NotImplementedException();
        }

        public static bool StartWith(string column, string value)
        {
            throw new NotImplementedException();
        }

        public static bool EndWith(string column, string value)
        {
            throw new NotImplementedException();
        }

        public static bool In<TColumn>(TColumn column, params TColumn[] values)
        {
            throw new NotImplementedException();
        }

        public static bool In<TColumn>(TColumn column, List<TColumn> values)
        {
            throw new NotImplementedException();
        }

        public static bool In<TColumn>(TColumn column, IEnumerable<TColumn> values)
        {
            throw new NotImplementedException();
        }

        public static bool InSet<TColumn>(TColumn column, string values)
        {
            throw new NotImplementedException();
        }

        public static bool InSub<T>(T column, string subQuery)
        {
            throw new NotImplementedException();
        }
    }
}
