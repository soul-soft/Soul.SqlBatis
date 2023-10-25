using System;
using System.Data;
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
        event Action OnTransactionCommitEnd;
        event Action OnTransactionRollbackEnd;
    }

    internal class DbContextTransaction : IDbContextTransaction
    {
        private IDbTransaction _transaction;

        public IDbTransaction DbTransaction => _transaction;

        public event Action OnTransactionCommitEnd;

        public event Action OnTransactionRollbackEnd;

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
                OnTransactionRollbackEnd?.Invoke();
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
                OnTransactionCommitEnd?.Invoke();
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
