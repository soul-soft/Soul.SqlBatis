using System;
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
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
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
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
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
            var tokens = BuildFilterSql();
            var fromSql = GetFromSql();
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
            var fromSql = GetFromSql();
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
            var fromSql = GetFromSql();
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
            var fromSql = GetFromSql();
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
            var fromSql = GetFromSql();
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

        private string GetFromSql()
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

        private string BuildFilterSql()
        {
            var filters = _tokens.Where(a=>a.Key!=DbExpressionType.Take)
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
