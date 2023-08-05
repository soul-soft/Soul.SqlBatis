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
		private readonly List<object> _annotations = new List<object>();

		public T Get<T>()
		{
			return _annotations.OfType<T>().FirstOrDefault();
		}


		public void Set(object value)
		{
			var type = value.GetType();
			if (_annotations.Any(a => a.GetType() == type))
			{
				var old = _annotations.Where(a => a.GetType() == type).First();
				_annotations.Remove(old);
			}
			_annotations.Add(value);
		}
	}
}
