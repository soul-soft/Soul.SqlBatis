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
		private readonly List<object> _annotations = new List<object>();

		public T Get<T>()
		{
			return _annotations.OfType<T>().FirstOrDefault();
		}

		public IEnumerator<object> GetEnumerator()
		{
			return _annotations.GetEnumerator();
		}

		public void Set(object value)
		{
			var type = value.GetType();
			var old = _annotations
				.Where(a => a.GetType() == type)
				.FirstOrDefault();
			if (old != null)
			{
				_annotations.Remove(old);
			}
			_annotations.Add(value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
