using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BMHRI.WCS.Server.Models
{
    public class WMSTasksManager
    {
        private static readonly Lazy<WMSTasksManager> lazy = new Lazy<WMSTasksManager>(() => new WMSTasksManager());
        public static WMSTasksManager Instance { get { return lazy.Value; } }

        public event EventHandler<EventArgs> WMSTaskAdded;
        public event EventHandler<EventArgs> WMSTaskChanged;
        public event EventHandler<EventArgs> WMSTaskDeleted;
        private List<WMSTask> wMSTaskList;
        public List<WMSTask> WMSTaskList
        {
            get
            {
                if (wMSTaskList == null)
                    wMSTaskList = MyDataTableExtensions.ToList<WMSTask>(SQLServerHelper.DataBaseReadToTable("SELECT * FROM dbo.WMSTaskList"));
                if (wMSTaskList == null) wMSTaskList = new List<WMSTask>();
                return wMSTaskList;
            }
        }
        public List<WMSMessage> WmsToWcsMessageList=new List<WMSMessage>();

        public void DeleteWMSTaskAtID(string wMSTaskSeqid)
        {
            if (string.IsNullOrEmpty(wMSTaskSeqid)) return;
            WMSTask wMSTask = WMSTaskList.Find(x => x.WMSSeqID == wMSTaskSeqid);
            if (wMSTask != null) DeleteWMSTask(wMSTask);
        }
        private void DeleteWMSTask(WMSTask wMSTask)
        {
            if (wMSTask == null||string.IsNullOrEmpty(wMSTask.WMSSeqID)) return;
            SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@WMSSeqID", wMSTask.WMSSeqID) };
            SQLServerHelper.ExeSQLStringWithParam("DELETE FROM [dbo].[WMSTaskList] WHERE WMSSeqID=@WMSSeqID", sqlParameters);
            WMSTaskList.Remove(wMSTask);
            OnWMSTaskDeleted(wMSTask);
        }

        public void FinishWMSTask(string wMSSeqID)
        {
            WMSTask wMSTask = WMSTaskList.Find(x => x.WMSSeqID == wMSSeqID);
            if (wMSTask == null) return;
            FinishWMSTask(wMSTask);
        }
        public void FinishWMSTask(WMSTask wMSTask)
        {
            if (wMSTask == null) return;
            UpdateWMSTaskStatus(wMSTask, WMSTaskStatus.TaskDone);
        }
        public void UpdateWMSTaskStatus(string wMSSeqID, WMSTaskStatus wMSTaskStatus)
        {
            WMSTask wMSTask = WMSTaskList.Find(x => x.WMSSeqID == wMSSeqID);
            if (wMSTask == null) return;
            UpdateWMSTaskStatus(wMSTask, wMSTaskStatus);
        }
        public void UpdateWMSTaskStatus(WMSTask wMSTask, WMSTaskStatus wMSTaskStatus)
        {
            try
            {
                WMSTask wMSTasko = WMSTaskList.Find(x => x.WMSSeqID == wMSTask.WMSSeqID);
                if (wMSTasko == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@TaskStatus",SqlNull(wMSTaskStatus)),
                    new SqlParameter("@WMSSeqID",SqlNull(wMSTask.WMSSeqID)),
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[WMSTaskList] " +
                    "SET [TaskStatus] = @TaskStatus" +
                    " WHERE WMSSeqID = @WMSSeqID", sqlParameters);
                wMSTasko.TaskStatus = wMSTaskStatus;

                OnWMSTaskChanged(wMSTasko);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateWMSTaskStatus ", ex);
            }
        }
        public void DeleteWMSAndWCSTask(WMSTask wMSTask)
        {
            if (wMSTask == null) return;
            WMSTask wMSTask1 = WMSTaskList.Find(x => x.WMSSeqID == wMSTask.WMSSeqID);
            if (wMSTask1 == null) return;
            SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@WMSSeqID", wMSTask1.WMSSeqID) };
            SQLServerHelper.ExeSQLStringWithParam("DELETE FROM [dbo].[WMSTaskList] WHERE WMSSeqID=@WMSSeqID", sqlParameters);
            WMSTaskList.Remove(wMSTask1);
            OnWMSTaskDeleted(wMSTask1);
            WCSTaskManager.Instance.DeleteWCSTasksAtWMSSeq(wMSTask1.WMSSeqID);
        }
        internal void AddWMSTask(WMSTask wMSTask)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    
                    new SqlParameter("@CreateTime",wMSTask.CreateTime),
                    new SqlParameter("@Warehouse",SqlNull(wMSTask.Warehouse)),
                    new SqlParameter("@TaskType",SqlNull(wMSTask.TaskType)),
                    new SqlParameter("@PalletNum",SqlNull(wMSTask.PalletNum)),
                    new SqlParameter("@Priority",wMSTask.Priority),
                    new SqlParameter("@FmLocation",SqlNull(wMSTask.FmLocation)),
                    new SqlParameter("@ToLocation",SqlNull(wMSTask.ToLocation)),
                    new SqlParameter("@TaskStatus",SqlNull(wMSTask.TaskStatus)),
                    new SqlParameter("@TaskSource",SqlNull(wMSTask.TaskSource)),
                    new SqlParameter("@WMSSeqID",SqlNull(wMSTask.WMSSeqID)),
                    new SqlParameter("@GaoDiBZ",SqlNull(wMSTask.GaoDiBZ)),
                    new SqlParameter("@Floor",SqlNull(wMSTask.Floor)),
                    new SqlParameter("@taskId",SqlNull(wMSTask.taskId)),
                    new SqlParameter("@WMSLocation",SqlNull(wMSTask.WMSLocation)),
                    new SqlParameter("@WMSLocation2",SqlNull(wMSTask.WMSLocation2)),
                    new SqlParameter("@LedMessage",SqlNull(wMSTask.LedMessage)),
                    //new SqlParameter("@WMSTaskID",SqlNull(wMSTask.WMSTaskID)),
                    //new SqlParameter("@CartHeight",SqlNull(wMSTask.CartHeight)),
                    //new SqlParameter("@CartLength",SqlNull(wMSTask.CartLength)),
                    //new SqlParameter("@CartType",SqlNull(wMSTask.CartType)),
                    //new SqlParameter("@CartWidth",SqlNull(wMSTask.CartWidth)),
                    //new SqlParameter("@Notes",SqlNull(wMSTask.Notes)),
                    //new SqlParameter("@EmptyFlag",wMSTask.EmptyFlag),
                    //new SqlParameter("@ToQty",SqlNull(wMSTask.ToQty)),
                    //new SqlParameter("@FmQty",SqlNull(wMSTask.FmQty))
                };
                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[WMSTaskList]" +
                    "([CreateTime],[Warehouse],[PalletNum],[TaskType],[FmLocation],[ToLocation],[Priority],[TaskStatus],[TaskSource],[WMSSeqID],[GaoDiBZ],[Floor],[taskId],[WMSLocation],[WMSLocation2],[LedMessage]) VALUES" +
                    " (@CreateTime,@Warehouse, @PalletNum, @TaskType, @FmLocation, @ToLocation, @Priority, @TaskStatus, @TaskSource, @WMSSeqID,@GaoDiBZ,@Floor,@taskId,@WMSLocation,@WMSLocation2,@LedMessage)", sqlParameters);
                WMSTaskList.Add(wMSTask);
                OnWMSTaskAdded(wMSTask);
            }
            catch (Exception ex)
            {
               //ex.ToString();
                LogHelper.WriteLog("AddWMSTask ", ex);
            }
        }

        public void FarOutboundClearHaveConfirm(string wMSSeqID)
        {
            WMSTask wMSTask = WMSTaskList.Find(x => x.WMSSeqID == wMSSeqID);
            if (wMSTask == null) return;
            FarOutboundClearHaveConfirm(wMSTask);
        }
        public void FarOutboundClearHaveConfirm(WMSTask wMSTask)
        {
            if (wMSTask == null) return;
            if (wMSTask.TaskType != WMSTaskType.Outbound&&wMSTask.TaskType!= WMSTaskType.Picking) return;
            UpdateWMSTaskStatus(wMSTask, WMSTaskStatus.Pick_Up_Stop);
            OnWMSTaskChanged(wMSTask);
        }
        public void InboundTwiceComfirm(string wMSSeqID)
        {
            WMSTask wMSTask = WMSTaskList.Find(x => x.WMSSeqID == wMSSeqID);
            if (wMSTask == null) return;
            InboundTwiceComfirm(wMSTask);
        }
        public void InboundTwiceComfirm(WMSTask wMSTask)
        {
            if (wMSTask == null) return;
            if (wMSTask.TaskType != WMSTaskType.Stacking && wMSTask.TaskType != WMSTaskType.Moving) return;
            UpdateWMSTaskStatus(wMSTask, WMSTaskStatus.Double_Inbound);
            OnWMSTaskChanged(wMSTask);
        }
        internal void EmptyUnStackConfirmWMSTask(string wMSSeqID)
        {
            WMSTask wMSTask = WMSTaskList.Find(x => x.WMSSeqID == wMSSeqID);
            if (wMSTask == null) return;
            EmptyUnStackConfirmWMSTask(wMSTask);
        }
        public void EmptyUnStackConfirmWMSTask(WMSTask wMSTask)
        {
            if (wMSTask == null) return;
            if (wMSTask.TaskType != WMSTaskType.Outbound && wMSTask.TaskType!=  WMSTaskType.Picking && wMSTask.TaskType != WMSTaskType.Moving) return;
            UpdateWMSTaskStatus(wMSTask, WMSTaskStatus.UnStack_Empty);
            OnWMSTaskChanged(wMSTask);
        }
        public object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;

            return obj;
        }
        #region WMSTask事件定义
        private void OnWMSTaskAdded(WMSTask wMSTask)
        {
            WMSTaskAdded?.Invoke(this, new WMSTaskEventArgs(wMSTask));
        }

        public void UpdateWMSTask(WMSTask wMSTask)
        {
            WMSTask wMSTaskOld = WMSTaskList.Find(x => x.WMSSeqID == wMSTask.WMSSeqID);
            if (wMSTaskOld == null) return;
            DeleteWMSTaskAtID(wMSTaskOld.WMSSeqID);
            AddWMSTask(wMSTask);
            OnWMSTaskChanged(wMSTask);
        }

        private void OnWMSTaskDeleted(WMSTask wMSTask)
        {
            WMSTaskDeleted?.Invoke(this, new WMSTaskEventArgs(wMSTask));
        }

        private void OnWMSTaskChanged(WMSTask wMSTask)
        {
            WMSTaskChanged?.Invoke(this, new WMSTaskEventArgs(wMSTask));
        }

        public void UpdateWMSTaskToLocation(string wMSSeqID, string position,string wmsLocation)
        {
            try
            {
                if (string.IsNullOrEmpty(wMSSeqID) || string.IsNullOrEmpty(position))
                    return;
                WMSTask wMSTask = WMSTaskList.Find(x => x.WMSSeqID == wMSSeqID);
                if (wMSTask == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                  {
                    new SqlParameter("@ToLocation",SqlNull(position)),
                    new SqlParameter("@WMSSeqID",SqlNull(wMSTask.WMSSeqID)),
                    new SqlParameter("@WMSLocation",SqlNull(wmsLocation)),
                  };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[WMSTaskList] " +
                    "SET [ToLocation] = @ToLocation,[WMSLocation] = @WMSLocation" +
                    " WHERE WMSSeqID = @WMSSeqID", sqlParameters);
                wMSTask.ToLocation = position;
                wMSTask.WMSLocation = wmsLocation;
                OnWMSTaskChanged(wMSTask);
            }
            catch(Exception ex)
            {
                LogHelper.WriteLog("UpdateWMSTaskToLocation ", ex);
            }
        }
        public void UpdateWMSTaskToLocation(string wMSSeqID, string position)
        {
            try
            {
                if (string.IsNullOrEmpty(wMSSeqID) || string.IsNullOrEmpty(position))
                    return;
                WMSTask wMSTask = WMSTaskList.Find(x => x.WMSSeqID == wMSSeqID);
                if (wMSTask == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                  {
                    new SqlParameter("@ToLocation",SqlNull(position)),
                    new SqlParameter("@WMSSeqID",SqlNull(wMSTask.WMSSeqID)),
                  };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[WMSTaskList] " +
                    "SET [ToLocation] = @ToLocation" +
                    " WHERE WMSSeqID = @WMSSeqID", sqlParameters);
                wMSTask.ToLocation = position;
                OnWMSTaskChanged(wMSTask);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateWMSTaskToLocation ", ex);
            }
        }
        public void UpdateWMSTaskToLocationAndStatus(WMSTask wMSTask)
        {
            try
            {
                if (wMSTask == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                  {
                    new SqlParameter("@TaskID",SqlNull(wMSTask.taskId)),
                    new SqlParameter("@ToLocation",SqlNull(wMSTask.ToLocation)),
                    new SqlParameter("@WMSSeqID",SqlNull(wMSTask.WMSSeqID)),
                    new SqlParameter("@TaskStatus",wMSTask.TaskStatus),
                    new SqlParameter("@FmLocation",wMSTask.FmLocation),
                  };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[WMSTaskList] " +
                    "SET [ToLocation] = @ToLocation,[FmLocation] = @FmLocation,[TaskID] = @TaskID,[TaskStatus] = @TaskStatus" +
                    " WHERE WMSSeqID = @WMSSeqID", sqlParameters);

                OnWMSTaskChanged(wMSTask);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateWMSTaskToLocation ", ex);
            }
        }
        #endregion
    }
    public class WMSTaskEventArgs : EventArgs
    {
        public WMSTaskEventArgs(WMSTask wMSTask)
        {
            WMSTask = wMSTask;
        }

        public WMSTask WMSTask { get; set; }
    }
}
