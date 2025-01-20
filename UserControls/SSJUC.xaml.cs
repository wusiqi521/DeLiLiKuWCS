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
    public partial class SSJUC : UserControl
    {
        public string DeviceNum { get; set; }
        public SSJUC()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            switch (menuItem.Header)
            {
                case "联机":
                    PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == DeviceNum).Online();
                    break;
                case "脱机":
                    PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == DeviceNum).OffLine();
                    break;
                case "连接网络":
                    PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == DeviceNum).Connect();
                    break;
                case "断开网络":
                    PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == DeviceNum).Disconnect();
                    break;
                case "设备状态":
                    SSJDetailWindow sSJDetailWindow = new SSJDetailWindow(DeviceNum);
                    sSJDetailWindow.ShowDialog();
                    break;
                case "变量列表":
                    //CreateTagListForm();
                    break;
                default:
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DeviceNum)) return;

            SSJDevice ssj_device = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == DeviceNum);
            if (ssj_device != null)
            {
                DataContext = ssj_device;
            }
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SSJDetailWindow sSJDetailWindow = new SSJDetailWindow(DeviceNum);
            sSJDetailWindow.ShowDialog();
        }

        //private void CreateDeviceDetailForm()
        //{
        //    if (deviceDetailForm == null)
        //    {
        //        deviceDetailForm = new SSJDetailWindow(DeviceNum);
        //        deviceDetailForm.Closed += DeviceDetailForm_Closed;
        //        deviceDetailForm.Show();
        //    }

        //    deviceDetailForm.Activate();
        //    //deviceDetailForm.Topmost = true;
        //}

        //private void CreateTagListForm()
        //{
        //    if (plcTagListForm == null)
        //    {
        //        plcTagListForm = new PLCTagListWindow(DeviceNum);
        //        plcTagListForm.Closed += PLCTagListForm_Closed;
        //        plcTagListForm.Show();
        //    }

        //    plcTagListForm.Activate();
        //}

        //private void PLCTagListForm_Closed(object sender, EventArgs e)
        //{
        //    plcTagListForm = null;
        //}

        //private void DeviceDetailForm_Closed(object sender, System.EventArgs e)
        //{
        //    deviceDetailForm = null;
        //}
    }
}
