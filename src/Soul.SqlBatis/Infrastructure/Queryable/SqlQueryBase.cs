using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    public abstract class SqlQueryBase
    {
        public abstract DynamicParameters Parameters { get; }
        public abstract string QuerySql { get; }
        public abstract string UpdateSql { get; }
        public abstract string DeleteSql { get; }
        public abstract string AnyQuerySql { get; }
        public abstract string AvgQuerySql { get; }
        public abstract string MaxQuerySql { get; }
        public abstract string MinQuerySql { get; }
        public abstract string SumQuerySql { get; }
        public abstract string CountQuerySql { get; }

        protected string StringJoin(string separator, params string[] values)
        {
            return string.Join(separator, values.Where(a => !string.IsNullOrEmpty(a)));
        }

        protected string StringJoin(string separator, IEnumerable<string> values)
        {
            return string.Join(separator, values.Where(a => !string.IsNullOrEmpty(a)));
        }
    }
}
