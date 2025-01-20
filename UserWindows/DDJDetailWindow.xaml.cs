using BMHRI.WCS.Server.DDJProtocol;
using BMHRI.WCS.Server.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// DDJDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DDJDetailWindow : Window
    {
        public DDJDevice dDJDevice;
        
        public ObservableCollection<WCSTask> WCSTaskCollection;
        public DDJDetailWindow()
        {
            InitializeComponent();
        }
        public DDJDetailWindow(DDJDevice dDJDevice)
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
                if (dDJDevice.PLCID == "DDJ01" || dDJDevice.PLCID == "DDJ02")
                {
                    
                    RecallButton.Visibility = Visibility.Collapsed;
                }
                else
                {
 
                    FloorRecallButton1.Visibility = Visibility.Collapsed;
                    FloorRecallButton2.Visibility = Visibility.Collapsed;
                    FloorRecallButton3.Visibility = Visibility.Collapsed;
                    FloorRecallButton4.Visibility = Visibility.Collapsed;
                    FloorRecallButton5.Visibility = Visibility.Collapsed;
                    FloorRecallButton6.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void WCSTaskAdded(object sender, EventArgs e)
        {
            if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
            Dispatcher.Invoke(new Action(delegate
            {
                WCSTaskCollection.Add(wCSTaskEventArgs.WCSTask);
                //WCSTaskNumLB.Content = WCSTaskCollection.Count;
            }));
        }
        private void WCSTaskChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
                foreach (WCSTask wCSTask in WCSTaskCollection)
                {
                    if (wCSTask.WCSSeqID == wCSTaskEventArgs.WCSTask.WCSSeqID)
                    {
                        int index = WCSTaskCollection.IndexOf(wCSTask);
                        WCSTaskCollection.Remove(wCSTask);
                        WCSTaskCollection.Insert(index, wCSTaskEventArgs.WCSTask);
                        break;
                    }
                }
            }));
        }
        private void WCSTaskDeleted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
                WCSTaskCollection.Remove(wCSTaskEventArgs.WCSTask);
                //WCSTaskNumLB.Content = WCSTaskCollection.Count;
            }));
        }

        private void OnlineBT_Click(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
            {
                dDJDevice.ReadyIntoTunnel = false;
                dDJDevice.Online();
            }
        }

        private void OfflineBT_Click(object sender, RoutedEventArgs e)
        {

            if (dDJDevice != null)
            {
                dDJDevice.OffLine();
            }
        }

        private void RecallInPlaceBT_Click(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
            {
                dDJDevice.RecallInPlace();
            }
        }

        private void ClearDB400BT_Click(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
                dDJDevice.DB4ClearZero();
        }

        private void ClearDB500BT_Click(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
                dDJDevice.DB5ClearZero();
        }

        private void UnavailabilityBT_Click(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
            {
                dDJDevice.ReadyIntoTunnel = true;
                //dDJDevice.Available = false;
                //PLCDeviceManager.Instance.SavePLCDeviceStatus(dDJDevice.PLCDeviceType, dDJDevice.PLCID, "Available", "0");
                //WMSTask wMSTask;
                //wMSTask = new WMSTask();
                //wMSTask.PalletNum = dDJDevice.PLCID.Substring(4, 1);
                //wMSTask.TaskType = WMSTaskType.DeviceMsg;
                //wMSTask.TaskSource = "WMS";
                //wMSTask.ToLocation = "@";
                //wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                //WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
        }
        private void AvailabilityBT_Click(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
            {
                dDJDevice.Available = true;
                PLCDeviceManager.Instance.SavePLCDeviceStatus(dDJDevice.PLCDeviceType, dDJDevice.PLCID, "Available", "1");
                WMSTask wMSTask;
                wMSTask = new WMSTask();
                wMSTask.PalletNum = dDJDevice.PLCID.Substring(4, 1);
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.TaskSource = "WMS";
                wMSTask.ToLocation = "D";
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
        }

        //空出库
        private void F20Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
                dDJDevice.EmtpyOutboundConfirm();
        }
        //双重确认
        private void F21Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
                dDJDevice.DoubleInboundConfirm();
        }   
        //货位不可达确认
        private void RecallBT_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            switch (button.Content)
            {
                case "一楼南召回":
                    if (dDJDevice != null && dDJDevice.ReadyIntoTunnel == false)
                    {
                        dDJDevice.RecallInPlace("000a");
                    }
                    break;
                case "二楼南召回":
                    if (dDJDevice != null && dDJDevice.ReadyIntoTunnel == false)
                    {
                        dDJDevice.RecallInPlace("000c");
                    }
                    break;
                case "一楼北召回":
                    if (dDJDevice != null && dDJDevice.ReadyIntoTunnel == false)
                    {
                        dDJDevice.RecallInPlace("000e");
                    }
                    break;
                case "二楼北召回":
                    if (dDJDevice != null && dDJDevice.ReadyIntoTunnel == false)
                    {
                        dDJDevice.RecallInPlace("000g");
                    }
                    break;
                case "三楼南召回":
                    if (dDJDevice != null && dDJDevice.ReadyIntoTunnel == false)
                    {
                        dDJDevice.RecallInPlace("000d");
                    }
                    break;
                case "三楼北召回":
                    if (dDJDevice != null && dDJDevice.ReadyIntoTunnel == false)
                    {
                        dDJDevice.RecallInPlace("000h");
                    }
                    break;
                case "模具库堆垛机召回":
                    if (dDJDevice != null && dDJDevice.ReadyIntoTunnel == false)
                    {
                        dDJDevice.RecallInPlace("000k");
                    }
                    break;
                default:
                    break;
            }
        }

        private void ConnectBT_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            switch (button.Content)
            {
                case "断开网络":
                    if (dDJDevice != null)
                    {
                        dDJDevice.Disconnect();
                    }
                    break;
                case "连接网络":
                    if (dDJDevice != null)
                    {
                        dDJDevice.AutoConnect = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private string GetWMSDeviceNum(string plcid)
        {
            if (string.IsNullOrEmpty(plcid) || plcid.Length < 5) return null;
            return plcid.Substring(3) + ",0";
        }

        private void F19Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
                dDJDevice.EmptyPickupConfirm();
        }

        private void send0j_Click(object sender, RoutedEventArgs e)
        {
            if (dDJDevice != null)
                dDJDevice.opendoor();
        }
    }
}
