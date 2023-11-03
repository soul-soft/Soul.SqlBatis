namespace Soul.SqlBatis.Infrastructure
{
    public interface ISqlQuery
    {
        DynamicParameters Parameters { get;  }
        string QuerySql { get; }
        string UpdateSql { get; }
        string DeleteSql { get; }
        string AnyQuerySql { get; }
        string AvgQuerySql { get; }
        string MaxQuerySql { get; }
        string MinQuerySql { get; }
        string SumQuerySql { get; }
        string CountQuerySql { get; }
    }
}
