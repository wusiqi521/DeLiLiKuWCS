using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.Models;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// UserLogin.xaml 的交互逻辑
    /// </summary>
    public partial class UserLogin : UserControl
    {
        public ObservableCollection<UserRole> UserMessCollection;
        public event EventHandler<EventArgs> UserMessAdded;
        public event EventHandler<EventArgs> UserMessChanged;
        public event EventHandler<EventArgs> UserMessDeleted;

        public List<UserRole> CurrentUserRole;
        //private static readonly Lazy<UserLogin> lazy = new Lazy<UserLogin>(() => new UserLogin());
        //public static UserLogin Instance { get { return lazy.Value; } }

        private List<UserRole> _UserRoleList;
        public List<UserRole> UserRoleList
        {
            get
            {
                if (_UserRoleList == null)
                    CreateUserAccountsList();
                return _UserRoleList;
            }
        }
        public string currentUserName;
        string UserRole = "";
        public UserLogin()
        {
            InitializeComponent();
            try
            {
                UserMessCollection = new ObservableCollection<UserRole>(UserRoleList);
                UserMessLV.ItemsSource = UserMessCollection;

                UserMessAdded += UserMessageAdded;
                UserMessChanged += UserMessageChanged;
                UserMessDeleted += UserMessageDeleted;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UserLogin() ", ex);
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserNameTB.Text.Trim();
            string passWord = PasswordTB.Password;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
            {
                LoginErrorLB.Visibility = Visibility.Visible;
                LoginErrorLB.Content = "用户名或密码为空，请重新输入！";
                return;
            }
            //if ((int)SQLServerHelper.DataBaseReadToObject(string.Format("SELECT COUNT(*) FROM [dbo].[User_Accounts] WHERE UserName='{0}' AND Password='{1}'", userName, passWord)) < 1)
            //{
            //    LoginErrorLB.Visibility = Visibility.Visible;
            //    LoginErrorLB.Content = "用户名或者密码错误，无法执行操作";
            //    return;
            //}
            UserRole userRole = UserRoleList.Find(x => x.UserName == userName && x.Password == passWord);
            if (userRole == null)
            {
                LoginErrorLB.Visibility = Visibility.Visible;
                LoginErrorLB.Content = "用户名或者密码错误，请重新输入！";
                return;
            }
            UserRole lastUser = UserRoleList.Find(x => x.IsCurrentUser == CurrentUser.LogIn);
            if (lastUser != null)
            {
                LoginErrorLB.Visibility = Visibility.Visible;
                LoginErrorLB.Content = "已有用户登录，请先退出其他用户!";
                return;
            }
            UserRole isAdmin = UserRoleList.Find(x => x.RoleType == UserRoleType.Administrator && x.UserName == userName);
            if (isAdmin != null)
                AddUserGB.Visibility = Visibility.Visible;
            else
                AddUserGB.Visibility = Visibility.Collapsed;
            LoginErrorLB.Visibility = Visibility.Visible;
            LoginErrorLB.Content = "登录成功!";
            UpdateUserAccountsResetCurrentUser();
            userRole.IsCurrentUser = CurrentUser.LogIn;
            UpdateUserAccountsSetCurrentUser(userRole);
            CurrentUserNameDes.Content = userRole.CurrentUserDesc;
            UserRefreshBT_Click();
            //CurrentUserRole.Add(userRole);   没有初始化，不能这么写 需加上 CurrentUserRole=New List<UserRole>();


            //GetCurrentUserPermission(userRole);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Role_RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radio_btn = (RadioButton)sender;
            if (radio_btn == null) return;
            if ((bool)radio_btn.IsChecked)
            {
                switch (radio_btn.Content)
                {
                    case "操作员":
                        DeviceRepaire.Visibility = Visibility.Visible;
                        TaskManager.Visibility = Visibility.Visible;
                        TaskSet.Visibility = Visibility.Visible;
                        //GoodsLocationMana.Visibility = Visibility.Visible;
                        //LogQuery.Visibility = Visibility.Visible;
                        UserRole = radio_btn.Content.ToString();
                        break;
                    case "管理员":
                        DeviceRepaire.Visibility = Visibility.Visible;
                        TaskManager.Visibility = Visibility.Visible;
                        TaskSet.Visibility = Visibility.Visible;
                        //GoodsLocationMana.Visibility = Visibility.Collapsed;
                        //LogQuery.Visibility = Visibility.Collapsed;
                        UserRole = radio_btn.Content.ToString();
                        break;
                }
            }
        }

        private void AddLoginRoleBtn_Click(object sender, RoutedEventArgs e)
        {
            string userName = AddUserNameTB.Text.Trim();
            string passWord = AddPasswordTB.Password;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
            {
                AlarmLB.Visibility = Visibility.Visible;
                AlarmLB.Content = "用户名或密码为空，请重新输入！";
                return;
            }
            var regex = new Regex(@"
                                     (?=.*[0-9])                     
                                     (?=.*[a-zA-Z])
                                     .{6,15} 
                                     ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            bool result = regex.IsMatch(passWord);

            if (!result)
            {
                AlarmLB.Visibility = Visibility.Visible;
                AlarmLB.Content = "密码必须同时包含数字和字母,且长度至少6位，至多15位！";
                return;
            }
            if (UserRole=="操作员"&&!(bool)DeviceRepaire.IsChecked && !(bool)TaskManager.IsChecked && !(bool)TaskSet.IsChecked)  //&& !(bool)GoodsLocationMana.IsChecked && !(bool)LogQuery.IsChecked
            {
                AlarmLB.Visibility = Visibility.Visible;
                AlarmLB.Content = "权限未设置，请选择权限！";
                return;
            }
            UserRole haveUserName = UserRoleList.Find(x => x.UserName == userName);
            if (haveUserName != null)
            {
                AlarmLB.Visibility = Visibility.Visible;
                AlarmLB.Content = "用户已存在！";
                return;
            }
            switch (UserRole)
            {
                case "操作员":
                case "管理员":
                    AddUserRoleList(userName, passWord, UserRole);
                    if (InsertIntoUserAccounts(UserRoleList[UserRoleList.Count - 1]))
                    {
                        AlarmLB.Visibility = Visibility.Visible;
                        AlarmLB.Content = "新增用户成功！";
                        AddUserNameTB.Text = "";
                        AddPasswordTB.Password = "";
                    }
                    break;
            }
        }

        public void AddUserRoleList(string userName, string passWord, string userRole)
        {
            UserRole user = new UserRole();
            user.UserName = userName;
            user.Password = passWord;
            switch (userRole)
            {
                case "操作员":
                    user.RoleType = UserRoleType.NormalUser;
                    break;
                case "管理员":
                    user.RoleType = UserRoleType.Administrator;
                    break;
            }
            if ((bool)DeviceRepaire.IsChecked)
            {
                user.RolePermission = "0/";
            }
            if ((bool)TaskManager.IsChecked)
            {
                user.RolePermission += "1/";
            }
            if ((bool)TaskSet.IsChecked)
            {
                user.RolePermission += "2/";
            }
            //if ((bool)GoodsLocationMana.IsChecked)
            //{
            //    user.RolePermission += "3/";
            //}
            //if ((bool)LogQuery.IsChecked)
            //{
            //    user.RolePermission += "4/";
            //}
            user.RolePermission = user.RolePermission.Substring(0, user.RolePermission.Length - 1);
            user.IsCurrentUser = CurrentUser.LogOut;
            user.CurrentUserDesc = userName + " 欢迎您！";
            UserRoleList.Add(user);
            OnUserMessAdded(user);
        }
        public void DeleteUserRoleList(string userName)
        {
            UserRole userRole = UserRoleList.Find(x => x.UserName == userName);
            if (userRole != null)
            {
                UserRoleList.Remove(userRole);
                OnUserMessDeleted(userRole);
                DeleteUserAccounts(userRole);
            }
        }
        public string GetCurrentUser()
        {
            CurrentUserRole = UserRoleList.FindAll(x=>x.IsCurrentUser==CurrentUser.LogIn);
            if (CurrentUserRole != null && CurrentUserRole.Count == 1)
                return CurrentUserRole[0].CurrentUserDesc;
            else 
                return null;
        }
        private bool InsertIntoUserAccounts(UserRole user)
        {
            if (user != null)
            {
                SqlParameter[] sqlParameters = new SqlParameter[] {
                new SqlParameter("@UserName", SqlNull(user.UserName)),
                new SqlParameter("@Password", SqlNull(user.Password)),
                new SqlParameter("@RoleType", SqlNull(user.RoleType)),
                new SqlParameter("@RolePermission",SqlNull(user.RolePermission)),
                new SqlParameter("@AcessLevel",SqlNull(user.AcessLevel)),
                new SqlParameter("@CreateTime",SqlNull(user.CreateTime)),
                new SqlParameter("@AddTime",SqlNull(user.AddTime)),
                new SqlParameter("@IsCurrentUser",SqlNull(user.IsCurrentUser))
                };
                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[User_Accounts]([UserName],[Password],[RoleType],[RolePermission],[AcessLevel],[CreateTime],[AddTime]),[IsCurrentUser]" +
                    " VALUES (@UserName,@Password ,@RoleType,@RolePermission,@AcessLevel,@CreateTime,@AddTime,@IsCurrentUser)", sqlParameters);
            }
            return true;
        }
        private bool DeleteUserAccounts(UserRole user)
        {
            if (user != null)
            {
                SqlParameter[] sqlParameters = new SqlParameter[] {
                new SqlParameter("@UserName", SqlNull(user.UserName)),
                new SqlParameter("@Password", SqlNull(user.Password)),
                new SqlParameter("@RoleType", SqlNull(user.RoleType)),
                new SqlParameter("@RolePermission",SqlNull(user.RolePermission)),
                new SqlParameter("@AcessLevel",SqlNull(user.AcessLevel)),
                new SqlParameter("@CreateTime",SqlNull(user.CreateTime)),
                new SqlParameter("@AddTime",SqlNull(user.AddTime))
                };
                SQLServerHelper.ExeSQLStringWithParam("DELETE FROM [dbo].[User_Accounts]" +
                    " WHERE UserName=@UserName", sqlParameters);
            }
            return true;
        }
        public void UpdateUserAccountsSetCurrentUser(UserRole user)
        {
            try
            {
                UserRole userCurr = UserRoleList.Find(x => x.UserName == user.UserName);
                if (userCurr == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@UserName",SqlNull(userCurr.UserName)),
                    //new SqlParameter("@Password",user.Password),
                    //new SqlParameter("@RoleType",user.RoleType),
                    //new SqlParameter("@RolePermission",user.RolePermission),
                    //new SqlParameter("@AcessLevel",user.AcessLevel),
                    //new SqlParameter("@CreateTime",user.CreateTime),
                    //new SqlParameter("@AddTime",user.AddTime),
                    new SqlParameter("@IsCurrentUser",SqlNull(userCurr.IsCurrentUser))
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[User_Accounts]" +
                    //"set [UserName]=@UserName" +
                    //",[Password]=@Password" +
                    //",[RoleType]=@RoleType" +
                    //",[RolePermission]=@RolePermission" +
                    //",[AcessLevel]=@AcessLevel" +
                    //",[CreateTime]=@CreateTime" +
                    //",[AddTime]=@AddTime" +
                    "set [IsCurrentUser]=@IsCurrentUser" +
                    " WHERE UserName=@UserName", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateUserAccounts ", ex);
            }
        }
        /// <summary>
        /// 登录新用户后，移除上一个登录人的登录状态 
        /// </summary>
        public void UpdateUserAccountsResetCurrentUser()
        {
            try
            {
                UserRole userLast = UserRoleList.Find(x => x.IsCurrentUser == CurrentUser.LogIn);
                if (userLast == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@UserName",SqlNull(userLast.UserName)),
                    new SqlParameter("@IsCurrentUser",CurrentUser.LogOut)
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[User_Accounts]" +
                    "SET [IsCurrentUser]=@IsCurrentUser" +
                    " WHERE UserName=@UserName", sqlParameters);
                userLast.IsCurrentUser = CurrentUser.LogOut;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateUserAccounts ", ex);
            }
        }
        public void CreateUserAccountsList()
        {
            if (_UserRoleList == null)
                _UserRoleList = MyDataTableExtensions.ToList<UserRole>(SQLServerHelper.DataBaseReadToTable("SELECT [UserName],[Password],[RoleType],[RolePermission],[AcessLevel],[CreateTime],[AddTime],[IsCurrentUser] FROM[dbo].[User_Accounts]"));
            if (_UserRoleList == null)
            {
                _UserRoleList = new List<UserRole>();
            }
        }
        //public void GetCurrentUserPermission(UserRole userRole)
        //{
        //    //其它窗口获取主窗口
        //    var _mainWindow = Application.Current.Windows
        //    .Cast<Window>()
        //    .FirstOrDefault(window => window is MainWindow) as MainWindow;
        //    UserRole CurrentUse = UserRoleList.Find(x => x.UserName == userRole.UserName);
        //    string permission = CurrentUse.RolePermission;
        //    string[] strPermission = permission.Split('/');
        //    foreach (string str in strPermission)
        //    {
        //        switch (str)
        //        {
        //            case "0":
        //                _mainWindow.LiveAnimationTI.IsEnabled = true;
        //                break;
        //            case "1":
        //                _mainWindow.TaskManagerTI.IsEnabled = true;
        //                break;
        //            case "2":
        //                _mainWindow.TaskSetTI.IsEnabled = true;
        //                break;
        //            case "3":
        //                _mainWindow.GoodsLocationManagerTI.IsEnabled = true;
        //                break;
        //            case "4":
        //                _mainWindow.LogQueryTI.IsEnabled = true;
        //                break;
        //        }
        //    }
        //}
        private void ExitLogin_Click(object sender, RoutedEventArgs e)
        {
            UpdateUserAccountsResetCurrentUser();
            LoginErrorLB.Content = "已退出登录！";
            UserNameTB.Text = "";
            PasswordTB.Password = "";
            LoginErrorLB.Visibility = Visibility.Visible;
            AddUserGB.Visibility = Visibility.Collapsed;
            CurrentUserNameDes.Content = "";
            UserRefreshBT_Click();
        }

        private void DeleteLoginRoleBtn_Click(object sender, RoutedEventArgs e)
        {
            string userName = AddUserNameTB.Text.Trim();
            if (string.IsNullOrEmpty(userName))
            {
                AlarmLB.Visibility = Visibility.Visible;
                AlarmLB.Content = "用户名为空，请重新输入！";
                return;
            }
            UserRole haveUserName = UserRoleList.Find(x => x.UserName == userName);
            if (haveUserName == null)
            {
                AlarmLB.Visibility = Visibility.Visible;
                AlarmLB.Content = "用户不存在！";
                return;
            }
            DeleteUserRoleList(userName);
            AlarmLB.Visibility = Visibility.Visible;
            AlarmLB.Content = "删除成功！";
        }
        //private void ResetALLPermission()
        //{
        //    var _mainWindow = Application.Current.Windows
        //    .Cast<Window>()
        //    .FirstOrDefault(window => window is MainWindow) as MainWindow;
        //    _mainWindow.LiveAnimationTI.IsEnabled = false;
        //    _mainWindow.TaskManagerTI.IsEnabled = false;
        //    _mainWindow.TaskSetTI.IsEnabled = false;
        //    _mainWindow.GoodsLocationManagerTI.IsEnabled = false;
        //    _mainWindow.LogQueryTI.IsEnabled = false;
        //}
        public static object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;
            return obj;
        }
        private void UserMessageAdded(object sender, EventArgs e)
        {
            if (e is not UserMessEventArgs userRoleEventArgs) return;
            Dispatcher.Invoke(new Action(delegate
            {
                UserMessCollection.Add(userRoleEventArgs.UserRole);
                UserNumLB.Content = UserMessCollection.Count;
            }));
        }
        private void UserMessageChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (e is not UserMessEventArgs userRoleEventArgs) return;
                foreach (UserRole userRole in UserMessCollection)
                {
                    if (userRole.UserName == userRoleEventArgs.UserRole.UserName)
                    {
                        int index = UserMessCollection.IndexOf(userRole);
                        UserMessCollection.Remove(userRole);
                        UserMessCollection.Insert(index, userRoleEventArgs.UserRole);
                        break;
                    }
                }
            }));
        }
        private void UserMessageDeleted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (!(e is UserMessEventArgs userRoleEventArgs)) return;
                UserMessCollection.Remove(userRoleEventArgs.UserRole);
                UserNumLB.Content = UserMessCollection.Count;
            }));
        }
        private void OnUserMessAdded(UserRole userRole)
        {
            UserMessAdded?.Invoke(this, new UserMessEventArgs(userRole));
        }

        private void OnUserMessDeleted(UserRole userRole)
        {
            UserMessDeleted?.Invoke(this, new UserMessEventArgs(userRole));
        }

        private void OnUserMessChanged(UserRole userRole)
        {
            UserMessChanged?.Invoke(this, new UserMessEventArgs(userRole));
        }

        private void UserRefreshBT_Click(object sender, RoutedEventArgs e)
        {
            UserMessCollection = new ObservableCollection<UserRole>(UserRoleList);
            UserMessLV.ItemsSource = null;
            UserMessLV.ItemsSource = UserMessCollection;
            UserNumLB.Content = UserMessCollection.Count;
        }
        private void UserRefreshBT_Click()
        {
            UserMessCollection = new ObservableCollection<UserRole>(UserRoleList);
            UserMessLV.ItemsSource = null;
            UserMessLV.ItemsSource = UserMessCollection;
            UserNumLB.Content = UserMessCollection.Count;
        }
    }
    public class UserMessEventArgs : EventArgs
    {
        public UserMessEventArgs(UserRole userRole)
        {
            UserRole = userRole;
        }

        public UserRole UserRole { get; set; }
    }
}
