using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soul.SqlBatis.Infrastructure
{
    public class SqlQuery : SqlQueryBase
    {
        private readonly IEntityType _entityType;
        private readonly Dictionary<DbExpressionToken, IEnumerable<string>> _tokens;

        public SqlQuery(IEntityType entityType, Dictionary<DbExpressionToken, IEnumerable<string>> tokens, DynamicParameters parameters)
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
            var filter = BuildFilterSql();
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
            return GetQuerySql(columns, view, filter);
        }

        private string Count()
        {
            var tokens = BuildFilterSql();
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

        private string GetViewAlias()
        {
            if (_tokens.ContainsKey(DbExpressionToken.As))
            {
                return _tokens[DbExpressionToken.As].Last();
            }
            return "t";
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

        private string GetQuerySql(string column, string view, string filter)
        {
            if (_tokens.Any(a => a.Key == DbExpressionToken.Skip) && _tokens.Any(a => a.Key == DbExpressionToken.Take))
            {
                var offset = _tokens[DbExpressionToken.Skip].Last();
                var take = _tokens[DbExpressionToken.Take].Last();
                var limit = $"OFFSET {offset} ROWS FETCH NEXT {take} ROW ONLY";
                var orderBy = GetOrderBySql(true);
                return StringJoin(" ", $"SELECT {column} FROM {view}", filter, orderBy, limit);
            }
            else if (!_tokens.Any(a => a.Key == DbExpressionToken.Skip) && _tokens.Any(a => a.Key == DbExpressionToken.Take))
            {
                var orderBy = GetOrderBySql(false);
                var take = _tokens[DbExpressionToken.Take].Last();
                return StringJoin(" ", $"SELECT TOP {take} {column} FROM {view}", filter, orderBy);
            }
            else if (_tokens.Any(a => a.Key == DbExpressionToken.Skip) && !_tokens.Any(a => a.Key == DbExpressionToken.Take))
            {
                var orderBy = GetOrderBySql(true);
                var offset = _tokens[DbExpressionToken.Skip].Last();
                var take = int.MaxValue;
                var limit = $"OFFSET {offset} ROWS FETCH NEXT {take} ROW ONLY";
                return StringJoin(" ", $"SELECT {column} FROM {view}", filter, limit, orderBy);
            }
            else
            {
                var orderBy = GetOrderBySql(false);
                return StringJoin(" ", $"SELECT {column} FROM {view}", filter, orderBy);
            }
        }

        private string BuildFilterSql()
        {
            var tokens = _tokens
                .Where(a => a.Key != DbExpressionToken.Take)
                .Where(a => a.Key != DbExpressionToken.Skip)
                .Where(a => a.Key != DbExpressionToken.OrderBy)
                .OrderBy(s => s.Key)
                .Select(item =>
                {
                    var connector = ", ";
                    if (item.Key == DbExpressionToken.Where || item.Key == DbExpressionToken.Having)
                        connector = " AND ";
                    var expressions = string.Join(connector, item.Value);
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
            return string.Join(" ", tokens);
        }

        private string GetOrderBySql(bool hasDefault)
        {
            if (_tokens.Any(a => a.Key == DbExpressionToken.OrderBy))
            {
                return $"ORDER BY {string.Join(",", _tokens[DbExpressionToken.OrderBy])}";
            }
            else if (hasDefault && _entityType.Properties.Any(a => a.IsKey))
            {
                return $"ORDER BY {_entityType.Properties.Where(a => a.IsKey).First().ColumnName}";
            }
            else if (hasDefault)
            {
                return $"ORDER BY (SELECT 1)";
            }
            return string.Empty;
        }
    }
}
