using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMHRI.WCS.Server.Models
{
    public class UserRole : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserRoleType RoleType { get; set; }
        public string RolePermission { get; set; }
        public int AcessLevel { get; set; }
        public DateTime CreateTime { get; set; }
        public string AddTime { get; set; }
        //public string CurrentUserDesc { get; set; }
        public CurrentUser IsCurrentUser { get; set; }

        private string currentUserDesc;
        public string CurrentUserDesc
        {
            get
            {
                if (string.IsNullOrEmpty(currentUserDesc))
                    currentUserDesc = UserName + " 欢迎您！";
                return currentUserDesc;
            }
            set
            {
                currentUserDesc = value;
                Notify("CurrentUserDesc");
            }
        }
        public UserRole()
        {
            CreateTime = DateTime.Now;
            AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffff");
        }
        
    }
    public enum UserRolePermission
    {
        LiveAnimation = 0,
        TaskManager = 1,
        TaskSet = 2,
        GoodsLocationManager = 3,
        LogQuery = 4
    }
    public enum UserRoleType
    {
        [Description("操作员")]
        NormalUser = 0,
        [Description("管理员")]
        Administrator = 1
    }
    public enum CurrentUser
    {
        [Description("未登录")]
        LogOut=0,
        [Description("已登录")]
        LogIn=1
    }
}
