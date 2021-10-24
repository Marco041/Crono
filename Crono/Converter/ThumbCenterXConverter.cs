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
    /// Change width of the phase central thumb. When is under 22 pixel reduce his width to prevent overlap with right or left thumb.
    /// </summary>
    class ThumbCenterXConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((double)value >=22)
                return (-(double)value / 2) + 6;
            return (-(double)value / 2) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
