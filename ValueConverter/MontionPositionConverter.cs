using System;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Windows.Data;

namespace BMHRI.WCS.Server.ValueConverter
{
    class XMontionPositionConverter : IValueConverter
    {
        double MaxP, MinP;
        public XMontionPositionConverter(double maxp, double minp)
        {
            MaxP = maxp;
            MinP = minp;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //int left = 850570, right = 9209;
            int left = 478091, right = 10473;
            double p = (int)value;
            if (p > left)
                return (MinP);
            else if (p < right)
                return MaxP;
            else
                return (p - left) / (right - left) * (MaxP - MinP) + MinP;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class XDmMontionPositionConverter : IValueConverter
    {
        double MaxP, MinP;
        string Warehouse;
        public XDmMontionPositionConverter(double maxp, double minp,string warehouse)
        {
            MaxP = maxp;
            MinP = minp;
            Warehouse = warehouse;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int left, right;
            if (Warehouse == "1")
            {
                left = 682072; right = 16549;
            }
            else 
            {
                left = 681905; right = 17108;
            }
            
            double p = (int)value;
            if (p > left)
                return (MinP);
            else if (p < right)
                return MaxP;
            else
                return (p - left) / (right - left) * (MaxP - MinP) + MinP;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class YMontionPositionConverter : IValueConverter
    {
        double MaxP, MinP;
        string Warehouse;
        public YMontionPositionConverter(double maxp, double minp,string warehouse)
        {
            MaxP = maxp;
            MinP = minp;
            Warehouse = warehouse;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int bottom, top;
            if (Warehouse == "2") 
            {
                bottom = 681905;
                top = 17108;
            }
            else 
            {
                bottom = 436805;
                top = 10626; 
            }
            double p = (int)value;
            if (p > bottom)
                return (MinP);
            else if (p < top)
                return MaxP;
            else
                return (p - bottom) / (top - bottom) * (MaxP - MinP) + MinP;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class YDMMontionPositionConverter : IValueConverter
    {
        double MaxP, MinP;
        public YDMMontionPositionConverter(double maxp, double minp)
        {
            MaxP = maxp;
            MinP = minp;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int bottom = 170505, top = 9751;
            double p = (int)value;
            if (p > bottom)
                return (MinP);
            else if (p < top)
                return MaxP;
            else
                return (p - bottom) / (top - bottom) * (MaxP - MinP) + MinP;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class YRGVMontionPositionConverter : IValueConverter
    {
        double MaxP, MinP;
        string RGV;
        public YRGVMontionPositionConverter(double minp, double maxp, string rgv)
        {
            MaxP = maxp;
            MinP = minp;
            RGV = rgv;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int bottom, top;
            if (RGV == "car11" || RGV == "car21")
            {
                bottom = 65000;
                top = 5000;
            }
            else
            {
                bottom = 66666;
                top = 3400;
            }
            double p = (int)value;
            if (p < top)
                return (MinP);
            else if (p > bottom)
                return MaxP;
            else
                return MaxP - (bottom - p) / (bottom - top) * (MaxP - MinP);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class XRGVMontionPositionConverter : IValueConverter
    {
        double MaxP, MinP;
        public XRGVMontionPositionConverter(double maxp, double minp)
        {
            MaxP = maxp;
            MinP = minp;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int bottom = 170505, top = 9751;
            double p = (int)value;
            if (p > bottom)
                return (MinP);
            else if (p < top)
                return MaxP;
            else
                return (p - bottom) / (top - bottom) * (MaxP - MinP) + MinP;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
