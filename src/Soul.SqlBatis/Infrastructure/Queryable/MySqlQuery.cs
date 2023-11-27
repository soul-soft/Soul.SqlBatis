using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    internal class MySqlQuery : SqlQueryBase
    {
        private readonly IEntityType _entityType;
        private readonly Dictionary<DbExpressionToken, IEnumerable<string>> _tokens;

        public MySqlQuery(IEntityType entityType, Dictionary<DbExpressionToken, IEnumerable<string>> tokens, DynamicParameters parameters)
        {
            _entityType = entityType;
            _tokens = tokens;
            Parameters = parameters;
        }

        public override DynamicParameters Parameters { get; }
        public override string QuerySql => Query();
        public override string UpdateSql => Update();
        public override string DeleteSql => Delete();
        public override string AnySql => Any();
        public override string AvgSql => Avg();
        public override string MaxSql => Max();
        public override string MinSql => Min();
        public override string SumSql => Sum();
        public override string CountSql => Count();
        public override string ViewAlias => GetViewAlias();

        private string Update()
        {
            var filter = BuildFilterSql();
            var view = GetView();
            return StringJoin(" ", $"UPDATE {view}", filter);
        }

        private string Delete()
        {
            var filter = BuildFilterSql();
            var view = GetView();
            return StringJoin(" ", $"DELETE FROM {view}", filter);
        }

        private string Query()
        {
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
            if (columns == "*")
            {
                columns = StringJoin(", ", _entityType.Properties.Select(s =>
                {
                    if (s.ColumnName == s.Property.Name)
                    {
                        return s.Property.Name;
                    }
                    return $"{s.ColumnName} AS {s.Property.Name}";
                }));
            }
            var view = GetView();
            return BuildQuery(columns, view, tokens);
        }

        private string Count()
        {
            var tokens = BuildFilterSql(DbExpressionToken.OrderBy);
            var columns = GetColumnSql();
            var fromSql = GetView();
            if (_tokens.ContainsKey(DbExpressionToken.GroupBy))
            {
                var sql = Query();
                return $"SELECT COUNT({columns}) FROM ({sql}) AS t";
            }
            return StringJoin(" ", $"SELECT COUNT({columns}) FROM {fromSql}", tokens);
        }

        private string Sum()
        {
            var tokens = BuildFilterSql();
            var fromSql = GetView();
            var columns = GetColumnSql();
            if (_tokens.ContainsKey(DbExpressionToken.GroupBy))
            {
                var sql = Query();
                return $"SELECT SUM({columns}) FROM ({sql}) AS t";
            }
            return StringJoin(" ", $"SELECT SUM({columns}) FROM {fromSql}", tokens);
        }

        private string Min()
        {
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
            var fromSql = GetView();
            if (_tokens.ContainsKey(DbExpressionToken.GroupBy))
            {
                var sql = Query();
                return $"SELECT MIN({columns}) FROM ({sql}) AS t";
            }
            return StringJoin(" ", $"SELECT MIN({columns}) FROM {fromSql}", tokens);
        }

        private string Max()
        {
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
            var fromSql = GetView();
            if (_tokens.ContainsKey(DbExpressionToken.GroupBy))
            {
                var sql = Query();
                return $"SELECT MAX({columns}) FROM ({sql}) AS t";
            }
            return StringJoin(" ", $"SELECT MAX({columns}) FROM {fromSql}", tokens);
        }

        private string Avg()
        {
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
            var fromSql = GetView();
            if (_tokens.ContainsKey(DbExpressionToken.GroupBy))
            {
                var sql = Query();
                return $"SELECT AVG({columns}) FROM ({sql}) AS t";
            }
            return StringJoin(" ", $"SELECT AVG({columns}) FROM {fromSql}", tokens);
        }

        private string Any()
        {
            var sql = Query();
            return $"SELECT EXISTS({sql}) AS Expr";
        }

        private string GetColumnSql()
        {
            if (_tokens.ContainsKey(DbExpressionToken.Select))
            {
                var columns = _tokens[DbExpressionToken.Select].First();
                return columns;
            }
            return "*";
        }

        private string GetViewAlias()
        {
            if (_tokens.ContainsKey(DbExpressionToken.As))
            {
                return _tokens[DbExpressionToken.As].Last();
            }
            return "t";
        }

        private string GetView()
        {
            if (_tokens.ContainsKey(DbExpressionToken.From))
            {
                return $"({_tokens[DbExpressionToken.From].Last()}) AS {ViewAlias}";
            }
            else
            {
                return $"{_entityType.TableName} AS {ViewAlias}";
            }
        }

        private string BuildQuery(string column, string view, string filter)
        {
            if (_tokens.Any(a => a.Key == DbExpressionToken.Skip) && _tokens.Any(a => a.Key == DbExpressionToken.Take))
            {
                var offset = _tokens[DbExpressionToken.Skip].Last();
                var take = _tokens[DbExpressionToken.Take].Last();
                return StringJoin(" ", $"SELECT {column} FROM {view}", filter, $"LIMIT {offset}, {take}");
            }
            else if (!_tokens.Any(a => a.Key == DbExpressionToken.Skip) && _tokens.Any(a => a.Key == DbExpressionToken.Take))
            {
                var offset = 0;
                var take = _tokens[DbExpressionToken.Take].Last();
                return StringJoin(" ", $"SELECT {column} FROM {view}", filter, $"LIMIT {offset}, {take}");
            }
            else if (_tokens.Any(a => a.Key == DbExpressionToken.Skip) && !_tokens.Any(a => a.Key == DbExpressionToken.Take))
            {
                var offset = _tokens[DbExpressionToken.Skip].Last();
                var take = int.MaxValue;
                return StringJoin(" ", $"SELECT {column} FROM {view}", filter, $"LIMIT {offset}, {take}");
            }
            return StringJoin(" ", $"SELECT {column} FROM {view}", filter);
        }

        private string BuildFilterSql(params DbExpressionToken[] filters)
        {
            var tokens = _tokens
                .Where(a => !filters.Contains(a.Key))
                .Where(a => a.Key != DbExpressionToken.Take)
                .Where(a => a.Key != DbExpressionToken.Skip)
                .OrderBy(s => s.Key)
                .Select(item =>
                {
                    var separator = ", ";
                    if (item.Key == DbExpressionToken.Where || item.Key == DbExpressionToken.Having)
                        separator = " AND ";
                    var expressions = StringJoin(separator, item.Value);
                    var sql = string.Empty;
                    switch (item.Key)
                    {
                        case DbExpressionToken.Set:
                            sql = $"SET {expressions}";
                            break;
                        case DbExpressionToken.Where:
                            sql = $"WHERE {expressions}";
                            break;
                        case DbExpressionToken.Join:
                            sql = $"{expressions}";
                            break;
                        case DbExpressionToken.GroupBy:
                            sql = $"Group By {expressions}";
                            break;
                        case DbExpressionToken.Having:
                            sql = $"HAVING {expressions}";
                            break;
                        case DbExpressionToken.OrderBy:
                            sql = $"ORDER BY {expressions}";
                            break;
                    }
                    return sql;
                });
            return StringJoin(" ", tokens);
        }
    }
}
