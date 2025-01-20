using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// HaiKangMessageDetailWindows.xaml 的交互逻辑
    /// </summary>
    public partial class HaiKangMessageDetailWindows : Window
    {
        public DataRowView myDataRowView;
        string reqMsg;
        string rspMsg;
        string msgDesc;
        string msgDir;
        public HaiKangMessageDetailWindows()
        {
            InitializeComponent();
        }
        public HaiKangMessageDetailWindows(DataRowView dataRowView)
        {
            InitializeComponent();
            myDataRowView = dataRowView;
            DisplayJsonText();
        }
        private void DisplayJsonText()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); //解决中文序列化被编码的问题
            options.Converters.Add(new DateTimeConverter()); //解决时间格式序列化的问题
            options.WriteIndented = true;
            reqMsg = myDataRowView.Row["ReqMsg"].ToString();
            rspMsg = myDataRowView.Row["RspMsg"].ToString();
            msgDesc = myDataRowView.Row["MsgDesc"].ToString();
            msgDir = myDataRowView.Row["MsgDirection"].ToString();
            //sdTime= myDataRowView.Row["SDTime"].ToString();

            var jsonDocument = JsonDocument.Parse(reqMsg);
            var formatJson = JsonSerializer.Serialize(jsonDocument, options);
            ReqPanel.Text = formatJson;
            try
            {
                jsonDocument = JsonDocument.Parse(rspMsg);
                formatJson = JsonSerializer.Serialize(jsonDocument, options);
            }
            catch { formatJson = rspMsg; }
            RspPanel.Text = formatJson;
        }

        private void SimulatReSendBT_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
