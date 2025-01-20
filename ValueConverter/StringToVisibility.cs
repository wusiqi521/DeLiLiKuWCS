using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BMHRI.WCS.Server.ValueConverter
{
    class StringToVisibility : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return Visibility.Hidden;
                //return "Hidden";
            }
            else
            {
                return Visibility.Visible;
                //return "Visible";
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
