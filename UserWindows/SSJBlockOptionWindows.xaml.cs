using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// DeviceOptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SSJBlockOptionWindows : Window
    {
        public string DeviceID;
        public string PLCID;
        public SSJDevice sSJDevice;
        public SSJDeviceBlock sSJDeviceBlock;
        public BaseModel baseModel;
        public SSJBlockOptionWindows(BaseModel base_model)
        {
            InitializeComponent();
            baseModel = base_model;
            if (baseModel == null) return;
            if (string.IsNullOrEmpty(baseModel.ModelID)) return;
            sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == baseModel.PLCID);
            if (sSJDevice == null) return;
            sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == baseModel.ModelID[1..]);
            if (sSJDeviceBlock == null) return;
            DataContext = sSJDeviceBlock;
            SizeToContent = SizeToContent.Height;
        }
        private void SaveOptionBTClick(object sender, RoutedEventArgs e)
        {
            if (sSJDeviceBlock == null) return;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@Position",SqlNull(sSJDeviceBlock.Position)),
                    new SqlParameter("@DeviceType",SqlNull(sSJDeviceBlock.DeviceType)),
                    new SqlParameter("@SystemType",SqlNull(sSJDeviceBlock.SystemType)),
                    new SqlParameter("@FLPosition",SqlNull(sSJDeviceBlock.Position)),
                    new SqlParameter("@MotionDirection",SqlNull(sSJDeviceBlock.MotionDirection)),
                    new SqlParameter("@StatusDBAddr",SqlNull(sSJDeviceBlock.StatusDBAddr)),
                    new SqlParameter("@Available",SqlNull(sSJDeviceBlock.Available)),
                    new SqlParameter("@QAddrBit1",SqlNull(sSJDeviceBlock.QAddrBit1)),
                    new SqlParameter("@QAddrBit2",SqlNull(sSJDeviceBlock.QAddrBit2)),
                    new SqlParameter("@MotionQAddr",SqlNull(sSJDeviceBlock.MotionQAddr)),
                     new SqlParameter("@PalletNumDBAddr",SqlNull(sSJDeviceBlock.PalletNumDBAddr)),
                      new SqlParameter("@Margin",SqlNull(sSJDeviceBlock.Margin)),
                };
                SQLServerHelper.ExeSQLStringWithParam(" UPDATE[dbo].[ConveyerBlocks]  " +
                    "SET [Available] = @Available" +
                    ",[MotionDirection] =@MotionDirection" +
                    ",[MotionQAddr] = @MotionQAddr" +
                    ",[QAddrBit1] = @QAddrBit1" +
                    ",[QAddrBit2] = @QAddrBit2 " +
                    ",[Margin] = @Margin " +
                    "WHERE Position = @Position", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ChangeOptionBTClick ", ex);
            }
        }
    public object SqlNull(object obj)
    {
        if (obj == null)
            return DBNull.Value;

        return obj;
    }
    private void ChangeOptionBTClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (string.IsNullOrEmpty(PositionTB.Text)) return;
            string position = PositionTB.Text;

            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == "SSJ0" + position.Substring(0, 1));
            if (sSJDevice == null) return;
            SQLServerHelper.DataBaseExecute(string.Format("UPDATE dbo.SenceModelList SET DeviceID = '{0}' WHERE ModelID ='{1}'", position, baseModel.ModelID));

            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == position);
            baseModel.ModelID = position;
            if (sSJDeviceBlock == null) return;
            DataContext = sSJDeviceBlock;
        }

        //private void SimilatBT_Click(object sender, RoutedEventArgs e)
        //{
        //    string PalletNum = PalletNumTB.Text;
        //    string StrAddr = StaAddrTB.Text;
        //    string EndAddr = EndAddrTB.Text;

        //    if (string.IsNullOrEmpty(PalletNum))
        //    {
        //        AlarmMLB.Content = "托盘条码不能为空！";
        //        return;
        //    }
        //    if (PalletNum.Length != 8)
        //    {
        //        AlarmMLB.Content = "托盘条码长度不够！";
        //        return;
        //    }

        //    if (sSJDevice == null)
        //    {
        //        AlarmMLB.Content = "无法找到PLC设备！";
        //        return;
        //    }

        //    if (sSJDeviceBlock == null)
        //    {
        //        AlarmMLB.Content = "无法找到设备-" + DeviceID;
        //        return;
        //    }

        //    //MyWebSocketClient.Instance.CreateSSJOcupty(sSJDevice.PLCID,sSJDeviceBlock.Position, PalletNum, StrAddr, EndAddr);
        //}

        private void ClearOcupty_Click(object sender, RoutedEventArgs e)
        {
            if (sSJDevice == null) return;
            if (sSJDeviceBlock == null) return;
            if (!sSJDeviceBlock.IsOccupied)
            {
                ClearOccupyAlarmLabel.Content = "该输送机处无目标，无法清除信息";
                return;
            }
            if (string.IsNullOrEmpty(sSJDeviceBlock.PalletNum))
            {
                ClearOccupyAlarmLabel.Content = "该输送机处无托盘号，无法清除信息";
                return;
            }
            ClearOccupyAlarmLabel.Content = "";
            sSJDevice.ClearOcupty(sSJDeviceBlock.PalletNum);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (sSJDevice == null) return;
            if (sSJDeviceBlock == null) return;
            if (sSJDeviceBlock.SystemType!= DeviceSystemType.InboundFinish&&sSJDeviceBlock.SystemType!= DeviceSystemType.InboundFinishOrOutboundBegin && sSJDeviceBlock.SystemType != DeviceSystemType.TotalPort)
            {
                ClearBT.Visibility= Visibility.Collapsed;
                TPHorizonLB.Visibility= Visibility.Collapsed;
                TPHorizonLB2.Visibility= Visibility.Collapsed;
            }
            if(sSJDeviceBlock.SystemType!= DeviceSystemType.OutboundBegin&&sSJDeviceBlock.SystemType!= DeviceSystemType.InboundFinishOrOutboundBegin && sSJDeviceBlock.SystemType != DeviceSystemType.TotalPort)
            {
                AllowLoadLB.Visibility= Visibility.Collapsed;
                AllowLoadLB2.Visibility= Visibility.Collapsed;
            }
            if (sSJDeviceBlock.SystemType != DeviceSystemType.Picking && sSJDeviceBlock.SystemType != DeviceSystemType.TotalPort)
            {
                modeLB.Visibility = Visibility.Collapsed;
                modeLBs.Visibility = Visibility.Collapsed;
            }
            if (sSJDeviceBlock.SystemType != DeviceSystemType.Picking && sSJDeviceBlock.SystemType != DeviceSystemType.TotalPort && sSJDeviceBlock.SystemType != DeviceSystemType.InboundBegin)
            {
                NotAGV.Visibility = Visibility.Collapsed;
                NotAGVs.Visibility = Visibility.Collapsed;
            }
        }

        //private void AGVSetDoneBT_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sSJDevice == null) return;
        //    if (sSJDeviceBlock == null) return;
        //    if (!sSJDeviceBlock.AGVSetPoint ) return;    //|| string.IsNullOrEmpty(sSJDeviceBlock.AGVToPosition)
        //    if (!sSJDeviceBlock.IsLoaded)
        //    {
        //        ClearOccupyAlarmLabel.Content = "该输送机处无料箱/托盘，无法生成AGV放货完成指令";
        //        return;
        //    }
        //    if (AGVSetPalletTB.Text.Trim().Length != 8)
        //    {
        //        ClearOccupyAlarmLabel.Content = "料箱/托盘号长度不合法!";
        //        return;
        //    }
        //    ClearOccupyAlarmLabel.Content = "";
        //    //O77788889730320000000000000000
        //    WMSMessage wMSMessage = new WMSMessage();
        //    wMSMessage.TRANS = "O" + AGVSetPalletTB.Text.Trim() + "7" + sSJDeviceBlock.Position + "0000000000000000";
        //    wMSMessage.guid = Guid.NewGuid().ToString("N");
        //    wMSMessage.TKDAT = DateTime.Now;
        //    wMSMessage.DEMO1 = null;
        //    wMSMessage.DEMO2 = null;
        //    wMSMessage.DEMO3 = null;
        //    wMSMessage.MsgDirection = WMSMessageDirection.Input;
        //    wMSMessage.ReturnMsg = WMSDBManager.ProcessWMSMessage(wMSMessage);
        //}
        //private void SingleInboundBT_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sSJDevice == null) return;
        //    if (sSJDeviceBlock == null) return;
        //    if (!sSJDeviceBlock.IsOccupied || sSJDeviceBlock.SystemType != DeviceSystemType.InboundFinish)
        //        return;
        //   // MyWebSocketClient.Instance.SinglePalletStack(sSJDeviceBlock.DDJID, sSJDeviceBlock.Position);
        //    Button button = sender as Button;
        //    if (button == null) return;
        //    button.Visibility = Visibility.Hidden;
        //}
    }
}
