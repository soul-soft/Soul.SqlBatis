﻿using System.Linq.Expressions;

namespace Soul.SqlBatis.Test
{

    public static class DbFunctions
    {
        [DbFunction(Name = "NOW")]
        public static DateTime Now()
        {
            throw new NotImplementedException();
        }

        [DbFunction(Name = "COUNT")]
        public static int Count(SqlToken token)
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
    }
}