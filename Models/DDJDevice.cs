using BMHRI.WCS.Server.DDJProtocol;
using BMHRI.WCS.Server.Tools;
using S7.Net;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Sockets;

namespace BMHRI.WCS.Server.Models
{
    public class DDJDevice : PLCDevice
    {
        public List<DataItem> DataItemList;
        private List<DataItem> DB45DataItemList;
        public List<DDJMessage> WCSTOPLCDB4MessageList;
        public List<DDJMessage> PLCTOWCSDB5MessageList;
        public List<DataItem> DDJStatusDataItemList;
        public bool AutoConnect;
        public string WCSTODEMATICMessage = "";

        public bool ReadyIntoTunnel = false;

        #region Property 
        private string wcs_to_plc_db4;
        public string WCSTOPLCDB4
        {
            get { return wcs_to_plc_db4; }
            set
            {
                if (wcs_to_plc_db4 != value)
                {
                    wcs_to_plc_db4 = value;
                    Notify(nameof(WCSTOPLCDB4));
                }
            }
        }
        private string plc_to_wcs_db5;
        public string PLCTOWCSDB5
        {
            get { return plc_to_wcs_db5; }
            set
            {
                if (plc_to_wcs_db5 != value)
                {
                    plc_to_wcs_db5 = value;
                    Notify(nameof(PLCTOWCSDB5));
                }
            }
        }
        private string tunnel;
        public string Tunnel
        {
            get {
                if (string.IsNullOrEmpty(tunnel))
                {
                    if (!string.IsNullOrEmpty(PLCID))
                        tunnel = string.Concat("000", PLCID.AsSpan(4, 1));
                }
                return tunnel; }
        }
        private bool hasPallet;
        public bool HasPallet
        {
            get { return hasPallet; }
            set
            {
                if (hasPallet != value)
                {
                    hasPallet = value;
                    Notify(nameof(HasPallet));
                }
            }
        }
      
        private bool available;
        public bool Available
        {
            get { return available; }
            set
            {
                if (available != value)
                {
                    available = value;
                    Notify(nameof(Available));
                    //UpdateWMSDBDDJStatus();
                }
            }
        }

        private bool isopendoor;
        public bool IsOpenDoor
        {
            get { return isopendoor; }
            set
            {
                if (isopendoor != value)
                {
                    isopendoor = value;
                    Notify(nameof(IsOpenDoor));
                    SetDoor();
                }
            }
        }

        private void SetDoor()
        {
            if (IsOpenDoor)
            {
                opendoor();
            }
        }

        private bool cannotdo;
        public bool CannotDo
        {
            get { return cannotdo; }
            set
            {
                if (cannotdo != value)
                {
                    cannotdo = value;
                    Notify(nameof(CannotDo));
                }
            }
        }
        private string faultCode;
        public string FaultCode
        {
            get { return faultCode; }
            set
            {
                if (faultCode != value)
                {
                    faultCode = value;
                    Notify(nameof(FaultCode));
                }
            }
        }
        private string faultContent;
        public string FaultContent
        {
            get
            {
                return faultContent;
            }
            set
            {
                if (faultContent != value)
                {
                    faultContent = value;
                    Notify(nameof(FaultContent));
                }
            }
        }
      
        private int liftingPosition;
        public int LiftingPosition
        {
            get { return liftingPosition; }
            set
            {
                if (Math.Abs(liftingPosition - value) > 500)
                {
                    liftingPosition = value;
                    Notify(nameof(LiftingPosition));
                }
            }
        }

        private int forkPosition;
        public int ForkPosition
        {
            get { return forkPosition; }
            set
            {
                if (Math.Abs(forkPosition - value) > 5)
                {
                    forkPosition = value;
                    Notify(nameof(ForkPosition));
                }
            }
        }
        private string fmLocation;
        public string FmLocation
        {
            get { return fmLocation; }
            set
            {
                fmLocation = value;
                Notify(nameof(FmLocation));
            }
        }
        private string toLocation;
        public string ToLocation
        {
            get { return toLocation; }
            set
            {
                toLocation = value;
                Notify(nameof(ToLocation));
            }
        }
        private DDJDeviceWorkState ddj_work_state;
        public DDJDeviceWorkState DDJWorkState
        {
            get { return ddj_work_state; }
            set
            {
                if (ddj_work_state != value)
                {
                    ddj_work_state = value;
                    Notify(nameof(DDJWorkState));
                    //UpdateWMSDBDDJStatus();
                }
            }
        }

        private void UpdateWMSDBDDJStatus()
        {
            WMSTask wMSTask = new WMSTask { };
            if (!Available || DDJWorkState == DDJDeviceWorkState.Offline || DDJWorkState == DDJDeviceWorkState.Fault || !WCSEnable || DDJWorkState == DDJDeviceWorkState.None)     //!WCSEnable
            {
                string ddjid = "";
                if(PLCID.Substring(4, 1)=="1")
                {
                    wMSTask.Warehouse = "1503";
                    ddjid = "1";
                }
                else if(PLCID.Substring(4, 1) == "2")
                {
                    wMSTask.Warehouse = "1504";
                    ddjid = "2";
                }
                else if (PLCID.Substring(4, 1) == "3")
                {
                    wMSTask.Warehouse = "1519";
                    ddjid = "1";
                }
                else
                {
                    wMSTask.Warehouse = "";
                }

                wMSTask.PalletNum =  ddjid;// PLCID.Substring(4,1);
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.TaskSource = "WMS";
                wMSTask.ToLocation = "B";//B表示不可用
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTask wMSTask1 = WMSTasksManager.Instance.WMSTaskList.Find(x => x.CreateTime == wMSTask.CreateTime);
                if (wMSTask1 != null)
                {
                    wMSTask.CreateTime = System.DateTime.Now.AddMilliseconds(10).ToString("yyyy-MM-dd HH:mm:ss.ffff");
                }
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
            else if(DDJWorkState == DDJDeviceWorkState.Standby&&WCSEnable)  
            {
                string ddjid = "";
                if (PLCID.Substring(4, 1) == "1")
                {
                    wMSTask.Warehouse = "1503";
                    ddjid = "1";
                }
                else if (PLCID.Substring(4, 1) == "2")
                {
                    wMSTask.Warehouse = "1504";
                    ddjid = "2";
                }
                else if (PLCID.Substring(4, 1) == "3")
                {
                    wMSTask.Warehouse = "1519";
                    ddjid = "1";
                }
                else
                {
                    wMSTask.Warehouse = "";
                }
                wMSTask.PalletNum = ddjid;
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.TaskSource = "WMS";
                wMSTask.ToLocation = "A";//    A表示可用
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTask wMSTask1 = WMSTasksManager.Instance.WMSTaskList.Find(x => x.CreateTime == wMSTask.CreateTime);
                if (wMSTask1 != null)
                {
                    wMSTask.CreateTime = System.DateTime.Now.AddMilliseconds(10).ToString("yyyy-MM-dd HH:mm:ss.ffff");
                }
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
            
        }

        private bool wcs_enable;
        public bool WCSEnable
        {
            get
            {
                return wcs_enable;
            }
            set
            {
                if (wcs_enable != value)
                {
                    wcs_enable = value;
                    Notify(nameof(WCSEnable));
                    //UpdateWMSDBDDJStatus();
                }
            }
        }

        internal void EmtpyOutboundConfirm()
        {
            throw new NotImplementedException();
        }

        internal void DoubleInboundConfirm()
        {
            throw new NotImplementedException();
        }

        public string pallet_num;
        public string PalletNum
        {
            get
            {
                return pallet_num;
            }
            set
            {
                if (pallet_num != value)
                {
                    pallet_num = value;
                    Notify(nameof(PalletNum));
                }
            }
        }
        private string motionRank;
        public string MotionRank
        {
            get { return motionRank; }
            set
            {
                if (motionRank != value)
                {
                    motionRank = value;
                    Notify(nameof(MotionRank));
                }
            }
        }
        private string liftLayer;
        public string LiftLayer
        {
            get { return liftLayer; }
            set
            {
                if (liftLayer != value)
                {
                    liftLayer = value;
                    Notify(nameof(LiftLayer));
                }
            }
        }
        private int motionPosition;
        public int MotionPosition
        {
            get { return motionPosition; }
            set
            {
                if (Math.Abs(motionPosition - value) > 7500)
                {
                    motionPosition = value;
                    Notify(nameof(MotionPosition));
                }
            }
        }
        #endregion
        #region
        public DDJDevice(string plcid)
        {
            PLCID = plcid;
        }
        public DDJDevice(string cpuType, string Ip, string slot, string rack, string decription, string device_type, string plcid) : base(cpuType, Ip, slot, rack, decription, device_type, plcid)
        {
            AutoConnect = true;
            ReadDDJDataConfig();
            WCSTOPLCDB4MessageList = new List<DDJMessage>();
            DB45DataItemList = DataItemList.FindAll(x => x.DataType == DataType.DataBlock && (x.DB == 4 || x.DB == 5));

            Task.Factory.StartNew(() => PLCCommunication(), TaskCreationOptions.LongRunning);
            //Task.Factory.StartNew(() => DeviceTaskProcess(), TaskCreationOptions.LongRunning);
        }
        #endregion
        public DDJDevice(string cpuType, string Ip, string slot, string rack, string decription, string device_type, string plcid, string avl) : base(cpuType, Ip, slot, rack, decription, device_type, plcid)
        {
            AutoConnect = true;
            ReadDDJDataConfig();
            if (WCSTOPLCDB4MessageList == null)  //为安全门控制程序所加，防止此处后执行，造成WCSTOPLCDB4MessageList有数据却被new
                WCSTOPLCDB4MessageList = new List<DDJMessage>();
            PLCTOWCSDB5MessageList = new List<DDJMessage>();
            DB45DataItemList = DataItemList.FindAll(x => x.DataType == DataType.DataBlock && (x.DB == 4 || x.DB == 5));
            DDJStatusDataItemList = DataItemList.FindAll(x => x.DB != 4 && x.DB != 5);
            Available = avl == "1";
            Task.Factory.StartNew(() => PLCCommunication(), TaskCreationOptions.LongRunning);
            ReadyIntoTunnel = false;
            //DDJWorkState = DDJDeviceWorkState.None;
            //Task.Factory.StartNew(() => DeviceTaskProcess(), TaskCreationOptions.LongRunning);
        }

        internal void EmptyPickupConfirm()
        {
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.TaskStatus == WCSTaskStatus.Doing);
            if (wCSTask == null) return;
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.PickEmpty);
        }

