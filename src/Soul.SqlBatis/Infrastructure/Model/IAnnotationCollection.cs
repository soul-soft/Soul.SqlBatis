using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
	public interface IAnnotationCollection
	{
		T Get<T>();
		void Set(object value);
	}

	public class AnnotationCollection : IAnnotationCollection
	{
		private readonly List<object> _metadata = new List<object>();

		public T Get<T>()
		{
			return _metadata.Cast<T>().FirstOrDefault();
		}


		public void Set(object value)
		{
			var type = value.GetType();
			if (_metadata.Any(a => a.GetType() == type))
			{
				var old = _metadata.Where(a => a.GetType() == type).First();
				_metadata.Remove(old);
			}
			_metadata.Add(value);
		}
	}
}
