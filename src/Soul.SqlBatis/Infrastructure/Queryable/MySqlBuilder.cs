using System.Collections.Generic;
using System.Linq;
using static Soul.SqlBatis.SqlBuilder;

namespace Soul.SqlBatis.Infrastructure
{
    public class MySqlBuilder
    {
        private readonly EntityType _entityType;
        private readonly Dictionary<DbExpressionType, IEnumerable<string>> _tokens = new Dictionary<DbExpressionType, IEnumerable<string>>();

        public MySqlBuilder(EntityType entityType, Dictionary<DbExpressionType, IEnumerable<string>> tokens)
        {
            _entityType = entityType;
            _tokens = tokens;
        }

        public string BuildSelect()
        {
            var tokens = BuildTokens();
            return string.Join(" ", $"SELECT {GetSelectSql()} FROM {GetFromSql()}", tokens);
        }

        public string GetFromSql()
        {
            if (_tokens.ContainsKey(DbExpressionType.From))
            {
                return _tokens[DbExpressionType.From].First();
            }
            else
            {
                return _entityType.TableName;
            }

        }

        public string GetSelectSql()
        {
            if (_tokens.ContainsKey(DbExpressionType.Select))
            {
                var columns = _tokens[DbExpressionType.Select].First();
                return columns;
            }
            else
            {
                var columns = _entityType.Properties.Select(s =>
                {
                    if (s.ColumnName == s.Member.Name)
                    {
                        return s.Member.Name;
                    }
                    return $"{s.ColumnName} AS {s.Member.Name}";
                });
                return string.Join(", ", columns);
            }
        }

        private IEnumerable<string> BuildTokens()
        {
            foreach (var item in _tokens.OrderBy(s => s.Key))
            {
                var connector = ", ";
                if (item.Key == DbExpressionType.Where || item.Key == DbExpressionType.GroupBy)
                    connector = " AND ";
                var expressions = string.Join(connector, item.Value);
                var sql = string.Empty;
                switch (item.Key)
                {
                    case DbExpressionType.Where:
                        sql = $"WHERE {expressions}";
                        break;
                    case DbExpressionType.Join:
                        sql = $"{expressions}";
                        break;
                    case DbExpressionType.GroupBy:
                        sql = $"Group By {expressions}";
                        break;
                    case DbExpressionType.Having:
                        sql = $"HAVING {expressions}";
                        break;
                    case DbExpressionType.OrderBy:
                        sql = $"ORDER BY {expressions}";
                        break;
                }
                yield return sql;
            }
        }
    }
}
