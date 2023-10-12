using System;
using System.Collections.Concurrent;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis
{
    public interface IModelProvider
    {
        IModel Create(Type contextType, Action<ModelBuilder> configure);
    }

    internal class DefaultModelProvider : IModelProvider
    {
        private static ConcurrentDictionary<Type, IModel> _contextModels = new ConcurrentDictionary<Type, IModel>();

        public IModel Create(Type type, Action<ModelBuilder> configure)
        {
            if (!typeof(DbContext).IsAssignableFrom(type))
            {
                throw new InvalidCastException(string.Format("{0} must derive from DbContext", type.Name));
            }
            return _contextModels.GetOrAdd(type, key =>
            {
                var modelBuilder = new ModelBuilder();
                configure(modelBuilder);
                return new Model(modelBuilder.Build().Values);
            });
        }
    }
}
