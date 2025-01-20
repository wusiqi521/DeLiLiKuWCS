using BMHRI.WCS.Server.Models;
using System.Windows;
using System;
using BMHRI.WCS.Server.DDJProtocol;
using System.Data.SqlClient;
using System.Data;
using BMHRI.WCS.Server.Tools;
using System.DirectoryServices;
using Microsoft.AspNetCore.Authentication;
using System.DirectoryServices.AccountManagement;

namespace BMHRI.WCS.Server.UserWindows
{
    /// <summary>
    /// WCSTaskOptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WCSTaskOptionWindow : Window
    {
        public WCSTask wCSTask;
        public WCSTaskOptionWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (wCSTask == null) return;
            PalletNum.Content += wCSTask.PalletNum;
            DeviceNum.Content += wCSTask.DeviceID;
            WMSID.Content += wCSTask.WMSSeqID;
            JobType.Content += wCSTask.TaskType.ToString();
            EndPosition.Content+= wCSTask.ToLocation;
            SizeToContent = SizeToContent.Height;
            if (wCSTask.DeviceID.Contains("DDJ"))
            {
                GoodLocationUC.Visibility = Visibility.Visible;
                GoodLocationUC.UpdateRowNumCBBRows(string.Concat("00", wCSTask.DeviceID.AsSpan(3, 2)));
            }
            else
            {
                GoodLocationUC.Visibility = Visibility.Collapsed;
            }
          
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
        public void IsCorrectUser()
        {
            string domainController = "192.168.88.130"; // 域控制器的 IP 地址或主机名
            string domain = "cnc.abc"; // 您的域名
            string userName = "cnc.abc\\WJ";
            string passWord = "W237100J_wj";

            // 连接到 Active Directory
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainController, domain,userName,passWord))
                {
                    // 查询用户
                    UserPrincipal user = UserPrincipal.FindByIdentity(context, userName); // 替换为要查询的用户名

                    if (user != null)
                    {
                        //Console.WriteLine($"User found: {user.DisplayName}");
                        //Console.WriteLine($"Email: {user.EmailAddress}");
                        //Console.WriteLine($"Last Logon: {user.LastLogon}");
                    }
                    else
                    {
                        //Console.WriteLine("User not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        /// <summary>
        /// 该段程序需要客户机加入域，并且以任意一个域用户登录windows系统，确保执行代码的用户具有访问域控制器的权限
        /// </summary>
        public void IsCorrectUsers()
        {
            string domain = "cnc.abc"; // 替换为你的域
            string username = "WJ"; // 输入的用户名
            string password = "W237100J_wj"; // 输入的密码
            string groupName = "WCS操作员"; // 替换为要检查的用户组名

            bool isAuthenticated = ValidateUser(domain, username, password);

            if (isAuthenticated)
            {
                //Console.WriteLine("用户验证成功！");
                bool isMember = IsUserInGroup(domain, username, groupName);
                if (isMember)
                {
                    Console.WriteLine($"{username} 是 {groupName} 组的成员。");
                }
                else
                {
                    Console.WriteLine($"{username} 不是 {groupName} 组的成员。");
                }
            }
            else
            {
                Console.WriteLine("用户验证失败！");
            }

        }
        static bool ValidateUser(string domain, string username, string password)
        {
            try
            {
                using (var entry = new DirectoryEntry($"LDAP://{domain}", username, password))
                {
                    // 尝试访问 DirectoryEntry 的某个属性来验证用户
                    object nativeObject = entry.NativeObject;
                    return true; // 如果成功访问，返回 true
                }
            }
            catch (DirectoryServicesCOMException)
            {
                // 认证失败
                return false;
            }
            catch (Exception ex)
            {
                // 处理其他可能的异常
                Console.WriteLine($"发生异常: {ex.Message}");
                return false;
            }
        }

        static bool IsUserInGroup(string domain, string username, string groupName)
        {
            try
            {
                using (var entry = new DirectoryEntry($"LDAP://{domain}"))
                {
                    using (var searcher = new DirectorySearcher(entry))
                    {
                        // 查找用户
                        searcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
                        searcher.PropertiesToLoad.Add("memberOf");

                        SearchResult result = searcher.FindOne();

                        if (result != null)
                        {
                            // 获取用户的所有组
                            foreach (string group in result.Properties["memberOf"])
                            {
                                // 提取组名
                                string groupCN = group.Substring(3, group.IndexOf(',') - 3);
                                if (string.Equals(groupCN, groupName, StringComparison.OrdinalIgnoreCase))
                                {
                                    return true; // 找到用户组
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生异常: {ex.Message}");
            }

            return false; // 默认返回 false
        }
        private void ManualStartClick(object sender, RoutedEventArgs e)
        {
            if (wCSTask == null) return;
            if (!OperationCheck("手动设备任务 开始－任务ID：" + wCSTask.WMSSeqID + "-设备ID：" + wCSTask.DeviceID + "-托盘号：" + wCSTask.PalletNum)) return;
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Waiting);
        }
        private void DeleteBTClick(object sender, RoutedEventArgs e)
        {
            if (wCSTask == null) return;
            if (!OperationCheck("手动设备任务 删除－任务ID：" + wCSTask.WMSSeqID + "-设备ID：" + wCSTask.DeviceID + "-托盘号：" + wCSTask.PalletNum)) return;
            WCSTaskManager.Instance.DeleteWCSTask(wCSTask);
        }
        private void ManualFinishClick(object sender, RoutedEventArgs e)
        {
            if (wCSTask == null) return;
            if (!OperationCheck("手动设备任务 完成－任务ID：" + wCSTask.WMSSeqID + "-设备ID：" + wCSTask.DeviceID + "-托盘号：" + wCSTask.PalletNum)) return;
            
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Done);
            DeMaticDDJ deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == wCSTask.DeviceID);
            if (deMaticDDJ != null)
            {
                deMaticDDJ.WCSEnable = true;
            }
        }
        private void ManualPickUpClick(object sender, RoutedEventArgs e)
        {
            if (wCSTask == null) return;
            if (!OperationCheck("手动设备任务 托盘取走－任务ID：" + wCSTask.WMSSeqID + "-设备ID：" + wCSTask.DeviceID + "-托盘号：" + wCSTask.PalletNum)) return;
            if (wCSTask.TaskType != WCSTaskTypes.SSJInbound)
            {
                AlarmLB.Content = "当前任务类型不支持 托盘取走 操作";
                return;
            }
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.DDJPicked);
        }

        private void DoubleStackClick(object sender, RoutedEventArgs e)
        {
            if (wCSTask == null) return;
            if (!OperationCheck("手动设备任务 双重入库确认－任务ID：" + wCSTask.WMSSeqID + "-设备ID：" + wCSTask.DeviceID + "-托盘号：" + wCSTask.PalletNum)) return;
            if (wCSTask.TaskType != WCSTaskTypes.DDJStack && wCSTask.TaskType != WCSTaskTypes.DDJStackMove)
            {
                AlarmLB.Content = "当前任务类型不支持 双重确认 操作";
                return;
            }
            if (int.Parse(wCSTask.DeviceID.Substring(3, 2)) <= 9)
            {
                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.StackDoubleConfirm);
            }
            else
            {
                string fmlocation = "ULAI" + wCSTask.DeviceID.Substring(3, 2) + "SR01LH11";   //载货台地址
                string receiver = "UL" + wCSTask.DeviceID.Substring(3, 2);
                DeMaticDDJ deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == wCSTask.DeviceID);
                if (deMaticDDJ != null)
                {
                    DEMATICMessage dEMATICMessage = new DEMATICMessage(wCSTask.DeviceID);
                    dEMATICMessage.SetTUMCMessage(receiver, fmlocation, deMaticDDJ.GetDematicLocation(wCSTask.ToLocation, wCSTask.DeviceID), wCSTask.PalletNum);
                    string WCSTODEMATICMessage = dEMATICMessage.Trans;
                    deMaticDDJ.SendToDematic(WCSTODEMATICMessage);
                }
            }
        }

        private void EmptyUnStackClick(object sender, RoutedEventArgs e)
        {
            if (wCSTask == null) return;
            if (!OperationCheck("手动设备任务 空出库确认－任务ID：" + wCSTask.WMSSeqID + "-设备ID：" + wCSTask.DeviceID + "-托盘号：" + wCSTask.PalletNum)) return;
            if (wCSTask.TaskType != WCSTaskTypes.DDJUnstack && wCSTask.TaskType != WCSTaskTypes.DDJStackMove)
            {
                AlarmLB.Content = "当前任务类型不支持 空出库确认 操作";
                return;
            }
            if (int.Parse(wCSTask.DeviceID.Substring(3, 2)) <= 9)
            {
                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.UnStackEmptyConfirm);
            }
            else
            {    //WCS主动下发取消指令(该项目不需要WCS取消任务，堆垛机自动取消)
                //string tolocation = GetDematicOutTaskToLoction(wCSTask.ToLocation, wCSTask.DeviceID);
                //string receiver = "UL" + wCSTask.DeviceID.Substring(3, 2);
                //DeMaticDDJ deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == wCSTask.DeviceID);
                //if (deMaticDDJ != null)
                //{
                //    DEMATICMessage dEMATICMessage = new DEMATICMessage(wCSTask.DeviceID);
                //    dEMATICMessage.SetTUMCMessage(receiver, deMaticDDJ.GetDematicLocation(wCSTask.FmLocation, wCSTask.DeviceID), tolocation, wCSTask.PalletNum);
                //    string WCSTODEMATICMessage = dEMATICMessage.Trans;
                //    deMaticDDJ.SendToDematic(WCSTODEMATICMessage);
                //}
            }
        }

