using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Crono.Converter
{
    /// <summary>
    /// Left thumb is moved to left when width is under 22 pixel
    /// </summary>
    public class ThumbLeftConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((double)value>=22)
                return (-(double)value / 2) - 6;
            return (-(double)value / 2) - 10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
