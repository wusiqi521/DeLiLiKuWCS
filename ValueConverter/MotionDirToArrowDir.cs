using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BMHRI.WCS.Server.ValueConverter
{
    class MotionDirToArrowDir : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FlowDirection flowDirection = FlowDirection.RightToLeft;
            switch (value.ToString())
            {
                case "None":
                case "Left":
                    break;
                case "Up":
                    break;
                case "Right":
                    flowDirection = FlowDirection.LeftToRight;
                    break;
                case "Down":
                    break;
                default:
                    break;
            }
            return flowDirection;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