        private void EndNearStopClick(object sender, RoutedEventArgs e)
        {
            //远端入库近端有货DDJ报双重入库
        }

        private void StaNearStopClick(object sender, RoutedEventArgs e)
        {
            if (wCSTask == null) return;
            if (!OperationCheck("手动设备任务 远端出库近端有货确认－任务ID：" + wCSTask.WMSSeqID + "-设备ID：" + wCSTask.DeviceID + "-托盘号：" + wCSTask.PalletNum)) return;
            if (wCSTask.TaskType != WCSTaskTypes.DDJUnstack)
            {
                AlarmLB.Content = "当前任务类型不支持 远端出库近端有货确认 操作";
                return;
            }
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.FarOutboundClearHaveConfirm);
        }
        private string GetDematicOutTaskToLoction(string to_location, string device_id)
        {
            string dematic_to_location = "";
            dematic_to_location = "ULAI" + device_id.Substring(3, 2) + "CR";
            if (to_location == "000a")
                dematic_to_location += "01" + "DS10";
            else if (to_location == "000b")
                dematic_to_location += "02" + "DS10";
            return dematic_to_location;
        }

        private void DirectDoneClick(object sender, RoutedEventArgs e)
        {
            if (wCSTask == null) return;
            if (!OperationCheck("手动设备任务 直出到位确认－任务ID：" + wCSTask.WMSSeqID + "-设备ID：" + wCSTask.DeviceID + "-托盘号：" + wCSTask.PalletNum)) return;
            if (wCSTask.TaskType != WCSTaskTypes.SSJOutbound && wCSTask.TaskType != WCSTaskTypes.SSJPickUpOutbound&&wCSTask.TaskType!= WCSTaskTypes.SSJInbound&&wCSTask.TaskType!= WCSTaskTypes.DDJDirect)
            {
                AlarmLB.Content = "当前任务类型不支持 直出到位 操作";
                return;
            }
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.SSJDirectDone);
            
        }

        private void ImprovePriorityClick(object sender, RoutedEventArgs e)
        {
            if (wCSTask == null) return;
            if (!OperationCheck("手动优先出库－任务ID：" + wCSTask.WMSSeqID + "-设备ID：" + wCSTask.DeviceID + "-托盘号：" + wCSTask.PalletNum)) return;
            if(wCSTask.TaskType!= WCSTaskTypes.DDJUnstack)
            {
                AlarmLB.Content = "当前任务类型不支持 优先出库操作";
                return;
            }
            WCSTaskManager.Instance.UpdateWCSTaskPri(wCSTask, 1);
        }

        private void StartDoingClick(object sender, RoutedEventArgs e)
        {
            if (wCSTask == null) return;
            if (!OperationCheck("手动设备任务 正在执行－任务ID：" + wCSTask.WMSSeqID + "-设备ID：" + wCSTask.DeviceID + "-托盘号：" + wCSTask.PalletNum)) return;
            
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Doing);
        }

        private void TestClick(object sender, RoutedEventArgs e)
        {
            IsCorrectUsers();
        }
    }
}
