using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace BMHRI.WCS.Server.Models
{
    public class WMSDBManager
    {
        private static readonly Lazy<WMSDBManager> lazy = new(() => new WMSDBManager());
        public static WMSDBManager Instance { get { return lazy.Value; } }
        private static string _connectionStr;
        public static string ConnectionStr
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionStr))
                {
                    Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    _connectionStr = appConfig.AppSettings.Settings["WMSSqlServerConnectionString"].Value;
                }
                return _connectionStr;
            }
        }
        const string to_good_location_wrong = "目的货位错误";
        const string fm_good_location_wrong = "起始货位错误";
        const string to_port_location_wrong = "目的地址错误";
        const string fm_port_location_wrong = "起始地址错误";
        const string task_create_ok = "任务创建成功";
        const string task_exists = "任务已存在";
        const string task_change_ok = "任务修改成功";
        const string task_find_failed = "WMS任务未找到";
        const string AGVTaskType_ApplyGet = "AGV申请取货";
        const string AGVTaskType_ApplySet = "AGV申请放货";
        const string AGVTaskType_ApplyEmptyPallet = "AGV申请空托盘";
        const string AGVTaskType_GetComplete = "AGV取货完成";
        const string AGVTaskType_SetComplete = "AGV放货完成";
        const string AgvNotAllowedSet_Re = "AGV不允许放货";
        const string WMSTaskSouce = "WMS";
        const string WCSTaskSouce = "WCS";
        const string DDJTaskStartStatus = "1";
        const string SSJTaskStartStatus = "2";
        const string AgvAllowedGet = "3";
        const string AgvAllowedSet = "4";
        const string AgvNotAllowedGet="5";
        const string AgvNotAllowedSet="6";
        const string SSJ1026Arrived = "7";

        const string AgvApplyGet = "3";
        const string AgvApplySet = "4";
        const string AgvApplyEmptyPallet = "5";
        const string AgvGetComplete = "6";
        const string AgvSetComplete = "7";
        //const string SSJPortModeIn = "7";
        //const string SSJPortModeOut = "8";


        private Timer WriteToWMSDBTimer;
        private Timer ReadFmWMSDBTimer;
        public void Start()
        {
           // Task.Factory.StartNew(() => CreateWMSDBDependency());
            CreateWriteToWMSDBTimer();
            CreateReadFmWMSDBTimer();
        }
        private void CreateReadFmWMSDBTimer()
        {
            ReadFmWMSDBTimer = new Timer(new TimerCallback(ReadFmWMSDB), null, 0, 1000);
        }
        private bool in_read_fm_wms_timer = false;
        private void ReadFmWMSDB(object state)
        {
            if (in_read_fm_wms_timer) return;
            in_read_fm_wms_timer = true;
            try
            {
                using SqlConnection MyConnection = new SqlConnection(ConnectionStr);
                using SqlCommand MyCommand = new SqlCommand("SELECT [TRANS],[TKDAT],[DEMO1],[DEMO2],[DEMO3],[guid],[Task_Priority],[ID]  FROM[dbo].[LTK_TO_MONI]", MyConnection);
                SystemManager.Instance.UpdateWmsConnectStatus("WMS", SystemStatus.Connected);
                MyCommand.CommandType = CommandType.Text;
                MyConnection.Open();
                using SqlDataReader sdr = MyCommand.ExecuteReader();
                while (sdr.Read())
                {
                    WMSMessage wMSMessage = new WMSMessage();
                    wMSMessage.TRANS = sdr["TRANS"].ToString();
                    wMSMessage.guid = sdr["guid"].ToString();
                    wMSMessage.TKDAT = (DateTime)sdr["TKDAT"];
                    wMSMessage.DEMO1 = sdr["DEMO1"].ToString();
                    wMSMessage.DEMO2 = sdr["DEMO2"].ToString();
                    wMSMessage.DEMO3 = sdr["DEMO3"].ToString();
                    wMSMessage.MsgDirection = WMSMessageDirection.Input;
                    wMSMessage.ReturnMsg = ProcessWMSMessage(wMSMessage);
                    InsertWMSMsgToLog(wMSMessage);
                    if(wMSMessage.ReturnMsg!=AgvNotAllowedSet_Re) //AGV不允许放货 不删
                        DeletWMSDBMessage(wMSMessage.guid);
                }
                sdr.Close();
            }
            catch (Exception ex)
            {
                SystemManager.Instance.UpdateWmsConnectStatus("WMS", SystemStatus.DisConnected);
                LogHelper.WriteLog("GetWMSMessageFromDB ", ex);
            }
            in_read_fm_wms_timer = false;
        }
        private void CreateWriteToWMSDBTimer()
        {
            WriteToWMSDBTimer = new Timer(new TimerCallback(ProcessWmsTasks), null, 0, 500);
        }
        private bool in_write_to_wms_timer = false;
        private void ProcessWmsTasks(object state)
        {
            if (in_write_to_wms_timer) return;
            in_write_to_wms_timer = true;
            try
            {
                List<WMSTask> wMSTasks = WMSTasksManager.Instance.WMSTaskList.FindAll(x =>
                x.TaskStatus == WMSTaskStatus.TaskDone ||
                x.TaskStatus == WMSTaskStatus.Double_Inbound ||
                x.TaskStatus == WMSTaskStatus.UnStack_Empty ||
                x.TaskStatus == WMSTaskStatus.Put_Down_Stop ||
                x.TaskStatus == WMSTaskStatus.Pick_Up_Stop ||
                x.TaskStatus == WMSTaskStatus.SSJ_APP_IN ||
                x.TaskStatus == WMSTaskStatus.SSJ_APP_EM ||
                x.TaskStatus == WMSTaskStatus.DeviceStatusChg||
                x.TaskStatus==WMSTaskStatus.LightLED||
                x.TaskStatus==WMSTaskStatus.UnLightLED||
                x.TaskStatus== WMSTaskStatus.DDJTaskStart||
                x.TaskStatus== WMSTaskStatus.AGVAllowedGet||
                x.TaskStatus== WMSTaskStatus.AGVAllowedSet||
                x.TaskStatus== WMSTaskStatus.SSJTaskStart||
                x.TaskStatus== WMSTaskStatus.AGVNotAllowedGet||
                x.TaskStatus== WMSTaskStatus.AGVNotAllowedSet||
                x.TaskStatus== WMSTaskStatus.SSJ1026Arrive||
                x.TaskStatus== WMSTaskStatus.SSJDirectDone||
                x.TaskStatus== WMSTaskStatus.SSJAlarm||
                x.TaskStatus== WMSTaskStatus.SSJMateriExist||
                x.TaskStatus == WMSTaskStatus.SSJMidArrive||
                x.TaskStatus == WMSTaskStatus.SSJPutMaterial
                );
                if (wMSTasks != null && wMSTasks.Count > 0)
                {

                    foreach (WMSTask wMSTask in wMSTasks)
                    {
                        WMSMessage wMSMessage = new WMSMessage();
                        if (wMSTask.TaskSource == "WCS")
                        {
                            WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                            continue;
                        }
                        switch (wMSTask.TaskStatus)
                        {
                            case WMSTaskStatus.SSJ_APP_IN:

                                wMSMessage.CreateHMessageToWMS(wMSTask.PalletNum, wMSTask.FmLocation, wMSTask.WeightNum, ((int)wMSTask.GaoDiBZ).ToString(),wMSTask.udf01, null);
                                wMSMessage.MsgParse = "WCS向WMS申请从输送机入库 托盘" + wMSMessage.GetPalletNum() + ",起始地" + wMSMessage.Get1Port();
                                //wMSMessage.CreateHMessageToWMS(wMSTask.PalletNum, wMSTask.FmLocation, null, null, null);
                                break;
                            case WMSTaskStatus.TaskDone:
                                switch(wMSTask.TaskType)
                                {
                                    case WMSTaskType.Directe:
                                    case WMSTaskType.Inbound:
                                        wMSMessage.CreateDMessageToWMS(wMSTask.PalletNum, wMSTask.ToLocation);
                                        wMSMessage.MsgParse = "WCS向WMS申请货位 托盘" + wMSMessage.GetPalletNum() + ",起始地" + wMSMessage.Get1Port();
                                        break;
                                    case WMSTaskType.Moving:
                                        wMSMessage.CreateMMessageToWMS(wMSTask.PalletNum, wMSTask.ToLocation, wMSTask.FmLocation);
                                        wMSMessage.MsgParse = "WCS向WMS反馈倒库完成 托盘" + wMSMessage.GetPalletNum() + ",起始地" + wMSMessage.Get1Goodlocation() + ",目的地" + wMSMessage.Get2Goodlocation();
                                        break;
                                    case WMSTaskType.Outbound:
                                    case WMSTaskType.Picking:
                                        wMSMessage.CreateFMessageToWMS(wMSTask.PalletNum, wMSTask.ToLocation, wMSTask.FmLocation);
                                        wMSMessage.MsgParse = "WCS向WMS反馈出库完成 托盘" + wMSMessage.GetPalletNum() + ",起始地" + wMSMessage.Get1Goodlocation() + ",目的地" + wMSMessage.Get1Port();
                                        break;
                                    case WMSTaskType.Stacking:
                                        wMSMessage.CreateEMessageToWMS(wMSTask.PalletNum, wMSTask.ToLocation, wMSTask.FmLocation);
                                        wMSMessage.MsgParse = "WCS向WMS反馈入库完成 托盘" + wMSMessage.GetPalletNum() + ",起始地" + wMSMessage.Get1Port() + ",目的地" + wMSMessage.Get1Goodlocation();
                                        break;
                                    case WMSTaskType.NoTaskQuit:
                                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                                        break;
                                    case WMSTaskType.SSJTo1026:
                                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                                        break;
                                }
                                //wMSMessage.CreateHMessageToWMS(wMSTask.PalletNum, wMSTask.FmLocation, null, null, null);
                                break;
                            case WMSTaskStatus.DeviceStatusChg:
                                string deviceStatus = "";
                                if (wMSTask.ToLocation == "B")
                                {
                                    deviceStatus = "脱机";
                                }
                                else if (wMSTask.ToLocation == "D")
                                {
                                    deviceStatus = "待机";
                                }
                                else if (wMSTask.ToLocation == "@")
                                {
                                    deviceStatus = "故障";
                                }
                                if (Convert.ToInt32(wMSTask.PalletNum) <= 7)
                                {
                                    wMSMessage.CreateSMessageToWMS(wMSTask.PalletNum, wMSTask.ToLocation);
                                    wMSMessage.MsgParse = "WCS向WMS反馈堆垛机状态 " + wMSTask.PalletNum + "号堆垛机 " + deviceStatus;
                                }
                                else
                                {
                                    wMSMessage.CreateSSJSMessageToWMS(wMSTask.PalletNum, wMSTask.WeightNum, wMSTask.FmLocation, wMSTask.ToLocation);
                                    wMSMessage.MsgParse = "WCS向WMS反馈输送机状态 " + wMSTask.FmLocation + "设备 " + deviceStatus + "故障代码" + wMSTask.WeightNum;
                                }
                                break;
                            case WMSTaskStatus.Double_Inbound:
                                wMSMessage.CreateQMessageToWMS(wMSTask.PalletNum, wMSTask.ToLocation, wMSTask.FmLocation,"10");
                                wMSMessage.MsgParse = "WCS向WMS反馈堆垛机双重入库 托盘" + wMSTask.PalletNum + ",起始地" + wMSMessage.Get1Port() + ",目的地" + wMSMessage.Get1Goodlocation();
                                break;
                            case WMSTaskStatus.Pick_Up_Stop:
                                wMSMessage.CreateQMessageToWMS(wMSTask.PalletNum, wMSTask.FmLocation, wMSTask.ToLocation, "30");
                                wMSMessage.MsgParse = "WCS向WMS反馈堆垛机远端出库近端有货 托盘" + wMSTask.PalletNum + ",起始地" + wMSMessage.Get1Goodlocation() + ",目的地" + wMSMessage.Get1Port();
                                //WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                                break;
                            case WMSTaskStatus.Put_Down_Stop:
                                wMSMessage.CreateQMessageToWMS(wMSTask.PalletNum, wMSTask.ToLocation, wMSTask.FmLocation, "40");
                                wMSMessage.MsgParse = "WCS向WMS反馈堆垛机远端入库近端有货 托盘" + wMSTask.PalletNum + ",起始地" + wMSMessage.Get1Port() + ",目的地" + wMSMessage.Get1Goodlocation();
                                break;
                            case WMSTaskStatus.UnStack_Empty:
                                //wMSMessage.CreateQMessageToWMS(wMSTask.PalletNum, wMSTask.FmLocation, wMSTask.ToLocation, "20");
                                wMSMessage.CreateFMessageToWMS(wMSTask.PalletNum, wMSTask.ToLocation, wMSTask.FmLocation);
                                wMSMessage.MsgParse = "WCS向WMS反馈堆垛机空出库 托盘" + wMSTask.PalletNum + ",起始地" + wMSMessage.Get1Goodlocation() + ",目的地" + wMSMessage.Get1Port();
                                break;
                            case WMSTaskStatus.SSJ_APP_EM:
                                wMSMessage.CreatePMessageToWMS(wMSTask.FmLocation,wMSTask.PalletNum);
                                string box_type = "";
                                if (wMSTask.PalletNum == "1")
                                    box_type = "蓝箱";
                                else if (wMSTask.PalletNum == "2")
                                    box_type = "白箱";
                                else
                                    box_type = "无";
                                wMSMessage.MsgParse = "WCS向WMS申请空托盘垛 托盘" + wMSTask.PalletNum + ",目的地" + wMSTask.FmLocation + ",料箱/托盘类型" + box_type;
                                //WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                                break;
                            case WMSTaskStatus.LightLED:
                                wMSMessage.CreateIMessageToWMS(wMSTask.PalletNum, wMSTask.ToLocation, wMSTask.FmLocation);
                                wMSMessage.MsgParse = "WCS向WMS反馈点亮LED信号 托盘" + wMSTask.PalletNum + ",目的地" + wMSTask.ToLocation;
                                break;
                            case WMSTaskStatus.UnLightLED:
                                wMSMessage.CreateXMessageToWMS(wMSTask.PalletNum,wMSTask.ToLocation);
                                wMSMessage.MsgParse = "WCS向WMS反馈熄灭LED信号 托盘" + wMSTask.PalletNum + ",目的地" + wMSTask.ToLocation;
                                break;
                            case WMSTaskStatus.DDJTaskStart:
                                wMSMessage.CreateNMessageToWMS(wMSTask.PalletNum, DDJTaskStartStatus);
                                wMSMessage.MsgParse = "WCS向WMS反馈堆垛机开始工作 托盘" + wMSTask.PalletNum ;
                                break;
                            case WMSTaskStatus.SSJTaskStart:
                                wMSMessage.CreateNMessageToWMS(wMSTask.PalletNum, SSJTaskStartStatus);
                                wMSMessage.MsgParse = "WCS向WMS反馈输送机正在出库 托盘" + wMSTask.PalletNum;
                                break;
                            case WMSTaskStatus.AGVAllowedGet:
                                wMSMessage.CreateNMessageToWMS(wMSTask.PalletNum, AgvAllowedGet);
                                wMSMessage.MsgParse = "WCS向WMS反馈AGV允许取货 托盘" + wMSTask.PalletNum;
                                break;
                            case WMSTaskStatus.AGVAllowedSet:
                                wMSMessage.CreateNMessageToWMS(wMSTask.PalletNum, AgvAllowedSet);
                                wMSMessage.MsgParse = "WCS向WMS反馈AGV允许放货 托盘" + wMSTask.PalletNum ;
                                break;
                            case WMSTaskStatus.AGVNotAllowedGet:
                                wMSMessage.CreateNMessageToWMS(wMSTask.PalletNum, AgvNotAllowedGet);
                                wMSMessage.MsgParse = "WCS向WMS反馈AGV不允许取货 托盘" + wMSTask.PalletNum;
                                break;
                            case WMSTaskStatus.AGVNotAllowedSet:
                                wMSMessage.CreateNMessageToWMS(wMSTask.PalletNum, AgvNotAllowedSet);
                                wMSMessage.MsgParse = "WCS向WMS反馈AGV不允许放货 托盘" + wMSTask.PalletNum;
                                break;
                            //case WMSTaskStatus.SSJPortModeIn:
                            //    wMSMessage.CreateNMessageToWMS(wMSTask.PalletNum, SSJPortModeIn,wMSTask.ToLocation);
                            //    break;
                            //case WMSTaskStatus.SSJPortModeOut:
                            //    wMSMessage.CreateNMessageToWMS(wMSTask.PalletNum, SSJPortModeOut,wMSTask.ToLocation);
                            //    break;
                            case WMSTaskStatus.SSJ1026Arrive:
                                wMSMessage.CreateNMessageToWMS(wMSTask.PalletNum, SSJ1026Arrived, wMSTask.ToLocation);
                                wMSMessage.MsgParse = "WCS向WMS反馈空托盘已从1027补到1026口 托盘" + wMSTask.PalletNum;
                                break;
                            case WMSTaskStatus.SSJDirectDone:
                                wMSMessage.CreateZMessageToWMS(wMSTask.PalletNum, wMSTask.FmLocation, wMSTask.ToLocation);
                                wMSMessage.MsgParse = "WCS向WMS反馈料箱已直出到位 料箱" + wMSTask.PalletNum;
                                break;
                            case WMSTaskStatus.SSJAlarm:
                                wMSMessage.CreateJMessageToWMS(wMSTask.PalletNum, wMSTask.FmLocation, wMSTask.ToLocation);
                                wMSMessage.MsgParse = "WCS向WMS反馈料箱异常到位 料箱" + wMSTask.PalletNum;
                                break;
                            case WMSTaskStatus.SSJMateriExist:
                                wMSMessage.CreateTMessageToWMS(wMSTask.FmLocation, wMSTask.PalletNum);
                                string materia_exist = "";
                                if (wMSTask.PalletNum == "1")
                                    materia_exist = "无料";
                                else if (wMSTask.PalletNum == "0")
                                    materia_exist = "有料";
                                else
                                    materia_exist = "未知";
                                wMSMessage.MsgParse = "WCS向WMS反馈目的地是否有料" + ",目的地" + wMSTask.FmLocation + ",是否有料:" + materia_exist;
                                break;
                            case WMSTaskStatus.SSJMidArrive:
                                wMSMessage.CreateGMessageToWMS(wMSTask.PalletNum, wMSTask.FmLocation,"1");
                                wMSMessage.MsgParse = "WCS向WMS反馈到达倒料拐角处 料箱" + wMSTask.PalletNum + ",目的地" + wMSTask.FmLocation;
                                break;
                            case WMSTaskStatus.SSJPutMaterial:
                                wMSMessage.CreateGMessageToWMS(wMSTask.PalletNum, wMSTask.FmLocation, "2");
                                wMSMessage.MsgParse = "WCS向WMS反馈开始倒料 料箱" + wMSTask.PalletNum + ",目的地" + wMSTask.FmLocation;
                                break;
                            default:
                                break;
                        }
                        if (!string.IsNullOrEmpty(wMSMessage.TRANS))
                        {
                            if (WriteToWMSDB(wMSMessage) > -1)
                            {
                                WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                            }
                        }
                    }
                }
               // Debug.WriteLine("ProcessWmsTasks "+DateTime.Now.ToShortTimeString());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ProcessWmsTasks", ex);
            }
            in_write_to_wms_timer = false;
        }
        public int WriteToWMSDB(WMSMessage wMSMessage)
        {
            int re_value = -1;
            if (wMSMessage is null) return re_value;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@TRANS",SqlNull(wMSMessage.TRANS)),
                    new SqlParameter("@TKDAT",SqlNull(wMSMessage.TKDAT)),
                    //new SqlParameter("@Task_Priority",wMSMessage.Task_Priority),
                     //new SqlParameter("@guid",SqlNull(wMSMessage.guid)),
                   // new SqlParameter("@DEMO1",SqlNull(wMSMessage.DEMO1)),
                   //new SqlParameter("@DEMO2",SqlNull(wMSMessage.DEMO2)),
                   //new SqlParameter("@DEMO3",SqlNull(wMSMessage.DEMO3)),
                };

                re_value = (int)ExeSQLStringWithParam("INSERT INTO [dbo].[LTK_TO_MANE]" +
                    "([TRANS],[TKDAT]) VALUES　" +
                    "(@TRANS, @TKDAT)", sqlParameters);
                InsertWMSMsgToLog(wMSMessage);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("InsertWMSMsgToLog", ex);
            }
            return re_value;
        }
        private void DeletWMSDBMessage(string guid)
        {
            try
            {
                if (string.IsNullOrEmpty(guid)) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@guid",SqlNull(guid)),
                };

                ExeSQLStringWithParam("DELETE FROM [dbo].[LTK_TO_MONI] WHERE guid =@guid ", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DeletWMSDBMessage", ex);
            }
        }
        public static string ProcessWMSMessage(WMSMessage wMSMessage)
        {
            string return_msg = "未知信息";
            if (wMSMessage == null) return "WMS消息为空";
            if (!wMSMessage.IsRightLength()) return "WMS消息长度错误";
            switch (wMSMessage.GetMessageType())
            {
                case "A":
                    return_msg = CreateInboundTask(wMSMessage);
                    wMSMessage.MsgParse = "WMS下发输送机入库任务 托盘" + wMSMessage.GetPalletNum() + ",起始地" + wMSMessage.Get1Port()+",目的地"+wMSMessage.Get2Port();
                    break;
                case "H":
                    return_msg = CreateSSJQuitTask(wMSMessage);
                    wMSMessage.MsgParse = "WMS下发无任务 托盘" + wMSMessage.GetPalletNum();
                    break;
                case "U":
                    return_msg = CreateStackTask(wMSMessage);
                    wMSMessage.MsgParse = "WMS下发堆垛机入库任务 托盘" + wMSMessage.GetPalletNum() + ",起始地" + wMSMessage.Get1Port() + ",目的地" + wMSMessage.Get1Goodlocation();
                    break;
                case "B":
                    return_msg = CreateOutboundTask(wMSMessage);
                    wMSMessage.MsgParse = "WMS下发出库任务 托盘" + wMSMessage.GetPalletNum() + ",起始地" + wMSMessage.Get1Goodlocation() + ",目的地" + wMSMessage.Get1Port();
                    break;
                case "M":
                    return_msg = CreateMovboundTask(wMSMessage);
                    wMSMessage.MsgParse = "WMS下发堆垛机倒库任务 托盘" + wMSMessage.GetPalletNum() + ",起始地" + wMSMessage.Get1Goodlocation() + ",目的地" + wMSMessage.Get2Goodlocation();
                    break;
                case "C":
                    return_msg = ChangeTask(wMSMessage);
                    wMSMessage.MsgParse = "WMS下发堆垛机双重改址任务 托盘" + wMSMessage.GetPalletNum() + ",起始地" + wMSMessage.Get1Port() + ",目的地改至" + wMSMessage.Get1Goodlocation();
                    break;
                case "Z":
                    return_msg = CreateSSJDirectTask(wMSMessage);
                    break;
                case "O":
                    return_msg = CreateAGVApplyPutGet(wMSMessage);
                    string AGVTaskType = wMSMessage.GetTaskType();
                    string agvTaskDesc = "";
                    if (AGVTaskType == AgvApplyGet)
                    {
                        agvTaskDesc = AGVTaskType_ApplyGet;
                    }
                    else if (AGVTaskType == AgvApplySet)
                    {
                        agvTaskDesc = AGVTaskType_ApplySet;
                    }
                    else if (AGVTaskType == AgvApplyEmptyPallet)
                    {
                        agvTaskDesc = AGVTaskType_ApplyEmptyPallet;
                    }
                    else if (AGVTaskType == AgvGetComplete)
                    {
                        agvTaskDesc = AGVTaskType_GetComplete;
                    }
                    else if (AGVTaskType == AgvSetComplete)
                    {
                        agvTaskDesc = AGVTaskType_SetComplete;
                    }
                    wMSMessage.MsgParse = "WMS下发AGV任务 托盘" + wMSMessage.GetPalletNum() + ",任务类型" + agvTaskDesc + ",地址" + wMSMessage.GetAGVPort();
                    break;
                case "G":
                    return_msg = CreateWeightTask(wMSMessage);
                    wMSMessage.MsgParse = "WMS下发料箱重量 托盘" + wMSMessage.GetPalletNum() + ",当前地址" + wMSMessage.GetWeightPort() + ",重量" + wMSMessage.GetWeigh();
                    break;
                default:
                    break;
            }
            return return_msg;
        }
        private static string CreateWeightTask(WMSMessage wMSMessage)
        {
            string port = wMSMessage.GetWeightPort();
            string pallet_num = wMSMessage.GetPalletNum();
            string weight = wMSMessage.GetWeigh();

            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(port));
            if (sSJDevice == null) return null;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == port);
            if (sSJDeviceBlock == null) return null;

            SSJMessage ssj_0K = new SSJMessage(sSJDevice.PLCID);
            ssj_0K.Set0KMessage(pallet_num, port, weight);
            sSJDevice.InsertIntoSSJSendList(ssj_0K);

            return task_create_ok;
        }
        private static string CreateSSJQuitTask(WMSMessage wMSMessage)
        {
            string fm_location = wMSMessage.Get1Port();
            string to_location = wMSMessage.Get2Port();
            string pallet_num = wMSMessage.GetPalletNum();

            string ssj_id = string.Concat("SSJ0", fm_location[..1]);
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);
            if (sSJDevice == null)
                return fm_port_location_wrong;
            SSJDeviceBlock fm_sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == fm_location);
            if (fm_sSJDeviceBlock == null)
                return fm_port_location_wrong;
            WMSTask wMSTask = new()
            {
                PalletNum = pallet_num,
                FmLocation = fm_location,
                ToLocation = to_location,
                TaskType = WMSTaskType.NoTaskQuit,
            };
            if (int.TryParse(wMSMessage.Task_Priority, out int pri))
            {
                wMSTask.Priority = pri;
            }
            wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
            wMSTask.TaskSource = "WMS";
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
            return task_create_ok;
        }
        private static string CreateSSJDirectTask(WMSMessage wMSMessage)
        {
            string fm_location = wMSMessage.Get1Port();
            string to_location = wMSMessage.Get2Port();
            string pallet_num = wMSMessage.GetPalletNum();

            string ssj_id = string.Concat("SSJ0", fm_location[..1]);
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);
            if (sSJDevice == null)
                return fm_port_location_wrong;
            SSJDeviceBlock fm_sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == fm_location);
            if (fm_sSJDeviceBlock == null)
                return fm_port_location_wrong;
            SSJDeviceBlock to_sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == to_location);
            if (to_sSJDeviceBlock == null)
                return to_port_location_wrong;
            WMSTask wMSTask = new()
            {
                PalletNum = pallet_num,
                FmLocation = fm_location,
                ToLocation = to_location,
                TaskType = WMSTaskType.Directe,
            };
            if (int.TryParse(wMSMessage.Task_Priority, out int pri))
            {
                wMSTask.Priority = pri;
            }
            wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
            wMSTask.TaskSource = "WMS";
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
            return task_create_ok;
        }
        private static string CreateStackTask(WMSMessage wMSMessage)
        {
            string to_Goodlocation = wMSMessage.Get1Goodlocation();
            string fm_location = wMSMessage.Get1Port();
            string pallet_num = wMSMessage.GetPalletNum();
            string gaodi_bz = wMSMessage.GetGaoDi();
            string ssj_id = string.Concat("SSJ0", fm_location[..1]);
            GoodsLocation to_goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == to_Goodlocation);
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);

            if (to_goodsLocation == null)
                return to_good_location_wrong;
            if (sSJDevice == null)
                return fm_port_location_wrong;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == fm_location);
            if (sSJDeviceBlock == null)
                return fm_port_location_wrong;

            WMSTask wMSTask = new()
            {
                PalletNum = pallet_num,
                FmLocation = fm_location,
                ToLocation = to_Goodlocation,
                TaskType = WMSTaskType.Stacking,
            };
            if (int.TryParse(wMSMessage.Task_Priority, out int pri))
            {
                wMSTask.Priority = pri;
            }
            wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
            wMSTask.TaskSource = "WMS";
            if(gaodi_bz=="1"||gaodi_bz=="0")
                wMSTask.GaoDiBZ = WMSGaoDiBZ.Low;
            else if(gaodi_bz=="2")
                wMSTask.GaoDiBZ = WMSGaoDiBZ.Height;
            wMSTask.Floor = sSJDeviceBlock.Floor;
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
            return task_create_ok;
        }
        private static string ChangeTask(WMSMessage wMSMessage)
        {
           string to_Goodlocation = wMSMessage.Get1Goodlocation();
            string fm_location = wMSMessage.Get1Port();
            string pallet_num = wMSMessage.GetPalletNum();
            string ssj_id = string.Concat("SSJ0", fm_location[..1]);
            GoodsLocation to_goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == to_Goodlocation);
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);

            if (to_goodsLocation == null)
                return to_good_location_wrong;
            if (sSJDevice == null)
                return fm_port_location_wrong;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == fm_location);
            if (sSJDeviceBlock == null)
                return fm_port_location_wrong;

            WMSTask wMSTask = new()
            {
                PalletNum = pallet_num,
                FmLocation = fm_location,
                ToLocation = to_Goodlocation,
                TaskType = WMSTaskType.Stacking,
            };
            if (int.TryParse(wMSMessage.Task_Priority, out int pri))
            {
                wMSTask.Priority = pri;
            }
            wMSTask.TaskStatus = WMSTaskStatus.GoodLocChg;
            wMSTask.TaskSource = "WMS";
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
            return task_create_ok;
        }
        private static string CreateMovboundTask(WMSMessage wMSMessage)
        {
            string fm_Goodlocation = wMSMessage.Get1Goodlocation();
            string to_Goodlocation = wMSMessage.Get2Goodlocation();
            string pallet_num = wMSMessage.GetPalletNum();
            GoodsLocation to_goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == to_Goodlocation);
            if (to_goodsLocation == null)
                return to_good_location_wrong;
            GoodsLocation fm_goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == fm_Goodlocation);
            if (fm_goodsLocation == null)
                return fm_good_location_wrong;
            WMSTask wMSTask = new()
            {
                PalletNum = pallet_num,
                FmLocation = fm_Goodlocation,
                ToLocation = to_Goodlocation,
                TaskType = WMSTaskType.Moving,
            };
            if (int.TryParse(wMSMessage.Task_Priority, out int pri))
            {
                wMSTask.Priority = pri;
            }
            wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
            wMSTask.TaskSource = "WMS";
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
            return task_create_ok;
        }
        private static string CreateOutboundTask(WMSMessage wMSMessage)
        {
            string fm_Goodlocation = wMSMessage.Get1Goodlocation();
            string to_location = wMSMessage.Get1Port();
            string pallet_num = wMSMessage.GetPalletNum();
            string ssj_id = string.Concat("SSJ0", to_location[..1]);
            if (ssj_id == "SSJ04") 
                ssj_id = "SSJ03";
            string outbound_type = wMSMessage.GetOutBoundType();
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == fm_Goodlocation);
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);

            if (goodsLocation == null)
                return fm_good_location_wrong;
            if (sSJDevice == null)
                return to_good_location_wrong;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == to_location);
            if (sSJDeviceBlock == null)
            {
                if (to_location.Substring(0, 1) != "4")
                    return to_port_location_wrong;
            }

            WMSTask wMSTask = new()
            {
                PalletNum = pallet_num,
                FmLocation = fm_Goodlocation,
                ToLocation = to_location,
                //TaskType = WMSTaskType.Outbound,
            };
            if (outbound_type == "0")
                wMSTask.TaskType = WMSTaskType.Outbound;
            else if (outbound_type == "2")
                wMSTask.TaskType = WMSTaskType.Picking;
            if (int.TryParse(wMSMessage.Task_Priority, out int pri))
            {
                wMSTask.Priority = pri;
            }
            wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
            wMSTask.TaskSource = "WMS";
            if(wMSTask.ToLocation.Substring(0,1)=="4")
                wMSTask.Floor = 2;
            else
                wMSTask.Floor = sSJDeviceBlock.Floor;
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
            return task_create_ok;
        }
        private static string GetSSJPLCID(string port)
        {
            string SSJPLCID = null;
            if(port.Length!=4) return null;
            SSJPLCID = string.Concat("SSJ0", port.AsSpan(0, 1));
            return SSJPLCID;
        }
        private static string CreateAGVApplyPutGet(WMSMessage wMSMessage)    //O 80000145 3 1026 0000000000000000
        {
            string AGVTaskType = wMSMessage.GetTaskType();
            string port = wMSMessage.GetAgvPort();
            string pallet_num = wMSMessage.GetPalletNum();
            if (AGVTaskType== AgvApplyGet)
            {
                SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(port));
                if (sSJDevice == null) return null;
                SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == port && x.AGVGetPoint == true);
                if(sSJDeviceBlock == null) return null;
                if (sSJDeviceBlock.IsOccupied && sSJDeviceBlock.PalletNum == pallet_num )    //&&sSJDeviceBlock.CurrentMode== DeviceModeType.OutboundMode
                {
                    WMSTask wMSTask=new WMSTask();
                    wMSTask.PalletNum = pallet_num;
                    wMSTask.TaskSource = "WMS";
                    wMSTask.TaskStatus = WMSTaskStatus.AGVAllowedGet;
                    WMSTasksManager.Instance.AddWMSTask(wMSTask);
                }
                else
                {
                    WMSTask wMSTask = new WMSTask();
                    wMSTask.PalletNum = pallet_num;
                    wMSTask.TaskSource = "WMS";
                    wMSTask.TaskStatus = WMSTaskStatus.AGVNotAllowedGet;
                    WMSTasksManager.Instance.AddWMSTask(wMSTask);
                }
            }
            else if(AGVTaskType== AgvApplySet)
            {
                
                SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(port));
                if (sSJDevice == null) return null;
                SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == port && x.AGVSetPoint==true);
                if (sSJDeviceBlock == null) return null;
                if (!sSJDeviceBlock.IsOccupied )   //&&sSJDeviceBlock.CurrentMode== DeviceModeType.InboundMode
                {
                    WMSTask wMSTask = new WMSTask();
                    wMSTask.PalletNum = pallet_num;
                    wMSTask.TaskSource = "WMS";
                    wMSTask.TaskStatus = WMSTaskStatus.AGVAllowedSet;
                    WMSTasksManager.Instance.AddWMSTask(wMSTask);
                }
                else
                {
                    //WMSTask wMSTask = new WMSTask();
                    //wMSTask.PalletNum = pallet_num;
                    //wMSTask.TaskSource = "WMS";
                    //wMSTask.TaskStatus = WMSTaskStatus.AGVNotAllowedSet;
                    //WMSTasksManager.Instance.AddWMSTask(wMSTask);
                    return AgvNotAllowedSet_Re;
                }
            }
            else if (AGVTaskType == AgvApplyEmptyPallet)
            {
                //SSJMessage ssj_0S = new SSJMessage(GetSSJPLCID(port));
                //ssj_0S.Set0SMessage(pallet_num, "1027", port);
                //SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(port));
                //if (sSJDevice != null)
                //    sSJDevice.InsertIntoSSJSendList(ssj_0S);
                WMSTask sSJTask = new()
                {
                    FmLocation = "1027",
                    ToLocation = port,
                    PalletNum = pallet_num,
                    TaskType = WMSTaskType.SSJTo1026,
                };
                if (int.TryParse(wMSMessage.Task_Priority, out int pri))
                {
                    sSJTask.Priority = pri;
                }
                sSJTask.TaskStatus = WMSTaskStatus.TaskAssigned;
                sSJTask.TaskSource = "WMS";
                WMSTasksManager.Instance.AddWMSTask(sSJTask);
            }
            else if (AGVTaskType == AgvGetComplete)
            {
                SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(port));
                if (sSJDevice != null)
                    sSJDevice.ClearOcupty(wMSMessage.GetPalletNum());
            }
            else if (AGVTaskType == AgvSetComplete)   //O 90000270 7 1001 0000000000000000
            {
                SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(port));
                if (sSJDevice == null) return null;
                SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == port && x.AGVSetPoint == true);
                if (sSJDeviceBlock == null) return null;
                if (!string.IsNullOrEmpty(sSJDeviceBlock.AGVToPosition))
                {
                    WCSTask wCSOutTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == pallet_num && (x.TaskType == WCSTaskTypes.SSJOutbound || x.TaskType == WCSTaskTypes.SSJPickUpOutbound));
                    if (wCSOutTask != null)    //网带炉区域出库任务，AGV放货完成;抛丸区满箱出库，AGV放至上料口
                    {
                        SSJMessage ssj_0b = new SSJMessage(sSJDevice.PLCID);
                        ssj_0b.Set0bMessage(wCSOutTask.PalletNum, port, wCSOutTask.ToLocation, "0");
                        sSJDevice.InsertIntoSSJSendList(ssj_0b);
                    }
                    else      //冷制区域AGV放货口入库； 滚加工西侧3260入库 ；抛丸区回库3145放货完成   
                    {
                        SSJMessage sSJMessage = new SSJMessage(sSJDevice.PLCID);
                        sSJMessage.Set0GMessage(pallet_num, port, sSJDeviceBlock.AGVToPosition);
                        sSJDevice.InsertIntoSSJSendList(sSJMessage);
                    }
                }
                else
                {
                    WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == pallet_num && (x.TaskType == WCSTaskTypes.SSJInbound || x.TaskType == WCSTaskTypes.SSJOutbound || x.TaskType == WCSTaskTypes.SSJPickUpOutbound));
                    if (wCSTask == null)
                    {
                        //滚加工接驳架料箱转到网带炉3154提升机入库(针对滚加工提升机异常)
                        if (sSJDeviceBlock.AreaType == AreaType.NetStove)
                        {
                            WMSTask wMSTask = new WMSTask();
                            wMSTask.TaskType = WMSTaskType.Inbound;
                            wMSTask.TaskStatus = WMSTaskStatus.SSJ_APP_IN;
                            wMSTask.PalletNum = pallet_num;
                            wMSTask.FmLocation = sSJDeviceBlock.Position;
                            wMSTask.TaskSource = "WMS";
                            wMSTask.WeightNum = "0000";
                            wMSTask.GaoDiBZ = WMSGaoDiBZ.Low;
                            wMSTask.Floor = sSJDeviceBlock.Floor;
                            WMSTasksManager.Instance.AddWMSTask(wMSTask);
                        }
                        else
                            return null;
                    }
                    //网带炉料箱入库 放置提升机口
                    else
                    {
                        SSJDeviceBlock to_blocky = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == wCSTask.ToLocation && x.SystemType == DeviceSystemType.InboundFinish && x.Floor == sSJDeviceBlock.Floor);
                        if (to_blocky == null) return null;
                        if (sSJDevice.SSJWorkState == SSJDeviceWorkState.Online || sSJDevice.SSJWorkState == SSJDeviceWorkState.Fault)
                        {
                            SSJMessage sSJMessage = new SSJMessage(sSJDevice.PLCID);
                            sSJMessage.Set0GMessage(pallet_num, port, to_blocky.Position);
                            sSJDevice.InsertIntoSSJSendList(sSJMessage);
                        }
                    }
                }
            }
            return task_create_ok;
        }
        private static string CreateInboundTask(WMSMessage wMSMessage)
        {
            string fm_location = wMSMessage.Get1Port();
            string to_location = wMSMessage.Get2Port();
            string pallet_num = wMSMessage.GetPalletNum();
            string gaodi_bz = wMSMessage.GetGaoDi();

            string ssj_id = string.Concat("SSJ0", fm_location[..1]);
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);
            if (sSJDevice == null)
                return fm_port_location_wrong;
            SSJDeviceBlock fm_sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == fm_location);
            if (fm_sSJDeviceBlock == null)
                return fm_port_location_wrong;
            SSJDeviceBlock to_sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == to_location&&x.SystemType==DeviceSystemType.InboundFinish);
            if (to_sSJDeviceBlock == null)
                return to_port_location_wrong;
            //WMSTask haveWMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == pallet_num && x.TaskType == WMSTaskType.Inbound &&x.TaskStatus== WMSTaskStatus.TaskAssigned && x.TaskSource == "WMS");
            //if (haveWMSTask == null)
            //{
                WMSTask wMSTask = new()
                {
                    PalletNum = pallet_num,
                    FmLocation = fm_location,
                    ToLocation = to_location,
                    TaskType = WMSTaskType.Inbound,
                };
                if (int.TryParse(wMSMessage.Task_Priority, out int pri))
                {
                    wMSTask.Priority = pri;
                }
                wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
                wMSTask.TaskSource = "WMS";
                if (gaodi_bz == "1" || gaodi_bz == "0")
                    wMSTask.GaoDiBZ = WMSGaoDiBZ.Low;
                else if (gaodi_bz == "2")
                    wMSTask.GaoDiBZ = WMSGaoDiBZ.Height;
                wMSTask.Floor = fm_sSJDeviceBlock.Floor;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            //}
            //else
            //    return task_exists;
            return task_create_ok;
        }
        public static void InsertWMSMsgToLog(WMSMessage wMSMessage)
        {
            if (wMSMessage == null) return;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@TRANS",SqlNull(wMSMessage.TRANS)),
                    new SqlParameter("@MsgDirection",SqlNull(wMSMessage.MsgDirection)),
                    //new SqlParameter("@TKDAT",SqlNull(wMSMessage.TKDAT)),
                    new SqlParameter("@TKDAT",DateTime.Now),
                    new SqlParameter("@Task_Priority",SqlNull(wMSMessage.Task_Priority)),
                    new SqlParameter("@guid",SqlNull(wMSMessage.guid)),
                    new SqlParameter("@DEMO1",SqlNull(wMSMessage.DEMO1)),
                    new SqlParameter("@DEMO2",SqlNull(wMSMessage.DEMO2)),
                    new SqlParameter("@DEMO3",SqlNull(wMSMessage.DEMO3)),
                    new SqlParameter("@ReturnMsg",SqlNull(wMSMessage.ReturnMsg)),
                    new SqlParameter("@MsgParse",SqlNull(wMSMessage.MsgParse))
                };

                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[MsgFromWMSLog]" +
                    "([TRANS],[TKDAT],[Task_Priority],[DEMO1],[DEMO2],[DEMO3],[ReturnMsg],[MsgDirection],[guid],[MsgParse]) VALUES　" +
                    "(@TRANS, @TKDAT, @Task_Priority, @DEMO1, @DEMO2, @DEMO3,@ReturnMsg, @MsgDirection,@guid,@MsgParse)", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("InsertWMSMsgToLog", ex);
            }
        }
        public object ExeSQLStringWithParam(string sqlstr, IDataParameter[] paramenters)
        {
            object obj = null;
            SqlConnection SqlConn = new SqlConnection(ConnectionStr);
            SqlCommand command = new SqlCommand(sqlstr, SqlConn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 180
            };
            try
            {
                if (SqlConn.State != ConnectionState.Open)
                {
                    SqlConn.Open();
                }
                command.Parameters.AddRange(paramenters);
                //command.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
                obj = command.ExecuteNonQuery();
                //obj = command.Parameters["@ReturnValue"].Value; //@Output_Value和具体的存储过程参数对应
                if (Equals(obj, null) || (Equals(obj, DBNull.Value)))
                {
                    return null;
                }
                else
                {
                    return obj;
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("WMSDBManager ExeSQLStringWithParam:", ex);
                return null;
            }
            finally
            {
                SqlConn.Dispose();
                command.Dispose();
            }
        }
        public static object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;
            return obj;
        }
    }
}
