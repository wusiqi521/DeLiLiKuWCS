using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BMHRI.WCS.Server.Models;

namespace BMHRI.WCS.Server.ValueConverter
{
    public class DirectionToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将 DeviceBlockMotionDirection 枚举值转换为 Visibility。
        /// </summary>
        /// <param name="value">绑定的 DeviceBlockMotionDirection 值。</param>
        /// <param name="targetType">目标绑定类型。</param>
        /// <param name="parameter">转换参数，指定要检查的方向（如 "Left", "Up", "Right", "Down"）。</param>
        /// <param name="culture">文化信息。</param>
        /// <returns>如果 MotionDirection 与参数匹配，则返回 Visible，否则返回 Collapsed。</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            if (!(value is DeviceBlockMotionDirection direction))
                return Visibility.Collapsed;

            if (!Enum.TryParse<DeviceBlockMotionDirection>(parameter.ToString(), out DeviceBlockMotionDirection directionToCheck))
                return Visibility.Collapsed;

            return direction == directionToCheck ? Visibility.Visible : Visibility.Collapsed;
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
