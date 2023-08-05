using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityTypeBuilder
	{
		private Type _type;

		private IAnnotationCollection _annotations;

		private ConcurrentDictionary<MemberInfo, EntityPropertyBuilder> _propertyBuilders;

		public EntityTypeBuilder(Type type)
		{
			_type = type;
			_annotations = new AnnotationCollection();
			_propertyBuilders = new ConcurrentDictionary<MemberInfo, EntityPropertyBuilder>();
		}

		public EntityTypeBuilder(EntityTypeBuilder target)
		{
			_type = target._type;
			_annotations = target._annotations;
			_propertyBuilders = target._propertyBuilders;
		}

		public void Ignore()
		{
			HasAnnotation(new NotMappedAttribute());
		}

		public void Igonre(params string[] propertyNames)
		{
			foreach (var item in propertyNames)
			{
				Property(item).HasAnnotation(new NotMappedAttribute());
			}
		}

		public void Igonre(params MemberInfo[] members)
		{
			foreach (var item in members)
			{
				Property(item).HasAnnotation(new NotMappedAttribute());
			}
		}

		public EntityPropertyBuilder Property(string propertyName)
		{
			return GetEntityPropertyBuilder(GetMember(propertyName));
		}

		public EntityPropertyBuilder Property(MemberInfo member)
		{
			return GetEntityPropertyBuilder(member);
		}

		public void HasKey(params string[] propertyNames)
		{
			var members = _type.GetProperties()
				.Where(a => propertyNames.Contains(a.Name))
				.ToArray();
			HasKey(members);
		}

		public void HasKey(params MemberInfo[] members)
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

		public void ToView(string name, string scheme = null)
		{
			HasAnnotation(new ViewAttribute(name)
			{
				Schema = scheme
			});
		}

		public void HasAnnotation(object annotation)
		{
			_annotations.Set(annotation);
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
				return newExpression.Members.ToArray();
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
			var member = _type.GetProperties()
				.Where(a => a.Name == propertyName)
				.FirstOrDefault();
			if (member == null)
			{
				throw new ModelException(string.Format("Unable to find member in {0}", _type.Name));
			}
			return member;
		}

		private EntityPropertyBuilder GetEntityPropertyBuilder(MemberInfo member)
		{
			return _propertyBuilders.GetOrAdd(member, key =>
			{
				return new EntityPropertyBuilder(key);
			});
		}

		public EntityType Build()
		{
			var properties = _propertyBuilders.Values
				.Select(s => s.Build())
				.ToList();
			foreach (var item in _type.GetProperties())
			{
				if (!properties.Any(a => a.Member == item))
				{
					properties.Add(new EntityProperty(item));
				}
			}
			return new EntityType(_type, _annotations, properties);
		}
	}

	public class EntityTypeBuilder<T> : EntityTypeBuilder
		where T : class
	{
		private EntityTypeBuilder _target;

		public EntityTypeBuilder(EntityTypeBuilder target) : base(target)
		{
			_target = target;
		}

		public void HasKey<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			_target.HasKey(GetMembers(expression));
		}

		public EntityPropertyBuilder<T> Property<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			var member = GetMember(expression);
			return new EntityPropertyBuilder<T>(_target.Property(member));
		}


		public void Igonre<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			var members = GetMembers(expression);
			_target.Igonre(members);
		}
	}
}
