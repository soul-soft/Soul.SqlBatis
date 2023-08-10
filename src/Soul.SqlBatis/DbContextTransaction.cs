using System;
using System.Data;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
    public class DbContextTransaction : IDisposable
    {
        private Action _reset;
        private IDbTransaction _transaction;

        internal DbContextTransaction(Action reset, IDbTransaction transaction)
        {
            _reset = reset;
            _transaction = transaction;
        }

        public void Dispose()
        {
            RollbackTransaction();
            _transaction?.Dispose();
            _transaction = null;
            _reset?.Invoke();
			_reset = null;
		}

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction = null;
            _reset?.Invoke();
			_reset = null;

		}

        public Task RollbackTransactionAsync()
        {
            RollbackTransaction();
            return Task.CompletedTask;
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null;
            _reset.Invoke();
			_reset = null;
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
