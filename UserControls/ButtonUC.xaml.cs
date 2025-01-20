using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// ButtonUC.xaml 的交互逻辑
    /// </summary>
    public partial class ButtonUC
    {
        public DDJDevice dDJDevice;
        public ButtonUC()
        {
            InitializeComponent();
        }

        private void AllOnlineBT_Click(object sender, RoutedEventArgs e)
        {
            if (!OperationCheck("堆垛机一键联机"))
            { 
                AlarmLabel.Content = "用户名或密码输入错误！";
                return;
            }
            
            foreach (DDJDevice ddj_device in PLCDeviceManager.Instance.DDJDeviceList)
                ddj_device.Online();
            AlarmLabel.Content = "";
        }

        private void AllOfflineBT_Click(object sender, RoutedEventArgs e)
        {
            if (!OperationCheck("堆垛机一键脱机"))
            {
                AlarmLabel.Content = "用户名或密码输入错误！";
                return;
            }
            foreach (DDJDevice ddj_device in PLCDeviceManager.Instance.DDJDeviceList)
                ddj_device.OffLine();
            AlarmLabel.Content = "";
        }

        private bool OperationCheck(string operationstr)
        {
            string username = UserNameTB.Text.Trim();
            string password = PasswordTB.Password;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                UserNameTB.Clear();
                PasswordTB.Clear();
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
                UserNameTB.Clear();
                PasswordTB.Clear();
                return false;
            }

            UserNameTB.Clear();
            PasswordTB.Clear();
            return true;
        }
    }
}
