using System;
using System.Collections;
using System.Collections.Generic;

namespace Soul.SqlBatis
{
    /// <summary>
    /// A dynamic parameters collection that implements IDictionary.
    /// </summary>
    public class DynamicParameters : IDictionary<string, object>
    {
        private readonly IDictionary<string, object> _items = new Dictionary<string, object>();

        public int Count => _items.Count;

        public ICollection<string> Keys => _items.Keys;

        public ICollection<object> Values => _items.Values;

        public bool IsReadOnly => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicParameters"/> class.
        /// </summary>
        public DynamicParameters() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicParameters"/> class with a single parameter.
        /// </summary>
        /// <param name="param">The parameter to add.</param>
        public DynamicParameters(object param)
        {
            Add(param);
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public object this[string key]
        {
            get => _items[key];
            set => _items[key] = value;
        }

        /// <summary>
        /// Adds a key-value pair to the collection.
        /// </summary>
        /// <param name="name">The key of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public void Add(string name, object value)
        {
            _items.Add(name, value);
        }

        /// <summary>
        /// Adds parameters from an object.
        /// </summary>
        /// <param name="param">The object containing parameters.</param>
        public void Add(object param)
        {
            if (param == null) return;

            var mapper = ObjectMapper.CreateMapper(param.GetType());
            var values = mapper(param);
            foreach (var item in values)
            {
                Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the parameter.</param>
        /// <returns>The value associated with the specified key.</returns>
        public object Get(string name)
        {
            if (_items.TryGetValue(name, out var value))
            {
                return value;
            }
            throw new KeyNotFoundException($"Parameter with name '{name}' not found.");
        }

        public bool ContainsKey(string key) => _items.ContainsKey(key);

        public bool Remove(string key) => _items.Remove(key);

        public bool TryGetValue(string key, out object value) => _items.TryGetValue(key, out value);

        public void Add(KeyValuePair<string, object> item) => _items.Add(item);

        public void Clear() => _items.Clear();

        public bool Contains(KeyValuePair<string, object> item) => _items.Contains(item);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, object> item) => _items.Remove(item);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        /// <summary>
        /// Merges another DynamicParameters instance into this instance.
        /// </summary>
        /// <param name="parameters">The DynamicParameters instance to merge.</param>
        public void Merge(DynamicParameters parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            foreach (var item in parameters)
            {
                this[item.Key] = item.Value; // Overwrites existing keys
            }
        }

        /// <summary>
        /// Gets all parameter names as a list.
        /// </summary>
        /// <returns>A list of parameter names.</returns>
        public List<string> GetParameterNames()
        {
            return new List<string>(_items.Keys);
        }

        /// <summary>
        /// Gets all parameter values as a list.
        /// </summary>
        /// <returns>A list of parameter values.</returns>
        public List<object> GetParameterValues()
        {
            return new List<object>(_items.Values);
        }
    }
}
