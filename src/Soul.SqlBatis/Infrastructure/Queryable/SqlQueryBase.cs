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
        public abstract string AnySql { get; }
        public abstract string AvgSql { get; }
        public abstract string MaxSql { get; }
        public abstract string MinSql { get; }
        public abstract string SumSql { get; }
        public abstract string CountSql { get; }
        public abstract string ViewAlias { get; }

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
