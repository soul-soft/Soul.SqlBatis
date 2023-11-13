using System;
using System.Data;

namespace Soul.SqlBatis
{
    public enum DBMS
    {
        MSSQL,
        MYSQL,
    }

    public interface IDbConnectionFactory
    {
        DBMS DBMS { get; }
        IDbConnection Create();
    }

    public class DelegateDbConnectionFactory
        : IDbConnectionFactory
    {
        private readonly Func<IDbConnection> _provider;

        public DBMS DBMS { get; }

        public DelegateDbConnectionFactory(Func<IDbConnection> provider, DBMS dbms)
        {
            _provider = provider;
            DBMS = dbms;
        }

        public IDbConnection Create()
        {
            return _provider();
        }
    }
}
