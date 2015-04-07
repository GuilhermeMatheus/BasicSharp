using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BasicSharp.IDE.Helpers
{
    /// <summary>
    /// Converter que filtra os itens nulos de uma Collection qualquer
    /// </summary>
    public class NullSafeCollectionConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var enumerable = value as IEnumerable;
            if (enumerable == null)
                return value;

            return from item in enumerable.Cast<object>()
                   where item != null
                   select item;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