        /// <summary>
        /// 读取PLC变量配置
        /// </summary>
        private void ReadDDJDataConfig()
        {
            if (DataItemList == null) DataItemList = new List<DataItem>();
            if (File.Exists(Environment.CurrentDirectory + "\\Configure" + "\\DDJ_Data.config"))
            {
                try
                {
                    XElement xDoc = XElement.Load(Environment.CurrentDirectory + "\\Configure" + "\\DDJ_Data.config");
                    var item = (from ele in xDoc.Elements("DeviceData")
                                where ele.Attribute("DeviceType").Value == "DDJ"
                                select ele).SingleOrDefault();
                    if (item != null)
                    {
                        foreach (XElement dNode in item.Elements("Data"))
                        {
                            DataItem data_item = new DataItem();
                            data_item.DataType = (DataType)Enum.Parse(typeof(DataType), dNode.Attribute("DataType").Value);
                            data_item.VarType = (VarType)Enum.Parse(typeof(VarType), dNode.Attribute("VarType").Value);
                            data_item.DB = int.Parse(dNode.Attribute("Db").Value);
                            data_item.StartByteAdr = int.Parse(dNode.Attribute("StartByteAdr").Value);
                            data_item.Count = int.Parse(dNode.Attribute("Count").Value);
                            DataItemList.Add(data_item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("堆垛机配置文件读取异常", ex);
                }
            }
        }
        public string GetVarName<T>(System.Linq.Expressions.Expression<Func<T, T>> exp)
        {
            return ((System.Linq.Expressions.MemberExpression)exp.Body).Member.Name;
        }
        private void PLCCommunication()
        {
            while (true)
            {
                try
                {
                    if (PLCConnectState == ConnectionStates.Connected)
                    {
                        //读取DB4和DB5                    
                        Plc.ReadMultipleVars(DDJStatusDataItemList);
                        RefreshDDJStatus();
                        Plc.ReadMultipleVars(DB45DataItemList);
                        ProcessWCSTOPLCDB4();
                        if (WCSEnable)
                        {
                            ProcessPLCTOWCSDB5();
                            if (DDJWorkState != DDJDeviceWorkState.Standby)
                            {
                                Task.Delay(300).Wait();
                                continue;
                            }
                            if (HasPallet)
                            {
                                if (!DDJPalletOnStandyProcess())
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                DDJWithoutPalletOnStandyProcess();
                            }
                            if (DDJWorkState != DDJDeviceWorkState.Standby)
                            {
                                Task.Delay(300).Wait();
                                continue;
                            }
                            if (!CannotDo)
                            {
                                if (!ReadyIntoTunnel)
                                {
                                    WCSTaskProcess();
                                    ProcessWCSTOPLCDB4();
                                }
                                else
                                {
                                    UpdateDDJStatus(DDJDeviceWorkState.Offline);
                                }
                                
                            }
                        }
                    }
                    else if (AutoConnect)
                    {
                        if (PLCConnectState == ConnectionStates.Disconnected)
                        {
                           //WCSTaskProcess();
                            Connect();
                        }
                    }
                    Task.Delay(300).Wait();
                }
                catch (PlcException ex)
                {
                    PlcExceptionParse(ex);
                    if (!Plc.IsConnected)
                    {
                        PLCConnectState = ConnectionStates.Disconnected;
                        UpdateDDJStatus(DDJDeviceWorkState.None);
                        //UpdateDDJStatus(DDJDeviceWorkState.Fault);
                       // UpdateWMSDBDDJStatus();
                    }
                    //LogHelper.WriteLog("读取PLC失败，" + PLCID + " " + ex.ToString());
                    Task.Delay(3000).Wait();
                }
            }
        }

        private bool DDJPalletOnStandyProcess()
        {
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == PalletNum && x.DeviceID == PLCID&&(x.TaskStatus==WCSTaskStatus.Waiting||x.TaskStatus==WCSTaskStatus.Doing||x.TaskStatus==WCSTaskStatus.Fault||x.TaskStatus== WCSTaskStatus.DDJPicked));
            if (wCSTask == null)
            {
                return false;
            }
            WCSTaskManager.Instance.UpdateWCSTaskPriAndStatus(wCSTask, 9, WCSTaskStatus.Waiting);
            return true;
        }
        private void DDJWithoutPalletOnStandyProcess()
        {
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && (x.TaskStatus == WCSTaskStatus.Doing||x.TaskStatus== WCSTaskStatus.Fault));
            if (wCSTask == null)
            {
                return;
            }
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Waiting);
            //优先故障任务取消
            //WCSTaskManager.Instance.UpdateWCSTaskPriAndStatus(wCSTask, 9, WCSTaskStatus.Waiting);
            return;
        }

