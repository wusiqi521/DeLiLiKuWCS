using BMHRI.WCS.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMHRI.WCS.Server.DDJProtocol;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// DmDDJDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DmDDJDetailWindow : Window
    {
        public string DeviceID;
        private DeMaticDDJ dDJDevice;
        public ObservableCollection<WCSTask> WCSTaskCollection;

        public DmDDJDetailWindow()
        {
            InitializeComponent();
        }
        public DmDDJDetailWindow(DeMaticDDJ dDJDevice)
        {
            InitializeComponent();
            this.dDJDevice = dDJDevice;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
            {
                DataContext = dDJDevice;
                Title = dDJDevice.PLCDecription;
                WCSTaskCollection = new ObservableCollection<WCSTask>();

            }
        }

        private void ConnectBT_Click(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
            {
                string receiver = "UL" + dDJDevice.PLCID.Substring(3, 2);
                DeMaticDDJ deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == dDJDevice.PLCID);
                if (deMaticDDJ != null)
                {
                    DEMATICMessage dEMATICMessage = new DEMATICMessage(dDJDevice.PLCID);
                    //dEMATICMessage.SetSTATMessage(receiver, "0752");
                    dEMATICMessage.SetSTAXMessage(receiver, "0830");
                    //dEMATICMessage.SetLIVEMessage(receiver, "0696");
                    string WCSTODEMATICMessage = dEMATICMessage.Trans;
                    deMaticDDJ.SendToDematic(WCSTODEMATICMessage);
                }
            }
        }

        private void RecallInPlaceBT_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AvailabilityBT_Click(object sender, RoutedEventArgs e)
        {
            string receiver = "UL" + dDJDevice.PLCID.Substring(3, 2);
            DeMaticDDJ deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == dDJDevice.PLCID);
            if (deMaticDDJ != null)
            {
                //deMaticDDJ.DDJWorkState = DeMaticDDJ.DDJDeviceWorkState.Standby;
                WMSTask wMSTask;
                wMSTask = new WMSTask();
                wMSTask.PalletNum = dDJDevice.PLCID.Substring(4, 1);
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.TaskSource = "WMS";
                wMSTask.ToLocation = "100";
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
        }

        private void UnableBT_Click(object sender, RoutedEventArgs e)
        {
            WMSTask wMSTask;
            wMSTask = new WMSTask();
            wMSTask.PalletNum = dDJDevice.PLCID.Substring(4, 1);
            wMSTask.TaskType = WMSTaskType.DeviceMsg;
            wMSTask.TaskSource = "WMS";
            wMSTask.ToLocation = "500";
            wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
        }

        private void WCSEnableBT_Click(object sender, RoutedEventArgs e)
        {
            DeMaticDDJ deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == dDJDevice.PLCID);
            if (deMaticDDJ != null)
            {
                deMaticDDJ.WCSEnable = true;
            }
        }
    }
}
