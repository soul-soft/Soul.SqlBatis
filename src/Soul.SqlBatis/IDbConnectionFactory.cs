using System;
using System.Data;

namespace Soul.SqlBatis
{
	public interface IDbConnectionFactory
	{
		IDbConnection Create();
	}

	public class DelegateDbConnectionFactory
		: IDbConnectionFactory
	{
		private readonly Func<IDbConnection> _provider;

		public DelegateDbConnectionFactory(Func<IDbConnection> provider)
		{
			_provider = provider;
		}

		public IDbConnection Create()
		{
			return _provider();
		}
	}
}
