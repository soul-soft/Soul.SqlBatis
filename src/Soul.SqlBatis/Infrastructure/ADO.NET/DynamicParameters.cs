using System.Collections.Generic;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis
{
    public class DynamicParameters
    {
        private Dictionary<string, object> _values = new Dictionary<string, object>();

        public IReadOnlyDictionary<string, object> Parameters => _values;

        public DynamicParameters()
        {

        }

        public void Add(object model)
        {
            if (model == null)
            {
                return;
            }
            var func = TypeMapper.CreateDeserializer(model.GetType());
            var values = func(model);
            foreach (var item in values)
            {
                _values.Add(item.Key, item.Value);
            }
        }

        public void Add(string name, object value)
        {
            _values.Add(name, value);
        }

        internal string AddAnonymous(object value)
        {
            var name = $"P_{_values.Count}";
            _values.Add(name, value);
            return name;
        }

        internal Dictionary<string, object> ToDictionary()
        {
            return _values;
        }
    }
}
