using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BMHRI.WCS.Server.ValueConverter
{
    public class MultiplyConverter : IValueConverter
    {
        /// <summary>
        /// 将输入值与转换参数相乘。
        /// </summary>
        /// <param name="value">绑定的源值（例如，ActualWidth 或 ActualHeight）。</param>
        /// <param name="targetType">目标绑定类型。</param>
        /// <param name="parameter">转换参数（比例因子，如 0.1 表示 10%。</param>
        /// <param name="culture">文化信息。</param>
        /// <returns>计算后的值，或 DependencyProperty.UnsetValue 如果转换失败。</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return DependencyProperty.UnsetValue;

            try
            {
                double input = System.Convert.ToDouble(value);
                double factor = System.Convert.ToDouble(parameter);
                return input * factor;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        /// <summary>
        /// 不实现反向转换。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
