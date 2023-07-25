using System;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Model
{
    public interface IEntityBuilder<T>
    {
        void HasKey<TProperty>(Expression<Func<T, TProperty>> expression);
        PropertyBuilder Property<TProperty>(Expression<Func<T, TProperty>> expression);
    }
}
