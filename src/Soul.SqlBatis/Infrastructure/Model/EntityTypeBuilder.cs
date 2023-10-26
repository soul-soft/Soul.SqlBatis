using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Soul.SqlBatis.Exceptions;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityTypeBuilder
	{
		private IEntityType _entityType;

		internal EntityTypeBuilder(IEntityType entityType)
		{
			_entityType = entityType;
		}

		public void Igonre(string[] propertyNames)
		{
			foreach (var item in propertyNames)
			{
				Property(item).HasAnnotation(new NotMappedAttribute());
			}
		}

		public void Igonre(MemberInfo[] members)
		{
			foreach (var item in members)
			{
				Property(item).HasAnnotation(new NotMappedAttribute());
			}
		}

		public EntityPropertyTypeBuilder Property(string propertyName)
		{
			return Property(GetMember(propertyName));
		}

		public EntityPropertyTypeBuilder Property(MemberInfo member)
		{
			return new EntityPropertyTypeBuilder(GetProperty(member));
		}

		public void HasKey(string[] propertyNames)
		{
			var members = _entityType.Type.GetProperties()
				.Where(a => propertyNames.Contains(a.Name))
				.ToArray();
			HasKey(members);
		}

		public void HasKey(MemberInfo[] members)
		{
			foreach (var item in members)
			{
				Property(item).HasAnnotation(new KeyAttribute());
			}
		}

		public void ToTable(string name, string schema = null)
		{
			if (!string.IsNullOrEmpty(schema))
			{
				HasAnnotation(new TableAttribute(name)
				{
					Schema = schema
				});
			}
			else
			{
				HasAnnotation(new TableAttribute(name));
			}
		}

		public void ToView(string name, string schema = null)
		{
			if (!string.IsNullOrEmpty(schema))
			{
				HasAnnotation(new ViewAttribute(name)
				{
					Schema = schema
				});
			}
			else
			{
				HasAnnotation(new ViewAttribute(name));
			}
		}

		public void HasAnnotation(object annotation)
		{
			_entityType.HasAnnotation(annotation);
		}

		protected MemberInfo GetMember(Expression expression)
		{
			if (expression is LambdaExpression lambdaExpression)
			{
				return GetMember(lambdaExpression.Body);
			}
			if (expression is MemberExpression memberExpression)
			{
				return memberExpression.Member;
			}
			throw new NotImplementedException();
		}

		protected MemberInfo[] GetMembers(Expression expression)
		{
			if (expression is LambdaExpression lambdaExpression)
			{
				return GetMembers(lambdaExpression.Body);
			}
			if (expression is NewExpression newExpression)
			{
				return newExpression.Arguments
					.OfType<MemberExpression>()
					.Select(s => s.Member)
					.ToArray();
			}
			if (expression is MemberExpression memberExpression)
			{
				return new MemberInfo[]
				{
					memberExpression.Member
				};
			}
			throw new NotImplementedException();
		}

		protected MemberInfo GetMember(string propertyName)
		{
			var member = _entityType.Type.GetProperties()
				.Where(a => a.Name == propertyName)
				.FirstOrDefault();
			if (member == null)
			{
				throw new ModelException(string.Format("Unable to find member in {0}", _entityType.Type.Name));
			}
			return member;
		}

		protected IEntityPropertyType GetProperty(MemberInfo member)
		{
			if (member is PropertyInfo property)
			{
				return _entityType.GetProperty(property);
			}
			throw new InvalidCastException();
		}
	}

	public class EntityTypeBuilder<T> : EntityTypeBuilder
		where T : class
	{
		internal EntityTypeBuilder(IEntityType target) 
			: base(target)
		{
		}

		public void HasKey<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			HasKey(GetMembers(expression));
		}

		public EntityPropertyBuilder<T> Property<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return new EntityPropertyBuilder<T>(GetProperty(GetMember(expression)));
		}

		public void Igonre<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			var members = GetMembers(expression);
			Igonre(members);
		}
	}
}
