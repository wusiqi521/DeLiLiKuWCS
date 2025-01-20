using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace BMHRI.WCS.Server.Models
{
    public class LoadArea : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int Status { get; set; }
        public string LoadAreaPort { get; set; }
        public string PLCID { get; set; }
    }
    public class LoadAreaManager
    {
        private static readonly Lazy<LoadAreaManager> lazy = new Lazy<LoadAreaManager>(() => new LoadAreaManager());
        public static LoadAreaManager Instance { get { return lazy.Value; } }

        private List<LoadArea> load_area_List;
        public List<LoadArea> LoadAreaList
        {
            get
            {
                if (load_area_List == null)
                    load_area_List = MyDataTableExtensions.ToList<LoadArea>(SQLServerHelper.DataBaseReadToTable("SELECT * FROM [dbo].[LoadAreaList]"));
                if (load_area_List == null) load_area_List = new List<LoadArea>();
                return load_area_List;
            }
        }

        public void UpdateLoadAreaStatus(LoadArea loadArea, int status)
        {
            try
            {
                if (loadArea == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@LoadAreaPort",loadArea.LoadAreaPort),
                    new SqlParameter("@Status",status),
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[LoadAreaList] SET [Status] = @Status WHERE LoadAreaPort=@LoadAreaPort", sqlParameters);
                loadArea.Status = status;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateLoadAreaStatus 写入出错!", ex);
            }
        }
    }
}
