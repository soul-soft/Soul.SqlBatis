﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    public class SqlQuery
    {
        public DynamicParameters Parameters { get; private set; }
        private readonly IEntityType _entityType;

        private readonly Dictionary<DbExpressionType, IEnumerable<string>> _tokens = new Dictionary<DbExpressionType, IEnumerable<string>>();

        public SqlQuery(IEntityType entityType, Dictionary<DbExpressionType, IEnumerable<string>> tokens, DynamicParameters parameters)
        {
            _entityType = entityType;
            _tokens = tokens;
            Parameters = parameters;
        }

        private string Update()
        {
            var filter = BuildFilterSql();
            var view = BuildFromSql();
            return string.Join(" ", $"UPDATE {view}", filter);
        }

        private string Delete()
        {
            var filter = BuildFilterSql();
            var view = BuildFromSql();
            return string.Join(" ", $"DELETE FROM {view}", filter);
        }

        private string Query()
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

        private string Count()
        {
            var tokens = BuildFilterSql(DbExpressionType.OrderBy);
            var columns = GetColumnSql();
            var fromSql = BuildFromSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Query();
                return $"SELECT COUNT({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT COUNT({columns}) FROM {fromSql}", tokens);
        }

        private string Sum()
        {
            var tokens = BuildFilterSql();
            var fromSql = BuildFromSql();
            var columns = GetColumnSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Query();
                return $"SELECT SUM({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT SUM({columns}) FROM {fromSql}", tokens);
        }

        private string Min()
        {
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
            var fromSql = BuildFromSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Query();
                return $"SELECT MIN({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT MIN({columns}) FROM {fromSql}", tokens);
        }

        private string Max()
        {
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
            var fromSql = BuildFromSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Query();
                return $"SELECT MAX({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT MAX({columns}) FROM {fromSql}", tokens);
        }

        private string Avg()
        {
            var tokens = BuildFilterSql();
            var columns = GetColumnSql();
            var fromSql = BuildFromSql();
            if (_tokens.ContainsKey(DbExpressionType.GroupBy))
            {
                var sql = Query();
                return $"SELECT AVG({columns}) FROM ({sql}) AS t";
            }
            return string.Join(" ", $"SELECT AVG({columns}) FROM {fromSql}", tokens);
        }

        private string Any()
        {
            var sql = Query();
            return $"SELECT EXISTS({sql}) AS Expr";
        }

        public string QuerySql => Query();
        public string UpdateSql => Update();
        public string DeleteSql => Delete();
        public string AnySql => Any();
        public string AvgSql => Avg();
        public string MaxSql => Max();
        public string MinSql => Min();
        public string SumSql => Sum();
        public string CountSql => Count();

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