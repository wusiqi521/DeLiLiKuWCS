using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.WebApi.Controllers;
using BMHRI.WCS.Server.WebApi.Protocols;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace BMHRI.WCS.Server.Models
{
    public class AgvManager
    {
        private static readonly Lazy<AgvManager> lazy = new Lazy<AgvManager>(() => new AgvManager());

        public static AgvManager Instance { get { return lazy.Value; } }
        public HttpClient MyWMSHttpClient;
        readonly string HaiKangWebApiStart = ConfigurationManager.AppSettings["HaiKangWebApiStart"];
        readonly string HaiKangWebApiConti = ConfigurationManager.AppSettings["HaiKangWebApiContinue"];
        readonly string HaiKangWebApiRelease = ConfigurationManager.AppSettings["HaiKangWebApiReleasePosition"];
        public List<AgvPosition> AgvPositionList;
        private AgvManager()
        {
            AgvPositionList = MyDataTableExtensions.ToList<AgvPosition>(SQLServerHelper.DataBaseReadToTable("SELECT * FROM dbo.AgvPositionList"));
            AgvPositionList ??= new List<AgvPosition>();
            MyWMSHttpClient = new HttpClient();
        }
        public void Start()
        {
            Task.Factory.StartNew(() => ProcesHKAgvTask(), TaskCreationOptions.LongRunning);
        }

        private void ProcesHKAgvTask()
        {
            while (true)
            {
                try
                {
                    List<WCSTask> wCSTasks = WCSTaskManager.Instance.WCSTaskList.FindAll(x => !string.IsNullOrEmpty(x.DeviceID) && x.DeviceID == WCSTaskManager.AGVID && x.TaskStatus == WCSTaskStatus.Waiting);
                    if (wCSTasks != null && wCSTasks.Any())
                    {
                        foreach (var wCSTask in wCSTasks)
                        {
                            if (wCSTask == null) continue;
                            
                            if (wCSTask.TaskType == WCSTaskTypes.AgvBound)
                            {
                                if (string.IsNullOrEmpty(wCSTask.FmLocation)) continue;
                                AgvPosition agvPositionFm = AgvPositionList.Find(x => x.Position == wCSTask.FmLocation);
                                if (agvPositionFm == null) continue;
                                AgvPosition agvPositionTo = AgvPositionList.Find(x => x.Position == wCSTask.ToLocation);
                                if (agvPositionTo == null) continue;
                                switch (agvPositionFm.PositionType)
                                {
                                    case AgvPositionSystemType.SSJDeviceBlockIn:
                                    case AgvPositionSystemType.SSJDeviceBlockOut:
                                        {
                                            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == agvPositionFm.SSJID);
                                            if (sSJDevice == null) continue;
                                            if (sSJDevice.SSJWorkState == SSJDeviceWorkState.None) continue;
                                            if (sSJDevice.SSJWorkState == SSJDeviceWorkState.Offline) continue;
                                            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == agvPositionFm.SSJPositon);
                                            if (sSJDeviceBlock == null) continue;
                                            if (!sSJDeviceBlock.IsOccupied) continue;
                                            if (sSJDeviceBlock.PalletNum != wCSTask.PalletNum) continue;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                                switch (agvPositionTo.PositionType)
                                {
                                    case AgvPositionSystemType.SSJDeviceBlockIn:
                                    case AgvPositionSystemType.SSJDeviceBlockOut:
                                        {
                                            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == agvPositionTo.SSJID);
                                            if (sSJDevice == null) continue;
                                            if (sSJDevice.SSJWorkState == SSJDeviceWorkState.None) continue;
                                            if (sSJDevice.SSJWorkState == SSJDeviceWorkState.Offline) continue;
                                            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == agvPositionTo.SSJPositon);
                                            if (sSJDeviceBlock == null) continue;
                                            if ((sSJDeviceBlock.Position == "2070" || sSJDeviceBlock.Position == "2103") && sSJDeviceBlock.CurrentMode == DeviceModeType.OutboundMode) continue;
                                            if(sSJDeviceBlock.Position=="2070")
                                                sSJDevice.SetAgvOpt2070(true);
                                            if(sSJDeviceBlock.Position=="2103")
                                                sSJDevice.SetAgvOpt2103(true);
                                            //if (sSJDeviceBlock.IsOccupied) continue;
                                            //if (!sSJDeviceBlock.AllowUnloading) continue;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                                //int ccount = WCSTaskManager.Instance.WCSTaskList.FindAll(x => x.DeviceID == WCSTaskManager.AGVID &&
                                //x.ToLocation == agvPositionTo.Position && (x.TaskStatus == WCSTaskStatus.Doing ||
                                //x.TaskStatus == WCSTaskStatus.AGVApplyPick ||
                                //x.TaskStatus == WCSTaskStatus.AGVApplyPut ||
                                //x.TaskStatus == WCSTaskStatus.AGVPickUped ||
                                //x.TaskStatus == WCSTaskStatus.AGVPutDowned)).Count;
                                //if (ccount > 0) continue;
                                SendStartTaskToHK(wCSTask);
                            }
                        }

                    }

                    List<WCSTask> wCSTaskApicks = WCSTaskManager.Instance.WCSTaskList.FindAll(x => !string.IsNullOrEmpty(x.DeviceID) && x.DeviceID == WCSTaskManager.AGVID && x.TaskStatus == WCSTaskStatus.AGVApplyPick);
                    if (wCSTaskApicks != null && wCSTaskApicks.Any())
                    {
                        foreach (WCSTask wCSTask in wCSTaskApicks)
                        {
                            AgvPosition agvPositionfm = AgvPositionList.Find(x => x.Position == wCSTask.FmLocation);
                            if (agvPositionfm == null) continue;

                            switch (agvPositionfm.PositionType)
                            {
                                case AgvPositionSystemType.GoodLocation:
                                    {
                                        if (agvPositionfm.IsAvailable && agvPositionfm.PalletNo == wCSTask.PalletNum && agvPositionfm.PositionStatus == AgvPositionStatus.FmPlace)
                                            SendContiTaskToHK(wCSTask);
                                        break;
                                    }
                                case AgvPositionSystemType.SSJDeviceBlockOut:
                                case AgvPositionSystemType.SSJDeviceBlockIn:
                                    {
                                        SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == agvPositionfm.SSJID);
                                        if (sSJDevice == null || sSJDevice.SSJWorkState == SSJDeviceWorkState.None) continue;
                                        SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == agvPositionfm.SSJPositon);
                                        if (sSJDeviceBlock == null || sSJDeviceBlock.IsFaulty) continue;
                                        if (!sSJDeviceBlock.IsOccupied) continue;
                                        if (sSJDeviceBlock.PalletNum != wCSTask.PalletNum) continue;
                                        SendContiTaskToHK(wCSTask);
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                    List<WCSTask> wCSTaskAputs = WCSTaskManager.Instance.WCSTaskList.FindAll(x => !string.IsNullOrEmpty(x.DeviceID) && x.DeviceID == WCSTaskManager.AGVID && x.TaskStatus == WCSTaskStatus.AGVApplyPut);
                    if (wCSTaskAputs != null && wCSTaskAputs.Any())
                    {
                        foreach (WCSTask wCSTask in wCSTaskAputs)
                        {
                            AgvPosition agvPositionTo = AgvPositionList.Find(x => x.Position == wCSTask.ToLocation);
                            if (agvPositionTo == null) continue;

                            switch (agvPositionTo.PositionType)
                            {
                                case AgvPositionSystemType.GoodLocation:
                                    {
                                        //if (agvPositionTo.IsAvailable && agvPositionTo.PalletNo == wCSTask.PalletNo && agvPositionTo.PositionStatus == AgvPositionStatus.ToPlace)
                                        SendContiTaskToHK(wCSTask);
                                        break;
                                    }
                                //case AgvPositionSystemType.SSJDeviceBlockOut:
                                //case AgvPositionSystemType.SSJDeviceBlockIn:
                                //    {
                                //        SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == agvPositionTo.SSJID);
                                //        if (sSJDevice == null || sSJDevice.SSJWorkState == SSJDeviceWorkState.None) continue;
                                //        SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == agvPositionTo.SSJPositon);
                                //        if (sSJDeviceBlock == null || sSJDeviceBlock.IsFaulty) continue;
                                //        if (sSJDeviceBlock.IsOccupied) continue;
                                //        if (!sSJDeviceBlock.AllowUnloading) continue;
                                //        sSJDevice.SetAgvOpt(true);
                                //        SendContiTaskToHK(wCSTask);
                                //        break;
                                //    }
                                default:
                                    break;
                            }
                        }
                    }
                    List<WCSTask> wCSTaskAllowputs = WCSTaskManager.Instance.WCSTaskList.FindAll(x => !string.IsNullOrEmpty(x.DeviceID) && x.DeviceID == WCSTaskManager.AGVID && x.TaskStatus == WCSTaskStatus.AGVAllowPut);
                    if (wCSTaskAllowputs != null && wCSTaskAllowputs.Any())
                    {
                        foreach (WCSTask wCSTask in wCSTaskAllowputs)
                        {
                            AgvPosition agvPositionTo = AgvPositionList.Find(x => x.Position == wCSTask.ToLocation);
                            if (agvPositionTo == null) continue;

                            switch (agvPositionTo.PositionType)
                            {
                                case AgvPositionSystemType.SSJDeviceBlockOut:
                                case AgvPositionSystemType.SSJDeviceBlockIn:
                                    SendContiTaskToHK(wCSTask);
                                    break;
                                default: 
                                    break;
                            }
                        }
                    }
                        List<WMSTask> wMSTasksRelease = WMSTasksManager.Instance.WMSTaskList.FindAll(x=>x.TaskStatus== WMSTaskStatus.AGVPositionChg);
                    if (wMSTasksRelease != null && wMSTasksRelease.Any())
                    {
                        foreach(WMSTask wMSTask in wMSTasksRelease)
                        {
                            SendReleasePositionToHK(wMSTask);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("ProcesHKAgvTask ", ex);
                }
                finally
                {
                    Task.Delay(1000).Wait();
                }
            }
        }



        private string FindAgvEmptyLocation(string ssjId, AgvPositionSystemType type)
        {
            string emptyLocationStr = null;

            string sqlstr = string.Format("SELECT top 1 Position FROM[dbo].[AgvPositionList] where ssjid = '{0}' and palletno is null and PositionType = 1 and IsAvailable = 1", ssjId);

            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr);

            if (obj != null)
            {
                emptyLocationStr = obj.ToString();
            }
            return emptyLocationStr;
        }



        private bool AgvWorkShopLocationIsEmpty(AgvPosition agvPosition)
        {

            string emptyLocationStr = null;

            string sqlstr = string.Format("SELECT PalletNo FROM [dbo].[AgvPositionList] where [Position]='{0}'  and IsAvailable = 1", agvPosition.Position);

            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr);

            if (obj != null)
            {
                emptyLocationStr = obj.ToString();
            }

            if (string.IsNullOrEmpty(emptyLocationStr)) return true;

            else return false;

        }

        private string FindAgv1LTolactionPalletNo(AgvPosition agvPosition)

        {

            string locationStr = null;

            string sqlstr = string.Format("SELECT PalletNo FROM [dbo].[AgvPositionList] where [Position]='{0}'  and IsAvailable = 1", agvPosition.Position);

            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr);

            if (obj != null)
                locationStr = obj.ToString();
            return locationStr;
        }

        public string FindLocationPalletNo(AgvPosition agvPosition)
        {
            string palletNo = null;
            string sqlstr = string.Format("SELECT PalletNo FROM[dbo].[AgvPositionList] where Position={0}", agvPosition.Position);
            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr);
            if (obj != null)
            {
                palletNo = obj.ToString();
            }
            return palletNo;
        }

        private AgvPosition FindAgvPosition(string fmLocation)
        {
            AgvPosition agvPosition = new AgvPosition();
            string sqlstr = string.Format("", fmLocation);
            return agvPosition;
        }



        public void SendStartTaskToHK(WCSTask wCSTask)
        {
            try
            {
                if (wCSTask == null) return;
                WCSToHkReqMsg wCSToHkReqMsg = new WCSToHkReqMsg(wCSTask);
                string rspstr = JsonHelp.Serialize(wCSToHkReqMsg);
                var data = new StringContent(rspstr, Encoding.UTF8, "application/json");
                HttpResponseMessage response = MyWMSHttpClient.PostAsync(HaiKangWebApiStart, data).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    InsertIntoWCS2HKLog("SendStartTaskToHK ", rspstr, result, AGVMsgDirection.Output);
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Doing);
                    WCSToHkRsqMsg wCSToHkRsqMsg = System.Text.Json.JsonSerializer.Deserialize<WCSToHkRsqMsg>(result);
                    if (wCSToHkRsqMsg.code == "0")
                    {
                        UpdateAGVPositionPalletNoStatus(wCSTask.ToLocation, wCSTask.PalletNum, AgvPositionStatus.ToPlace);
                        UpdateAGVPositionPalletNoStatus(wCSTask.FmLocation, wCSTask.PalletNum, AgvPositionStatus.FmPlace);
                    }
                    else WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Fault);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SendToWMSTaskDone ", ex);
            }

        }

        public void SendContiTaskToHK(WCSTask wCSTask)
        {
            try
            {
                if (wCSTask == null) return;
                WCSToHkContinueReqMsg wCSToHkContinueReqMsg = new WCSToHkContinueReqMsg(wCSTask);
                string rspstr = JsonHelp.Serialize(wCSToHkContinueReqMsg);
                var data = new StringContent(rspstr, Encoding.UTF8, "application/json");
                HttpResponseMessage response = MyWMSHttpClient.PostAsync(HaiKangWebApiConti, data).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    InsertIntoWCS2HKLog("SendContiTaskToHK ", rspstr, result, AGVMsgDirection.Output);
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Doing);
                    WCSToHkRsqMsg wCSToHkRsqMsg = System.Text.Json.JsonSerializer.Deserialize<WCSToHkRsqMsg>(result);
                    if (wCSToHkRsqMsg.code != "0")
                        WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Fault);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SendContiTaskToHK ", ex);
            }

        }
        public void SendReleasePositionToHK(WMSTask wMSTask)
        {
            try
            {
                if (wMSTask == null) return;
                WCSToHkReleasePositionReqMsg wCSToHkReleasePositionReqMsg = new WCSToHkReleasePositionReqMsg();
                string rspstr = JsonHelp.Serialize(wCSToHkReleasePositionReqMsg);
                var data = new StringContent(rspstr, Encoding.UTF8, "application/json");
                HttpResponseMessage response = MyWMSHttpClient.PostAsync(HaiKangWebApiRelease, data).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    InsertIntoWCS2HKLog("SendReleasePositionToHK ", rspstr, result, AGVMsgDirection.Output);
                    WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                    WCSToHkRsqMsg wCSToHkRsqMsg = System.Text.Json.JsonSerializer.Deserialize<WCSToHkRsqMsg>(result);
                    if(wCSToHkRsqMsg.code!="0")
                        UpdateAGVPositionPalletNoStatus(wMSTask.FmLocation, null, AgvPositionStatus.None);

                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SendContiTaskToHK ", ex);
            }

        }

        public void InsertIntoWCS2HKLog(string msgDesc, string reqMsg, string rspMsg, AGVMsgDirection msgdir)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@MsgDesc",SqlNull(msgDesc)),
                    new SqlParameter("@MsgDirection",SqlNull(msgdir)),
                    new SqlParameter("@ReqMsg",SqlNull(reqMsg)),
                    new SqlParameter("@RspMsg",SqlNull(rspMsg)),
                };

                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[WCS2HiKaJsonLog] " +
                    "([MsgDesc],[MsgDirection],[ReqMsg],[RspMsg])VALUES" +
                    "(@MsgDesc,@MsgDirection , @ReqMsg ,@RspMsg)", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("InserIntoDAFuMsgLog ", ex);
            }
        }
        public static object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;
            return obj;
        }
        public void UpdateAGVPositionPalletNoStatus(string position, string palletNo, AgvPositionStatus agvPositionStatus)
        {
            if (string.IsNullOrEmpty(position)) return;
            AgvPosition agvPosition = AgvPositionList.Find(x => x.Position == position);
            if (agvPosition == null) return;
            agvPosition.PalletNo = palletNo;
            agvPosition.PositionStatus = agvPositionStatus;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@PalletNo",SqlNull(agvPosition.PalletNo)),
                    new SqlParameter("@PositionStatus",SqlNull(agvPosition.PositionStatus)),
                    new SqlParameter("@Position",SqlNull(agvPosition.Position))
                };

                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[AgvPositionList]SET" +
                    " [PalletNo] = @PalletNo" +
                    ",[PositionStatus] = @PositionStatus" +
                    " WHERE [Position] = @Position ", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateAGVLocatiionPalletNoStatus 写入出错!", ex);
            }
        }
        public void UpdateAGVPositionPalletNo(string position, string palletNo)
        {
            if (string.IsNullOrEmpty(position)) return;
            AgvPosition agvPosition = AgvPositionList.Find(x => x.Position == position);
            if (agvPosition == null) return;
            agvPosition.PalletNo = palletNo;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@PalletNo",SqlNull(agvPosition.PalletNo)),
                    new SqlParameter("@Position",SqlNull(agvPosition.Position))
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[AgvPositionList]SET" +
                    " [PalletNo] = @PalletNo" +
                    " WHERE [Position] = @Position ", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateAGVLocatiionPalletNoStatus 写入出错!", ex);
            }
        }
        public void UpdateAGVPositionIsAvailable(string position, bool isAvailable)
        {
            if (string.IsNullOrEmpty(position)) return;
            AgvPosition agvPosition = AgvPositionList.Find(x => x.Position == position);
            if (agvPosition == null) return;
            agvPosition.IsAvailable = isAvailable;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@IsAvailable",SqlNull(agvPosition.IsAvailable)),
                    new SqlParameter("@Position",SqlNull(agvPosition.Position))
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[AgvPositionList]SET" +
                    " [IsAvailable] = @IsAvailable" +
                    " WHERE [Position] = @Position ", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateAGVLocatiionPalletNoStatus 写入出错!", ex);
            }
        }
        public void UpdateAGVPositionIsAvailable(AgvPosition position, bool isAvailable)
        {
            if (position == null) return;
            AgvPosition agvPosition = AgvPositionList.Find(x => x.Position == position.Position);
            if (agvPosition == null) return;
            agvPosition.IsAvailable = isAvailable;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@IsAvailable",SqlNull(agvPosition.IsAvailable)),
                    new SqlParameter("@Position",SqlNull(agvPosition.Position))
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[AgvPositionList]SET" +
                    " [IsAvailable] = @IsAvailable" +
                    " WHERE [Position] = @Position ", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateAGVLocatiionPalletNoStatus 写入出错!", ex);
            }
        }
        public void UpdateAGVPositionPalletNo(AgvPosition agvPosition, string palletNo)
        {
            AgvPosition agvPosition1 = AgvPositionList.Find(x => x.Position == agvPosition.Position);
            if (agvPosition1 == null) return;
            agvPosition1.PalletNo = palletNo;
            UpdateAGVPositionPalletNo(agvPosition1.Position, palletNo);
        }

        
        //public void InserIntoWMSJsonMsgLog(string msgDesc, MsgDirection msgDir, WMSUpdateAgvPositionReqMsg wMSUpdateAgvPositionReqMsg, WMSAssignTaskRspMsg wMSAssignTaskRspMsg)
        //{
        //    string rspstr = JsonHelp.Serialize(wMSAssignTaskRspMsg);
        //    string reqstr = JsonHelp.Serialize(wMSUpdateAgvPositionReqMsg);
        //    Task.Factory.StartNew(() => { InserIntoWMSJsonMsgLog(msgDesc, msgDir, reqstr, rspstr); });
        //}

        public void InserIntoWMSJsonMsgLog(string msgDesc, MsgDirection msgDir, string req_msg, string rsp_msg)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@MsgDesc",SqlNull(msgDesc)),
                    new SqlParameter("@MsgDirection",SqlNull(msgDir)),
                    new SqlParameter("@ReqMsg",SqlNull(req_msg)),
                    new SqlParameter("@RspMsg",SqlNull(rsp_msg)),
                };
                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[WCS2WMSJsonLog] " +
                    "([MsgDesc],[MsgDirection],[ReqMsg],[RspMsg])VALUES" +
                    "(@MsgDesc, @MsgDirection , @ReqMsg ,@RspMsg)", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("InserIntoWMSJsonMsgLog ", ex);
            }
        }
    }

    public class AgvDevice
    {
        public string DeviceID { get; set; }
    }

    public class WCSToHkReqMsg
    {

        //"reqCode": "",请求编号，每个请求都要一个唯一编号， 同一个请求重复提交， 使用同一编号。

        public string reqCode { get; set; }

        //"reqTime": "", 请求时间截 格式: “yyyy-MM-dd HH:mm:ss”。 

        public string reqTime { get; set; }

        //"clientCode": "",

        public string clientCode { get; set; }

        //"tokenCode": "",

        public string tokenCode { get; set; }

        //"taskTyp": "F01",任务类型，与在RCS-2000端配置的主任务类型编号一致。

        public string taskTyp { get; set; }

        //"sceneTyp": "",

        //public string sceneTyp { get; set; }

        //"ctnrTyp": "",

        public string ctnrTyp { get; set; }

        //"ctnrCode": "",

        public string ctnrCode { get; set; }

        //"wbCode": "",
        public string ctnrNum {  get; set; }
        public string taskMode {  get; set; }

        public string wbCode { get; set; }

        public List<PositionCodePath> positionCodePath { get; set; }

        //"podCode": "100001",货架编号，不指定货架可以为空

        public string podCode { get; set; }

        //"podDir": "",

        public string podDir { get; set; }

        //"podTyp": "",

        public string podTyp { get; set; }

        //"materialLot": "",

        public string materialLot { get; set; }
        public string materialType {  get; set; }

        //"priority": "",

        public string priority { get; set; }

        //"agvCode": "",

        public string agvCode { get; set; }

        //"taskCode": "",任务单号

        public string taskCode { get; set; }
        public string groupId {  get; set; }
        public string agvTyp {  get; set; }
        public string positionSelStrategy { get; set; }

        //"data": ""

        public string data { get; set; }

        public WCSToHkReqMsg()

        {

            reqTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            reqCode = Guid.NewGuid().ToString();

        }

        public WCSToHkReqMsg(WCSTask wCSTask)
        {

            reqTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            reqCode = Guid.NewGuid().ToString("N");
            if (wCSTask != null)
            {
                taskCode = wCSTask.WCSSeqID;

                //List<PositionCodePath> positionCodePathList = new List<PositionCodePath>();

                positionCodePath = new List<PositionCodePath>();

                PositionCodePath positionCodePathfm = new PositionCodePath();

                positionCodePathfm.positionCode = wCSTask.FmLocation;

                positionCodePathfm.type = "00";

                positionCodePath.Add(positionCodePathfm);

                taskTyp = wCSTask.TaskType.ToString();
                #region 此段程序用于AGV任务从A点到B点，经过途径点C需要将托盘放下交换叉取方向
                //string[] tolocations = { "XD222", "XD223" };
                ////string[] mdlocations = { "XD555", "XD556" };
                //if (tolocations.Contains(wCSTask.ToLocation) && !tolocations.Contains(wCSTask.FmLocation))
                //{
                //    PositionCodePath positionCodePathto1 = new PositionCodePath();
                //    positionCodePathto1.positionCode = "XD555";
                //    positionCodePathto1.type = "00";
                //    positionCodePath.Add(positionCodePathto1);

                //    PositionCodePath positionCodePathto2 = new PositionCodePath();
                //    positionCodePathto2.positionCode = "XD556";
                //    positionCodePathto2.type = "00";
                //    positionCodePath.Add(positionCodePathto2);
                //    taskTyp = "XDHX";
                //}
                //if (!tolocations.Contains(wCSTask.ToLocation) && tolocations.Contains(wCSTask.FmLocation))
                //{
                //    PositionCodePath positionCodePathto1 = new PositionCodePath();
                //    positionCodePathto1.positionCode = "XD556";
                //    positionCodePathto1.type = "00";
                //    positionCodePath.Add(positionCodePathto1);

                //    PositionCodePath positionCodePathto2 = new PositionCodePath();
                //    positionCodePathto2.positionCode = "XD555";
                //    positionCodePathto2.type = "00";
                //    positionCodePath.Add(positionCodePathto2);
                //    taskTyp = "XDHX";
                //}
                #endregion
                PositionCodePath positionCodePathto = new PositionCodePath();
                positionCodePathto.positionCode = wCSTask.ToLocation;
                positionCodePathto.type = "00";
                positionCodePath.Add(positionCodePathto);

                //ctnrTyp = "1";

                //AgvPosition agvPositionfm = AgvManager.Instance.AgvPositionList.Find(x => x.Position == wCSTask.FmLocation);
                //AgvPosition agvPositionto = AgvManager.Instance.AgvPositionList.Find(x => x.Position == wCSTask.ToLocation);
                //if (agvPositionfm.UndrCanDO && agvPositionto.UndrCanDO)
                //{
                //    taskTyp = "A104";
                //    positionCodePathto.positionCode = agvPositionto.UndrPositionID;
                //    positionCodePathfm.positionCode = agvPositionfm.UndrPositionID;
                //}
            }
        }
    }

    public class WCSToHkContinueReqMsg
    {
        //"reqCode": "",请求编号，每个请求都要一个唯一编号， 同一个请求重复提交， 使用同一编号。

        public string reqCode { get; set; }

        //"reqTime": "", 请求时间截 格式: “yyyy-MM-dd HH:mm:ss”。 

        public string reqTime { get; set; }

        //"clientCode": "",

        public string clientCode { get; set; }

        //"tokenCode": "",

        public string tokenCode { get; set; }

        //"wbCode": "",

        public string wbCode { get; set; }

        //public List<PositionCodePath> positionCodePath { get; set; }

        //"podCode": "100001",货架编号，不指定货架可以为空

        public string podCode { get; set; }

        //"agvCode": "",

        public string agvCode { get; set; }

        //"taskCode": "",任务单号

        public string taskCode { get; set; }
        public string taskSeq { get; set; }
        public PositionCodePath nextPositionCode { get; set; }


        public WCSToHkContinueReqMsg()
        {
            reqTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            reqCode = Guid.NewGuid().ToString();
        }

        public WCSToHkContinueReqMsg(WCSTask wCSTask)
        {
            reqTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            reqCode = Guid.NewGuid().ToString("N");

            if (wCSTask != null)
            {
                taskCode = wCSTask.WCSSeqID;

                nextPositionCode = new PositionCodePath();
                nextPositionCode.positionCode = wCSTask.ToLocation;
                nextPositionCode.type = "00";
            }
        }
    }
    public class WCSToHkReleasePositionReqMsg
    {
        public string reqCode {  get; set; }
        public string reqTime {  get; set; }
        public string clientCode {  get; set; }
        public string tokenCode {  get; set; }
        public string podCode {  get; set; }
        public string positionCode {  get; set; }
        public string podDir {  get; set; }
        public string characterValue {  get; set; }
        public string indBind {  get; set; }
        public WCSToHkReleasePositionReqMsg()
        {
            reqTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            reqCode = Guid.NewGuid().ToString();
        }
    }

    public class WCSToHkRsqMsg

    {

        // { "code": "1",

        // "data": "", "interrupt": false, "message": "站点集合中有空站点", "msgErrCode": "0x3a80B001", "reqCode": "228b2a1c5a144287b45dae864d5216be"}

        public string code { get; set; }

    }

    public class PositionCodePath
    {
        //"positionCode": "p01",

        public string positionCode { get; set; }

        //"type": "00"

        public string type { get; set; }
    }

    public class AgvPosition : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string SSJID { get; set; }

        public string Position { get; set; }
        public string Describe { get; set; }

        public string SSJPositon { get; set; }

        public string FLPosition { get; set; }
        public string FLLPosition { get; set; }
        public int GroupID { set; get; }
        public AgvPositionSystemType PositionType { get; set; }

        private string palletNo;

        public string PalletNo
        {

            get { return palletNo; }
            set
            {
                palletNo = value;
                Notify(nameof(PalletNo));
            }
        }

        private AgvPositionStatus positionStatus;

        public AgvPositionStatus PositionStatus
        {
            get { return positionStatus; }
            set
            {
                positionStatus = value;
                Notify(nameof(PositionStatus));
            }
        }

        private bool isAvailable;

        public bool IsAvailable
        {
            get { return isAvailable; }
            set
            {
                isAvailable = value;
                Notify(nameof(IsAvailable));
            }
        }
        public bool ForkCanDo { get; set; }
        public bool UndrCanDO { get; set; }

        //XD223 XD222
        public bool NeedChg { get; set; }
        public string ForkPositionID { set; get; }
        public string UndrPositionID { set; get; }
    }

    public enum AgvPositionSystemType
    {
        [Description("类型未知")]
        None,
        [Description("AGV货位")]
        GoodLocation,
        [Description("出库站台")]
        SSJDeviceBlockOut,
        [Description("入库站台")]
        SSJDeviceBlockIn,
    }

    public enum AgvPositionStatus
    {
        [Description("位置为空")]
        None,
        [Description("位置中")]
        InPlace,
        [Description("正在入位")]
        ToPlace,
        [Description("正在出位")]
        FmPlace,
    }
}