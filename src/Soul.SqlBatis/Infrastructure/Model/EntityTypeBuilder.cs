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

		public virtual IAnnotationCollection Annotations { get; }

		public ConcurrentDictionary<MemberInfo, EntityPropertyBuilder> PropertyBuilders { get; }

		public EntityTypeBuilder(Type type)
		{
			Type = type;
			Annotations = new AnnotationCollection();
			PropertyBuilders = new ConcurrentDictionary<MemberInfo, EntityPropertyBuilder>();
		}

		public void Ignore()
		{
			HasAnnotation(new NotMappedAttribute());
		}

		public EntityPropertyBuilder Property(string property)
		{
			return GetEntityPropertyBuilder(GetMember(property));
		}

		public EntityPropertyBuilder Property(MemberInfo member)
		{
			return GetEntityPropertyBuilder(member);
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
				return GetMember(lambdaExpression.Body);
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

		private EntityPropertyBuilder GetEntityPropertyBuilder(MemberInfo member)
		{
			return PropertyBuilders.GetOrAdd(member, key =>
			{
				return new EntityPropertyBuilder(key);
			});
		}

		public EntityType Build()
		{
			var properties = PropertyBuilders.Values
				.Select(s => s.Build())
				.ToList();
			foreach (var item in Type.GetProperties())
			{
				if (!properties.Any(a => a.Member == item))
				{
					properties.Add(new EntityProperty(item));
				}
			}
			return new EntityType(Type, Annotations, properties);
		}
	}

	public class EntityTypeBuilder<T> : EntityTypeBuilder
		where T : class
	{
		private EntityTypeBuilder _target;

		public override IAnnotationCollection Annotations => _target.Annotations;

		public EntityTypeBuilder(EntityTypeBuilder target) : base(typeof(T))
		{
			_target = target;
		}

		public void Ignore<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			_target.Property(GetMember(expression))
				.HasAnnotation(new NotMappedAttribute());
		}

		public void HasKey<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			_target.HasAnnotation(new NotMappedAttribute());
		}

		public EntityPropertyBuilder<T> Property<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			var member = GetMember(expression);
			return new EntityPropertyBuilder<T>(_target.Property(member));
		}
	}
}
