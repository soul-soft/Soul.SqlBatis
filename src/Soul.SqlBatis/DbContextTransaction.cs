using System;
using System.Data;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
    public class DbContextTransaction : IDisposable
    {
        private Action _callback;
        private IDbTransaction _transaction;

        internal DbContextTransaction(Action reset, IDbTransaction transaction)
        {
            _callback = reset;
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
