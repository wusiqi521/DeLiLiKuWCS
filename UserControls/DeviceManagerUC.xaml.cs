using BMHRI.WCS.Server.Models;
using System.Windows;
using System.Windows.Controls;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// DeviceManagerUC.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceManagerUC : UserControl
    {
        public DeviceManagerUC()
        {
            InitializeComponent();
            DeviceManagerPanel_Loaded();
        }

        private void DeviceManagerPanel_Loaded()
        {
            foreach (SSJDevice ssj_device in PLCDeviceManager.Instance.SSJDeviceList)
            {
                SSJUC ssj_uc = new SSJUC();
                ssj_uc.DeviceName.Content = ssj_device.PLCDecription;
                ssj_uc.DeviceNum = ssj_device.PLCID;
                ssj_uc.Width = 120;
                ssj_uc.Height = 120;
                DeviceManagerPanel.Children.Add(ssj_uc);

            }
            foreach (DDJDevice ddj_device in PLCDeviceManager.Instance.DDJDeviceList)
            {
                DDJUC ddj_uc = new DDJUC(ddj_device);
                ddj_uc.DeviceName.Content = ddj_device.PLCDecription;
                //ddj_uc.DeviceNum = ddj_device.PLCID;
                ddj_uc.Width = 120;
                ddj_uc.Height = 120;
                DeviceManagerPanel.Children.Add(ddj_uc);
            }
            foreach (DeMaticDDJ ddj_device in DeMaticDDJManager.Instance.DeMaticDDJList)
            {
                DmDDJUC ddj_uc = new DmDDJUC(ddj_device);
                ddj_uc.DeviceName.Content = ddj_device.PLCDecription;
                //ddj_uc.DeviceNum = ddj_device.PLCID;
                ddj_uc.Width = 120;
                ddj_uc.Height = 120;
                DeviceManagerPanel.Children.Add(ddj_uc);
            }
            foreach (FZJDevice fzj_device in PLCDeviceManager.Instance.FZJDeviceList)
            {
                FZJUC fzj_uc = new FZJUC();
                fzj_uc.DeviceName.Content = fzj_device.PLCDecription;
                fzj_uc.DeviceNum = fzj_device.PLCID;
                fzj_uc.Width = 120;
                fzj_uc.Height = 120;
                DeviceManagerPanel.Children.Add(fzj_uc);
            }
            //foreach (Systems system in SystemManager.Instance.SystemList)
            //{
            //    WMSUC wMSUC = new WMSUC();
            //    wMSUC.DeviceName.Content = system.SystemType;
            //    wMSUC.SystemNum = system.ID;
            //    wMSUC.Width = 120;
            //    wMSUC.Height = 120;
            //    DeviceManagerPanel.Children.Add(wMSUC);
            //}
        }

        private void AllDeviceOnlineBT_Click(object sender, RoutedEventArgs e)
        {
            foreach (DDJDevice dDJDevice in PLCDeviceManager.Instance.DDJDeviceList)
            {
                if (dDJDevice.PLCConnectState == PLCDevice.ConnectionStates.Connected)
                {
                    dDJDevice.Online();
                }
            }
            foreach (SSJDevice sSJDevice in PLCDeviceManager.Instance.SSJDeviceList)
            {
                if (sSJDevice.PLCConnectState == PLCDevice.ConnectionStates.Connected)
                {
                    sSJDevice.Online();
                }
            }
        }

        private void AllDeviceOfflineBT_Click(object sender, RoutedEventArgs e)
        {
            foreach (DDJDevice dDJDevice in PLCDeviceManager.Instance.DDJDeviceList)
            {
                if (dDJDevice.PLCConnectState == PLCDevice.ConnectionStates.Connected)
                {
                    dDJDevice.OffLine();
                }
            }
            foreach (SSJDevice sSJDevice in PLCDeviceManager.Instance.SSJDeviceList)
            {
                if (sSJDevice.PLCConnectState == PLCDevice.ConnectionStates.Connected)
                {
                    sSJDevice.OffLine();
                }
            }
        }
    }
}
