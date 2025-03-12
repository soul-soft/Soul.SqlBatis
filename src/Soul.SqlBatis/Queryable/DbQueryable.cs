using Soul.SqlBatis.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Soul.SqlBatis
{
    public class DbQueryable<T> : IDbQueryable<T>
    {
        protected DbContext DbContext;

        public IEntityType EntityType { get; }

        public bool IsTracking { get; private set; }

        public DynamicParameters Parameters { get; }

        public Dictionary<DbQueryTokenType, List<DbQueryToken>> Tokens { get; } = new Dictionary<DbQueryTokenType, List<DbQueryToken>>();

        internal DbQueryable(DbContext context, DynamicParameters parameters)
        {
            DbContext = context;
            IsTracking = false;
            EntityType = context.Model.FindEntityType(typeof(T));
            Parameters = parameters;
        }

        private DbQueryable(DbContext context, IEntityType entityType, bool isTracking, DynamicParameters parameters)
        {
            DbContext = context;
            IsTracking = isTracking;
            EntityType = entityType;
            Parameters = parameters;
        }

        internal DbContext GetDbContext()
        {
            return DbContext;
        }

        public IDbQueryable<T> As(string name)
        {
            AddToken(DbQueryTokenType.As, name);
            return this;
        }

        public IDbQueryable<T> AsTracking()
        {
            IsTracking = true;
            return this;
        }

        public IDbQueryable<T> AsNoTracking()
        {
            IsTracking = false;
            return this;
        }

        public IDbQueryable<TResult> Clone<TResult>()
        {
            var query = new DbQueryable<TResult>(DbContext, EntityType, IsTracking, Parameters);
            foreach (var item in Tokens)
            {
                foreach (var value in item.Value)
                {
                    query.AddToken(item.Key, value.Value);
                }
            }
            return query;
        }

        public IDbQueryable<T> GroupBy(string expression, bool flag = true)
        {
            AddToken(DbQueryTokenType.GroupBy, expression, flag);
            return this;
        }

        public IDbQueryable<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            AddToken(DbQueryTokenType.GroupBy, (Expression)expression, flag);
            return this;
        }

        public IDbQueryable<T> OrderBy(string expression, bool flag = true)
        {
            AddToken(DbQueryTokenType.OrderBy, expression, flag);
            return this;
        }

        public IDbQueryable<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            AddToken(DbQueryTokenType.OrderBy, (Expression)expression, flag);
            return this;
        }

        public IDbQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            AddToken(DbQueryTokenType.OrderByDescending, (Expression)expression, flag);
            return this;
        }

        public IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            AddToken(DbQueryTokenType.Select, (Expression)expression, true);
            return Clone<TResult>();
        }

        public IDbQueryable<T> Skip(int offset)
        {
            AddToken(DbQueryTokenType.Skip, offset);
            return this;
        }

        public IDbQueryable<T> Take(int count)
        {
            AddToken(DbQueryTokenType.Take, count);
            return this;
        }

        public IDbQueryable<T> Where(string expression, bool flag = true)
        {
            AddToken(DbQueryTokenType.Where, expression, flag);
            return this;
        }

        public IDbQueryable<T> Where(Expression<Func<T, bool>> expression, bool flag = true)
        {
            AddToken(DbQueryTokenType.Where, (Expression)expression, flag);
            return this;
        }

        public IDbQueryable<T> Having(string expression, bool flag = true)
        {
            AddToken(DbQueryTokenType.Having, expression, flag);
            return this;
        }

        public IDbQueryable<T> Having<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            AddToken(DbQueryTokenType.Having, (Expression)expression, flag);
            return this;
        }

        public void AddToken<TToken>(DbQueryTokenType type, TToken token, bool flag = true)
        {
            if (!flag)
                return;
            if (!Tokens.ContainsKey(type))
            {
                Tokens[type] = new List<DbQueryToken>();
            }
            Tokens[type].Add(DbQueryToken.New(token));
        }

        private string GetAlias()
        {
            if (!Tokens.ContainsKey(DbQueryTokenType.As))
            {
                return string.Empty;
            }
            var name = Tokens[DbQueryTokenType.As].Last().As<string>();
            return DbContext.Model.Format(name);
        }

        public (SqlBuilder, DynamicParameters) Build(Action<DbQueryableOptions> configureOptions = null)
        {
            var options = new DbQueryableOptions();
            configureOptions?.Invoke(options);
            var alias = GetAlias();
            var parameters = Parameters;
            var sqlBuilder = DbContext.CreateSqlBuilder();
            var sqlExpressionContext = new SqlExpressionContext(alias, DbContext.Model, parameters, DbContext.GetSettings());

            foreach (var item in Tokens)
            {
                if (item.Key == DbQueryTokenType.Take)
                {
                    var token = item.Value.Last();
                    sqlBuilder.Take(token.As<int>());
                }
                else if (item.Key == DbQueryTokenType.Skip)
                {
                    var token = item.Value.Last();
                    sqlBuilder.Skip(token.As<int>());
                }
                else if (item.Key == DbQueryTokenType.Select)
                {
                    var token = item.Value.Last();
                    if (token.Is<Expression>())
                    {
                        var columns = SqlColumnExpressionParser.Parse(sqlExpressionContext, token.As<Expression>());
                        foreach (var column in columns)
                        {
                            if (options.HasColumnsAlias)
                            {
                                sqlBuilder.Select($"{column.Value} AS {column.Key}");
                            }
                            else
                            {
                                sqlBuilder.Select($"{column.Value}");
                            }
                        }
                    }
                }
                else if (item.Key == DbQueryTokenType.Where)
                {
                    item.Value.ForEach(token =>
                    {
                        if (token.Is<Expression>())
                        {
                            var sql = SqlExpressionParser.Parse(sqlExpressionContext, token.As<Expression>());
                            sqlBuilder.Where(sql);
                        }
                        else if (token.Is<string>())
                        {
                            sqlBuilder.Where(token.As<string>());
                        }
                    });
                }
                else if (item.Key == DbQueryTokenType.GroupBy)
                {
                    item.Value.ForEach(token =>
                    {
                        if (token.Is<Expression>())
                        {
                            var columns = SqlColumnExpressionParser.Parse(sqlExpressionContext, token.As<Expression>());
                            foreach (var column in columns)
                            {
                                sqlBuilder.GroupBy(column.Value);
                            }
                        }
                        else if (token.Is<string>())
                        {
                            sqlBuilder.GroupBy(token.As<string>());
                        }
                    });
                }
                else if (item.Key == DbQueryTokenType.Having)
                {
                    item.Value.ForEach(token =>
                    {
                        if (token.Is<Expression>())
                        {
                            var sql = SqlExpressionParser.Parse(sqlExpressionContext, token.As<Expression>());
                            sqlBuilder.Having(sql);
                        }
                        else if (token.Is<string>())
                        {
                            sqlBuilder.Having(token.As<string>());
                        }
                    });
                }
                else if (item.Key == DbQueryTokenType.OrderBy)
                {
                    item.Value.ForEach(token =>
                    {
                        if (token.Is<Expression>())
                        {
                            var columns = SqlColumnExpressionParser.Parse(sqlExpressionContext, token.As<Expression>());
                            foreach (var column in columns)
                            {
                                sqlBuilder.OrderBy($"{column.Value} ASC");
                            }
                        }
                        else if (token.Is<string>())
                        {
                            sqlBuilder.OrderBy(token.As<string>());
                        }
                    });
                }
                else if (item.Key == DbQueryTokenType.OrderByDescending)
                {
                    item.Value.ForEach(token =>
                    {
                        if (token.Is<Expression>())
                        {
                            var columns = SqlColumnExpressionParser.Parse(sqlExpressionContext, token.As<Expression>());
                            foreach (var column in columns)
                            {
                                sqlBuilder.OrderBy($"{column.Value} DESC");
                            }
                        }
                        else if (token.Is<string>())
                        {
                            var sql = token.As<string>();
                            sqlBuilder.OrderBy($"{sql} DESC");
                        }
                    });
                }
                else if (item.Key == DbQueryTokenType.Setters)
                {
                    var token = item.Value.Last();
                    if (token.Is<Expression>())
                    {
                        var setters = SqlUpdateExpressionParser.Parse(sqlExpressionContext, token.As<Expression>());
                        foreach (var setter in setters)
                        {
                            sqlBuilder.Set(setter);
                        }
                    }
                }
            }

            if (options.HasDefaultColumns && !Tokens.ContainsKey(DbQueryTokenType.Select))
            {
                var columns = EntityType.GetProperties()
                        .Where(a => !a.IsNotMapped())
                        .Select(s => options.HasColumnsAlias ? $"{s.ColumnName} AS {DbContext.Model.Format(s.Property.Name)}" : s.ColumnName);
                foreach (var item in columns)
                {
                    sqlBuilder.Select(item);
                }
            }
            var view = EntityType.TableName;
            if (!string.IsNullOrEmpty(alias))
            {
                view = $"{view} AS {alias}";
            }
            sqlBuilder.From(view);
            if (options.UseDefaultOrder)
            {
                if (EntityType.GetProperties().Any(a => a.IsKey()))
                {
                    var keyProperty = EntityType.GetProperties().Where(a => a.IsKey()).First();
                    sqlBuilder.OrderBy($"{keyProperty.ColumnName} ASC", UsingDefaultOrder(sqlBuilder, keyProperty.ColumnName));
                }
            }
            return (sqlBuilder, parameters);
        }


        private bool UsingDefaultOrder(SqlBuilder sb, string column)
        {
            if (!sb.Tokens.ContainsKey(nameof(SqlBuilder.OrderBy))) 
            {
                return true;
            }
            var orders = sb.Tokens[nameof(SqlBuilder.OrderBy)];
            if (orders.Any(a => $"{column} ASC".Equals(a, StringComparison.OrdinalIgnoreCase) || $"{column} DESC".Equals(a, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            return true;
        }
    }
}
