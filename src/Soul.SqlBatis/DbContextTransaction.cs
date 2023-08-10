﻿using System;
using System.Data;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
    public interface IDbContextTransaction : IDisposable
    {
        void CommitTransaction();
        Task CommitTransactionAsync();
        void RollbackTransaction();
        Task RollbackTransactionAsync();
        IDbTransaction DbTransaction { get; }
    }

    internal class DbContextTransaction : IDbContextTransaction
    {
        private Action _callback;
        
        private IDbTransaction _transaction;

        public IDbTransaction DbTransaction => _transaction;

        internal DbContextTransaction(IDbTransaction transaction, Action callback)
        {
            _callback = callback;
            _transaction = transaction;
        }

        public void Dispose()
        {
            RollbackTransaction();
		}

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction = null;
            _transaction?.Dispose();
            _callback?.Invoke();
			_callback = null;

		}

        public Task RollbackTransactionAsync()
        {
            RollbackTransaction();
            return Task.CompletedTask;
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            _transaction = null;
            _callback.Invoke();
			_callback = null;
		}

        public Task CommitTransactionAsync()
        {
            CommitTransaction();
            return Task.CompletedTask;
        }

        public IDbTransaction GetDbTransaction()
        {
            return _transaction;
        }
    }
}
