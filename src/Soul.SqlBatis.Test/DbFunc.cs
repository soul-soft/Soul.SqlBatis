namespace Soul.SqlBatis.Test
{
    [DbFunction]
    public static class DbFunc
    {
        [DbFunction(Format = "*")]
        public static int Count()
        {
            throw new NotImplementedException();
        }

        [DbFunction(Name = "COUNT", Format = "DISTINCT {c}")]
        public static int DistictCount<T>(T c)
        {
            throw new NotImplementedException();
        }

        public static int Count<T>(T column)
        {
            throw new NotImplementedException();
        }

        public static T IF<T>(bool column, T value1, T value2)
        {
            throw new NotImplementedException();
        }
    }
}
