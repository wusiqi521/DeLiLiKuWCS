using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.UserWindows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// SSJUC.xaml 的交互逻辑
    /// </summary>
    public partial class FZJUC : UserControl
    {
        public string DeviceNum { get; set; }
        public FZJUC()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DeviceNum)) return;

            FZJDevice fzj_device = PLCDeviceManager.Instance.FZJDeviceList.Find(x => x.PLCID == DeviceNum);
            if (fzj_device != null)
            {
                DataContext = fzj_device;
            }
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FZJDetailWindow fzjdetailwindow = new FZJDetailWindow(DeviceNum);
            fzjdetailwindow.ShowDialog();
        }
    }
}
