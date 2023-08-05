using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityTypeBuilder
	{
		public Type Type { get; }

		public IAnnotationCollection Annotations { get; }

		public ConcurrentDictionary<MemberInfo, EntityPropertyBuilder> PropertyBuilders { get; }

		public EntityTypeBuilder(Type type)
		{
			Type = type;
			Annotations = new AnnotationCollection();
			PropertyBuilders = new ConcurrentDictionary<MemberInfo, EntityPropertyBuilder>();
		}

		public void Ignore(string propertyName)
		{
			Property(GetMember(propertyName))
				.HasAnnotation(new NotMappedAttribute());
		}

		public EntityPropertyBuilder Property(string property)
		{
			return BuilderEntityProperty(GetMember(property));
		}

		public EntityPropertyBuilder Property(MemberInfo member)
		{
			return BuilderEntityProperty(member);
		}

		public void ToTable(string name, string scheme = null)
		{
			HasAnnotation(new TableAttribute(name)
			{
				Schema = scheme
			});
		}

		public void ToView(string name, string scheme = null)
		{
			HasAnnotation(new ViewAttribute(name)
			{
				Schema = scheme
			});
		}

		public void HasAnnotation(object annotation)
		{
			Annotations.Set(annotation);
		}

		protected MemberInfo GetMember(Expression expression)
		{
			if (expression is LambdaExpression lambdaExpression)
			{
				GetMember(lambdaExpression.Body);
			}
			if (expression is MemberExpression memberExpression)
			{
				return memberExpression.Member;
			}
			throw new NotImplementedException();
		}

		protected MemberInfo GetMember(string propertyName)
		{
			var member = Type.GetProperties()
				.Where(a => a.Name == propertyName)
				.FirstOrDefault();
			if (member == null)
			{
				throw new ModelException(string.Format("Unable to find member in {0}", Type.Name));
			}
			return member;
		}

		private EntityPropertyBuilder BuilderEntityProperty(MemberInfo member)
		{
			return PropertyBuilders.GetOrAdd(member, key =>
			{
				return new EntityPropertyBuilder(key);
			});
		}
	}

	public class EntityTypeBuilder<T> : EntityTypeBuilder
		where T : class
	{
		private EntityTypeBuilder _target;

		public EntityTypeBuilder(EntityTypeBuilder target) : base(typeof(T))
		{
			_target = target;
		}

		public void Ignore<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			_target.Property(GetMember(expression)).HasAnnotation(new NotMappedAttribute());
		}

		public void HasKey<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			throw new NotImplementedException();
		}

		public EntityPropertyBuilder<T> Property<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return new EntityPropertyBuilder<T>(Property(GetMember(expression)));
		}
	}
}
