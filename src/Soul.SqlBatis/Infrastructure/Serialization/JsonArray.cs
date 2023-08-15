using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Soul.SqlBatis
{
	[JsonValue]
	public class JsonArray<T> : ICollection<T>, INotifyCollectionChanged
	{
		private readonly List<T> _array = new List<T>();

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public int Count => ((ICollection<T>)_array).Count;

		public bool IsReadOnly => ((ICollection<T>)_array).IsReadOnly;

		private void NotifyCollectionChanged(NotifyCollectionChangedAction changedAction)
		{
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(changedAction));
		}

		public void Add(T item)
		{
			NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
			((ICollection<T>)_array).Add(item);
		}

		public void Clear()
		{
			NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
			((ICollection<T>)_array).Clear();
		}

		public bool Contains(T item)
		{
			return ((ICollection<T>)_array).Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			((ICollection<T>)_array).CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)_array).GetEnumerator();
		}

		public bool Remove(T item)
		{
			NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
			return ((ICollection<T>)_array).Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_array).GetEnumerator();
		}
	}
}
