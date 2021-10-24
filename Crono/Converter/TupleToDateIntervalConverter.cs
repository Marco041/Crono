using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Crono.Converter
{
    class TupleToDateIntervalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            if(value != null && value != "")
                return ((Tuple<DateTime, DateTime>)value).Item1.ToString("dd/MM/yyyy") + " - " + ((Tuple<DateTime, DateTime>)value).Item2.ToString("dd/MM/yyyy");
            return null;
        }
        

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
