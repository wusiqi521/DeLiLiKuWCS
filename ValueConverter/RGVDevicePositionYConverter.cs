using System;
using System.Globalization;
using System.Windows.Data;

namespace BMHRI.WCS.Server.ValueConverter
{
    class RGVDevicePositionYConverter : IValueConverter
    {
        //ScaleY,offsety
        double ScaleY;
        double OffsetY;
        public RGVDevicePositionYConverter(double scaley, double offsety)
        {
            ScaleY = scaley;
            OffsetY = offsety;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double p = (int)value / 10 * ScaleY + OffsetY - 13;
            return p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
