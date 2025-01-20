using S7.Net.Types;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BMHRI.WCS.Server.ValueConverter
{
    class DataItemValueToString : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DataItem)value).Value;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
