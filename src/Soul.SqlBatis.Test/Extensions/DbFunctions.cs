using System.Linq.Expressions;

namespace Soul.SqlBatis.Test
{

    public static class DbFunctions
    {
        [DbFunction(Name = "NOW")]
        public static DateTime Now()
        {
            throw new NotImplementedException();
        }

        [DbFunction(Name = "COUNT", Format = "DIDCOUNT {token}")]
        public static int Count(RawSql token)
        {
            throw new NotImplementedException();
        }

        [DbFunction(Name = "AVG")]
        public static decimal Avg<T>(T column)
        {
            throw new NotImplementedException();
        }


        [DbFunction(Name = "JSON_EXTRACT")]
        public static TPath JsonExtract<TPath>(object column, string path)
        {
            throw new NotImplementedException();
        }

        [DbFunction(Name = "DATE")]
        internal static DateTime Date(DateTime? creationTime)
        {
            throw new NotImplementedException();
        }
    }
}
