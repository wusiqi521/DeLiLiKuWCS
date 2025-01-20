using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.UserWindows;
using System.Windows;
using System.Windows.Controls;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// DDJUC.xaml 的交互逻辑
    /// </summary>
    public partial class DDJUC : UserControl
    {
        public DDJDevice dDJDevice;
        public string DeviceNum { get; set; }
        public DDJUC()
        {
            InitializeComponent();
        }

        public DDJUC(DDJDevice dJDevice)
        {
            InitializeComponent();
            this.dDJDevice = dJDevice;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
           
            switch (menuItem.Header)
            {
                case "联机":
                    //dDJDevice = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == DeviceNum);
                    if (dDJDevice != null)
                    {
                        dDJDevice.ReadyIntoTunnel = false;
                        dDJDevice.Online();
                    }
                    break;
                case "脱机":
                    
                    if (dDJDevice != null)
                    {
                        dDJDevice.OffLine();
                    }
                    break;

                case "故障解除":
                    
                    if (dDJDevice != null)
                    {
                        dDJDevice.RecallInPlace();
                    }                   
                    break;

                case "紧急停车":
                    
                    break;
                case "断开连接":
                    if (dDJDevice != null)
                    {
                        dDJDevice.Disconnect();
                    }
                    break;
                case "连接设备":
                    if (dDJDevice != null)
                    {
                        dDJDevice.AutoConnect = true;
                    }
                    break;
                //case "一楼召回":
                //    if (dDJDevice != null)
                //    {
                //        dDJDevice.RecallInPlace("000a");
                //    }
                //    break;
                //case "二楼召回":
                    
                //    if (dDJDevice != null)
                //    {
                //        dDJDevice.RecallInPlace("000b");
                //    }
                //    break;
                //case "四楼召回":
                    
                //    if (dDJDevice != null)
                //    {
                //        dDJDevice.RecallInPlace("000c");
                //    }
                //    break;
                case "设备属性":
                    DDJDetailWindow dDJDetailForm = new DDJDetailWindow(dDJDevice);
                    dDJDetailForm.ShowDialog();
                    break;
                default:
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
            {
                DataContext = dDJDevice;
            }
        }

        private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DDJDetailWindow dDJDetailForm = new DDJDetailWindow(dDJDevice);
            dDJDetailForm.ShowDialog();
        }
    }
}
