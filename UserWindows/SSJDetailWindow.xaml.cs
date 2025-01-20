using BMHRI.WCS.Server.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// SSJDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SSJDetailWindow : Window
    {
        public string DeviceID;
        private SSJDevice sSJDevice;

        public SSJDetailWindow()
        {
            InitializeComponent();
        }

        public SSJDetailWindow(string device_id)
        {
            InitializeComponent();
            DeviceID = device_id;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DeviceID)) return;
            sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == DeviceID);
            if (sSJDevice != null)
            {
                DataContext = sSJDevice;
                Title = sSJDevice.PLCDecription;
            }
            blocksLV.ItemsSource = sSJDevice.DeviceBlockList.OrderBy(x=>x.SystemType);
            
        }

        private void OnlineBT_Click(object sender, RoutedEventArgs e)
        {
            if (sSJDevice != null)
                sSJDevice.Online();
        }

        private void OfflineBT_Click(object sender, RoutedEventArgs e)
        {
            if (sSJDevice != null)
                sSJDevice.OffLine();
        }

        private void ClearDB400BT_Click(object sender, RoutedEventArgs e)
        {
            if (sSJDevice != null)
                sSJDevice.DB4ClearZero();
        }

        private void ClearDB500BT_Click(object sender, RoutedEventArgs e)
        {
            if (sSJDevice != null)
                sSJDevice.DB5ClearZero();
        }

        private void InerstMultiMessage_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(mmun.Text, out int k);
            for (int i = 0; i < k; i++)
            {
                sSJDevice.Online();
                sSJDevice.OffLine();
            }
        }

        private void SaveTagListBT_Click(object sender, RoutedEventArgs e)
        {
            //DataTable dr = ((DataView)PLCTagsDG.ItemsSource).Table;
            //DataTable dr = Tools.DataTableExtensions.ToDataTable(ssj_device.TagList);
            //SQLServerHelper.BulkToDB(dr, "Tag_Table");
            //System.Collections.Generic.List<DataItem> data_item_list = new System.Collections.Generic.List<DataItem>();

            //SQLServerHelper.DataBaseExecute(string.Format("DELETE FROM [dbo].[Tag_Table] WHERE PLCID='{0}'", DeviceID));
            //for (int i = 0; i < PLCTagsDG.Items.Count; i++)
            //{
            //    DataItem data_item = (DataItem)PLCTagsDG.Items[i];
            //    if (data_item != null)
            //    {
            //        SqlParameter[] sqlParameters = new SqlParameter[] {
            //            new SqlParameter("@id",i+1),
            //    new SqlParameter("@plcid", DeviceID),
            //    new SqlParameter("@name", string.IsNullOrEmpty(data_item.Name)? "" :data_item.Name),
            //    new SqlParameter("@data_type", data_item.DataType.ToString()),
            //    new SqlParameter("@var_type", data_item.VarType.ToString()),
            //    new SqlParameter("@db", data_item.DB),
            //    new SqlParameter("@bit_adr", data_item.BitAdr),
            //    new SqlParameter("@start_byte_adr", data_item.StartByteAdr),
            //    new SqlParameter("@count", data_item.Count)
            //    };
            //        SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[Tag_Table] ([ID],[PLCID],[Name],[DataType],[VarType],[DB],[StartByteAdr],[BitAdr],[Count])  VALUES (@id,@plcid,@name,@data_type,@var_type,@db,@start_byte_adr,@bit_adr,@count)", sqlParameters);
            //    }
            //}
        }

        private void RecallInPlaceBT_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void AutoCreateInboundTaskCB_Checked(object sender, RoutedEventArgs e)
        //{
        //    //sSJDevice.IsAutoCreateInboundTask = (bool)AutoCheck.IsChecked;
        //}

        private void FaultListBT_Click(object sender, RoutedEventArgs e)
        {
            blocksLV.ItemsSource = null;
            blocksLV.ItemsSource = sSJDevice.DeviceBlockList.FindAll(x => x.IsFaulty == true);
        }

        private void AllListBT_Click(object sender, RoutedEventArgs e)
        {
            blocksLV.ItemsSource = null;
            blocksLV.ItemsSource = sSJDevice.DeviceBlockList;
        }
        private void InboundListBT_Click(object sender, RoutedEventArgs e)
        {
            blocksLV.ItemsSource = null;
            blocksLV.ItemsSource = sSJDevice.DeviceBlockList.FindAll(x => x.SystemType == DeviceSystemType.InboundFinish);
        }
        private void OutboundListBT_Click(object sender, RoutedEventArgs e)
        {
            blocksLV.ItemsSource = null;
            blocksLV.ItemsSource = sSJDevice.DeviceBlockList.FindAll(x => x.SystemType == DeviceSystemType.OutboundBegin);
        }
        private void AlarmBT_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            if (button.Content.ToString() == "开始报警")
            {
                //sSJDevice.Alarm(true);
            }
            else if (button.Content.ToString() == "停止报警")
            {
                //sSJDevice.Alarm(false);
            }
        }
        private void CancelOcuptyBT_Click(object sender, RoutedEventArgs e)
        {
            SSJDeviceBlock sSJDeviceBlock = (SSJDeviceBlock)blocksLV.SelectedItem;
            if (sSJDeviceBlock == null) return;
            if (!sSJDeviceBlock.IsOccupied) return;
            if (sSJDevice == null) return;
            sSJDevice.ClearOcupty(sSJDeviceBlock.PalletNum);
        }

        private void XApplyInboundBT_Click(object sender, RoutedEventArgs e)
        {
            if (sSJDevice == null) return;
            SSJDeviceBlock sSJDeviceBlock = (SSJDeviceBlock)blocksLV.SelectedItem;
            if (sSJDeviceBlock == null) return;
            sSJDevice.XApplyInbound(sSJDeviceBlock);
        }

        private void ClearDB400BT01_Click(object sender, RoutedEventArgs e)
        {
            sSJDevice.DB4ClearZero_01();
        }
    }
}
