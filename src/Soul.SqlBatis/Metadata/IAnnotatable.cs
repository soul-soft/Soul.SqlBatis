using System.Collections.Generic;

namespace Soul.SqlBatis.Metadata
{
    public interface IAnnotatable
    {
        string FindAnnotation(string name);
    }

    public abstract class Annotatable: IAnnotatable
    {
        private readonly Dictionary<string, string> _items = new Dictionary<string, string>();

        public string FindAnnotation(string name)
        {
            _items.TryGetValue(name, out var value);

            return value;
        }

        public void SetAnnotation(string name, string value)
        {
            _items[name] = value;
        }
    }
}
