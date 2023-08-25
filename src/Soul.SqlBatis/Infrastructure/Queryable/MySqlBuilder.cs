using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    internal class MySqlBuilder
    {
        private readonly IEntityType _entityType;

        private readonly Dictionary<DbExpressionType, IEnumerable<string>> _tokens = new Dictionary<DbExpressionType, IEnumerable<string>>();

        public MySqlBuilder(IEntityType entityType, Dictionary<DbExpressionType, IEnumerable<string>> tokens)
        {
            _entityType = entityType;
            _tokens = tokens;
        }

        public string Update()
        {
            var filter = BuildFilterSql();
            var view = BuildFromSql();
            return string.Join(" ", $"UPDATE {view}", filter);
        }
     
        public string Delete()
        {
            var filter = BuildFilterSql();
            var view = BuildFromSql();
            return string.Join(" ", $"DELETE FROM {view}", filter);
        }

        public string Select()
        {
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
            if (columns == "*")
            {
                columns = string.Join(", ", _entityType.Properties.Select(s =>
                {
                    if (s.ColumnName == s.Property.Name)
                    {
                        return s.Property.Name;
                    }
                    return $"{s.ColumnName} AS {s.Property.Name}";
                }));
            }
            var view = BuildFromSql();
            var limit = GetLimitSql();
            var sql = string.Join(" ", $"SELECT {columns} FROM {view}", tokens);
            if (string.IsNullOrEmpty(limit))
            {
                return sql;
            }
            return string.Join(" ", sql, limit);
        }

        public string Count()
        {
            var tokens = BuildFilterSql(DbExpressionType.OrderBy);
            var columns = GetColumnSql();
            var fromSql = BuildFromSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Select();
                return $"SELECT COUNT({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT COUNT({columns}) FROM {fromSql}", tokens);
        }

        public string Sum()
        {
            var tokens = BuildFilterSql();
            var fromSql = BuildFromSql();
            var columns = GetColumnSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Select();
                return $"SELECT SUM({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT SUM({columns}) FROM {fromSql}", tokens);
        }

        public string Average()
        {
            var tokens = BuildFilterSql();
            var fromSql = BuildFromSql();
            var columns = GetColumnSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Select();
                return $"SELECT MAX({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT MAX({columns}) FROM {fromSql}", tokens);
        }

        public string Min()
        {
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
            var fromSql = BuildFromSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Select();
                return $"SELECT MIN({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT MIN({columns}) FROM {fromSql}", tokens);
        }

        public string Max()
        {
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
            var fromSql = BuildFromSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Select();
                return $"SELECT MAX({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT MIN({columns}) FROM {fromSql}", tokens);
        }

        public string Avg()
        {
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
            var fromSql = BuildFromSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Select();
                return $"SELECT AVG({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT AVG({columns}) FROM {fromSql}", tokens);
        }

        public string Any()
        {
            var sql = Select();
            return $"SELECT EXISTS({sql}) AS Expr";
        }

        private string BuildFromSql()
        {
            if (_tokens.ContainsKey(DbExpressionType.From))
            {
                return $"({_tokens[DbExpressionType.From].First()}) as t";
            }
            return _entityType.TableName;
        }

        private string GetColumnSql()
        {
            if (_tokens.ContainsKey(DbExpressionType.Select))
            {
                var columns = _tokens[DbExpressionType.Select].First();
                return columns;
            }
            return "*";
        }

        private string GetLimitSql()
        {
            if (_tokens.Any(a => a.Key == DbExpressionType.Skip) && _tokens.Any(a => a.Key == DbExpressionType.Take))
            {
                var offset = _tokens[DbExpressionType.Skip].Last();
                var take = _tokens[DbExpressionType.Take].Last();
                return $"LIMIT {offset}, {take}";
            }
            else if (!_tokens.Any(a => a.Key == DbExpressionType.Skip) && _tokens.Any(a => a.Key == DbExpressionType.Take))
            {
                var offset = 0;
                var take = _tokens[DbExpressionType.Take].Last();
                return $"LIMIT {offset}, {take}";
            }
            else if (_tokens.Any(a => a.Key == DbExpressionType.Skip) && !_tokens.Any(a => a.Key == DbExpressionType.Take))
            {
                var offset = _tokens[DbExpressionType.Skip].Last();
                var take = int.MaxValue;
                return $"LIMIT {offset}, {take}";
            }
            return string.Empty;
        }

        private string BuildFilterSql(params DbExpressionType[] filters)
        {
            var tokens = _tokens
                .Where(a => !filters.Contains(a.Key))
                .Where(a => a.Key != DbExpressionType.Take)
                .Where(a => a.Key != DbExpressionType.Skip)
                .OrderBy(s => s.Key)
                .Select(item =>
                {
                    var connector = ", ";
                    if (item.Key == DbExpressionType.Where || item.Key == DbExpressionType.Having)
                        connector = " AND ";
                    var expressions = string.Join(connector, item.Value);
                    var sql = string.Empty;
                    switch (item.Key)
                    {
                        case DbExpressionType.Set:
                            sql = $"SET {expressions}";
                            break;
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
            return string.Join(" ", tokens);
        }
    }
}
