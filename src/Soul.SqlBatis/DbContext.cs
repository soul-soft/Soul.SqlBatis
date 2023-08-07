﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Soul.SqlBatis.Infrastructure;
using static Dapper.SqlMapper;

namespace Soul.SqlBatis
{
	public abstract class DbContext : IDisposable
	{
		private Model _model;

		private IDbConnection _connection;

		public DbContextOptions Options { get; }

		private DbContextTransaction _currentDbTransaction;

		public DbContextTransaction CurrentDbTransaction => _currentDbTransaction;

		public Model Model => _model;

		private ChangeTracker _changeTracker = new ChangeTracker();

		public ChangeTracker ChangeTracker => _changeTracker;

		public DbContext(DbContextOptions options)
		{
			Options = options;
			_model = ModelCreating();
			_connection = options.ConnecionProvider();
		}

		public DbSet<T> Set<T>()
			where T : class
		{
			return new DbSet<T>(this);
		}

		public EntityEntry<T> Entry<T>(T entity)
			where T : class
		{
			return _changeTracker.TrackGraph(entity);
		}

		public void Add<T>(T entity)
			where T : class
		{
			Entry(entity).State = EntityState.Added;
		}

		public void AddRange<T>(IEnumerable<T> entities)
			where T : class
		{
			foreach (var entity in entities)
			{
				Add(entity);
			}
		}

		public void Update<T>(T entity)
			where T : class
		{
			Entry(entity).State = EntityState.Modified;
		}

		public void UpdateRange<T>(IEnumerable<T> entities)
			where T : class
		{
			foreach (var entity in entities)
			{
				Update(entity);
			}
		}

		public void Delete<T>(T entity)
			where T : class
		{
			Entry(entity).State = EntityState.Deleted;
		}

		public void DeleteRange<T>(IEnumerable<T> entities)
			where T : class
		{
			foreach (var entity in entities)
			{
				Delete(entity);
			}
		}

		public int SaveChanges()
		{
			return new DbBatchCommand(this).SaveChanges();
		}

		public Task<int> SaveChangesAsync()
		{
			return new DbBatchCommand(this).SaveChangesAsync();
		}


		public IDbConnection GetDbConnection()
		{
			return _connection;
		}

		public IDbTransaction GetDbTransaction()
		{
			return CurrentDbTransaction?.GetDbTransaction();
		}

		public void OpenDbConnection()
		{
			if (_connection.State == ConnectionState.Closed)
			{
				_connection.Open();
			}
		}

		public async Task OpenDbConnectionAsync()
		{
			if (_connection.State == ConnectionState.Closed)
			{
				if (_connection is DbConnection connection)
				{
					await connection.OpenAsync();
				}
				else
				{
					_connection.Open();
				}
			}
		}

		public void ColseDbConnection()
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}

		public Task ColseDbConnectionAsync()
		{
			ColseDbConnection();
			return Task.CompletedTask;
		}

		public DbContextTransaction BeginTransaction()
		{
			var autoCloase = false;
			if (_connection.State == ConnectionState.Closed)
			{
				OpenDbConnection();
				autoCloase = true;
			}
			var transaction = _connection.BeginTransaction();
			_currentDbTransaction = new DbContextTransaction(() =>
			{
				_currentDbTransaction = null;
				if (autoCloase)
				{
					ColseDbConnection();
				}
			}, transaction);
			return CurrentDbTransaction;
		}

		public async Task<DbContextTransaction> BeginTransactionAsync()
		{
			var autoCloase = false;
			if (_connection.State == ConnectionState.Closed)
			{
				await OpenDbConnectionAsync();
				autoCloase = true;
			}
			var transaction = _connection.BeginTransaction();
			_currentDbTransaction = new DbContextTransaction(() =>
			{
				_currentDbTransaction = null;
				if (autoCloase)
				{
					ColseDbConnection();
				}
			}, transaction);
			return CurrentDbTransaction;
		}

		private Model ModelCreating()
		{
			return ModelBuilder.CreateDbContextModel(GetType(), OnModelCreating);
		}

		protected virtual void OnModelCreating(ModelBuilder builder)
		{

		}

		public virtual IEnumerable<T> Query<T>(string sql, object param = null)
		{
			return _connection.Query<T>(sql, param, GetDbTransaction(), false);
		}

		public virtual Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
		{
			return _connection.QueryAsync<T>(sql, param, GetDbTransaction());
		}

		public virtual int Execute(string sql, object param = null)
		{
			return _connection.Execute(sql, param, GetDbTransaction());
		}

		public virtual Task<int> ExecuteAsync(string sql, object param = null)
		{
			return _connection.ExecuteAsync(sql, param, GetDbTransaction());
		}

		public virtual T ExecuteScalar<T>(string sql, object param = null)
		{
			return _connection.ExecuteScalar<T>(sql, param, GetDbTransaction());
		}

		public virtual Task<T> ExecuteScalarAsync<T>(string sql, object param = null)
		{
			return _connection.ExecuteScalarAsync<T>(sql, param, GetDbTransaction());
		}

		public void Dispose()
		{
			_currentDbTransaction?.Dispose();
			_currentDbTransaction = null;
			_connection?.Close();
			_connection?.Dispose();
			_connection = null;
		}
	}
}