        private void PlcExceptionParse(PlcException plc_ex)
        {
            if (plc_ex.ErrorCode != ErrorCode.NoError)
            {
                switch (plc_ex.ErrorCode)
                {
                    case ErrorCode.ConnectionError:
                    case ErrorCode.IPAddressNotAvailable:
                        PLCConnectState = ConnectionStates.Disconnected;
                        break;
                    case ErrorCode.ReadData:
                    case ErrorCode.SendData:
                    case ErrorCode.WriteData:
                    case ErrorCode.WrongCPU_Type:
                    case ErrorCode.WrongNumberReceivedBytes:
                    case ErrorCode.WrongVarFormat:
                        UpdateDDJStatus(DDJDeviceWorkState.Fault);
                       // UpdateWMSDBDDJStatus();
                        break;
                    default: break;
                }
            }
            //LogHelper.WriteLog("PLC通讯失败，" + PLCID + " ErrorCode = " + plc_ex.ErrorCode + " " + plc_ex.ToString());
        }

        private bool IsPalletInPlace(WCSTask wCSTask)
        {
            if (wCSTask == null) return false;
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PortNum == wCSTask.FmLocation);
            if (sSJDevice == null) return false;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.DDJID == PLCID && x.SystemType == DeviceSystemType.InboundFinish);
            if (sSJDeviceBlock == null || sSJDeviceBlock.IsFaulty || !sSJDeviceBlock.IsOccupied || sSJDeviceBlock.PalletNum != wCSTask.PalletNum)
                return false;
            return true;
        }

        public void DB4ClearZero()
        {
            if (PLCConnectState == ConnectionStates.Connected)
            {
                DDJMessage ddj_00db4 = new DDJMessage(PLCID);
                ddj_00db4.SetClearDB4Message();
                _ =InsertDDJSendList(ddj_00db4);
            }
        }
        public void DB5ClearZero()
        {
            if (PLCConnectState == ConnectionStates.Connected)
            {
                DDJMessage ssj_00db5 = new DDJMessage(PLCID);
                ssj_00db5.SetClearDB5Message();
                PLCTOWCSDB5MessageList.Add(ssj_00db5);
            }
        }

        #region  故障处理

        private void SetDDJStandy()
        {
            UpdateDDJStatus(DDJDeviceWorkState.Standby);
            FaultCode = null;
            FaultContent = null;
        }
        private void SetDDJFault(string fault_code,string pallet_num)
        {
            if (string.IsNullOrEmpty(fault_code)) return;
            UpdateDDJStatus(DDJDeviceWorkState.Fault);
            DDJFault dDJFault = DDJFaultList.Instance.FaultList.Find(x => x.FaultCode == fault_code);
            if (string.IsNullOrEmpty(dDJFault.FaultContent)) return;

            FaultCode = fault_code;
            FaultContent = dDJFault.FaultContent;
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == pallet_num&&x.DeviceID==PLCID);
            if (wCSTask == null) return;
            switch (FaultCode)
            {
                case "F20":
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.UnStackEmpty);
                    break;
                case "F21":
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.StackDouble);
                    break;
                case "F63":
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.FarOutboundClearHave);
                    break;
                default:
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Fault);
                    break;
            }
        }
        #endregion
        public void WCSTaskProcess()
        {
            try
            {
                List<WCSTask> wCSTasks;
                if (HasPallet)
                {
                    wCSTasks = WCSTaskManager.Instance.WCSTaskList.FindAll(
                        x => !string.IsNullOrEmpty(x.DeviceID) && x.DeviceID == PLCID &&
                        x.TaskStatus == WCSTaskStatus.Waiting && x.PalletNum == PalletNum)
                        .OrderByDescending(x => x.TaskPri)
                        .ThenBy(x => x.CreateTime).ToList();
                }
                else
                {
                    wCSTasks = WCSTaskManager.Instance.WCSTaskList.FindAll(
                        x => !string.IsNullOrEmpty(x.DeviceID) && x.DeviceID == PLCID &&
                        x.TaskStatus == WCSTaskStatus.Waiting)
                        .OrderByDescending(x => x.TaskPri)
                        .ThenBy(x => x.CreateTime).ToList();
                }
                if (wCSTasks == null || wCSTasks.Count < 1) return;
                List<WCSTask> dDJTasks = new List<WCSTask>();
                foreach (WCSTask wCSTask in wCSTasks)
                {
                    switch (wCSTask.TaskType)
                    {
                        case WCSTaskTypes.DDJStack:
                            if (DDJStackTaskCanDo(wCSTask))
                                dDJTasks.Add(wCSTask);
                            break;
                        case WCSTaskTypes.DDJDirect:
                            if (DDJDirecTaskCanDo(wCSTask))
                                dDJTasks.Add(wCSTask);
                            break;
                        case WCSTaskTypes.DDJStackMove:
                            if (DDJMoveTaskCanDo(wCSTask))
                                dDJTasks.Add(wCSTask);
                            break;
                        case WCSTaskTypes.DDJUnstack:
                            if (DDJUnStackTaskCanDo(wCSTask))
                                dDJTasks.Add(wCSTask);
                            break;
                        default:
                            break;
                    }
                }
                if(dDJTasks.Count>0)
                {
                    DDJMessage ddj_message = new DDJMessage(PLCID);
                    switch (dDJTasks[0].TaskType)
                    {
                        case WCSTaskTypes.DDJStack:
                            WMSTask wMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.WMSSeqID == dDJTasks[0].WMSSeqID);
                            if (wMSTask != null)
                            {
                                ddj_message.Set0aMessage(dDJTasks[0].PalletNum, dDJTasks[0].FmLocation, dDJTasks[0].ToLocation, ((int)wMSTask.GaoDiBZ).ToString());
                            }
                            break;
                        case WCSTaskTypes.DDJUnstack:
                            ddj_message.Set0bMessage(dDJTasks[0].PalletNum, dDJTasks[0].FmLocation, dDJTasks[0].ToLocation);
                            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(dDJTasks[0].ToLocation));
                            SSJDeviceBlock ssjUnStackPosition = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == Tunnel && x.Floor== dDJTasks[0].Floor&&(x.SystemType==DeviceSystemType.OutboundBegin|| x.SystemType == DeviceSystemType.TotalPort|| x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin));
                            if(ssjUnStackPosition != null)
                            {
                                if(ssjUnStackPosition.Position == "1216" || ssjUnStackPosition.Position == "1226")
                                {
                                    sSJDevice.OutTaskLimitDataItem(int.Parse(ssjUnStackPosition.Position.Substring(2, 2)), 7, 1);
                                }
                                else
                                {
                                    sSJDevice.OutTaskLimitDataItem(int.Parse(ssjUnStackPosition.Position.Substring(0, 1)+ssjUnStackPosition.Position.Substring(2, 2)), 7, 1);
                                }
                                
                            }
                            //if (dDJTasks[0].DeviceID == "DDJ01" && dDJTasks[0].ToLocation=="000a")
                            //{
                            //    SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(dDJTasks[0].ToLocation));
                            //    if (sSJDevice != null)
                            //    {
                            //        sSJDevice.OutTaskLimitDataItem(15, 1, 1);
                            //    }
                            //}
                            //else if((dDJTasks[0].DeviceID == "DDJ01" || dDJTasks[0].DeviceID=="DDJ02") && dDJTasks[0].ToLocation == "000d")
                            //{
                            //    SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(dDJTasks[0].ToLocation));
                            //    if (sSJDevice != null)
                            //    {
                            //        if(dDJTasks[0].DeviceID == "DDJ01")
                            //            sSJDevice.OutTaskLimitDataItem(174, 3, 1);
                            //        else
                            //            sSJDevice.OutTaskLimitDataItem(125, 3, 1);
                            //    }
                            //}
                            break;
                        case WCSTaskTypes.DDJDirect:
                            ddj_message.Set0eMessage(dDJTasks[0].PalletNum, dDJTasks[0].FmLocation, dDJTasks[0].ToLocation);
                            SSJDevice sSJDevice_DDJDirect = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(dDJTasks[0].ToLocation));
                            SSJDeviceBlock ssjDJDirectPosition = sSJDevice_DDJDirect.DeviceBlockList.Find(x => x.Tunnel == Tunnel && x.Floor == dDJTasks[0].Floor && (x.SystemType == DeviceSystemType.OutboundBegin || x.SystemType == DeviceSystemType.TotalPort || x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin));
                            if (ssjDJDirectPosition != null)
                            {
                                if (ssjDJDirectPosition.Position == "1216" || ssjDJDirectPosition.Position == "1226")
                                {
                                    sSJDevice_DDJDirect.OutTaskLimitDataItem(int.Parse(ssjDJDirectPosition.Position.Substring(2, 2)), 7, 1);
                                }
                                else
                                {
                                    sSJDevice_DDJDirect.OutTaskLimitDataItem(int.Parse(ssjDJDirectPosition.Position.Substring(0, 1) + ssjDJDirectPosition.Position.Substring(2, 2)), 7, 1);
                                }

                            }
                            break;
                        case WCSTaskTypes.DDJStackMove:
                            //if (int.Parse(dDJTasks[0].DeviceID.Substring(3, 2)) < 5)
                           // {
                                ddj_message.Set0mMessage(dDJTasks[0].PalletNum, dDJTasks[0].FmLocation, dDJTasks[0].ToLocation);
                          //  }                           
                            break;
                        default:
                            break;
                    }
                    if (ddj_message == null) return;
                    InsertDDJSendList(ddj_message);
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(dDJTasks[0], WCSTaskStatus.Doing);
                }
                DDJMessage dDJMessage = new DDJMessage();
              
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("WCSTaskProcess " + PLCID, ex);
            }
        }

        private bool DDJUnStackTaskCanDo(WCSTask wCSTask)//
        {
            if (wCSTask == null || wCSTask.TaskType != WCSTaskTypes.DDJUnstack) return false;
            //SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PortNum == wCSTask.ToLocation);
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(wCSTask.ToLocation));
            if (sSJDevice == null || sSJDevice.SSJWorkState == SSJDeviceWorkState.Offline || sSJDevice.SSJWorkState == SSJDeviceWorkState.None || sSJDevice.SSJWorkState == SSJDeviceWorkState.Manual) return false;
            //SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == Tunnel&&x.SystemType==DeviceSystemType.OutboundBegin);
            SSJDeviceBlock sSJDeviceBlocks = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == Tunnel && (x.SystemType == DeviceSystemType.OutboundBegin||x.SystemType== DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.TotalPort) &&x.Floor==wCSTask.Floor);
            if(sSJDeviceBlocks == null) return false;
            if (!sSJDeviceBlocks.AllowUnloading) return false;
            string toLocation = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == wCSTask.PalletNum).ToLocation;
            SSJDeviceBlock sSJToDeviceBlocks = sSJDevice.DeviceBlockList.Find(x => x.Position == toLocation);
            //SSJDeviceBlock sSJToDeviceBlocks = sSJDevice.DeviceBlockList.Find(x => x.Position == wCSTask.WMSTaskTolocation);
            if(sSJToDeviceBlocks.Position !="2114" && sSJToDeviceBlocks.Position != "2124" && sSJToDeviceBlocks.Position != "3114" && sSJToDeviceBlocks.Position != "3124")
            {
                if(sSJToDeviceBlocks.SystemType == DeviceSystemType.Picking || sSJToDeviceBlocks.SystemType == DeviceSystemType.TotalPort)//只需要判断可入可出的出入库模式，该项目所有出库的都是可入可出的 好像可以省略
                {
                    if (sSJToDeviceBlocks.CurrentMode == DeviceModeType.InboundMode) return false;
                }
                
            }
            string fm_location=wCSTask.FmLocation;
            WCSTask wCSMoveOrOutboundTask = WCSTaskManager.Instance.WCSTaskList.Find(x => (x.TaskType == WCSTaskTypes.DDJStackMove|| x.TaskType == WCSTaskTypes.DDJUnstack) && x.DeviceID == wCSTask.DeviceID && x.FmLocation == GetClearLocation(fm_location)&&x.TaskStatus== WCSTaskStatus.Waiting);
            if (wCSMoveOrOutboundTask == null)
                return true;
            else
            {
                //无须提高近端出库或者倒库任务优先级 因为远端出库任务不会添加到ddjTask列表
                //WCSTaskManager.Instance.UpdateWCSTaskPri(wCSMoveOrOutboundTask, wCSTask.TaskPri + 1);
                return false;
            }
        }

        private string GetClearLocation(string far_location)
        {
            string clear_location = "";
            string pai = far_location.Substring(0, 2);
            switch (pai)
            {
                case "01":

                    clear_location = string.Concat(string.Format("{0:D2}", Convert.ToInt32(pai) + 1), far_location.AsSpan(2, 6));
                    break;
                case "04":
               
                    clear_location = string.Concat(string.Format("{0:D2}", Convert.ToInt32(pai) - 1), far_location.AsSpan(2, 6));
                    break;
                default:
                    clear_location = "00000000";
                    break;
            }
            return clear_location;
        }

        private bool DDJMoveTaskCanDo(WCSTask wCSTask)
        {
            GoodsLocation fm_goodlocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == wCSTask.FmLocation && x.Warehouse == wCSTask.Warehouse);
            GoodsLocation to_goodlocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == wCSTask.ToLocation && x.Warehouse == wCSTask.Warehouse);
            if (fm_goodlocation == null || to_goodlocation == null) return false;
            if (!fm_goodlocation.Available || !to_goodlocation.Available) return false;
            return true;
        }

        private bool DDJDirecTaskCanDo(WCSTask wCSTask)
        {
            if (wCSTask == null || wCSTask.TaskType != WCSTaskTypes.DDJDirect) return false;
            SSJDevice sSJDeviceFm = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(wCSTask.FmLocation));
            SSJDeviceBlock sSJDeviceBlockFm = sSJDeviceFm.DeviceBlockList.Find(x => x.Position == wCSTask.WMSTaskFmlocation);//判断ddj搬运任务的的时候，巷道口的托盘号一致情况

            if (sSJDeviceBlockFm == null) return false;
            if (!HasPallet)   //有托盘，则不需要看入库口占位等信息
            {
                if (sSJDeviceBlockFm.PalletNum != wCSTask.PalletNum) return false;    //!sSJDeviceBlock.IsOccupied ||
            }
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(wCSTask.ToLocation));
            if (sSJDevice == null || sSJDevice.SSJWorkState == SSJDeviceWorkState.Offline || sSJDevice.SSJWorkState == SSJDeviceWorkState.None || sSJDevice.SSJWorkState == SSJDeviceWorkState.Manual) return false;
            //SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == Tunnel&&x.SystemType==DeviceSystemType.OutboundBegin);
            SSJDeviceBlock sSJDeviceBlocks = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == Tunnel && (x.SystemType == DeviceSystemType.OutboundBegin || x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.TotalPort) && x.Floor == wCSTask.Floor);
            if (sSJDeviceBlocks == null) return false;
            if (!sSJDeviceBlocks.AllowUnloading) return false;
            string toLocation = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == wCSTask.PalletNum).ToLocation;
            SSJDeviceBlock sSJToDeviceBlocks = sSJDevice.DeviceBlockList.Find(x => x.Position == toLocation);
            if (sSJToDeviceBlocks.SystemType == DeviceSystemType.Picking || sSJToDeviceBlocks.SystemType == DeviceSystemType.TotalPort)//只需要判断可入可出口的出入库模式，该项目所有出库的都是可入可出的 好像可以省略
            {
                 if (sSJToDeviceBlocks.CurrentMode == DeviceModeType.InboundMode) return false;
            }

            
            return true;
        }
        
        private bool DDJStackTaskCanDo(WCSTask wCSTask)//上架任务增加WMSTaskFmlocation字段，原先的判断方式不能筛选唯一的入库口地址
        {
            if (wCSTask == null || wCSTask.TaskType != WCSTaskTypes.DDJStack) return false;           
            //SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PortNum == wCSTask.FmLocation);
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(wCSTask.FmLocation));
            if (sSJDevice == null||sSJDevice.SSJWorkState==SSJDeviceWorkState.Offline|| sSJDevice.SSJWorkState == SSJDeviceWorkState.None || sSJDevice.SSJWorkState == SSJDeviceWorkState.Manual) return false;

            // SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel ==Tunnel&&(x.SystemType==DeviceSystemType.InboundFinish||x.SystemType== DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.TotalPort) &&x.Floor==wCSTask.Floor);
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == wCSTask.WMSTaskFmlocation);

            if (sSJDeviceBlock == null) return false;
            if (!HasPallet)   //有托盘，则不需要看入库口占位等信息
            {
                if (sSJDeviceBlock.PalletNum != wCSTask.PalletNum) return false;    //!sSJDeviceBlock.IsOccupied ||
            }
            string clear_location_pai = wCSTask.ToLocation.Substring(0, 2);
            if (wCSTask.Warehouse=="1519")
            {
                if (HaveFarOutbound(wCSTask, clear_location_pai)) return false;
                if (HaveNearMove(wCSTask, wCSTask.ToLocation)) return false;
            }
            return true;
        }
        private bool HaveFarOutbound(WCSTask wCSTask,string pai)
        {
            //if(wCSTask == null&&wCSTask.TaskType!= WCSTaskTypes.DDJStack) return false;
            string clear_location_pai = pai;
            string far_location = "";
            switch (clear_location_pai)
            {
                case "02":
                    far_location = string.Concat(string.Format("{0:D2}", Convert.ToInt32(clear_location_pai) - 1), wCSTask.ToLocation.AsSpan(2, 6));
                    break;
                case "03":
                    far_location = string.Concat(string.Format("{0:D2}", Convert.ToInt32(clear_location_pai) + 1), wCSTask.ToLocation.AsSpan(2, 6));
                    break;
                default:
                    break;
            }
            WCSTask wCSOutboundTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.TaskType == WCSTaskTypes.DDJUnstack && x.FmLocation == far_location&&x.TaskStatus== WCSTaskStatus.Waiting);
            if (wCSOutboundTask != null)
            {
                //无须提高远端出库任务优先级 不会将该近端入库加入ddjTask列表
                //wCSOutboundTask.TaskPri = wCSTask.TaskPri + 1;
                //WCSTaskManager.Instance.UpdateWCSTaskDB(wCSOutboundTask);
                return true;
            }
            return false;
        }
        private bool HaveNearMove(WCSTask wCSTask, string nearLocation)
        {
            WCSTask wCSOutboundTask = WCSTaskManager.Instance.WCSTaskList.Find(x => (x.TaskType == WCSTaskTypes.DDJUnstack||x.TaskType== WCSTaskTypes.DDJStackMove) && x.FmLocation == nearLocation && x.TaskStatus == WCSTaskStatus.Waiting);
            if (wCSOutboundTask != null)
            {
                //无须提高远端出库任务优先级 不会将该近端入库加入ddjTask列表
                //wCSOutboundTask.TaskPri = wCSTask.TaskPri + 1;
                //WCSTaskManager.Instance.UpdateWCSTaskDB(wCSOutboundTask);
                return true;
            }
            return false;
        }

        public object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;

            return obj;
        }
        #region 设备操作
        public void Online()
        {
            if (Plc != null && PLCConnectState == ConnectionStates.Connected)
            {
                WCSEnable = true;
                DDJMessage dDJMessage = new DDJMessage(PLCID);
                dDJMessage.Set0LMessage();
                InsertDDJSendList(dDJMessage);
            }
        }
        public void RecallInPlace()
        {
            if (Plc != null && PLCConnectState == ConnectionStates.Connected && WCSEnable)
            {
                //WCSEnable = true;
                DDJMessage dDJMessage = new DDJMessage(PLCID);
                dDJMessage.Set0dMessage("000z");
                if (FaultCode == "F62" && CannotDo) return;
                if (IsOpenDoor) return;//如果安全门仍是打开，故障不能解除。
                InsertDDJSendList(dDJMessage);
            }
        }

        public void opendoor()
        {
            if (Plc != null && PLCConnectState == ConnectionStates.Connected )
            {
                //WCSEnable = true;
                DDJMessage dDJMessage = new DDJMessage(PLCID);
                dDJMessage.Set0jMessage();
                //if (FaultCode == "F62" && CannotDo) return;
                InsertDDJSendList(dDJMessage);
            }
        }

        public void RecallInPlace(string port_num)
        {
            if (Plc != null && PLCConnectState == ConnectionStates.Connected && WCSEnable)
            {
                DDJMessage dDJMessage = new DDJMessage(PLCID);
                dDJMessage.Set0dMessage(port_num);
                InsertDDJSendList(dDJMessage);
            }
        }
        public void OffLine()
        {
            WCSEnable = false;
            if (Plc != null && PLCConnectState == ConnectionStates.Connected)
            {
                DDJMessage dDJMessage = new DDJMessage(PLCID);
                dDJMessage.Set0BMessage();
                InsertDDJSendList(dDJMessage);
                DDJWorkState = DDJDeviceWorkState.Offline;
                //UpdateWMSDBDDJStatus();
            }
        }
        public void UrgentStop()
        {
            if (Plc != null && PLCConnectState == ConnectionStates.Connected)
            {
                DDJMessage dDJMessage = new DDJMessage(PLCID);
                InsertDDJSendList(dDJMessage);
            }
        }
        #endregion
        #region 消息分析
        private void ProcessWCSTOPLCDB4()
        {
            WCSTOPLCDB4 = DB45DataItemList.Find(x => x.DB == 4).Value.ToString();
            if (WCSTOPLCDB4.Length < 42) return;
            if (WCSTOPLCDB4.Substring(29, 1) != "0") return;
            if (WCSTOPLCDB4MessageList.Count < 1) return;
            if (PLCConnectState != ConnectionStates.Connected) return;

            WCSTOPLCDB4MessageList = WCSTOPLCDB4MessageList.OrderBy(x => x.SendPriority).ThenBy(x => x.Tkdat).ToList();
            DDJMessage ddj_message = WCSTOPLCDB4MessageList[0];

            if (ddj_message != null)      //&& ddj_message.MessageType != "00"
            {
                if (WritePLCDataItem(DB45DataItemList.Find(x => x.DB == 4), ddj_message.Trans))
                {
                    //Task.Factory.StartNew(() => DeleteDDJSendList(ddj_message));
                    WCSTOPLCDB4MessageTransfor(ddj_message);
                    DeleteDDJSendList(ddj_message);
                }
            }
        }
        private void ProcessPLCTOWCSDB5()
        {
            PLCTOWCSDB5 = DB45DataItemList.Find(x => x.DB == 5).Value.ToString();
            if (PLCTOWCSDB5.Length == 42)
            {
                if (PLCTOWCSDB5.Substring(28, 2) == "01")
                {
                    //Task.Factory.StartNew(() => PLCTOWCSMessageParse(PLCTOWCSDB5));
                    PLCTOWCSMessageParse(PLCTOWCSDB5);
                    WritePLCDataItem(DB45DataItemList.Find(x => x.DB == 5), PLCTOWCSDB5.Substring(0, 28) + "10");
                }
            }
            if (PLCTOWCSDB5MessageList.Count > 0)
            {
                if (WritePLCDataItem(DB45DataItemList.Find(x => x.DB == 5), PLCTOWCSDB5MessageList[0].GetTransDB4()))
                    PLCTOWCSDB5MessageList.RemoveAt(0);
            }

        }

        private void WCSTOPLCDB4MessageTransfor(DDJMessage dDJMessage)
        {
            string wCSTOPLCDB4Message = "";
            if (dDJMessage == null) return;
            switch (dDJMessage.MessageType)
            {
                case "0a":
                    wCSTOPLCDB4Message = "WCS向堆垛机发送入库指令 托盘号" + dDJMessage.GetPalletNum() + "起始地" + dDJMessage.Trans.Substring(dDJMessage.PortIndex, dDJMessage.PortLength) + "货位" + dDJMessage.Trans.Substring(dDJMessage.StartRowIndex, dDJMessage.LocationLength);
                    break;
                case "0b":
                    wCSTOPLCDB4Message = "WCS向堆垛机发送出库指令 托盘号" + dDJMessage.GetPalletNum() + "货位" + dDJMessage.Trans.Substring(dDJMessage.StartRowIndex, dDJMessage.LocationLength) + "目的地" + dDJMessage.Trans.Substring(dDJMessage.PortIndex, dDJMessage.PortLength);
                    break;
                case "0m":
                    wCSTOPLCDB4Message = "WCS向堆垛机发送倒库指令 托盘号" + dDJMessage.GetPalletNum() + "货位从" + dDJMessage.Trans.Substring(dDJMessage.StartRowIndex, dDJMessage.LocationLength) + "倒至" + dDJMessage.Trans.Substring(dDJMessage.EndRowIndex, dDJMessage.LocationLength);
                    break;
                case "0Y":
                    wCSTOPLCDB4Message = "WCS向堆垛机回复入库货已取走确认 托盘号" + dDJMessage.GetPalletNum();
                    break;
                case "0E":
                    wCSTOPLCDB4Message = "WCS向堆垛机回复入库完成确认 托盘号" + dDJMessage.GetPalletNum() + "货位" + dDJMessage.Trans.Substring(dDJMessage.StartRowIndex, dDJMessage.LocationLength);
                    break;
                case "0F":
                    wCSTOPLCDB4Message = "WCS向堆垛机回复出库完成确认 托盘号" + dDJMessage.GetPalletNum() + "货位" + dDJMessage.Trans.Substring(dDJMessage.StartRowIndex, dDJMessage.LocationLength);
                    break;
                case "0L":
                    wCSTOPLCDB4Message = "WCS向堆垛机发送联机申请";
                    break;
                case "0B":
                    wCSTOPLCDB4Message = "WCS向堆垛机发送脱机信号";
                    break;
                case "0A":
                    wCSTOPLCDB4Message = "WCS向堆垛机发送联机应答信号";
                    break;
                case "0d":
                    wCSTOPLCDB4Message = "WCS向堆垛机发送故障解除信号";
                    break;
                case "0S":
                    wCSTOPLCDB4Message = "WCS向堆垛机发送允许卸货 托盘号" + dDJMessage.GetPalletNum();
                    break;
                default:
                    wCSTOPLCDB4Message = "";
                    break;
            }
            dDJMessage.MsgParse = wCSTOPLCDB4Message;
        }

        public void PLCTOWCSMessageParse(string trans)
        {
            DDJMessage ddj_message = null;
            try
            {
                ddj_message = new DDJMessage(trans, PLCID, DDJMessageDirection.Receive);
                switch (ddj_message.MessageType)
                {
                    case "0A":
                        if (WCSEnable)
                        {
                            UpdateDDJStatus(DDJDeviceWorkState.Online);
                            DDJMessage dDJMessage = new DDJMessage(PLCID);
                            dDJMessage.SetAAMessage();
                            InsertDDJSendList(dDJMessage);
                            ddj_message.MsgParse = "堆垛机向WCS回复联机许可";
                        }
                        break;
                    case "0B":
                        UpdateDDJStatus(DDJDeviceWorkState.Offline);
                        WCSEnable = false;
                        //UpdateWMSDBDDJStatus();
                        ddj_message.MsgParse = "堆垛机向WCS发送脱机状态";
                        break;
                    case "0@":
                        FaultCode = ddj_message.Trans.Substring(19, 3);
                        SetDDJFault(FaultCode,ddj_message.GetPalletNum());
                       // UpdateWMSDBDDJStatus();
                        ddj_message.MsgParse = "堆垛机向WCS上报故障 托盘号" + ddj_message.GetPalletNum() + "故障内容:" + FaultContent;
                        break;
                    case "0D":
                        DDJ0DParse(ddj_message);
                        ddj_message.MsgParse = "堆垛机向WCS反馈待机状态";
                        break;
                    case "0C":
                        DDJ0CParse(ddj_message);
                        if (WCSEnable)
                            DDJWorkState = DDJDeviceWorkState.Working;
                        ddj_message.MsgParse = "堆垛机向WCS反馈正在工作 托盘号" + ddj_message.GetPalletNum();
                        break;
                    case "0X":
                        DDJ0XParse(ddj_message);
                        ddj_message.MsgParse = "堆垛机出库申请卸货 托盘号" + ddj_message.GetPalletNum();
                        break;
                    case "0E":
                        DDJ0EParse(ddj_message);
                        ddj_message.MsgParse = "堆垛机向WCS反馈入库完成 托盘号" + ddj_message.GetPalletNum() + " 货位地址" + ddj_message.Trans.Substring(ddj_message.StartRowIndex, ddj_message.StartRowLength) + "排" + ddj_message.Trans.Substring(ddj_message.StartRankIndex, ddj_message.StartRankLength) + "列" + ddj_message.Trans.Substring(ddj_message.StartLayerIndex, ddj_message.StartLayerLength) + "层";
                        break;
                    case "0F":
                        DDJ0FParse(ddj_message);
                        ddj_message.MsgParse = "堆垛机向WCS反馈出库完成 托盘号" + ddj_message.GetPalletNum() + " 货位地址" + ddj_message.Trans.Substring(ddj_message.StartRowIndex, ddj_message.StartRowLength) + "排" + ddj_message.Trans.Substring(ddj_message.StartRankIndex, ddj_message.StartRankLength) + "列" + ddj_message.Trans.Substring(ddj_message.StartLayerIndex, ddj_message.StartLayerLength) + "层";
                        break;
                    case "0G":
                        DDJ0GParse(ddj_message);
                        ddj_message.MsgParse = "堆垛机向WCS反馈直出完成 托盘号" + ddj_message.GetPalletNum();
                        break;
                    case "0M":
                        DDJ0MParse(ddj_message);
                        ddj_message.MsgParse = "堆垛机向WCS反馈倒库完成 托盘号" + ddj_message.GetPalletNum();
                        break;
                    case "0Y":
                        DDJ0YParse(ddj_message);
                        ddj_message.MsgParse = "堆垛机向WCS反馈入库货已取走 托盘号" + ddj_message.GetPalletNum();
                        break;
                    default:
                        ddj_message.MsgParse = "";
                        break;
                }
                InsertIntoDDJMsgLog(ddj_message);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(PLCID+"-PLCTOWCSMessageParse 电报处理异常 ", ex);
            }
        }
        private void DDJ0DParse(DDJMessage ddj_message)
        {
            PalletNum = ddj_message.GetPalletNum();
            SetDDJStandy();
            if (PalletNum[..1] == "a")
                SetDDJFault("F90", PalletNum);
           // UpdateWMSDBDDJStatus();
        }
        private string DDJ0MParse(DDJMessage ddj_message)
        {
            string restr = "";
            DDJMessage ddjmsg = new DDJMessage(PLCID);
            ddjmsg.SetMMMessage(ddj_message.Trans);
            InsertDDJSendList(ddjmsg);
            WCSTask wCSTask = null;
            string palletnum = ddj_message.GetPalletNum();
            if (!string.IsNullOrEmpty(palletnum))
            {
                wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.PalletNum == palletnum && x.TaskType == WCSTaskTypes.DDJStackMove);
                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Done);
                restr = "堆垛机" + PLCID + "移库完成，托盘号：" + ddj_message.GetPalletNum();
            }
            return restr;
        }
        private string DDJ0XParse(DDJMessage ddj_message)
        {
            string restr = "";
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.PalletNum == ddj_message.GetPalletNum() && (x.TaskType == WCSTaskTypes.DDJUnstack || x.TaskType == WCSTaskTypes.DDJDirect));

            if (AllowUnload(wCSTask))
            {
                DDJMessage ddjmsg = new DDJMessage(PLCID);
                ddjmsg.Set0SMessage(ddj_message.Trans);
                InsertDDJSendList(ddjmsg);
            }
            return restr;
        }
        private string DDJ0YParse(DDJMessage ddj_message)
        {
            DDJMessage dDJMessage = new DDJMessage(ddj_message.Trans, PLCID, DDJMessageDirection.Send);
            InsertDDJSendList(dDJMessage);
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.PalletNum == ddj_message.GetPalletNum());
            if (wCSTask != null)
            {
                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.DDJPicked);
                //SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PortNum == wCSTask.FmLocation);
                SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(wCSTask.FmLocation));
                if (sSJDevice != null) 
                    sSJDevice.ClearOcupty(wCSTask.PalletNum);
            }
            return "堆垛机" + PLCID + "取盘成功，托盘号：" + ddj_message.GetPalletNum();
        }
        public string GetSSJPLCID(string portNum)
        {
            if (portNum == "000a")
                return "SSJ01";
            else if (portNum == "000b")
                return "SSJ01";
            else if (portNum == "000c")
                return "SSJ01";
            else if (portNum == "000d")
                return "SSJ01";
            else if (portNum == "000e")
                return "SSJ02";
            else if (portNum == "000f")
                return "SSJ02";
            else if (portNum == "000g")
                return "SSJ02";
            else if (portNum == "000h")
                return "SSJ02";
            else if (portNum == "000k")
                return "SSJ06";
            else
                return "";
        }
        private string DDJ0EParse(DDJMessage ddj_message)
        {
            string restr = "";
            DDJMessage dDJMessage = new DDJMessage(ddj_message.Trans, PLCID, DDJMessageDirection.Send);
            InsertDDJSendList(dDJMessage);
            string pallet_num = ddj_message.GetPalletNum();
            WCSTask wCSTask = null;
            if (!string.IsNullOrEmpty(pallet_num))
                wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.PalletNum == pallet_num && x.TaskType == WCSTaskTypes.DDJStack);
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Done);
            restr = "堆垛机" + PLCID + "入库完成，托盘号：" + ddj_message.GetPalletNum();
            List<WCSTask> wCSTaskNextOutboundTask = WCSTaskManager.Instance.WCSTaskList.FindAll(
                    x => x.DeviceID == PLCID &&
                    x.TaskType == WCSTaskTypes.DDJUnstack)
                    .OrderByDescending(x => x.TaskPri)
                    .ThenBy(x => x.CreateTime).ToList();
            if (wCSTaskNextOutboundTask == null || wCSTaskNextOutboundTask.Count < 1) return restr;
            WCSTaskManager.Instance.UpdateWCSTaskPri(wCSTaskNextOutboundTask[0], 1);
            //wCSTaskNextOutboundTask[0].TaskPri = 1;
            //WCSTaskManager.Instance.UpdateWCSTaskDB(wCSTaskNextOutboundTask[0]);
            return restr;
        }
        private string DDJ0CParse(DDJMessage ddj_message)
        {
            string restr = "";
            string pallet_num = ddj_message.GetPalletNum();
            //WMSTask wMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == pallet_num &&x.TaskSource=="WMS"&& (x.TaskType== WMSTaskType.Stacking||x.TaskType== WMSTaskType.Outbound||x.TaskType== WMSTaskType.Picking||x.TaskType== WMSTaskType.Moving||x.TaskType== WMSTaskType.InToOut));
            //if(wMSTask==null) return restr;
            //WMSTask wMSTask1 = new WMSTask();
            //wMSTask1.PalletNum = pallet_num;
            //wMSTask1.TaskType = WMSTaskType.DeviceMsg;
            //wMSTask1.TaskSource = "WMS";
            //wMSTask1.TaskStatus = WMSTaskStatus.DDJTaskStart;
            //WMSTasksManager.Instance.AddWMSTask(wMSTask1);
            restr = "堆垛机" + PLCID + "开始工作，托盘号：" + ddj_message.GetPalletNum();
            return restr;
        }
        private string DDJ0FParse(DDJMessage ddj_message)
        {
            string restr = "";
            DDJMessage dDJMessage = new DDJMessage(ddj_message.Trans, PLCID, DDJMessageDirection.Send);
            InsertDDJSendList(dDJMessage);
            string pallet_num = ddj_message.GetPalletNum();
            WCSTask wCSTask = null;
            if (!string.IsNullOrEmpty(pallet_num))
                wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.PalletNum == pallet_num && x.TaskType == WCSTaskTypes.DDJUnstack);
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Done);
            //WMSTask wMSTask1 = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == pallet_num && x.TaskSource == "WMS" && (x.TaskType == WMSTaskType.Outbound || x.TaskType == WMSTaskType.Picking || x.TaskType == WMSTaskType.InToOut));
            //if (wMSTask1 == null) return restr;
            //WMSTask wMSTask = new WMSTask();
            //wMSTask.PalletNum = ddj_message.GetPalletNum();
            //wMSTask.TaskType = WMSTaskType.DeviceMsg;
            //wMSTask.TaskSource = "WMS";
            //wMSTask.TaskStatus = WMSTaskStatus.SSJTaskStart;
            //WMSTasksManager.Instance.AddWMSTask(wMSTask);
            restr = "堆垛机" + PLCID + "出库完成，托盘号：" + ddj_message.GetPalletNum() ;
            return restr;
        }

        private string DDJ0GParse(DDJMessage ddj_message)
        {
            string restr = "";
            DDJMessage dDJMessage = new DDJMessage(ddj_message.Trans, PLCID, DDJMessageDirection.Send);
            InsertDDJSendList(dDJMessage);

            string pallet_num = ddj_message.GetPalletNum();
            WCSTask wCSTask = null;
            if (!string.IsNullOrEmpty(pallet_num))
                wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.PalletNum == pallet_num && x.TaskType == WCSTaskTypes.DDJDirect);
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Done);

            restr = "堆垛机" + PLCID + "直出完成，托盘号：" + ddj_message.GetPalletNum();
            return restr;
        }
        private void SendDDJTaskMessage(WCSTask wCSTask)
        {
            if (wCSTask == null) return;
            DDJMessage dDJMessage =new  DDJMessage();
            if (dDJMessage == null) return;
            InsertDDJSendList(dDJMessage);
        }
        public bool InsertDDJSendList(DDJMessage ddj_message)
        {
            if (ddj_message == null)
                return false;
            if (ddj_message.MessageType == null)
                return false;
            if (ddj_message.Trans == null || ddj_message.Trans.Length < DDJMessage.TatolLength)
                return false;
            try
            {
                if (WCSTOPLCDB4MessageList == null)     
                    WCSTOPLCDB4MessageList = new List<DDJMessage>();    //安全门控制程序所添加 防止DDJ PLC未创建没初始化WCSTOPLCDB4MessageList,就直接add
                WCSTOPLCDB4MessageList.Add(ddj_message);
                //SqlParameter[] sqlParameters = new SqlParameter[] {
                //    new SqlParameter("@trans", ddj_message.Trans),
                //    new SqlParameter("@PlcId", ddj_message.PLCID),
                //    new SqlParameter("@priority", ddj_message.SendPriority),
                //    new SqlParameter("@Tkdat", ddj_message.Tkdat),
                //    new SqlParameter("@MsgSeqID", ddj_message.MsgSeqID)
                //    };b
                //Task.Factory.StartNew(() => SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[DDJ_Send_Buffer]([PLCID],[Trans],[Tkdat],[SendPriority],[MsgSeqID]) VALUES(@PlcId,@trans,@Tkdat,@priority,@MsgSeqID)", sqlParameters));
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("插入堆垛机发送信息队列异常 InsertDDJSendList，" + PLCID + " " + ex.ToString());
            }
            return true;
        }
      
        private bool DeleteDDJSendList(DDJMessage ddj_message)
        {
            try
            {
                if (ddj_message != null)
                {
                    WCSTOPLCDB4MessageList.Remove(ddj_message);
                    SqlParameter[] sqlParameters = new SqlParameter[] {
                    new SqlParameter("@trans", ddj_message.Trans),
                    new SqlParameter("@PlcId", ddj_message.PLCID),
                    new SqlParameter("@priority", ddj_message.SendPriority),
                    new SqlParameter("@Direction", ddj_message.MsgDir),
                    new SqlParameter("@MsgSeqID", ddj_message.MsgSeqID),
                    new SqlParameter("@Tkdat", ddj_message.Tkdat),
                    new SqlParameter("@MsgParse", ddj_message.MsgParse)
                    };
                    SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[PLC_Message_Log]" +
                        "([PLCID],[Direction],[Trans],[Tkdat],[SendPriority],[MsgSeqID],[MsgParse]) VALUES" +
                        "(@PlcId, @Direction, @trans, @Tkdat, @priority, @MsgSeqID,@MsgParse)", sqlParameters);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("删除堆垛机发送信息异常 DeleteSSJSendBuffer，" + PLCID, ex);
            }
            return true;
        }
        //public bool UpdateSSJSendListDB(SSJMessage ssj_message)
        //{
        //    if (ssj_message != null)
        //    {
        //        SqlParameter[] sqlParameters = new SqlParameter[] {
        //            new SqlParameter("@trans", ssj_message.Trans),
        //            new SqlParameter("@PlcId", ssj_message.PLCID),
        //            new SqlParameter("@priority", ssj_message.SendPriority),
        //            new SqlParameter("@MsgSeqID", ssj_message.MsgSeqID),
        //            new SqlParameter("@Tkdat", ssj_message.Tkdat),
        //            new SqlParameter("@MsgParse", ssj_message.MsgParse)
        //        };
        //        SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[SSJ_Send_Buffer]SET" +
        //            " [PLCID] = @PLCID" +
        //            ",[Trans] = @Trans" +
        //            ",[SendPriority] = @priority" +
        //            ",[Tkdat] = @Tkdat" +
        //            ",[MsgParse] = @MsgParse" +
        //            " WHERE [MsgSeqID] = @MsgSeqID ", sqlParameters);
        //    }
        //    return true;
        //}
        private void RefreshDDJStatus()
        {
            try
            {
                if (DDJStatusDataItemList.Count < 4) return;
                if (DDJStatusDataItemList[6].Value != null)
                    PalletNum = DDJStatusDataItemList[6].Value.ToString();               
                if (DDJStatusDataItemList[4].Value != null)
                    HasPallet = (bool)DDJStatusDataItemList[4].Value;               
                if (DDJStatusDataItemList[0].Value != null)
                    MotionPosition = (int)DDJStatusDataItemList[0].Value;
                if (DDJStatusDataItemList[1].Value != null)
                    LiftingPosition = (int)DDJStatusDataItemList[1].Value;
                if (DDJStatusDataItemList[2].Value != null)
                    ForkPosition = (int)DDJStatusDataItemList[2].Value;
                //if (DDJStatusDataItemList[7].Value != null)
                //    FmLocation = DDJStatusDataItemList[7].Value.ToString();
                //if (DDJStatusDataItemList[8].Value != null)
                //    ToLocation = DDJStatusDataItemList[8].Value.ToString();


                //if (DDJStatusDataItemList[8].Value != null)
                //    MotionRank = DDJStatusDataItemList[8].Value.ToString();
                //if (DDJStatusDataItemList[9].Value != null)
                //    LiftLayer = DDJStatusDataItemList[9].Value.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("RefreshDDJStatus 刷新堆垛机" + PLCID, ex);
            }

        }
        #endregion
        private bool AllowUnload(WCSTask wCSTask)
        {
            SSJDevice sSJDevice = null;
            if (wCSTask == null) return false;
           // sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PortNum == wCSTask.ToLocation);
            sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(wCSTask.ToLocation));
            if (sSJDevice == null) return false;
            if (sSJDevice.SSJWorkState != SSJDeviceWorkState.Online
               && sSJDevice.SSJWorkState != SSJDeviceWorkState.Fault)
                return false;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => (x.SystemType == DeviceSystemType.OutboundBegin||x.SystemType== DeviceSystemType.InboundFinishOrOutboundBegin||x.SystemType== DeviceSystemType.TotalPort) && x.Tunnel == Tunnel && x.Floor==wCSTask.Floor);
            if (sSJDeviceBlock == null) return false;
            if (!sSJDeviceBlock.AllowUnloading) return false;
            return true;
        }
        private void InsertIntoDDJMsgLog(DDJMessage ddj_message)
        {
            if (ddj_message != null)
            {
                SqlParameter[] sqlParameters = new SqlParameter[] {
                new SqlParameter("@trans", ddj_message.Trans),
                new SqlParameter("@PlcId", ddj_message.PLCID),
                new SqlParameter("@MsgSeqID", ddj_message.MsgSeqID),
                new SqlParameter("@Direction",ddj_message.MsgDir ),
                new SqlParameter("@Tkdat",ddj_message.Tkdat ),
                new SqlParameter("@SendPriority",ddj_message.SendPriority ),
                new SqlParameter("@MsgParse",ddj_message.MsgParse )
                };
                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[PLC_Message_Log]([PLCID],[Trans],[Direction],[Tkdat],[SendPriority],[MsgSeqID],[MsgParse]) VALUES (@PlcId,@trans ,@Direction,@Tkdat,@SendPriority,@MsgSeqID,@MsgParse)", sqlParameters);
            }
        }
        public void UpdateDDJStatus(DDJDeviceWorkState device_state)
        {
            if (DDJWorkState == device_state) return;
            DDJWorkState = device_state;
            //DDJStatusChanged?.Invoke(this, new DDJStatusEventArgs(this));
        }
        public enum DDJDeviceWorkState
        {
            None ,
            Online ,
            Standby,
            Working ,
            Fault ,
            Offline,
            Manual,
        }
    }
    
    public sealed class DDJFaultList
    {
        private static readonly Lazy<DDJFaultList> lazy = new Lazy<DDJFaultList>(() => new DDJFaultList());

        public List<DDJFault> FaultList;

        public static DDJFaultList Instance { get { return lazy.Value; } }

        DDJFaultList()
        {
            FaultList = new List<DDJFault>();
            DataTable dt = SQLServerHelper.DataBaseReadToTable("SELECT[故障号]," +
                "[故障名称] ,[标志位] FROM[dbo].[DDJ_Fault_Code]");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DDJFault ddj_fault = new DDJFault
                {
                    FaultCode = dt.Rows[i]["故障号"].ToString(),
                    FaultContent = dt.Rows[i]["故障名称"].ToString(),
                    PLCAddress = dt.Rows[i]["标志位"].ToString()
                };
                FaultList.Add(ddj_fault);
            }
        }
    }
    public struct DDJFault
    {
        public string FaultCode { get; set; }
        public string FaultContent { get; set; }
        public string PLCAddress { get; set; }
    }


}
