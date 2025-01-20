using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMHRI.WCS.Server.Models
{
    public class LoginCheckManager
    {
        private static readonly Lazy<LoginCheckManager> lazy = new Lazy<LoginCheckManager>(() => new LoginCheckManager());
        public static LoginCheckManager Instance { get { return lazy.Value; } }

        private List<LoginCheck> loginCheckList;
        public List<LoginCheck> LoginCheckList
        {
            get
            {
                if (loginCheckList == null)
                    loginCheckList = MyDataTableExtensions.ToList<LoginCheck>(SQLServerHelper.DataBaseReadToTable("SELECT * FROM[dbo].[LoginCheck]"));
                if (loginCheckList == null) loginCheckList = new List<LoginCheck>();
                return loginCheckList;
            }
        }

        public void UpdateLoginCheck(string deviceID, string token, string registerNo, DateTime dateTimeExpire)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceID) || string.IsNullOrEmpty(token)) return;
                LoginCheck loginCheck = LoginCheckList.Find(x => x.DeviceID == deviceID);
                if (loginCheck == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@DeviceID",deviceID),
                    new SqlParameter("@RegisterNo",registerNo),
                    new SqlParameter("@Token",token),
                    new SqlParameter("@GetTime",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new SqlParameter("@ExpireTime",dateTimeExpire.ToString("yyyy-MM-dd HH:mm:ss")),
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[LoginCheck] SET [RegisterNo] = @RegisterNo,[Token] = @Token,[GetTime] = @GetTime,[ExpireTime] = @ExpireTime where DeviceID=@DeviceID  ", sqlParameters);
                loginCheck.RegisterNo = registerNo;
                loginCheck.Token = token;
                loginCheck.GetTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                loginCheck.ExpireTime = dateTimeExpire.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateLoginToken 写入出错!", ex);
            }
        }

    }


    public class LoginCheck
    {
        public string DeviceID { get; set; }
        public string Token { get; set; }
        public string RegisterNo { get; set; }
        public string GetTime { get; set; }
        public string ExpireTime { get; set; }
    }





}
