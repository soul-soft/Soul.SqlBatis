using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
    public class SqlBuilder
    {
        private readonly SqlBuilderOptions _options;

        private readonly Dictionary<string, List<string>> _tokens = new Dictionary<string, List<string>>();

        public SqlBuilder(SqlBuilderOptions options = null)
        {
            _options = options ?? new SqlBuilderOptions();
        }

        public void AddToken(string type, string token)
        {
            if (!_tokens.ContainsKey(type))
            {
                _tokens[type] = new List<string>();
            }
            _tokens[type].Add(token);
        }

        public SqlBuilder Clone()
        {
            var sb = new SqlBuilder();
            foreach (var item in _tokens)
            {
                foreach (var token in item.Value)
                {
                    sb.AddToken(item.Key, token);
                }
            }
            return sb;
        }

        public SqlBuilder From(string view)
        {
            AddToken(nameof(From), view);
            return this;
        }

        public SqlBuilder Set(string column)
        {
            AddToken(nameof(Set), column);
            return this;
        }

        public SqlBuilder Take(int count)
        {
            AddToken(nameof(Take), count.ToString());
            return this;
        }

        public SqlBuilder Skip(int offset)
        {
            AddToken(nameof(Skip), offset.ToString());
            return this;
        }

        public SqlBuilder Page(int index, int size)
        {
            Skip((index - 1) * size);
            Take(size);
            return this;
        }

        public SqlBuilder Select(string sql, bool flag = true)
        {
            if (flag)
                AddToken(nameof(Select), sql);
            return this;
        }

        public SqlBuilder Where(string sql, bool flag = true)
        {
            if (flag)
                AddToken(nameof(Where), sql);
            return this;
        }

        public SqlBuilder OrderBy(string sql, bool flag = true)
        {
            if (flag)
                AddToken(nameof(OrderBy), sql);
            return this;
        }

        public SqlBuilder GroupBy(string sql, bool flag = true)
        {
            if (flag)
                AddToken(nameof(GroupBy), sql);
            return this;
        }

        public SqlBuilder Having(string sql, bool flag = true)
        {
            if (flag)
                AddToken(nameof(Having), sql);
            return this;
        }

        public string WhereSql
        {
            get
            {
                if (!_tokens.ContainsKey(nameof(Where)))
                {
                    return string.Empty;
                }
                var expression = _tokens[nameof(Where)].Aggregate((x, y) => $"{x} AND {y}");
                return $"WHERE\r\n\t{expression}";
            }
        }

        public string GroupSql
        {
            get
            {
                if (!_tokens.ContainsKey(nameof(GroupBy)))
                {
                    return string.Empty;
                }
                var expression = _tokens[nameof(GroupBy)].Aggregate((x, y) => $"{x}, {y}");
                return $"GROUP BY\r\n\t{expression}";
            }
        }

        public string HavingSql
        {
            get
            {
                if (!_tokens.ContainsKey(nameof(Having)))
                {
                    return string.Empty;
                }
                var expression = _tokens[nameof(Having)].Aggregate((x, y) => $"{x}, {y}");
                return $"HAVING\r\n\t{expression}";
            }
        }

        public string OrderSql
        {
            get
            {
                if (!_tokens.ContainsKey(nameof(OrderBy)))
                {
                    return string.Empty;
                }
                var expression = _tokens[nameof(OrderBy)].Aggregate((x, y) => $"{x}, {y}");
                return $"ORDER BY\r\n\t{expression}";
            }
        }

        public string SelectSql
        {
            get
            {
                if (!_tokens.ContainsKey(nameof(Select)))
                {
                    return "*";
                }
                var columns = string.Join(",\r\n\t", _tokens[nameof(Select)]);
                return $"{columns}";
            }
        }

        public string View
        {
            get 
            {
                var from = "/**VIEW**/";
                if (_tokens.ContainsKey(nameof(From)))
                {
                    from = _tokens[nameof(From)].Last();
                }
                return from;
            }
        }

        public string FromSql
        {
            get
            {
                return $"FROM\r\n\t{View}";
            }
        }

        public string LimitSql
        {
            get
            {
                var offset = 0;
                var count = int.MaxValue;
                if (!_tokens.ContainsKey(nameof(Skip)) && !_tokens.ContainsKey(nameof(Take)))
                {
                    return string.Empty;
                }
                if (_tokens.ContainsKey(nameof(Skip)))
                {
                    offset = Convert.ToInt32(_tokens[nameof(Skip)].Last());
                }
                if (_tokens.ContainsKey(nameof(Take)))
                {
                    count = Convert.ToInt32(_tokens[nameof(Take)].Last());
                }
                return string.Format(_options.LimitFormat, offset, count);
            }
        }

        public string QuerySql
        {
            get
            {
                var selectSql = $"SELECT\r\n\t{SelectSql}";
                var items = new List<string>()
                {
                    selectSql, FromSql, WhereSql, GroupSql, HavingSql, OrderSql, LimitSql
                }
                .Where(a => !string.IsNullOrEmpty(a));
                return string.Join("\r\n", items);
            }
        }

        public string CountSql
        {
            get
            {
                var selectSql = $"SELECT\r\n\tCOUNT({SelectSql})";
                var items = new List<string>()
                {
                    selectSql, FromSql, WhereSql, GroupSql, HavingSql
                }
               .Where(a => !string.IsNullOrEmpty(a));
                return string.Join("\r\n", items);
            }
        }

        public string SumSql
        {
            get
            {
                var selectSql = $"SELECT\r\n\tSUM({SelectSql})";
                var items = new List<string>()
                {
                    selectSql, FromSql, WhereSql, GroupSql, HavingSql
                }
               .Where(a => !string.IsNullOrEmpty(a));
                return string.Join("\r\n", items);
            }
        }

        public string MaxSql
        {
            get
            {
                var selectSql = $"SELECT\r\n\tMAX({SelectSql})";
                var items = new List<string>()
                {
                    selectSql, FromSql, WhereSql, GroupSql, HavingSql
                }
               .Where(a => !string.IsNullOrEmpty(a));
                return string.Join("\r\n", items);
            }
        }

        public string MinSql
        {
            get
            {
                var selectSql = $"SELECT\r\n\tMIX({SelectSql})";
                var items = new List<string>()
                {
                    selectSql, FromSql, WhereSql, GroupSql, HavingSql
                }
               .Where(a => !string.IsNullOrEmpty(a));
                return string.Join("\r\n", items);
            }
        }

        public string AvgSql
        {
            get
            {
                var selectSql = $"SELECT\r\n\tAVG({SelectSql})";
                var items = new List<string>()
                {
                    selectSql, FromSql, WhereSql, GroupSql, HavingSql
                }
               .Where(a => !string.IsNullOrEmpty(a));
                return string.Join("\r\n", items);
            }
        }

        public string AnySql
        {
            get
            {
                return $"SELECT EXISTS (\r\n{QuerySql}\r\n)";
            }
        }

        public string UpdateSql
        {
            get 
            {
                var setters = _tokens[nameof(Set)].ToList();
                if (setters.Count > 0)
                {
                    setters[0] = $"SET {setters[0]}";
                }
                var setterSql = string.Join(",\r\n", setters);
                var items = new string[] 
                {
                    $"UPDATE {View}", setterSql, WhereSql
                };
                return string.Join("\r\n", items);
            }
        }

        public string DeleteSql
        {
            get 
            {
                var items = new string[] 
                {
                    $"DELETE FROM {View}" , WhereSql
                };
                return string.Join("\r\n", items);
            }
        }
    }
}
