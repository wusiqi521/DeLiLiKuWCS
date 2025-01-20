using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.UserWindows;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// AgvPositionListUC.xaml 的交互逻辑
    /// </summary>
    public partial class AgvPositionListUC : UserControl
    {
        public AgvPositionListUC()
        {
            InitializeComponent();
            AgvPositionLayerListUC agvPositionLayerListUC1 = new AgvPositionLayerListUC("一楼AGV货位管理",1);
            Grid.SetColumn(agvPositionLayerListUC1,0);
            AgvPositionLayerListUC agvPositionLayerListUC2 = new AgvPositionLayerListUC("二楼AGV货位管理", 2);
            Grid.SetColumn(agvPositionLayerListUC2, 1);
            Gd.Children.Add(agvPositionLayerListUC1);
            Gd.Children.Add(agvPositionLayerListUC2);
        }

       

        private void AGVStaAddrCBB_DropDownClosed(object sender, EventArgs e)
        {
            if (AGVStaAddrCBB.SelectedItem == null) return;
            AgvPosition agvPosition = AGVStaAddrCBB.SelectedItem as AgvPosition;
            if (agvPosition == null) return;
            AGVPaletNoTB.Text = agvPosition.PalletNo;
            AGVEndAddrCBB.ItemsSource = AgvManager.Instance.AgvPositionList.FindAll(x => x.GroupID == agvPosition.GroupID 
            &&x.Position!=agvPosition.Position
            && x.IsAvailable);
        }

        private void CreateAGVDirectBT_Click(object sender, RoutedEventArgs e)
        {
            if (AGVStaAddrCBB.SelectedItem == null) return;
            AgvPosition agvPositionfm = AGVStaAddrCBB.SelectedItem as AgvPosition;
            if (agvPositionfm == null) return;

            if (AGVStaAddrCBB.SelectedItem == null) return;
            AgvPosition agvPositionto = AGVEndAddrCBB.SelectedItem as AgvPosition;
            if (agvPositionto == null) return;

            if(string.IsNullOrEmpty(AGVPaletNoTB.Text)) return;
            //WMSTask wMSTask = new WMSTask();
            //wMSTask.PalletNo = AGVPaletNoTB.Text;
            //wMSTask.FmLocation = agvPositionfm.FLPosition;
            //wMSTask.ToLocation=agvPositionto.FLPosition;
            //wMSTask.TaskType = WMSTaskType.MV;
            //wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
            //WMSTasksManager.Instance.AddWMSTask(wMSTask);
           WCSTask wCSTask = new WCSTask();
            wCSTask.FmLocation = agvPositionfm.Position;
            wCSTask.ToLocation= agvPositionto.Position;
            wCSTask.PalletNum = AGVPaletNoTB.Text;
            wCSTask.DeviceID =WCSTaskManager.AGVID;
            wCSTask.TaskStatus = WCSTaskStatus.Waiting;
            wCSTask.TaskType = WCSTaskTypes.AgvBound;
            wCSTask.WMSSeqID = wCSTask.WCSSeqID;
            WCSTaskManager.Instance.AddWCSTask(wCSTask);
            AGVStaAddrCBB.SelectedIndex = -1;
            AGVEndAddrCBB.SelectedIndex = -1;
            AGVPaletNoTB.Text = null;
        }

        private void AGVStaAddrCBB_DropDownOpened(object sender, EventArgs e)
        {
            AGVStaAddrCBB.ItemsSource = AgvManager.Instance.AgvPositionList.FindAll(x => x.IsAvailable);
            AGVEndAddrCBB.ItemsSource = null;
            AGVPaletNoTB.Text = null;
        }
    }
}
