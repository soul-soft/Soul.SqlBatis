using System;
using System.Data;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
	public class DbContextTransaction : IDisposable
	{
		private IDbTransaction _transaction;

		public DbContextTransaction(IDbTransaction transaction)
		{
			_transaction = transaction;
		}

		public void Dispose()
		{
			RollbackTransaction();
			_transaction?.Dispose();
			_transaction = null;
		}

		public void RollbackTransaction()
		{
			_transaction?.Rollback();
		}

		public Task RollbackTransactionAsync()
		{
			RollbackTransaction();
			return Task.CompletedTask;
		}

		public void CommitTransaction()
		{
			_transaction?.Commit();
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
