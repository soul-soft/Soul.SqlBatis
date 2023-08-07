using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
	public interface IAttributeCollection : IEnumerable<object>
	{
		T Get<T>();
		void Set(object value);
	}

	public class AttributeCollection : IAttributeCollection
	{
		private readonly List<object> _attributes = new List<object>();

		public AttributeCollection(IEnumerable<object> attributes)
		{
			foreach (var item in attributes)
			{
				Set(item);
			}
		}

		public T Get<T>()
		{
			return _attributes.OfType<T>().FirstOrDefault();
		}

		public IEnumerator<object> GetEnumerator()
		{
			return _attributes.GetEnumerator();
		}

		public void Set(object value)
		{
			var type = value.GetType();
			var old = _attributes
				.Where(a => a.GetType() == type)
				.FirstOrDefault();
			if (old != null)
			{
				_attributes.Remove(old);
			}
			_attributes.Add(value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
