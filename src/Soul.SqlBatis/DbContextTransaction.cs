using System;
using System.Data;

namespace Soul.SqlBatis
{
    public class DbContextTransaction : IDisposable
    {
        private Action _callback;
        private bool _isSubmit = false;
        private IDbTransaction _transaction;

        public IDbTransaction DbTransaction => _transaction;

        internal DbContextTransaction(IDbTransaction transaction, Action callback)
        {
            _callback = callback;
            _transaction = transaction;
        }

        public void CommitTransaction()
        {
            _transaction.Commit();
            _isSubmit = true;
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
            _isSubmit = true;
        }

        public void Dispose()
        {
            try
            {
                if (!_isSubmit)
                {
                    _transaction.Rollback();
                }
                _transaction?.Dispose();
                _transaction = null;
            }
            catch { }
            try
            {
                _callback?.Invoke();
                _callback = null;
            }
            catch { }
        }
    }
}
