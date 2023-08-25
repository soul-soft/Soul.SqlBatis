using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Soul.SqlBatis.Test
{
    public class QueryModel
    {
        public OrderModel Orders { get; set; } = new OrderModel();
    }

    public class OrderModel : IDictionary<string, int>
    {
        private Dictionary<string, int> _orders = new Dictionary<string, int>();

        public int this[string key] { get => ((IDictionary<string, int>)_orders)[key]; set => ((IDictionary<string, int>)_orders)[key] = value; }

        public IDictionary<string, int> Orders => _orders;

        public ICollection<string> Keys => ((IDictionary<string, int>)_orders).Keys;

        public ICollection<int> Values => ((IDictionary<string, int>)_orders).Values;

        public int Count => ((ICollection<KeyValuePair<string, int>>)_orders).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<string, int>>)_orders).IsReadOnly;

        public bool IsAsc(string name)
        {
            if (!_orders.Keys.Any(a=>a.ToUpper() == name))
            {
                return false;
            }
            var value = _orders.Where(a => a.Key.ToUpper() == name).First().Value;
            if (value == 0)
            {
                return true;
            }
            return false;
        }
        public bool IsDesc(string name)
        {
            if (!_orders.Keys.Any(a => a.ToUpper() == name))
            {
                return false;
            }
            var value = _orders.Where(a => a.Key.ToUpper() == name).First().Value;
            if (value == 1)
            {
                return true;
            }
            return false;
        }

        public void Add(string key, int value)
        {
            ((IDictionary<string, int>)_orders).Add(key, value);
        }

        public void Add(KeyValuePair<string, int> item)
        {
            ((ICollection<KeyValuePair<string, int>>)_orders).Add(item);
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<string, int>>)_orders).Clear();
        }

        public bool Contains(KeyValuePair<string, int> item)
        {
            return ((ICollection<KeyValuePair<string, int>>)_orders).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, int>)_orders).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, int>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, int>>)_orders).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, int>>)_orders).GetEnumerator();
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, int>)_orders).Remove(key);
        }

        public bool Remove(KeyValuePair<string, int> item)
        {
            return ((ICollection<KeyValuePair<string, int>>)_orders).Remove(item);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out int value)
        {
            return ((IDictionary<string, int>)_orders).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_orders).GetEnumerator();
        }
    }
}
