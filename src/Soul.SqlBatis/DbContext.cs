﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Soul.SqlBatis.Infrastructure;

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

        public virtual int Execute<T>(string sql, object param = null)
        {
            return _connection.Execute(sql, param, GetDbTransaction());
        }

        public virtual Task<int> ExecuteAsync<T>(string sql, object param = null)
        {
            return _connection.ExecuteAsync(sql, param, GetDbTransaction());
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
