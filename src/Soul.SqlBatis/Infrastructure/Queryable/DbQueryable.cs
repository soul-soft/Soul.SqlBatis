using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis
{
    public abstract class DbQueryable
    {
        private readonly Type _type;

        protected Type EntityType => _type;

        private readonly DbContext _context;

        protected DbContext DbContext => _context;

        protected IModel Model => _context.Model;

        protected ChangeTracker ChangeTracker => _context.ChangeTracker;

        private bool isTracking = false;

        protected bool IsTracking { get => isTracking; private set => isTracking = value; }

        private readonly List<DbExpression> _expressions = new List<DbExpression>();

        public IReadOnlyCollection<DbExpression> DbExpressions => _expressions;

        private readonly DynamicParameters _parameters;

        protected DynamicParameters Parameters { get => _parameters; }

        public DbQueryable(DbContext context, Type type, DynamicParameters parameters)
        {
            _type = type;
            _context = context;
            _parameters = parameters;
            IsTracking = context.Options.EnableQueryTracking;

        }

        public DbQueryable(DbContext context, Type type, List<DbExpression> expressions, DynamicParameters parameters)
        {
            _type = type;
            _context = context;
            _expressions = expressions;
            _parameters = parameters;
            IsTracking = false;
        }

        protected void AddExpression(DbExpression expression)
        {
            _expressions.Add(expression);
        }

        internal void AsTracking()
        {
            IsTracking = true;
        }

        internal void AsNoTracking()
        {
            IsTracking = false;
        }

        internal List<T> Query<T>(string sql, object param)
        {
            var list = _context.Query<T>(sql, param).ToList();
            if (IsTracking)
            {
                var result = new List<T>();
                foreach (var item in list)
                {
                    var entry = ChangeTracker.TrackGraph(item);
                    result.Add(entry.Entity);
                }
            }
            return list;
        }

        internal SqlQueryBase BuildSqlQuery()
        {
            var tokens = new DbExpressionBuilder(Model, _parameters, _expressions).Build();
            var entityType = Model.GetEntityType(EntityType);
            var dbms = _context.Options.ConnectionFactory.DBMS;
            if (dbms == DBMS.MYSQL)
            {
                return new MySqlQuery(entityType, tokens, _parameters);
            }
            return new SqlQuery(entityType, tokens, _parameters);
        }

        internal async Task<List<T>> QueryAsync<T>(string sql, object param, CancellationToken? cancellationToken = null)
        {
            var list = await _context.QueryAsync<T>(sql, param, cancellationToken: cancellationToken);
            if (IsTracking)
            {
                var result = new List<T>();
                foreach (var item in list)
                {
                    var entry = ChangeTracker.TrackGraph(item);
                    result.Add(entry.Entity);
                }
                return result;
            }
            return list.ToList();
        }

        internal int Execute(string sql, object param)
        {
            return _context.Execute(sql, param);
        }

        internal Task<int> ExecuteAsync(string sql, object param, CancellationToken? cancellationToken = null)
        {
            return _context.ExecuteAsync(sql, param, cancellationToken: cancellationToken);
        }

        internal T ExecuteScalar<T>(string sql, object param)
        {
            return _context.ExecuteScalar<T>(sql, param);
        }

        internal async Task<T> ExecuteScalarAsync<T>(string sql, object param, CancellationToken? cancellationToken = null)
        {
            return await _context.ExecuteScalarAsync<T>(sql, param, cancellationToken: cancellationToken);
        }
    }

    public class DbQueryable<T> : DbQueryable, IDbQueryable<T>
    {
        IModel IDbQueryable<T>.Model => DbContext.Model;


        public DbQueryable(DbContext context, Type type, DynamicParameters parameters)
            : base(context, type, parameters)
        {

        }

        public DbQueryable(DbContext context, Type type, string fromSql, DynamicParameters parameters)
           : base(context, type, parameters)
        {
            AddExpression(DbExpression.FromSqlExpression(fromSql, DbExpressionType.From));
        }

        private DbQueryable(DbContext context, Type type, List<DbExpression> expressions, DynamicParameters parameters)
            : base(context, type, expressions, parameters)
        {

        }

        public (SqlBuilder, DynamicParameters) Build()
        {
            var sb = new SqlBuilder();
            var builder = new DbExpressionBuilder(Model, Parameters, DbExpressions);
            var tokens = builder.Build();
            foreach (var item in tokens.Where(a => a.Key == DbExpressionType.Where))
            {
                sb.Where(string.Join(" AND ", item.Value));
            }
            return (sb, Parameters);
        }

        public IDbQueryable<T> Clone()
        {
            var query = new DbQueryable<T>(DbContext, EntityType, DbExpressions.ToList(), Parameters);
            return query;
        }

        public IDbQueryable<TResult> Clone<TResult>()
        {
            var query = new DbQueryable<TResult>(DbContext, EntityType, DbExpressions.ToList(), Parameters);
            return query;
        }

        public IDbQueryable<T> GroupBy(RawSql sql, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromSqlExpression(sql, DbExpressionType.GroupBy));
            return this;
        }

        public IDbQueryable<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromExpression(expression, DbExpressionType.GroupBy));
            return this;
        }

        public IDbQueryable<T> Having(RawSql sql, bool flag = true)
        {
            if (flag)
            {
                AddExpression(DbExpression.FromSqlExpression(sql, DbExpressionType.Having));
            }
            return this;
        }

        public IDbQueryable<T> Having(Expression<Func<T, bool>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromExpression(expression, DbExpressionType.Having));
            return this;
        }

        public IDbQueryable<T> OrderBy(RawSql sql, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromSqlExpression(sql, DbExpressionType.OrderBy));
            return this;
        }

        public IDbQueryable<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromExpression(expression, DbExpressionType.OrderBy));
            return this;
        }

        public IDbQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromExpression(expression, DbExpressionType.OrderByDescending));
            return this;
        }

        public IDbQueryable<TResult> Select<TResult>(RawSql sql, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromSqlExpression(sql, DbExpressionType.Select));
            return Clone<TResult>();
        }

        public IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromExpression(expression, DbExpressionType.Select));
            return Clone<TResult>();
        }

        public IDbQueryable<T> SetProperty<TResult>(Expression<Func<T, TResult>> column, TResult value, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromSetExpression(column, Expression.Constant(value)));
            return this;
        }

        public IDbQueryable<T> Set<TResult>(Expression<Func<T, TResult>> column, Expression<Func<T, TResult>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromSetExpression(column, expression));
            return this;
        }

        public IDbQueryable<T> Skip(int count, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromSqlExpression(count.ToString(), DbExpressionType.Skip));
            return this;
        }

        public IDbQueryable<T> Take(int count, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromSqlExpression(count.ToString(), DbExpressionType.Take));
            return this;
        }

        public IDbQueryable<T> Where(RawSql sql, bool flag = true)
        {
            if (flag)
            {
                AddExpression(DbExpression.FromSqlExpression(sql, DbExpressionType.Where));
            }
            return this;
        }

        public IDbQueryable<T> Where(Expression<Func<T, bool>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromExpression(expression, DbExpressionType.Where));
            return this;
        }
    }
}
