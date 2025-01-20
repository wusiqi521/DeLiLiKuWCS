using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// DeviceOptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RGVBlockOptionWindows : Window
    {
        public string DeviceID;
        public string PLCID;
        public SSJDevice sSJDevice;
        public SSJDeviceBlock sSJDeviceBlock;
        public RGVDevice rGVDevice;
        public BaseModel baseModel;
        public RGVBlockOptionWindows(BaseModel base_model)
        {
            InitializeComponent();
            baseModel = base_model;
            if (baseModel == null) return;
            if (string.IsNullOrEmpty(baseModel.ModelID)) return;
            sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == baseModel.PLCID);
            if (sSJDevice == null) return;
            rGVDevice = sSJDevice.RGVDeviceList.Find(x => x.DeviceID == baseModel.ModelID);
            if (rGVDevice == null) return;
            DataContext = rGVDevice;
            SizeToContent = SizeToContent.Height;
        }
        //private void SaveOptionBTClick(object sender, RoutedEventArgs e)
        //{
        //    if (sSJDeviceBlock == null) return;
        //    try
        //    {
        //        SqlParameter[] sqlParameters = new SqlParameter[]
        //        {
        //            new SqlParameter("@Position",SqlNull(sSJDeviceBlock.Position)),
        //            new SqlParameter("@DeviceType",SqlNull(sSJDeviceBlock.DeviceType)),
        //            new SqlParameter("@SystemType",SqlNull(sSJDeviceBlock.SystemType)),
        //            new SqlParameter("@FLPosition",SqlNull(sSJDeviceBlock.Position)),
        //            new SqlParameter("@MotionDirection",SqlNull(sSJDeviceBlock.MotionDirection)),
        //            new SqlParameter("@StatusDBAddr",SqlNull(sSJDeviceBlock.StatusDBAddr)),
        //            new SqlParameter("@Available",SqlNull(sSJDeviceBlock.Available)),
        //            new SqlParameter("@QAddrBit1",SqlNull(sSJDeviceBlock.QAddrBit1)),
        //            new SqlParameter("@QAddrBit2",SqlNull(sSJDeviceBlock.QAddrBit2)),
        //            new SqlParameter("@MotionQAddr",SqlNull(sSJDeviceBlock.MotionQAddr)),
        //             new SqlParameter("@PalletNumDBAddr",SqlNull(sSJDeviceBlock.PalletNumDBAddr)),
        //              new SqlParameter("@Margin",SqlNull(sSJDeviceBlock.Margin)),
        //        };
        //        SQLServerHelper.ExeSQLStringWithParam(" UPDATE[dbo].[ConveyerBlocks]  " +
        //            "SET [Available] = @Available" +
        //            ",[MotionDirection] =@MotionDirection" +
        //            ",[MotionQAddr] = @MotionQAddr" +
        //            ",[QAddrBit1] = @QAddrBit1" +
        //            ",[QAddrBit2] = @QAddrBit2 " +
        //            ",[Margin] = @Margin " +
        //            "WHERE Position = @Position", sqlParameters);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.WriteLog("ChangeOptionBTClick ", ex);
        //    }
        //}
        public object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;

            return obj;
        }
        //private void ChangeOptionBTClick(object sender, RoutedEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    if (string.IsNullOrEmpty(PositionTB.Text)) return;
        //    string position = PositionTB.Text;

        //    SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == "SSJ0" + position.Substring(0, 1));
        //    if (sSJDevice == null) return;
        //    SQLServerHelper.DataBaseExecute(string.Format("UPDATE dbo.SenceModelList SET DeviceID = '{0}' WHERE ModelID ='{1}'", position, baseModel.ModelID));

        //    SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == position);
        //    baseModel.ModelID = position;
        //    if (sSJDeviceBlock == null) return;
        //    DataContext = sSJDeviceBlock;
        //}
    }
}
