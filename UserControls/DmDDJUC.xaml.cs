using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.UserWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// DmDDJUC.xaml 的交互逻辑
    /// </summary>
    public partial class DmDDJUC : UserControl
    {
        public DeMaticDDJ dDJDevice;
        public DmDDJUC()
        {
            InitializeComponent();
        }
        public DmDDJUC(DeMaticDDJ ddjDevice)
        {
            InitializeComponent();
            dDJDevice = ddjDevice;
            DataContext = dDJDevice;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            
            switch (menuItem.Header)
            {
                case "联机":
                    if (dDJDevice != null)
                    {
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
                        dDJDevice.ConnectToDematicTcpServer();
                    }
                    break;
                case "一楼召回":
                    if (dDJDevice != null)
                    {
                        //dDJDevice.RecallInPlace("000a");
                    }
                    break;
                case "二楼召回":

                    if (dDJDevice != null)
                    {
                        //dDJDevice.RecallInPlace("000b");
                    }
                    break;
                case "四楼召回":

                    if (dDJDevice != null)
                    {
                        //dDJDevice.RecallInPlace("000c");
                    }
                    break;
                case "设备属性":
                    DmDDJDetailWindow dDJDetailForm = new DmDDJDetailWindow(dDJDevice);
                    dDJDetailForm.ShowDialog();
                    break;
                default:
                    break;
            }
        }

        private void UserControl_Loaded()
        {
            if (dDJDevice != null)
            {
                DataContext = dDJDevice;
            }
        }

        private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DmDDJDetailWindow dDJDetailForm = new DmDDJDetailWindow(dDJDevice);
            dDJDetailForm.ShowDialog();
        }
    }
}
