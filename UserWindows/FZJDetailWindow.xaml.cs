using BMHRI.WCS.Server.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// SSJDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FZJDetailWindow : Window
    {
        public string DeviceID;
        private FZJDevice fZJDevice;

        public FZJDetailWindow()
        {
            InitializeComponent();
        }

        public FZJDetailWindow(string device_id)
        {
            InitializeComponent();
            DeviceID = device_id;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DeviceID)) return;
            fZJDevice = PLCDeviceManager.Instance.FZJDeviceList.Find(x => x.PLCID == DeviceID);
            if (fZJDevice != null)
            {
                DataContext = fZJDevice;
                Title = fZJDevice.PLCDecription;
            }
        }
    }
}
