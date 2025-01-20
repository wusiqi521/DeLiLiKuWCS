using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BMHRI.WCS.Server.ValueConverter
{
    class F20ToValueConverter : IValueConverter
    {
        //空出库
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return DependencyProperty.UnsetValue;
            string faultcode = value.ToString();
            if (faultcode == "F20")
            {
                return parameter;
            }
            else
                return DependencyProperty.UnsetValue;

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return object.Equals(value, parameter);
        }
        #endregion
    }

    class F21ToValueConverter : IValueConverter
    {
        //双重确认
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return DependencyProperty.UnsetValue;
            string faultcode = value.ToString();
            if (faultcode == "F21")
            {
                return parameter;
            }
            else
                return DependencyProperty.UnsetValue;

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return object.Equals(value, parameter);
        }
        #endregion
    }
    class F19ToValueConverter : IValueConverter
    {
        //双重确认
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return DependencyProperty.UnsetValue;
            string faultcode = value.ToString();
            if (faultcode == "F19")
            {
                return parameter;
            }
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
