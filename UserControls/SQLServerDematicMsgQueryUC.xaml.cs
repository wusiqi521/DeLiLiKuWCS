using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.UserWindows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BMHRI.WCS.Server.Models;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// SQLMsgQueryUC.xaml 的交互逻辑
    /// </summary>
    public partial class SQLServerDematicMsgQueryUC : UserControl
    {
        public SQLServerDematicMsgQueryUC()
        {
            InitializeComponent();   
        }

        public bool IsDeviceMsgQuery { set; get; }
        public string SQLServerTableName { set; get; }
        public string SQLServerQueryName { set; get; }

        private void QueryBT_Click(object sender, RoutedEventArgs e)
        {
            AlarmLB.Content = "";
            string str = StrTB.Text.Trim();
            //SELECT [PLCID],[MsgType],[Msg],[Tkdat],dbo.function_plc_message_parse(PLCID,MsgType ,Msg) AS 'MsgParse' FROM dbo.t_PLC_Message_Log
            //string sqlstr = string.Format("SELECT [PLCID],[Direction],[Trans],dbo.function_plc_message_parse(PLCID,[Direction],[Trans]) AS 'MsgParse',[Tkdat],[MsgSeqID] FROM [dbo].[SSJ_Message_Log] WHERE ", SQLServerTableName);
            //string sqlstr = string.Format("SELECT [PLCID],PLC_msg_direction([Direction]),[Trans],[Tkdat],[MsgSeqID] FROM[dbo].[PLC_Message_Log] WHERE ", SQLServerTableName);
            string sqlstr = string.Format("SELECT [PLCID],[Direction],[Trans],[Tkdat],[MsgSeqID],[MsgParse] FROM [dbo].{0} WHERE ", SQLServerTableName);
            DateTime sta_date_time = StartDP.SelectedDate.Value;
            DateTime end_date_time = EndDP.SelectedDate.Value;

            if (sta_date_time < end_date_time)
                sqlstr += "Tkdat >= '" + sta_date_time.ToShortDateString() + "' and Tkdat <= '" + end_date_time.ToShortDateString() + "'";
            else if (sta_date_time == end_date_time)
                sqlstr += "Tkdat >= '" + sta_date_time.ToShortDateString() + "' ";
            if (!string.IsNullOrEmpty(str))
                sqlstr += " AND Trans LIKE('%" + str + "%') ";
            string plcid = null;
            if (DeviceIDCB.SelectedValue!=null)
                plcid = DeviceIDCB.SelectedValue.ToString();
            if (IsDeviceMsgQuery && !string.IsNullOrEmpty(plcid) && plcid.Length == 5)
            {
                sqlstr += "AND PLCID = '" + plcid + "' ";
            }

            sqlstr += "ORDER BY Tkdat DESC,Direction";

            try
            {
                DataTable dataTable = SQLServerHelper.DataBaseReadToTable(sqlstr);
                if (dataTable != null)
                    SqlQueryDG.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                AlarmLB.Content = ex.Message;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsDeviceMsgQuery)
            {
                DeviceIDCB.Visibility = Visibility.Visible;
                if (DeviceIDCB.Items.Count==0)
                {
                    List<DematicInfor> PLCInforList = new List<DematicInfor>();

                    PLCInforList.Add(new DematicInfor("请选择设备", "0"));
                    //foreach ( SSJDevice sSJDevice in PLCDeviceManager.Instance.SSJDeviceList)
                    //{
                    //    PLCInforList.Add(new DematicInfor(sSJDevice.PLCDecription, sSJDevice.PLCID));
                    //}
                    foreach (DeMaticDDJ deMaticDDJ in DeMaticDDJManager.Instance.DeMaticDDJList)
                    {
                        PLCInforList.Add(new DematicInfor(deMaticDDJ.PLCDecription, deMaticDDJ.PLCID));
                    }

                    DeviceIDCB.ItemsSource = PLCInforList;
                    DeviceIDCB.DisplayMemberPath = "Name";
                    DeviceIDCB.SelectedValuePath = "PLCID";
                    DeviceIDCB.SelectedIndex = 0;
                }
            }
            else DeviceIDCB.Visibility = Visibility.Collapsed;


            
            if (string.IsNullOrEmpty(SQLServerQueryName)) return;
            //if(string.IsNullOrEmpty(TitlLB.Content.ToString()))
                TitlLB.Content = SQLServerQueryName;
        }

        private void WMSTaskLV_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListViewItem lv = (ListViewItem)sender;
            if (lv is null) return;
            
            if (lv.DataContext is DataRowView dataView)
            {
                if (dataView is null) return;

                DeviceMessageDetailWindow deviceMessageDetailWindow  = new DeviceMessageDetailWindow();
                deviceMessageDetailWindow.dataRowView = dataView;
                deviceMessageDetailWindow.ShowDialog();
            }
        }
    }

    class DematicInfor
    {
        public DematicInfor(string v1, string v2)
        {
            Name = v1;
            PLCID = v2;
        }

       public string Name { get; set; }
       public string PLCID { get; set; }
    }
}
