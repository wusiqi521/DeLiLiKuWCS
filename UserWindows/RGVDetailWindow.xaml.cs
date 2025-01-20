using BMHRI.WCS.Server.Models;
using System.Windows;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// RGVDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RGVDetailWindow : Window
    {
        public RGVDetailWindow()
        {
            InitializeComponent();
        }
        public RGVDetailWindow(RGVDevice rGVDevice)
        {
            InitializeComponent();
            if (rGVDevice == null) return;
            Title = "小车" + rGVDevice.DeviceID + "状态";
            DataContext = rGVDevice;
            SizeToContent = SizeToContent.Height;
        }
    }
}
