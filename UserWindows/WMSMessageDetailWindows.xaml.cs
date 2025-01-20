using BMHRI.WCS.Server.Models;
using System;
using System.Data;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;
using BMHRI.WCS.Server.Tools;
using System.Data.SqlClient;
using System.Windows.Controls;
using BMHRI.WCS.Server.WebApi.Protocols;
using BMHRI.WCS.Server.SSJProtocol;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// WMSMessageDetailWindows.xaml 的交互逻辑
    /// </summary>
    public partial class WMSMessageDetailWindows : Window
    {
        public DataRowView myDataRowView;
        string reqMsg;
        string rspMsg;
        string msgDesc;
        string msgDir;
        string sdTime;
        public WMSMessageDetailWindows(DataRowView dataRowView)
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
            sdTime = myDataRowView.Row["MsgTime"].ToString();
            MsgDescLB.Content += msgDesc;
            SdTimeLB.Content += sdTime;
            if (msgDir == "1")
                MsgDirLB.Content += "收到";
            else MsgDirLB.Content += "发送";
            var jsonDocument = JsonDocument.Parse(reqMsg);
            var formatJson = JsonSerializer.Serialize(jsonDocument, options);
            ReqPanel.Text = formatJson;

            jsonDocument = JsonDocument.Parse(rspMsg);
            formatJson = JsonSerializer.Serialize(jsonDocument, options);
            RspPanel.Text = formatJson;
        }
        private bool OperationCheck(string operationstr)
        {
            string username = UserNameTB.Text.Trim();
            string password = PasswordTB.Password;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                AlarmLB.Content = "输入的用户名与密码不正确，请重新输入！";
                return false;
            }

            string InstrString = "用户：" + username + operationstr;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserName",username),
                new SqlParameter("@Password",password),
                new SqlParameter("@Instruction",InstrString),
                new SqlParameter("@ReturnValue", SqlDbType.Int)
            };

            if ((int)SQLServerHelper.ExeProcedure("PR_WCS_User_Operation", sqlParameters) == -1)
            {
                AlarmLB.Content = "用户名或者密码错误，无法执行操作";
                return false;
            }
            else
            {
                AlarmLB.Content = "操作成功";
                return true;
            }
        }
        private void SimulatReSendBT_Click(object sender, RoutedEventArgs e)
        {
            if (msgDir == "0")  //暂时只处理WCS给WMS的反馈
            {
                if (!OperationCheck("重新发送WMS信息－消息类型：" + msgDesc + "-方向：" + msgDir + "-内容：" + reqMsg)) return;
                switch (msgDesc)
                {
                    case "MoveFinish ":
                        //ManualMoveFinishToWMS(reqMsg);
                        break;
                    case "OutboundFinish ":
                        //ManualOutboundFinishToWMS(reqMsg);
                        break;
                    case "InboundFinish ":
                       // ManualStackingFinishToWMS(reqMsg);
                        break;
                    case "AGVMove ":
                        //ManualAGVMoveFinishToWMS(reqMsg);
                        break;
                    default:
                        AlarmLB.Content = "该任务类型不支持重发！";
                        break;
                }
                ReSendBT.Visibility = Visibility.Collapsed;
            }
            else
            {
                AlarmLB.Content = "该任务类型不支持重发！";
            }
        }
        //private void ManualStackingFinishToWMS(string reqMsg)
        //{
        //    TaskFinishReqMsg inboundFinishReqMsg = JsonSerializer.Deserialize<TaskFinishReqMsg>(reqMsg);
        //    try
        //    {
        //        if (inboundFinishReqMsg != null)
        //        {
        //            WMSTask wMSTask = new()
        //            {
        //                taskId = inboundFinishReqMsg.taskId,
        //                PalletNum = inboundFinishReqMsg.palletCode,
        //                WMSLocation = inboundFinishReqMsg.toLocation,
        //                TaskSource = "WMS",
        //                TaskStatus = WMSTaskStatus.TaskDone,
        //                TaskType = WMSTaskType.Stacking
        //            };
        //            WMSTasksManager.Instance.AddWMSTask(wMSTask);
        //        }
        //        else
        //        {
        //            AlarmLB.Content = "该任务类型不支持重发！";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogHelper.WriteLog("ManualStackingFinishToWMS ", e);
        //    }
        //}
        //private void ManualMoveFinishToWMS(string reqMsg)
        //{
        //    TaskFinishReqMsg moveFinishReqMsg = JsonSerializer.Deserialize<TaskFinishReqMsg>(reqMsg);
        //    try
        //    {
        //        if (moveFinishReqMsg != null)
        //        {
        //            WMSTask wMSTask = new()
        //            {
        //                taskId = moveFinishReqMsg.taskId,
        //                PalletNum = moveFinishReqMsg.palletCode,
        //                WMSLocation = moveFinishReqMsg.fmLocation,
        //                TaskSource = "WMS",
        //                TaskStatus = WMSTaskStatus.TaskDone,
        //                TaskType = WMSTaskType.Moving,
        //                WMSLocation2 = moveFinishReqMsg.toLocation
        //            };
        //            WMSTasksManager.Instance.AddWMSTask(wMSTask);
        //        }
        //        else
        //        {
        //            AlarmLB.Content = "该任务类型不支持重发！";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogHelper.WriteLog("ManualMoveFinishToWMS ", e);
        //    }
        //}
        //private void ManualOutboundFinishToWMS(string reqMsg)
        //{
        //    TaskFinishReqMsg outboundFinishReqMsg = JsonSerializer.Deserialize<TaskFinishReqMsg>(reqMsg);
        //    try
        //    {
        //        if (outboundFinishReqMsg != null)
        //        {
        //            WMSTask wMSTask = new()
        //            {
        //                taskId = outboundFinishReqMsg.taskId,
        //                PalletNum = outboundFinishReqMsg.palletCode,
        //                WMSLocation = outboundFinishReqMsg.fmLocation,
        //                TaskSource = "WMS",
        //                TaskStatus = WMSTaskStatus.TaskDone,
        //                TaskType = WMSTaskType.Outbound,
        //                ToLocation = outboundFinishReqMsg.toLocation
        //            };
        //            WMSTasksManager.Instance.AddWMSTask(wMSTask);
        //        }
        //        else
        //        {
        //            AlarmLB.Content = "该任务类型不支持重发！";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogHelper.WriteLog("ManualOutboundFinishToWMS ", e);
        //    }
        //}
        //private void ManualAGVMoveFinishToWMS(string reqMsg)
        //{
        //    TaskFinishReqMsg outboundFinishReqMsg = JsonSerializer.Deserialize<TaskFinishReqMsg>(reqMsg);
        //    try
        //    {
        //        if (outboundFinishReqMsg != null)
        //        {
        //            WMSTask wMSTask = new()
        //            {
        //                taskId = outboundFinishReqMsg.taskId,
        //                PalletNum = outboundFinishReqMsg.palletCode,
        //                FmLocation = outboundFinishReqMsg.fmLocation,
        //                TaskSource = "WMS",
        //                TaskStatus = WMSTaskStatus.TaskDone,
        //                TaskType = WMSTaskType.Outbound,
        //                ToLocation = outboundFinishReqMsg.toLocation
        //            };
        //            WMSTasksManager.Instance.AddWMSTask(wMSTask);
        //        }
        //        else
        //        {
        //            AlarmLB.Content = "该任务类型不支持重发！";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogHelper.WriteLog("ManualAGVMoveFinishToWMS ", e);
        //    }
        //}
        private string TransPortToSSJDeviceFormat(string port)
        {
            string transPort;
            if (port == null) return "";
            switch (port)
            {
                case "1FWCOUT1":
                    transPort = "1585";
                    break;
                case "1FWCOUT2":
                    transPort = "1575";
                    break;
                case "1FIN1":
                    transPort = "1570";
                    break;
                case "1FIN2":
                    transPort = "1565";
                    break;
                case "1FWBOUT1":
                    transPort = "1555";
                    break;
                case "1FIN3":
                    transPort = "1550";
                    break;
                case "1FWBOUT2":
                    transPort = "1535";
                    break;
                case "1FWAOUT1":
                    transPort = "1525";
                    break;
                case "1FIN4":
                    transPort = "1520";
                    break;
                case "2FWCINOUT1":
                    transPort = "2580";
                    break;
                case "2FWCINOUT2":
                    transPort = "2570";
                    break;
                case "2FWBINOUT1":
                    transPort = "2560";
                    break;
                case "2FWBINOUT2":
                    transPort = "2540";
                    break;
                case "2FWAINOUT1":
                    transPort = "2530";
                    break;
                case "2FWAINOUT2":
                    transPort = "2520";
                    break;
                case "100y":
                    transPort = "100y";
                    break;
                case "200y":
                    transPort = "200y";
                    break;
                default:
                    transPort = "100y";
                    break;
            }
            return transPort;
        }
        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (dataRowView == null) return;

        //    WMSMessage wMSMessage = new WMSMessage();
        //    wMSMessage.guid= dataRowView.Row["guid"].ToString();
        //    wMSMessage.TRANS= dataRowView.Row["Trans"].ToString();
        //    wMSMessage.TKDAT =(DateTime) dataRowView.Row["Tkdat"];
        //    wMSMessage.MsgDirection =(WMSMessageDirection)dataRowView.Row["MsgDirection"];
        //    wMSMessage.ReturnMsg = dataRowView.Row["ReturnMsg"].ToString();
        //    if (wMSMessage == null)
        //    {
        //        AlarmLB.Content = wMSMessage.TRANS + " " + wMSMessage.guid + " " + "转换失败";
        //        return;
        //    }
        //    DataContext = wMSMessage;
        //}
    }
}
