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

        [DbFunction(Name = "COUNT", Format = "DISTINCT {column}")]
        public static int CountDistinct(object column)
        {
            throw new NotImplementedException();
        }

        [DbFunction(Name = "COUNT", Format = "*")]
        public static int CountX()
        {
            throw new NotImplementedException();
        }

        [DbFunction(Name = "COUNT")]
        public static int Count(object column)
        {
            throw new NotImplementedException();
        }

        [DbFunction(Name = "DISTINCT")]
        public static T Distinct<T>(T column)
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
