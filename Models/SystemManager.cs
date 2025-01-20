using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace BMHRI.WCS.Server.Models
{
    public class Systems : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private SystemStatus connect_status;
        public SystemStatus ConnectStatus
        {
            get { return connect_status; }
            set
            {
                connect_status = value;
                Notify("ConnectStatus");
            }
        }
        private SystemStatus wMSServerConnectStatus;
        public SystemStatus WMSServerConnectStatus
        {
            get { return wMSServerConnectStatus; }
            set
            {
                wMSServerConnectStatus = value;
                Notify("WMSServerConnectStatus");
            }
        }

        public int ID { get; set; }
        public string SystemType { get; set; }
        public string IP { get; set; }
        //public string SystType { get; set; }
    }
    public enum SystemStatus
    {
        DisConnected=0,
        Connected=1
    }
    public class SystemManager
    {
        private static readonly Lazy<SystemManager> lazy = new Lazy<SystemManager>(() => new SystemManager());
        public static SystemManager Instance { get { return lazy.Value; } }

        private List<Systems> systemList;
        public List<Systems> SystemList
        {
            get
            {
                if (systemList == null)
                    systemList = MyDataTableExtensions.ToList<Systems>(SQLServerHelper.DataBaseReadToTable("SELECT * FROM [dbo].[SystemList]"));
                if (systemList == null) systemList = new List<Systems>();
                return systemList;
            }
        }

        public void UpdateStatusDB(string systemType, SystemStatus systemStatus)
        {
            try
            {
                if (systemType == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@systemStatus",systemStatus),
                    new SqlParameter("@systemType",systemType),
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[SystemList] SET [ConnectStatus] = @systemStatus WHERE SystemType=@systemType", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateStatus 写入出错!", ex);
            }
        }
        public void UpdateStatus(int id, SystemStatus systemStatus)
        {
            try
            {
                Systems system = Instance.SystemList.Find(x => x.ID == id);
                if (system == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@systermStatus",systemStatus),
                    new SqlParameter("@ID",system.ID)
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[SystemList] SET [ConnectionStatus] = @systemStatus WHERE ID=@ID", sqlParameters);
                system.ConnectStatus = systemStatus;

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateStatus 写入出错!", ex);
            }
        }
        public void UpdateWmsConnectStatus(string systemType, SystemStatus state)
        {
            Systems system = Instance.SystemList.Find(x => x.SystemType == systemType);
            if (system == null) return;
            system.WMSServerConnectStatus = state;
            system.ConnectStatus = state;
            UpdateStatusDB(systemType, state);
        }
    }
}
