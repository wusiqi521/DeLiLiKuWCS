using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.UserControls;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;
using System.Windows.Controls;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// WMSTaskOptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WMSTaskOptionWindow : Window
    {
        public WMSTask MyWMSTask;
        private List<string> _outPortList = new List<string>
        {
            "1115",
            "1125",
            "1215",
            "1225",
            "2116",
            "2126",
            "2216",
            "2226",
            "3116",
            "3126",
            "3216",
            "3226"
        };
        public WMSTaskOptionWindow(WMSTask wMSTask)
        {
            InitializeComponent();
            MyWMSTask = wMSTask;
            DisplayJsonText();
            CmbOutPorts.ItemsSource = _outPortList;
        }
        private void DisplayMemberInfo()
        {
           var t= MyWMSTask.GetType();
            var propertyInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var propertyInfo in propertyInfos)
            {
                Label label = new Label();
                label.VerticalAlignment = VerticalAlignment.Center;
                label.Content= propertyInfo.Name ;
                //RootPanel.Children.Add(label);
                TextBox textBox = new TextBox();
                textBox.VerticalAlignment = VerticalAlignment.Center;
                object obj = propertyInfo.GetValue(MyWMSTask);
                if(obj!=null)
                textBox.Text = obj.ToString();
                //RootPanel.Children.Add(textBox);
            }
        }
        private void DisplayJsonText()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); //解决中文序列化被编码的问题
            options.Converters.Add(new DateTimeConverter()); //解决时间格式序列化的问题
            options.WriteIndented = true;
            string ss = JsonSerializer.Serialize(MyWMSTask, options);

            var jsonDocument = JsonDocument.Parse(ss);

            var formatJson = JsonSerializer.Serialize(jsonDocument, options);
            RootPanel.Text = formatJson;
        }

        private void DeleteWMSTaskBT_Click(object sender, RoutedEventArgs e)
        {
            if (MyWMSTask == null) return;
            WMSTasksManager.Instance.DeleteWMSTaskAtID(MyWMSTask.WMSSeqID);
        }

        private void ManulFinishBT_Click(object sender, RoutedEventArgs e)
        {
            if (MyWMSTask == null) return;
            WMSTasksManager.Instance.FinishWMSTask(MyWMSTask);
        }

        private void DoubleInConfirmBT_Click(object sender, RoutedEventArgs e)
        {
            if (MyWMSTask == null) return;
            if(MyWMSTask.TaskType!= WMSTaskType.Stacking&&MyWMSTask.TaskType!=WMSTaskType.Moving)
            {
                return;
            }
            WMSTasksManager.Instance.InboundTwiceComfirm(MyWMSTask);
        }

        private void EmptyPickConfirmBT_Click(object sender, RoutedEventArgs e)
        {
            if (MyWMSTask == null) return;
            if (MyWMSTask.TaskType != WMSTaskType.Outbound&& MyWMSTask.TaskType != WMSTaskType.Moving)
            {
                return;
            }
            WMSTasksManager.Instance.EmptyUnStackConfirmWMSTask(MyWMSTask);
        }

        private void CreateWCSTaskBT_Click(object sender, RoutedEventArgs e)
        {
            if (MyWMSTask == null) return;
            WMSTasksManager.Instance.UpdateWMSTaskStatus(MyWMSTask, WMSTaskStatus.TaskAssigned);
        }

        private void PutDownStopConfirmBT_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PickUpStopConfirmBT_Click(object sender, RoutedEventArgs e)
        {
            if (MyWMSTask == null) return;
            WCSTaskManager.Instance.DeleteWCSTasksAtWMSSeq(MyWMSTask.WMSSeqID);
            WMSTasksManager.Instance.UpdateWMSTaskStatus(MyWMSTask, WMSTaskStatus.Pick_Up_Stop);
        }

        private void TransTunnelBT_Click(object sender, RoutedEventArgs e)
        {
            if (MyWMSTask == null) return;
            string selectedAddress = CmbOutPorts.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedAddress)) return;
            if (MyWMSTask.TaskType != WMSTaskType.Outbound) return;
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.WMSSeqID == MyWMSTask.WMSSeqID && x.TaskType==WCSTaskTypes.DDJUnstack);
            WCSTask wCSTask2 = WCSTaskManager.Instance.WCSTaskList.Find(x => x.WMSSeqID == MyWMSTask.WMSSeqID && x.TaskType == WCSTaskTypes.SSJOutbound);
            if (wCSTask == null || wCSTask2 ==null ) return;
            if(wCSTask.TaskStatus != WCSTaskStatus.Waiting) return;
            WCSTaskManager.Instance.DeleteWCSTasksAtWMSSeq(MyWMSTask.WMSSeqID);
            WMSTasksManager.Instance.UpdateWMSTaskToLocation(MyWMSTask.WMSSeqID, selectedAddress);
            WMSTasksManager.Instance.UpdateWMSTaskStatus(MyWMSTask.WMSSeqID, WMSTaskStatus.ChangeOutBound);
        }
    }
}
