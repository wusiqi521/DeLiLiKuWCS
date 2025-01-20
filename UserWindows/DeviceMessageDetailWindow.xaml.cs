using BMHRI.WCS.Server.DDJProtocol;
using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// DeviceDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceMessageDetailWindow : Window
    {
        public DataRowView dataRowView;
        public string MsgType;

        public DeviceMessageDetailWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (dataRowView == null) return;

            string plcid = dataRowView.Row["PLCID"].ToString();
            string trans = dataRowView.Row["Trans"].ToString();
            int dir = (int)dataRowView.Row["Direction"];
            string msgParse = dataRowView.Row["MsgParse"].ToString();
            if (plcid.Substring(0, 3) == "SSJ")
            {
                MsgType = "SSJ";
                SSJMessage sSJMessage = new SSJMessage(trans, plcid, (SSJMsgDirection)dir,msgParse);
                if (sSJMessage == null)
                {
                    AlarmLB.Content = trans + " " + plcid + " " + "转换失败";
                    return;
                }
                DataContext = sSJMessage;
            }
            if (plcid.Substring(0, 3) == "DDJ")
            {
                MsgType = "DDJ";
                DDJMessage dDJMessage = new DDJMessage(trans, plcid, (DDJMessageDirection)dir,msgParse);
                if (dDJMessage == null)
                {
                    AlarmLB.Content = trans + " " + plcid + " " + "转换失败";
                    return;
                }
                DataContext = dDJMessage;
            }
        }

        private void SimulatReSendBT_Click(object sender, RoutedEventArgs e)
        {
            if (MsgType == "SSJ")
            {
                if (!(DataContext is SSJMessage sSJMessage)) return;
                SSJDevice sSJ = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == sSJMessage.PLCID);
                if (sSJ == null) return;
                if (!OperationCheck("重新发送设备交换信息－设备编号：" + sSJMessage.PLCID + "-方向：" + sSJMessage.Direction + "-内容：" + sSJMessage.Trans)) return;
                if (sSJMessage.Direction == SSJMsgDirection.Receive)
                    sSJ.PLCTOWCSMessageParse(sSJMessage.Trans);
                else if (sSJMessage.Direction == SSJMsgDirection.Send)
                    sSJ.InsertIntoSSJSendList(sSJMessage);
            }
            else if (MsgType == "DDJ")
            {
                if (!(DataContext is DDJMessage dDJMessage)) return;
                DDJDevice dDJ = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == dDJMessage.PLCID);
                if (dDJ == null) return;
                if (!OperationCheck("重新发送设备交换信息－设备编号：" + dDJMessage.PLCID + "-方向：" + dDJMessage.MsgDir + "-内容：" + dDJMessage.Trans)) return;
                if (dDJMessage.MsgDir == DDJMessageDirection.Receive)
                    dDJ.PLCTOWCSMessageParse(dDJMessage.Trans);
                else if (dDJMessage.MsgDir == DDJMessageDirection.Send)
                    dDJ.InsertDDJSendList(dDJMessage);
            }
            ReSendBT.Visibility = Visibility.Collapsed;
        }

        private bool OperationCheck(string operationstr)
        {
            string username = UserNameTB.Text.Trim();
            string password = PasswordTB.Password;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                AlarmLB.Content = "输入的用户名与密码不正确，请重新输入！";
                return false; ;
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
    }
}
