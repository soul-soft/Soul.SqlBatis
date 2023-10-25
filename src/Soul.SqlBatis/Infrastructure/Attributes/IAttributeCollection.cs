using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    public class AttributeCollection : IEnumerable<object>
    {
        private readonly List<object> _metadata = new List<object>();

        public IReadOnlyCollection<object> Metadata => _metadata;

        public AttributeCollection(IEnumerable<object> attributes)
        {
            foreach (var item in attributes)
            {
                Set(item);
            }
        }

        public T Get<T>()
        {
            return _metadata.OfType<T>().FirstOrDefault();
        }

        public bool Any<T>()
        {
            return _metadata.OfType<T>().Any();
        }

        public IEnumerator<object> GetEnumerator()
        {
            return _metadata.GetEnumerator();
        }

        public void Remove(Type type)
        {
            var atrtribute = _metadata.Where(a => a.GetType() == type).FirstOrDefault();
            if (atrtribute != null)
            {
                _metadata.Remove(atrtribute);
            }
        }

        public void Set(object value)
        {
            var type = value.GetType();
            var old = _metadata
                .Where(a => a.GetType() == type)
                .FirstOrDefault();
            if (old != null)
            {
                _metadata.Remove(old);
            }
            _metadata.Add(value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
