using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.UserWindows;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// SQLMsgQueryUC.xaml 的交互逻辑
    /// </summary>
    public partial class SQLServerMsgQueryUC : UserControl
    {
        public SQLServerMsgQueryUC()
        {
            InitializeComponent();

        }
        public string SQLServerTableName { set; get; }
        public string SQLServerQueryName { set; get; }

        private void QueryBT_Click(object sender, RoutedEventArgs e)
        {
            AlarmLB.Content = "";
            string str = StrTB.Text.Trim();
            string sqlstr = string.Format("SELECT * FROM {0} WHERE ", SQLServerTableName);
            DateTime sta_date_time = StartDP.SelectedDate.Value;
            DateTime end_date_time = EndDP.SelectedDate.Value;

            if (sta_date_time < end_date_time)
                sqlstr += "MsgTime >= '" + sta_date_time.ToShortDateString() + "' and MsgTime <= '" + end_date_time.ToShortDateString() + "'";
            else if (sta_date_time == end_date_time)
                sqlstr += "MsgTime >= '" + sta_date_time.ToShortDateString() + "'";
            if (!string.IsNullOrEmpty(str))
                sqlstr += " AND ReqMsg LIKE('%" + str + "%') ";
            sqlstr += " ORDER BY MsgTime DESC";

            try
            {
                DataTable dataTable = SQLServerHelper.DataBaseReadToTable(sqlstr);                  
                SqlQueryLV.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                AlarmLB.Content = ex.Message;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
          
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

                WMSMessageDetailWindows wMSMessageDetailWindows = new(dataView);
                //wMSMessageDetailWindows.dataRowView = dataView;
                wMSMessageDetailWindows.ShowDialog();
            }
        }

    }
}
