using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace BMHRI.WCS.Server.Models
{
    public class CircleQueue : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Position { get; set; }
        public string PalletNum { get; set; }
        public CircleQueueTaskType TaskType { get; set; }
        public DateTime CircleQueueTime {  get; set; }
        public int InCircleMBAddr { get; set; }
        public int InCircleMBAddr2 {  get; set; }
        public bool Locked {  get; set; }
        public CircleQueue()
        {
            CircleQueueTime = DateTime.Now;
        }
    }
    public class CircleQueueManager
    {
        private static readonly Lazy<CircleQueueManager> lazy = new Lazy<CircleQueueManager>(() => new CircleQueueManager());
        public static CircleQueueManager Instance { get { return lazy.Value; } }

        private List<CircleQueue> circle_queue_List;
        public List<CircleQueue> CircleQueueList
        {
            get
            {
                if (circle_queue_List == null)
                    circle_queue_List = MyDataTableExtensions.ToList<CircleQueue>(SQLServerHelper.DataBaseReadToTable("SELECT * FROM [dbo].[CircleQueueList]"));
                if (circle_queue_List == null) circle_queue_List = new List<CircleQueue>();
                return circle_queue_List;
            }
        }

        public void UpdateCircleQueueLocked(CircleQueue circleQueue, bool locked)
        {
            try
            {
                if (circleQueue == null) return;
                CircleQueue circleQueue1 = CircleQueueList.Find(x => x.Position == circleQueue.Position);
                if (circleQueue1 == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@Position",circleQueue.Position),
                    new SqlParameter("@Locked",locked),
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[CircleQueueList] SET [Locked] = @Locked WHERE Position=@Position", sqlParameters);
                circleQueue.Locked = locked;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateCircleQueueLocked 写入出错!", ex);
            }
        }
        public void AddCircleQueueTask(CircleQueue circleQueue)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@CircleQueueTime",circleQueue.CircleQueueTime),
                    new SqlParameter("@Position",SqlNull(circleQueue.Position)),
                    new SqlParameter("@PalletNum",SqlNull(circleQueue.PalletNum)),
                    new SqlParameter("@TaskType",circleQueue.TaskType),
                    new SqlParameter("@Locked",SqlNull(circleQueue.Locked)),
                };
                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[CircleQueueList]" +
                    "([CircleQueueTime],[Position],[PalletNum],[TaskType],[Locked]) VALUES" +
                    " (@CircleQueueTime, @Position, @PalletNum, @TaskType, @Locked)", sqlParameters);
                CircleQueueList.Add(circleQueue);
            }
            catch (Exception ex)
            {
                //ex.ToString();
                LogHelper.WriteLog("AddCircleQueueTask ", ex);
            }
        }
        public void DeleteCircleQueueTask(CircleQueue circleQueue)
        {
            try
            {
                if (circleQueue == null) return;
                CircleQueue circleQueue1=CircleQueueList.Find(x=>x.Position == circleQueue.Position);
                if (circleQueue1==null) return;
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Position", circleQueue.Position) };
                SQLServerHelper.ExeSQLStringWithParam("DELETE FROM [dbo].[CircleQueueList] WHERE Position=@Position", sqlParameters);
                CircleQueueList.Remove(circleQueue);
            }
            catch (Exception ex)
            {
                //ex.ToString();
                LogHelper.WriteLog("AddCircleQueueTask ", ex);
            }
        }
        public object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;

            return obj;
        }
    }
    public enum CircleQueueTaskType
    {
        None = 0,
        Inbound = 1,
        Outbound = 2
    }
}
