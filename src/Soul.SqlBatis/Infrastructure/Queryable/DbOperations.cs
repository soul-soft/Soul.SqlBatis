﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Soul.SqlBatis
{
    public static class DbOperations
    {
        public static bool Between<T1, T2, T3>(T1 column, T2 start, T3 end)
        {
            throw new NotImplementedException();
        }

        public static bool Contains(string column, string value)
        {
            throw new NotImplementedException();
        }

        public static bool StartsWith(string column, string value)
        {
            throw new NotImplementedException();
        }

        public static bool EndsWith(string column, string value)
        {
            throw new NotImplementedException();
        }

        public static bool Like(string column, string value)
        {
            throw new NotImplementedException();
        }

        public static bool In<T1, T2>(T1 column, IEnumerable<T2> values)
        {
            throw new NotImplementedException();
        }

        public static bool In<T1, T2>(T1 column, params T2[] values)
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

        public static T Raw<T>(string sql)
        {
            throw new NotImplementedException();
        }

        public static T IF<T>(object test, T value1, T value2)
        {
            throw new NotImplementedException();
        }

        public static DbSwitchCase Switch(object when, object then)
        {
            throw new NotImplementedException();
        }
    }
}
