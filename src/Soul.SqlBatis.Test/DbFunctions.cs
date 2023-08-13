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
        public static int Count<T>(T column)
        {
            throw new NotImplementedException();
        }

        [DbFunction(Name = "AVG")]
        public static decimal Avg<T>(T column)
        {
            throw new NotImplementedException();
        }
    }
}
