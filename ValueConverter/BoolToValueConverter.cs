using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BMHRI.WCS.Server.ValueConverter
{
    class BoolToValueConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return parameter;
            else
                return DependencyProperty.UnsetValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return object.Equals(value, parameter);
        }
        #endregion
    }
}
