using System;
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
        event Action OnTransactionCommitted;
        event Action OnTransactionRollback;
    }

    internal class DbContextTransaction : IDbContextTransaction
    {
        private IDbTransaction _transaction;

        public IDbTransaction DbTransaction => _transaction;

        public event Action OnTransactionCommitted;

        public event Action OnTransactionRollback;

        internal DbContextTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Dispose()
        {
            RollbackTransaction();
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction?.Rollback();
                _transaction?.Dispose();
                _transaction = null;
                OnTransactionRollback?.Invoke();
            }
        }

        public Task RollbackTransactionAsync()
        {
            RollbackTransaction();
            return Task.CompletedTask;
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction?.Commit();
                _transaction?.Dispose();
                _transaction = null;
                OnTransactionCommitted?.Invoke();
            }
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
