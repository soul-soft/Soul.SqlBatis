using System;
using System.Data;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
	public class DbContextTransaction : IDisposable
	{
		private readonly DbContext _context;
		private IDbTransaction _transaction;

		public DbContextTransaction(DbContext context,IDbTransaction transaction)
		{
			_context = context;
			_transaction = transaction;
		}

		public void Dispose()
		{
			RollbackTransaction();
			_transaction?.Dispose();
			_transaction = null;
			_context.CurrentDbTransaction = null;
		}

		public void RollbackTransaction()
		{
			_transaction?.Rollback();
			_transaction = null;
			_context.CurrentDbTransaction = null;
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
			_context.CurrentDbTransaction = null;
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
