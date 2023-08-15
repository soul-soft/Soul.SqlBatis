using System;
using System.Collections.Generic;
using System.Text;

namespace Soul.SqlBatis
{
    public interface IValueConverter
    {
        T ToDbValue<T>();
        void FromDbValue();
    }
}
