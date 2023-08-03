namespace Soul.SqlBatis.Test
{
   
    public static class SqlFunctions
    {
        [DbFunction(Name = "NOW")]
        public static DateTime Now()
        {
            throw new NotImplementedException();
        }
    }
}
