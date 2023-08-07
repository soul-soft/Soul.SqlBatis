using System.Collections.Generic;
using System.Linq;

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

        public string Select()
        {
            var tokens = BuildFilter();
            var columns = GetColumns();
            if (columns == "*")
            {
                columns = string.Join(", ", _entityType.Properties.Select(s =>
                {
                    if (s.ColumnName == s.Member.Name)
                    {
                        return s.Member.Name;
                    }
                    return $"{s.ColumnName} AS {s.Member.Name}";
                }));
            }
            var view = GetFromSql();
            return string.Join(" ", $"SELECT {columns} FROM {view}", tokens);
        
        }

        public string Count()
        {
            var tokens = BuildFilter();
            var columns = GetColumns();
            var fromSql = GetFromSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Select();
                return $"SELECT COUNT({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT COUNT({columns}) FROM {fromSql}", tokens);
        }

        public string Sum()
        {
            var tokens = BuildFilter();
            var fromSql = GetFromSql();
            var columns = GetColumns();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Select();
                return $"SELECT SUM({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT SUM({columns}) FROM {fromSql}", tokens);
        }

        public string Max()
        {
            var tokens = BuildFilter();
            var fromSql = GetFromSql();
            var columns = GetColumns();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Select();
                return $"SELECT MAX({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT MAX({columns}) FROM {fromSql}", tokens);
        }

        public string Min()
        {
            var tokens = BuildFilter();
            var columns = GetColumns();
            var fromSql = GetFromSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Select();
                return $"SELECT MIN({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT MIN({columns}) FROM {fromSql}", tokens);
        }

        public string Avg()
        {
            var tokens = BuildFilter();
            var columns = GetColumns();
            var fromSql = GetFromSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Select();
                return $"SELECT AVG({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT AVG({columns}) FROM {fromSql}", tokens);
        }

        private string GetFromSql()
        {
            if (_tokens.ContainsKey(DbExpressionType.From))
            {
                return _tokens[DbExpressionType.From].First();
            }
            return _entityType.TableName;
        }

        private string GetColumns()
        {
            if (_tokens.ContainsKey(DbExpressionType.Select))
            {
                var columns = _tokens[DbExpressionType.Select].First();
                return columns;
            }
            return "*";
        }

        private string BuildFilter()
        {
            var filters = _tokens.OrderBy(s => s.Key).Select(item =>
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
                return sql;
            });
            return string.Join(" ", filters);
        }
    }
}
