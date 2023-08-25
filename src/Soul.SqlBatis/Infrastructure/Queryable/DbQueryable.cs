using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis
{
    public abstract class DbQueryable
    {
        private readonly Type _type;

        protected Type Type => _type;

        private readonly DbContext _context;
        
        protected DbContext DbContext => _context;

		protected Model Model => _context.Model;

        protected ChangeTracker ChangeTracker => _context.ChangeTracker;

        protected bool IsTracking { get; private set; } = false;


        private readonly List<DbExpression> _expressions = new List<DbExpression>();


        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

        protected IReadOnlyCollection<DbExpression> Expressions => _expressions;

        protected IReadOnlyDictionary<string, object> Parameters => _parameters;

        public DbQueryable(DbContext context, Type type)
        {
            _type = type;
            _context = context;
        }

        public DbQueryable(DbContext context, Type type, List<DbExpression> expressions, Dictionary<string, object> parameters)
        {
            _type = type;
            _context = context;
            _expressions = expressions;
            _parameters = parameters;
        }

        protected void AddParameters(object param)
        {
            if (param == null)
            {
                return;
            }
            if (param is Dictionary<string, object> values)
            {
                foreach (var item in values)
                {
                    _parameters.Add(item.Key, item.Value);
                }
                return;
            }
            var properties = param.GetType().GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name;
                var value = property.GetValue(param);
                _parameters.Add(name, value);
            }
            return;
        }

        protected void AddExpression(DbExpression expression)
        {
            _expressions.Add(expression);
        }

		internal void AsTracking()
		{
            IsTracking = true;
		}

		internal (string, object) BuildSelect()
        {
            var tokens = new DbExpressionBuilder(Model, _parameters, _expressions).Build();
            var entityType = Model.GetEntityType(Type);
            var sb = new MySqlBuilder(entityType, tokens);
            return (sb.Select(), Parameters);
        }

		internal (string, object) BuildCount()
        {
            var tokens = new DbExpressionBuilder(Model, _parameters, _expressions).Build();
            var entityType = Model.GetEntityType(Type);
            var sb = new MySqlBuilder(entityType, tokens);
            return (sb.Count(), Parameters);
        }

		internal (string, object) BuildSum()
        {
            var tokens = new DbExpressionBuilder(Model, _parameters, _expressions).Build();
            var entityType = Model.GetEntityType(Type);
            var sb = new MySqlBuilder(entityType, tokens);
            return (sb.Sum(), Parameters);
        }

		internal (string, object) BuildMax()
        {
            var tokens = new DbExpressionBuilder(Model, _parameters, _expressions).Build();
            var entityType = Model.GetEntityType(Type);
            var sb = new MySqlBuilder(entityType, tokens);
            return (sb.Max(), Parameters);
        }

		internal (string, object) BuildMin()
        {
            var tokens = new DbExpressionBuilder(Model, _parameters, _expressions).Build();
            var entityType = Model.GetEntityType(Type);
            var sb = new MySqlBuilder(entityType, tokens);
            return (sb.Min(), Parameters);
        }

		internal (string, object) BuildAny()
        {
            var tokens = new DbExpressionBuilder(Model, _parameters, _expressions).Build();
            var entityType = Model.GetEntityType(Type);
            var sb = new MySqlBuilder(entityType, tokens);
            return (sb.Any(), Parameters);
        }

		internal (string, object) BuildAverage()
        {
            var tokens = new DbExpressionBuilder(Model, _parameters, _expressions).Build();
            var entityType = Model.GetEntityType(Type);
            var sb = new MySqlBuilder(entityType, tokens);
            return (sb.Average(), Parameters);
        }

        internal List<T> Query<T>(string sql,object param)
        { 
            var list = _context.Query<T>(sql, param).ToList();
            if (IsTracking)
            {
				var result = new List<T>();
				foreach (var item in list)
                {
					var entry = ChangeTracker.TrackGraph(item);
					entry.State = EntityState.Unchanged;
					result.Add(entry.Entity);
				}
			}
            return list;
        }

		internal async Task<List<T>> QueryAsync<T>(string sql, object param)
		{
			var list = (await _context.QueryAsync<T>(sql, param)).ToList();
			if (IsTracking)
			{
                var result = new List<T>();
				foreach (var item in list)
				{
                    var entry = ChangeTracker.TrackGraph(item);
					entry.State = EntityState.Unchanged;
                    result.Add(entry.Entity);
				}
                return result;
			}
			return list.ToList();
		}
		
        internal T ExecuteScalar<T>(string sql, object param)
		{
			return _context.ExecuteScalar<T>(sql, param);
		}

		internal async Task<T> ExecuteScalarAsync<T>(string sql, object param)
		{
			return await _context.ExecuteScalarAsync<T>(sql, param);
		}
	}

    public class DbQueryable<T> : DbQueryable, IDbQueryable<T>
    {
        public DbQueryable(DbContext context, Type type)
            : base(context, type)
        {

        }

        private DbQueryable(DbContext context, Type type, List<DbExpression> expressions, Dictionary<string, object> parameters)
            : base(context, type, expressions, parameters)
        {

        }

        public IDbQueryable<T> Clone()
        {
            var query = new DbQueryable<T>(DbContext, Type, Expressions.ToList(), Parameters.ToDictionary(s => s.Key, s => s.Value));
            return query;
        }

        public IDbQueryable<TResult> Clone<TResult>()
        {
            var query = new DbQueryable<TResult>(DbContext, Type, Expressions.ToList(), Parameters.ToDictionary(s => s.Key, s => s.Value));
            return query;
        }

        public IDbQueryable<T> FromSql(DbSqlExpression sql, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromSqlExpression(sql, DbExpressionType.From));
            return this;
        }

        public IDbQueryable<T> GroupBy(DbSqlExpression sql, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromSqlExpression(sql, DbExpressionType.GroupBy));
            return this;
        }

        public IDbQueryable<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromLambdaExpression(expression, DbExpressionType.GroupBy));
            return this;
        }

        public IDbQueryable<T> Having(DbSqlExpression sql, object param = null, bool flag = true)
        {
            if (flag)
            {
                AddParameters(param);
                AddExpression(DbExpression.FromSqlExpression(sql, DbExpressionType.Having));
            }
            return this;
        }

        public IDbQueryable<T> Having(Expression<Func<T, bool>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromLambdaExpression(expression, DbExpressionType.Having));
            return this;
        }

        public IDbQueryable<T> OrderBy(DbSqlExpression sql, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromSqlExpression(sql, DbExpressionType.OrderBy));
            return this;
        }

        public IDbQueryable<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromLambdaExpression(expression, DbExpressionType.OrderBy));
            return this;
        }

        public IDbQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromLambdaExpression(expression, DbExpressionType.OrderByDescending));
            return this;
        }

        public IDbQueryable<TResult> Select<TResult>(DbSqlExpression sql, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromSqlExpression(sql, DbExpressionType.Select));
            return Clone<TResult>();
        }

        public IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromLambdaExpression(expression, DbExpressionType.Select));
            return Clone<TResult>();
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

        public IDbQueryable<T> Where(DbSqlExpression sql, object param = null, bool flag = true)
        {
            if (flag)
            {
                AddParameters(param);
                AddExpression(DbExpression.FromSqlExpression(sql, DbExpressionType.Where));
            }
            return this;
        }

        public IDbQueryable<T> Where(Expression<Func<T, bool>> expression, bool flag = true)
        {
            if (flag)
                AddExpression(DbExpression.FromLambdaExpression(expression, DbExpressionType.Where));
            return this;
        }
    }
}
