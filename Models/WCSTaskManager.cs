using BMHRI.WCS.Server.Tools;
using NPOI.OpenXmlFormats.Spreadsheet;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static BMHRI.WCS.Server.Models.DeMaticDDJ;

namespace BMHRI.WCS.Server.Models
{
    class WCSTaskManager
    {
        private static readonly Lazy<WCSTaskManager> lazy = new Lazy<WCSTaskManager>(() => new WCSTaskManager());
        public static WCSTaskManager Instance { get { return lazy.Value; } }

        public event EventHandler<EventArgs> WCSTaskAdded;
        public event EventHandler<EventArgs> WCSTaskChanged;
        public event EventHandler<EventArgs> WCSTaskDeleted;
        public const string AGVID = "HIKAGV";

        private Timer TaskProcessTimer;

        private List<WCSTask> wCSTaskList;
        public List<WCSTask> WCSTaskList
        {
            get
            {
                if (wCSTaskList == null)
                    wCSTaskList = MyDataTableExtensions.ToList<WCSTask>(SQLServerHelper.DataBaseReadToTable("SELECT * FROM[dbo].[WCSTaskList]"));
                if (wCSTaskList == null) wCSTaskList = new List<WCSTask>();
                return wCSTaskList;
            }
        }
        #region 定时处理Task
        private bool in_update_wms_tast_timer = false;
        private void CreateSendDataToWMSTimer()
        {
            TaskProcessTimer = new Timer(new TimerCallback(ProcessWMSTaskList), null, 0, 2000);
        }
        private WCSTaskManager()
        {
            CreateSendDataToWMSTimer();
        }
        public void ProcessWMSTaskList(object state)
        {
            if (in_update_wms_tast_timer) return;
            in_update_wms_tast_timer = true;
            try
            {
                List<WMSTask> wMSTasks = WMSTasksManager.Instance.WMSTaskList.FindAll(x => x.TaskStatus == WMSTaskStatus.TaskAssigned||x.TaskStatus==WMSTaskStatus.GoodLocChg|| x.TaskStatus == WMSTaskStatus.TaskDoingingConfirm || x.TaskStatus == WMSTaskStatus.ChangeOutBound).OrderByDescending(x => x.Priority).ThenBy(x => x.CreateTime).ToList();
                foreach (WMSTask wMSTask in wMSTasks)
                {
                    WMSTaskStatus wMSTaskStatus = CreateWCSTask(wMSTask);
                    WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask, wMSTaskStatus);
                }
            }

            catch (Exception ex)
            {
                LogHelper.WriteLog("ProcessWCSTaskList ", ex);
                //ex.ToString();
                Task.Delay(3000);
            }
            finally
            {
                in_update_wms_tast_timer = false;
            }
        }

        private WMSTaskStatus CreateWCSTask(WMSTask wMSTask)
        {
            string palno = wMSTask.PalletNum;
            string tolocation = wMSTask.ToLocation;
            string fmlocation = wMSTask.FmLocation;
            string availddj = "";
            if (string.IsNullOrEmpty(palno) || (palno.Length != 10 && palno.Length != 12))
            {
                return WMSTaskStatus.TrayID_Wrong;
            }

            if (string.IsNullOrEmpty(fmlocation))
            {
                return WMSTaskStatus.Inb_Fm_Wrong;
            }

            if (string.IsNullOrEmpty(tolocation))
            {
                return WMSTaskStatus.Inb_To_Wrong;
            }

            //#region 入库申请回复
            switch (wMSTask.TaskType)
            {

                case WMSTaskType.Directe://提升机换层
                    WCSTask ssjDirectTask = new WCSTask();
                    ssjDirectTask.PalletNum = wMSTask.PalletNum;
                    ssjDirectTask.FmLocation = wMSTask.FmLocation;
                    ssjDirectTask.ToLocation = wMSTask.ToLocation;
                    ssjDirectTask.WMSSeqID = wMSTask.WMSSeqID;
                    ssjDirectTask.TaskStep = 1;
                    ssjDirectTask.TaskType = WCSTaskTypes.SSJInbound;
                    ssjDirectTask.DeviceID = GetSSJID(wMSTask.FmLocation);
                    ssjDirectTask.TaskStatus = WCSTaskStatus.Waiting;
                    ssjDirectTask.GaoDiBZ = (WCSGaoDiBZ)(int)wMSTask.GaoDiBZ;
                    ssjDirectTask.Floor = wMSTask.Floor;
                    ssjDirectTask.Warehouse = wMSTask.Warehouse;
                    AddWCSTask(ssjDirectTask);
                    break;
                case WMSTaskType.NoTaskQuit:
                case WMSTaskType.Inbound://输送机入库
                    if ((wMSTask.FmLocation == wMSTask.ToLocation && wMSTask.ToLocation != "1125" && wMSTask.ToLocation != "1115") || wMSTask.ToLocation == "0000")
                    {
                        WCSTask ssjTask = new WCSTask();
                        ssjTask.PalletNum = wMSTask.PalletNum;
                        ssjTask.FmLocation = wMSTask.FmLocation;
                        ssjTask.ToLocation = "0000";// wMSTask.ToLocation
                        ssjTask.WMSSeqID = wMSTask.WMSSeqID;
                        ssjTask.TaskStep = 1;
                        ssjTask.TaskType = WCSTaskTypes.SSJInbound;
                        ssjTask.DeviceID = GetSSJID(wMSTask.FmLocation);
                        ssjTask.TaskStatus = WCSTaskStatus.Waiting;
                        ssjTask.GaoDiBZ = (WCSGaoDiBZ)(int)wMSTask.GaoDiBZ;
                        ssjTask.Warehouse = wMSTask.Warehouse;
                        //WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                        AddWCSTask(ssjTask);
                    }
                    else
                    {
                        WCSTask ssjTask = new WCSTask();
                        ssjTask.PalletNum = wMSTask.PalletNum;
                        ssjTask.FmLocation = wMSTask.FmLocation;
                        //ssjTask.ToLocation =wMSTask.ToLocation;
                        if (wMSTask.ToLocation.Substring(0, 3) == "000")
                        {
                            //if(wMSTask.Warehouse=="1519")
                            //{
                            //    ssjTask.ToLocation = "0003";
                            //}
                            //else
                            //{
                            ssjTask.ToLocation = wMSTask.ToLocation;
                            //}

                        }
                        else
                        {
                            ssjTask.ToLocation = GetTunel(wMSTask.ToLocation);
                            //ssjTask.ToLocation = wMSTask.ToLocation;
                        }
                        ssjTask.WMSSeqID = wMSTask.WMSSeqID;
                        ssjTask.TaskStep = 1;
                        ssjTask.TaskType = WCSTaskTypes.SSJInbound;
                        ssjTask.DeviceID = GetSSJID(wMSTask.FmLocation);
                        ssjTask.TaskStatus = WCSTaskStatus.Waiting;
                        ssjTask.GaoDiBZ = (WCSGaoDiBZ)(int)wMSTask.GaoDiBZ;
                        ssjTask.Floor = wMSTask.Floor;
                        ssjTask.Warehouse = wMSTask.Warehouse;
                        AddWCSTask(ssjTask);
                    }
                    break;
                case WMSTaskType.Stacking:
                    WCSTask ddjStackTask = WCSTaskList.Find(x => x.TaskType == WCSTaskTypes.DDJStack && x.PalletNum == wMSTask.PalletNum);

                    if (ddjStackTask == null)
                    {

                        ddjStackTask = new WCSTask();
                        ddjStackTask.PalletNum = wMSTask.PalletNum;

                        ddjStackTask.FmLocation = GetDdjPortNumFromPosition(wMSTask.FmLocation);
                        // ddjStackTask.SourceLocation = wMSTask.FmLocation;//讲position传给wcstask用于ddj中的taskcando
                        ddjStackTask.ToLocation = wMSTask.ToLocation;//货位地址
                        ddjStackTask.TaskPri = wMSTask.Priority + 1;
                        ddjStackTask.WMSSeqID = wMSTask.WMSSeqID;
                        ddjStackTask.TaskStep = 2;
                        ddjStackTask.TaskType = WCSTaskTypes.DDJStack;
                        ddjStackTask.DeviceID = GetDDJIDFromGoodLocationWarehouse(wMSTask.ToLocation, wMSTask.Warehouse);//根据position和warehouse可以确定唯一ddjid
                        ddjStackTask.GaoDiBZ = (WCSGaoDiBZ)(int)wMSTask.GaoDiBZ;
                        ddjStackTask.Warehouse = wMSTask.Warehouse;
                        ddjStackTask.WMSTaskFmlocation = wMSTask.FmLocation;//巷道口的输送机块的position
                        if (wMSTask.TaskStatus == WMSTaskStatus.GoodLocChg)
                            ddjStackTask.TaskStatus = WCSTaskStatus.StackChged;
                        else
                        {
                            if (wMSTask.TaskSource == "WMS")
                            {
                                ddjStackTask.TaskStatus = WCSTaskStatus.Waiting;
                            }
                            else
                                //ddjStackTask.TaskStatus = WCSTaskStatus.Cannot;
                                ddjStackTask.TaskStatus = WCSTaskStatus.Waiting;
                        }
                        ddjStackTask.Floor = wMSTask.Floor;
                        AddWCSTask(ddjStackTask);
                    }
                    else
                    {
                        ddjStackTask.ToLocation = wMSTask.ToLocation;
                        ddjStackTask.WMSSeqID = wMSTask.WMSSeqID;
                        ddjStackTask.TaskStatus = WCSTaskStatus.StackChged;
                        ddjStackTask.Warehouse = wMSTask.Warehouse;
                        UpdateWCSTask(ddjStackTask);
                        DeMaticDDJ deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == ddjStackTask.DeviceID);
                        if (deMaticDDJ != null)
                        {
                            //deMaticDDJ.UpdateDematicBusyState(ddjStackTask.DeviceID, false);
                            deMaticDDJ.DDJWorkState = DDJDeviceWorkState.Standby;
                            deMaticDDJ.WCSEnable = true;
                        }
                    }
                    break;
                case WMSTaskType.InToOut:
                    WCSTask ssjinbound = new()
                    {
                        PalletNum = wMSTask.PalletNum,
                        FmLocation = wMSTask.FmLocation,
                        Warehouse = wMSTask.Warehouse,

                        //TaskPri = wMSTask.Priority,
                        WMSSeqID = wMSTask.WMSSeqID,
                        TaskStep = 1,
                        TaskType = WCSTaskTypes.SSJInbound,
                        DeviceID = GetSSJID(wMSTask.FmLocation),
                        TaskStatus = WCSTaskStatus.Waiting,
                        Floor = int.Parse(GetSSJBlockFloor(wMSTask.FmLocation))
                    };
                    if (wMSTask.ToLocation == "0000")
                    {
                        ssjinbound.ToLocation = wMSTask.ToLocation;//倒库任务退回
                        ssjinbound.TaskPri = wMSTask.Priority;
                        AddWCSTask(ssjinbound);
                    }
                    else
                    {
                        ssjinbound.ToLocation = "000" + wMSTask.FmLocation.Substring(2, 1);//不涉及1519
                        ssjinbound.TaskPri = wMSTask.Priority;
                        AddWCSTask(ssjinbound);
                        WCSTask stacking = new()
                        {
                            PalletNum = wMSTask.PalletNum,
                            FmLocation = GetDdjPortNumFromPosition(wMSTask.FmLocation),//入库申请的输送机口地址也可调用该方法
                                                                                       // FmLocation = GetSSJPositionFromGoodsLocationWareHous_Outbound(wMSTask.ToLocation,wMSTask.FmLocation, wMSTask.Warehouse),
                            ToLocation = GetDdjPortNumFromPosition(wMSTask.ToLocation),
                            TaskPri = wMSTask.Priority,
                            WMSSeqID = wMSTask.WMSSeqID,
                            TaskStep = 2,
                            TaskType = WCSTaskTypes.DDJDirect,
                            DeviceID = "DDJ0" + wMSTask.ToLocation.Substring(2, 1),
                            TaskStatus = WCSTaskStatus.Cannot,
                            Floor = int.Parse(GetSSJBlockFloor(wMSTask.ToLocation)),//搬运任务的层是目的地层
                            Warehouse = wMSTask.Warehouse,
                           // WMSTaskTolocation = wMSTask.ToLocation
                        };



                        SSJDevice sSJDevice2 = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssjinbound.DeviceID);
                        SSJDeviceBlock fm_blocky2 = sSJDevice2.DeviceBlockList.Find(x => x.Position == wMSTask.FmLocation);
                        SSJDeviceBlock to_blocky2;
                        DDJDevice dDJDevice2 = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == stacking.DeviceID);
                        if (fm_blocky2.Floor == 1 && (fm_blocky2.SystemType == DeviceSystemType.Picking || fm_blocky2.SystemType == DeviceSystemType.TotalPort))
                        {
                            to_blocky2= sSJDevice2.DeviceBlockList.Find(x => x.Tunnel == dDJDevice2.Tunnel && (x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.TotalPort) && x.Floor == fm_blocky2.Floor);
                        }
                        else
                        {
                            to_blocky2 = sSJDevice2.DeviceBlockList.Find(x => x.Tunnel == dDJDevice2.Tunnel && (x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.InboundFinish) && x.Floor == fm_blocky2.Floor);
                        }
                        stacking.WMSTaskFmlocation = to_blocky2.Position;//准备上架的起始位置

                        AddWCSTask(stacking);

                        WCSTask ssjoutbound = new()
                        {
                            PalletNum = wMSTask.PalletNum,
                            FmLocation = "000" + wMSTask.FmLocation.Substring(2, 1),//输送机出库任务的巷道要与入库保持一致，此处用入库
                                                                                    // FmLocation = GetSSJPositionFromGoodsLocationWareHous_Outbound(wMSTask.ToLocation,wMSTask.FmLocation, wMSTask.Warehouse),
                            ToLocation = wMSTask.ToLocation,
                            TaskPri = wMSTask.Priority,
                            WMSSeqID = wMSTask.WMSSeqID,
                            TaskStep = 3,
                            DeviceID = GetSSJID(wMSTask.ToLocation),
                            TaskStatus = WCSTaskStatus.Cannot,
                            Floor = int.Parse(GetSSJBlockFloor(wMSTask.ToLocation)),//搬运任务的层是目的地层
                            TaskType = WCSTaskTypes.SSJOutbound,
                            Warehouse = wMSTask.Warehouse
                        };
                        AddWCSTask(ssjoutbound);
                    }
                    
                    break;
                case WMSTaskType.Moving:
                    WCSTask ddjMoveTask= WCSTaskList.Find(x => x.TaskType == WCSTaskTypes.DDJStackMove && x.PalletNum == wMSTask.PalletNum);
                    if (ddjMoveTask == null)
                    {
                        ddjMoveTask = new WCSTask();
                        ddjMoveTask.PalletNum = wMSTask.PalletNum;
                        ddjMoveTask.FmLocation = wMSTask.FmLocation;
                        ddjMoveTask.ToLocation = wMSTask.ToLocation;
                        ddjMoveTask.TaskPri = wMSTask.Priority;
                        ddjMoveTask.WMSSeqID = wMSTask.WMSSeqID;
                        ddjMoveTask.TaskStep = 1;
                        ddjMoveTask.TaskType = WCSTaskTypes.DDJStackMove;
                        ddjMoveTask.DeviceID = GetDDJIDFromGoodLocationWarehouse(wMSTask.ToLocation, wMSTask.Warehouse);
                        ddjMoveTask.TaskStatus = WCSTaskStatus.Waiting;
                        ddjMoveTask.Warehouse = wMSTask.Warehouse;
                        AddWCSTask(ddjMoveTask);
                    }
                    else
                    {
                        ddjMoveTask.ToLocation = wMSTask.ToLocation;
                        ddjMoveTask.WMSSeqID = wMSTask.WMSSeqID;
                        ddjMoveTask.TaskStatus = WCSTaskStatus.StackChged;
                        UpdateWCSTask(ddjMoveTask);
                    }
                    break;
                case WMSTaskType.Outbound:
                case WMSTaskType.Picking:
                case WMSTaskType.MovingToOutbound:
                    
                    WCSTask ddjUnstackTask = new()
                    {
                        PalletNum = wMSTask.PalletNum,
                        FmLocation = wMSTask.FmLocation,
                        Warehouse = wMSTask.Warehouse,
                        ToLocation = GetDdjPortNumFromPosition(wMSTask.ToLocation),
                        //TaskPri = wMSTask.Priority,
                        WMSSeqID = wMSTask.WMSSeqID,
                        TaskStep = 1,
                        TaskType = WCSTaskTypes.DDJUnstack,
                        DeviceID = GetDDJIDFromGoodLocationWarehouse(wMSTask.FmLocation,wMSTask.Warehouse),
                        TaskStatus = WCSTaskStatus.Waiting,
                        Floor= int.Parse(GetSSJBlockFloor(wMSTask.ToLocation)),
                       // WMSTaskTolocation = wMSTask.ToLocation//出库任务的终点

                    };
                    ddjUnstackTask.TaskPri = wMSTask.Priority;
                    AddWCSTask(ddjUnstackTask);
                    WCSTask ssjOutboundTask = new()
                    {
                        PalletNum = wMSTask.PalletNum,
                        FmLocation = GetSSJTunelFromGoodsLocationWareHouse(wMSTask.FmLocation,wMSTask.Warehouse),
                       // FmLocation = GetSSJPositionFromGoodsLocationWareHous_Outbound(wMSTask.ToLocation,wMSTask.FmLocation, wMSTask.Warehouse),
                        ToLocation = wMSTask.ToLocation,
                        TaskPri = wMSTask.Priority,
                        WMSSeqID = wMSTask.WMSSeqID,
                        TaskStep = 2,
                        DeviceID = GetSSJID(wMSTask.ToLocation),
                        TaskStatus = WCSTaskStatus.Cannot,
                        Floor= int.Parse(GetSSJBlockFloor(wMSTask.ToLocation)),
                        Warehouse=wMSTask.Warehouse
                        
                    };
                    if (wMSTask.TaskType == WMSTaskType.Outbound||wMSTask.TaskType == WMSTaskType.MovingToOutbound)
                    {
                        ssjOutboundTask.TaskType = WCSTaskTypes.SSJOutbound;
                    }
                    else if(wMSTask.TaskType == WMSTaskType.Picking)
                    {
                        ssjOutboundTask.TaskType = WCSTaskTypes.SSJPickUpOutbound;
                    }
                    AddWCSTask(ssjOutboundTask);
                    break;
                case WMSTaskType.AutoInbound:
                    WCSTask ssjInboundTask = new WCSTask()
                    {
                        PalletNum = wMSTask.PalletNum,
                        FmLocation = wMSTask.FmLocation,
                        WMSSeqID = wMSTask.WMSSeqID,
                        TaskStep = 1,
                        TaskType = WCSTaskTypes.SSJInbound,
                        DeviceID = GetSSJID(wMSTask.FmLocation),
                        TaskStatus = WCSTaskStatus.Waiting,
                        GaoDiBZ = (WCSGaoDiBZ)(int)wMSTask.GaoDiBZ,
                        Floor = wMSTask.Floor,
                        Warehouse = wMSTask.Warehouse
                    };
                    if (wMSTask.ToLocation.Substring(0, 3) == "000")
                    {
                        ssjInboundTask.ToLocation = wMSTask.ToLocation;
                    }
                    else
                    {
                         ssjInboundTask.ToLocation = GetSSJTunelFromGoodsLocationWareHouse(wMSTask.ToLocation, wMSTask.Warehouse);
                       // ssjInboundTask.ToLocation = GetSSJPositionFromGoodsLocationWareHous_inbound(wMSTask.FmLocation, wMSTask.ToLocation, wMSTask.Warehouse);
                    }
                    AddWCSTask(ssjInboundTask);
                    WCSTask ddjStackingTask = new WCSTask()
                    {
                        PalletNum = wMSTask.PalletNum,
                        ToLocation = wMSTask.ToLocation,
                        FmLocation = GetInTaskStackingDdjPortNumWarhouse(wMSTask.FmLocation, wMSTask.ToLocation, wMSTask.Warehouse),//两侧同时入库 也要区分哪个口地址
                        TaskPri = wMSTask.Priority,
                        WMSSeqID = wMSTask.WMSSeqID,
                        TaskStep = 2,
                        TaskType = WCSTaskTypes.DDJStack,
                        DeviceID = GetDDJIDFromGoodLocationWarehouse(wMSTask.ToLocation, wMSTask.Warehouse),
                        TaskStatus = WCSTaskStatus.Cannot,
                        Floor = wMSTask.Floor,
                        //WMSTaskFmlocation = wMSTask.FmLocation//传入申请位置position，用于ddjstackcando找到对应的巷道口输送机
                        Warehouse =wMSTask.Warehouse
                    };
                    SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssjInboundTask.DeviceID);
                    SSJDeviceBlock fm_blocky = sSJDevice.DeviceBlockList.Find(x => x.Position == wMSTask.FmLocation);
                    SSJDeviceBlock to_blocky;
                    DDJDevice dDJDevice = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == ddjStackingTask.DeviceID);
                    if (fm_blocky.Floor == 1 && (fm_blocky.SystemType == DeviceSystemType.Picking || fm_blocky.SystemType == DeviceSystemType.TotalPort))
                    {
                        to_blocky = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == dDJDevice.Tunnel && (x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.TotalPort) && x.Floor == fm_blocky.Floor);
                    }
                    else
                    {
                        to_blocky = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == dDJDevice.Tunnel && (x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.InboundFinish) && x.Floor == fm_blocky.Floor);
                    }
                    ddjStackingTask.WMSTaskFmlocation = to_blocky.Position;//准备上架的起始位置


                    AddWCSTask(ddjStackingTask);
                    break;
                case WMSTaskType.DoubleWithoutLocation:            //堆垛机双重入库 无货位 WCS生成的任务 wMSTask.FmLocation=ZHT+堆垛机号
                    WCSTask ddjOnloadUnstackTask = new()
                    {
                        PalletNum = wMSTask.PalletNum,
                        FmLocation = wMSTask.FmLocation,
                        ToLocation = GetDdjPortNumFromPosition(wMSTask.ToLocation),
                        TaskPri = wMSTask.Priority,
                        WMSSeqID = wMSTask.WMSSeqID,
                        TaskStep = 1,
                        TaskType = WCSTaskTypes.DDJUnstack,
                        //DeviceID = string.Concat("DDJ0", wMSTask.FmLocation.AsSpan(3, 1)),
                        DeviceID = GetDDJIDFromGoodLocationWarehouse(wMSTask.FmLocation, wMSTask.Warehouse),
                        TaskStatus = WCSTaskStatus.Waiting,
                        Floor = wMSTask.Floor
                        ,Warehouse=wMSTask.Warehouse
                    };
                    AddWCSTask(ddjOnloadUnstackTask);
                    WCSTask ssjOnloadOutboundTask = new()
                    {
                        PalletNum = wMSTask.PalletNum,
                        //FmLocation = "000" + wMSTask.FmLocation.Substring(3, 1),
                         FmLocation = GetSSJTunelFromGoodsLocationWareHouse(wMSTask.FmLocation, wMSTask.Warehouse),
                       // FmLocation = GetSSJPositionFromGoodsLocationWareHous_Outbound(wMSTask.ToLocation,wMSTask.FmLocation, wMSTask.Warehouse),
                        ToLocation = wMSTask.ToLocation,
                        TaskPri = wMSTask.Priority,
                        WMSSeqID = wMSTask.WMSSeqID,
                        TaskStep = 2,
                        DeviceID = GetSSJID(wMSTask.ToLocation),
                        TaskStatus = WCSTaskStatus.Cannot,
                        Floor = wMSTask.Floor,
                        TaskType = WCSTaskTypes.SSJOutbound,
                        Warehouse = wMSTask.Warehouse
                    };
                    AddWCSTask(ssjOnloadOutboundTask);
                    break;
                case WMSTaskType.SSJOutbound://输送机出库任务（单机）
                    WCSTask ssjOutbound = new WCSTask()
                    {
                        PalletNum = wMSTask.PalletNum,
                       // FmLocation = GetTunelFromDeviceBlock(wMSTask.FmLocation),
                        // FmLocation = wMSTask.FmLocation,
                        ToLocation = wMSTask.ToLocation,
                        TaskPri = wMSTask.Priority,
                        WMSSeqID = wMSTask.WMSSeqID,
                        TaskStep = 1,
                        DeviceID = GetSSJID(wMSTask.ToLocation),
                        TaskStatus = WCSTaskStatus.Waiting,
                        Floor = wMSTask.Floor,
                        TaskType = WCSTaskTypes.SSJOutbound,
                        Warehouse = wMSTask.Warehouse
                    };
                    //if(wMSTask.FmLocation.Substring(0,2)=="13" || wMSTask.FmLocation.Substring(0, 2) == "14" ||wMSTask.FmLocation.Substring(0, 2) == "15")
                    //{
                    //    ssjOutbound.FmLocation = wMSTask.FmLocation; 
                    //}
                    //else { 
                        
                        ssjOutbound.FmLocation = GetTunelFromDeviceBlock(wMSTask.FmLocation);
                    //}
                    AddWCSTask(ssjOutbound);
                    break;
                case WMSTaskType.AGVMove:
                    AgvPosition agvPositionMdFm = AgvManager.Instance.AgvPositionList.Find(x => x.FLLPosition==wMSTask.FmLocation||x.FLLPosition==wMSTask.FmLocation);
                    if (agvPositionMdFm == null)
                    { return WMSTaskStatus.Mov_Fm_Wrong; }
                    AgvPosition agvPositionMdTo = AgvManager.Instance.AgvPositionList.Find(x => x.FLLPosition == wMSTask.ToLocation || x.FLLPosition == wMSTask.ToLocation);
                    if(agvPositionMdTo == null)
                    {
                        return WMSTaskStatus.Mov_To_Wrong;
                    }
                    WCSTask agvTask = new WCSTask()
                    {
                        FmLocation = agvPositionMdFm.Position,
                        ToLocation = agvPositionMdTo.Position,
                        DeviceID = AGVID,
                        WMSSeqID = wMSTask.WMSSeqID,
                        PalletNum = wMSTask.PalletNum,
                        TaskType = WCSTaskTypes.AgvBound,
                        TaskStep = 3,
                        TaskStatus = WCSTaskStatus.Cannot,
                    };
                    AddWCSTask(agvTask);
                    break;
                case WMSTaskType.None:
                    break;
                default:
                    break;
            }
            return WMSTaskStatus.TaskDoing;
        }
        private string GetTunelFromDeviceBlock(string position)
        {
            if (string.IsNullOrEmpty(position)) return "0000";
            //string ssj_id = string.Concat("SSJ0", position.AsSpan(0, 1));
            string ssj_id=GetSSJID(position);
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);
            if (sSJDevice == null) return "0000";
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == position);
            if (sSJDeviceBlock == null) return "0000";
            return sSJDeviceBlock.Tunnel;
        }
        private string GetTunel(string position)
        {
            if (string.IsNullOrEmpty(position)) return "0000";
            //string ssj_id = string.Concat("SSJ0", position.AsSpan(0, 1));
            string ssj_id = GetSSJID(position);
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);
            if (sSJDevice == null) return "0000";
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == position);
            if (sSJDeviceBlock == null) return "0000";
            return sSJDeviceBlock.Tunnel;
        }
        public string GetSSJID(string toLocation)
        {
            //string ssjid = "";
            //if (string.IsNullOrEmpty(toLocation)) return null;
            //ssjid = string.Concat("SSJ0", toLocation.AsSpan(0, 1));
            //return ssjid;
            string plcid = null;
            string sqlstr = string.Format("select PLCID from dbo.ConveyerBlocks where Position='{0}'", toLocation);
            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr);
            if (obj == null) return "";
            if (string.IsNullOrEmpty(obj.ToString())) return "";
            plcid = obj.ToString();
            return plcid;
        }
        private string GetSSJBlockFloor(string toLocation)
        {
            //string ssjid = "";
            //if (string.IsNullOrEmpty(toLocation)) return null;
            //ssjid = string.Concat("SSJ0", toLocation.AsSpan(0, 1));
            //return ssjid;
            string Floor = null;
            string sqlstr = string.Format("select Floor from dbo.ConveyerBlocks where Position='{0}'", toLocation);
            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr);
            if (obj == null) return "";
            if (string.IsNullOrEmpty(obj.ToString())) return "";
            Floor = obj.ToString();
            return Floor;
        }
        private string GetInTaskStackingDdjPortNum(string fmLocation, string toLocation)//根据离开的输送机position和货位地址确定堆垛机口地址
        {
            string ssj_id;
            //ssj_id = string.Concat("SSJ0", fmLocation.AsSpan(0, 1));
            ssj_id=GetSSJID(fmLocation);//获取输送机块的PLCID
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == toLocation);
            if (goodsLocation == null) return null;//判断货位是否存在
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);//
            if (sSJDevice == null) return null;
            SSJDeviceBlock sSJFmDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == fmLocation);
            if (sSJFmDeviceBlock == null) return null;
            SSJDeviceBlock sSJToDeviceBlock = sSJDevice.DeviceBlockList.Find(x => (x.SystemType == DeviceSystemType.InboundFinish || x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin) && x.Floor == sSJFmDeviceBlock.Floor && x.Tunnel == goodsLocation.Tunnel);
            if (sSJToDeviceBlock == null) return null;
            return sSJToDeviceBlock.PortNum;
        }
        private string GetInTaskStackingDdjPortNumWarhouse(string fmLocation, string toLocation,string warehouse)//根据输送机地址和货位地址和仓库名称确定靠近巷道口地址
        {
            string ssj_id;
            //ssj_id = string.Concat("SSJ0", fmLocation.AsSpan(0, 1));
            ssj_id = GetSSJID(fmLocation);//获取输送机块的PLCID
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == toLocation && x.Warehouse == warehouse);
            if (goodsLocation == null) return null;//判断货位是否存在
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);//
            if (sSJDevice == null) return null;
            SSJDeviceBlock sSJFmDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == fmLocation);
            if (sSJFmDeviceBlock == null) return null;

            SSJDeviceBlock sSJToDeviceBlock; // = sSJDevice.DeviceBlockList.Find(x => (x.SystemType == DeviceSystemType.InboundFinish || x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin) && x.Floor == sSJFmDeviceBlock.Floor && x.Tunnel == goodsLocation.Tunnel);

            if (sSJFmDeviceBlock.Floor == 1 && (sSJFmDeviceBlock.SystemType == DeviceSystemType.Picking || sSJFmDeviceBlock.SystemType == DeviceSystemType.TotalPort))
            {
                sSJToDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == goodsLocation.Tunnel && (x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.TotalPort) && x.Floor == sSJFmDeviceBlock.Floor);
            }
            else
            {
                sSJToDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == goodsLocation.Tunnel && (x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.InboundFinish) && x.Floor == sSJFmDeviceBlock.Floor);
            }


            if (sSJToDeviceBlock == null) return null;
            return GetDdjPortNumFromPosition(sSJToDeviceBlock.Position);
        }

        
        //private string GetStackingDdjPortNum(string toLocation)
        //{
        //    if (string.IsNullOrEmpty(toLocation)) return null;
        //    //string ssj_id = string.Concat("SSJ0", toLocation.AsSpan(0, 1));
        //    string ssj_id;
        //    ssj_id = GetSSJIDFromDeviceBlock(toLocation);
        //    SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);
        //    if (sSJDevice == null) return null;
        //    SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == toLocation);
        //    if (sSJDeviceBlock == null) return null;
        //    return sSJDeviceBlock.PortNum;
        //}
        //private string GetSSJIDFromDeviceBlock(string toLocation)
        //{
        //    string ssjid = "";
        //    if (string.IsNullOrEmpty(toLocation)) return null;
        //    //ssjid = string.Concat("SSJ0", toLocation.AsSpan(0, 1));
        //    if (sSJ01StackingPort.Contains(toLocation) || sSJ01UnstackingPort.Contains(toLocation) || sSJ01InApplyPort.Contains(toLocation) || sSJ01OutboundPort.Contains(toLocation))
        //        ssjid = "SSJ01";
        //    else if (sSJ02StackingPort.Contains(toLocation) || sSJ02UnstackingPort.Contains(toLocation) || sSJ02InApplyPort.Contains(toLocation) || sSJ02OutboundPort.Contains(toLocation))
        //        ssjid = "SSJ02";
        //    else
        //        ssjid = "";
        //    return ssjid;
        //}
        private string GetSSJTunelFromGoodsLocation(string goodlocation)
        {
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == goodlocation);
            if (goodsLocation == null) return null;
            return goodsLocation.Tunnel;
        }
        private string GetSSJTunel(string goodlocation)
        {
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == goodlocation);
            if (goodsLocation == null) return null;
            return goodsLocation.Tunnel;
        }
        private string GetSSJTunelFromGoodsLocationWareHouse(string goodlocation, string warehouse)
        {
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == goodlocation && x.Warehouse==warehouse);
            if (goodsLocation == null) return null;
            return goodsLocation.Tunnel;
        }
        private string GetSSJPositionFromGoodsLocationWareHous_inbound(string fmLocation, string toLocation, string warehouse)//根据入出库的输送机地址和货位地址和仓库名称确定靠近巷道口的输送机地址
        {
            string ssj_id;
            //ssj_id = string.Concat("SSJ0", fmLocation.AsSpan(0, 1));
            ssj_id = GetSSJID(fmLocation);//获取输送机块的PLCID
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == toLocation && x.Warehouse == warehouse);
            if (goodsLocation == null) return null;//判断货位是否存在
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);//
            if (sSJDevice == null) return null;
            SSJDeviceBlock sSJFmDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == fmLocation);
            if (sSJFmDeviceBlock == null) return null;
            SSJDeviceBlock sSJToDeviceBlock = sSJDevice.DeviceBlockList.Find(x => (x.SystemType == DeviceSystemType.InboundFinish || x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin ) && x.Floor == sSJFmDeviceBlock.Floor && x.Tunnel == goodsLocation.Tunnel);
            if (sSJToDeviceBlock == null) return null;
            return sSJToDeviceBlock.Position;
        }
        private string GetSSJPositionFromGoodsLocationWareHous_Outbound(string fmLocation, string toLocation, string warehouse)//根据入出库的输送机地址和货位地址和仓库名称确定靠近巷道口的输送机地址
        {
            string ssj_id;
            //ssj_id = string.Concat("SSJ0", fmLocation.AsSpan(0, 1));
            ssj_id = GetSSJID(fmLocation);//获取输送机块的PLCID
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == toLocation && x.Warehouse == warehouse);
            if (goodsLocation == null) return null;//判断货位是否存在
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);//
            if (sSJDevice == null) return null;
            SSJDeviceBlock sSJFmDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == fmLocation);
            if (sSJFmDeviceBlock == null) return null;
            SSJDeviceBlock sSJToDeviceBlock = sSJDevice.DeviceBlockList.Find(x => (x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.OutboundBegin) && x.Floor == sSJFmDeviceBlock.Floor && x.Tunnel == goodsLocation.Tunnel);
            if (sSJToDeviceBlock == null) return null;
            return sSJToDeviceBlock.Position;
        }
        private string GetSSJTunelWareHouse(string goodlocation,  string warehouse)
        {
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == goodlocation && x.Warehouse == warehouse);
            if (goodsLocation == null) return null;
            return goodsLocation.Tunnel;
        }
        private string GetDdjUnloadDeviceID(WMSTask wMSTask)
        {
            if (string.IsNullOrEmpty(wMSTask.ToLocation)) return null;
            string ssj_id= string.Concat("SSJ0", wMSTask.ToLocation.AsSpan(0, 1));
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);
            if (sSJDevice == null) return null;
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == wMSTask.FmLocation);
            if (goodsLocation == null) return null;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == goodsLocation.Tunnel&&x.SystemType==DeviceSystemType.InboundBegin);
            if (sSJDeviceBlock == null) return null;
            return sSJDeviceBlock.Position;
        }
        private string GetDdjPortNumFromTolocation(string toLocation)
        {
            SSJDevice sSJDevice = null;
            string plcid = null;
            string sqlstr = string.Format("select PLCID from dbo.ConveyerBlocks where Position='{0}'", toLocation);
            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr);
            if (obj == null) return "";
            if (string.IsNullOrEmpty(obj.ToString())) return "";
            plcid = obj.ToString();
            sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == plcid);
            if(sSJDevice == null) return "";
            return sSJDevice.PortNum;
        }
        private string GetDdjPortNum(string toLocation)
        {
            //if (string.IsNullOrEmpty(toLocation)) return null;
            //string ssj_id = string.Concat("SSJ0", toLocation.AsSpan(0, 1));
            
            //SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);
            //if (sSJDevice == null) return null;
            //return sSJDevice.PortNum;
            SSJDevice sSJDevice=PLCDeviceManager.Instance.SSJDeviceList.Find(x=>x.PLCID==GetSSJID(toLocation));
            if (sSJDevice == null) return "";
            return sSJDevice.PortNum;
        }
         private string GetDdjPortNumFromPosition(string toLocation)//得力项目从输送机设备号(可以是入库申请的设备号，也可以是货位申请的想到号)获取堆垛机口地址
        {
            string ddjport = "";
                switch (toLocation.Substring(0, 2))
                {
                    case "11":
                    if (toLocation.Substring(3, 1) == "5") { ddjport = "000a"; }
                    else 
                    { 
                        ddjport = "000b"; 
                    }
                        break;
                    case "12":
                    if (toLocation.Substring(3, 1) == "1" || toLocation.Substring(3, 1) == "0") { ddjport = "000f"; }
                    else
                    {
                        ddjport = "000e";
                    }
                        break;
                    case "21":
                    ddjport = "000c";
                        break;
                    case "22":
                    ddjport = "000g";
                        break;
                    case "31":
                    ddjport = "000d";
                        break;
                    case "32":
                    ddjport = "000h";
                    break;
                case "40":
                    ddjport = "000k";
                    break;
                //case "5":
                //    port_num = "000b";
                //    break;
                default:
                    ddjport = null;
                        break;
                }
            return ddjport;
        }
        public void UpdateWCSTaskPriAndStatus(WCSTask wCSTask, int pri, WCSTaskStatus wCSTaskStatus)
        {
            try
            {
                if (wCSTask == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@TaskStatus",SqlNull( wCSTaskStatus)),
                    new SqlParameter("@TaskPri",SqlNull( pri)),
                    new SqlParameter("@WCSSeqID",SqlNull(wCSTask.WCSSeqID))
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[WCSTaskList] SET [TaskStatus] = @TaskStatus,[TaskPri] = @TaskPri WHERE WCSSeqID=@WCSSeqID", sqlParameters);
                wCSTask.TaskStatus = wCSTaskStatus;
                wCSTask.TaskPri = pri;
                OnWCSTaskChanged(wCSTask);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateWCSTaskPriAndStatus 写入出错!", ex);
            }
        }
        public void UpdateWCSTaskPri(WCSTask wCSTask, int pri)
        {
            try
            {
                if (wCSTask == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@TaskPri",SqlNull( pri)),
                    new SqlParameter("@WCSSeqID",SqlNull(wCSTask.WCSSeqID))
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[WCSTaskList] SET [TaskPri] = @TaskPri WHERE WCSSeqID=@WCSSeqID", sqlParameters);
                wCSTask.TaskPri = pri;
                OnWCSTaskChanged(wCSTask);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateWCSTaskPri 写入出错!", ex);
            }
        }
        public void UpdateWCSTask(WCSTask wCSTask)
        {
            if (wCSTask == null) return;
            WCSTask wCSTask1 = WCSTaskList.Find(x => x.WCSSeqID == wCSTask.WCSSeqID);
            if (wCSTask1 == null) return;
            WCSTaskList.Remove(wCSTask1);
            WCSTaskList.Add(wCSTask);
            UpdateWCSTaskDB(wCSTask);
            OnWCSTaskChanged(wCSTask);
            OnWCSTaskStatusChanged(wCSTask);
        }
        public void UpdateWCSTaskDB(WCSTask wCSTask)
        {
            if (wCSTask == null) return;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@WMSSeqID",SqlNull(wCSTask.WMSSeqID)),
                    new SqlParameter("@WCSSeqID",SqlNull(wCSTask.WCSSeqID)),
                    new SqlParameter("@CreateTime",SqlNull(wCSTask.CreateTime)),
                    new SqlParameter("@TaskType",SqlNull(wCSTask.TaskType)),
                    new SqlParameter("@DeviceID",SqlNull(wCSTask.DeviceID)),
                    new SqlParameter("@PalletNum",SqlNull(wCSTask.PalletNum)),
                    new SqlParameter("@TaskPri",SqlNull(wCSTask.TaskPri)),
                    new SqlParameter("@FmLocation",SqlNull(wCSTask.FmLocation)),
                    new SqlParameter("@ToLocation",SqlNull(wCSTask.ToLocation)),
                    new SqlParameter("@TaskStep",SqlNull(wCSTask.TaskStep)),
                    new SqlParameter("@TaskStatus",SqlNull(wCSTask.TaskStatus)),
                    new SqlParameter("@StartTime",SqlNull(wCSTask.StartTime)),
                    new SqlParameter("@FinishTime",SqlNull(wCSTask.FinishTime)),
                };

                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[WCSTaskList]SET" +
                    " [WMSSeqID] = @WMSSeqID" +
                    //",[WCSSeqID] = @WCSSeqID"+
                    ",[PalletNum] = @PalletNum" +
                    ",[FmLocation] = @FmLocation" +
                    ",[ToLocation] = @ToLocation" +
                    ",[CreateTime] = @CreateTime" +
                    ",[StartTime] = @StartTime" +
                    ",[FinishTime] = @FinishTime" +
                    ",[DeviceID] = @DeviceID" +
                    ",[TaskType] = @TaskType" +
                    ",[TaskStatus] = @TaskStatus" +
                    ",[TaskPri] = @TaskPri" +
                    ",[TaskStep] = @TaskStep" +
                    " WHERE [WCSSeqID] = @WCSSeqID ", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateWCSTask 写入出错!", ex);
            }
        }
        private string GetDDJIDFromGoodLocation(string toLocation)
        {
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == toLocation);
            if (goodsLocation == null) return null;
            return goodsLocation.DDJID;
        }
        private string GetDDJIDFromGoodLocationWarehouse(string toLocation, String Warehouse)
        {
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == toLocation && x.Warehouse== Warehouse);
            if (goodsLocation == null ) return null;
            return goodsLocation.DDJID;
        }
        private string GetSSJIDInboundFromWMSTask(string fmLocation)
        {
            if (string.IsNullOrEmpty(fmLocation)) return "";
            return string.Concat("SSJ0", fmLocation.AsSpan(0, 1));
        }
        #endregion
        public void AddWCSTask(WCSTask wCSTask)
        {
            if (wCSTask == null) return;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@Warehouse",SqlNull(wCSTask.Warehouse)),
                    new SqlParameter("@WMSSeqID",wCSTask.WMSSeqID),
                    new SqlParameter("@WCSSeqID",wCSTask.WCSSeqID),
                    new SqlParameter("@CreateTime",wCSTask.CreateTime),
                    new SqlParameter("@TaskType",SqlNull(wCSTask.TaskType)),
                    new SqlParameter("@DeviceID",wCSTask.DeviceID),
                    new SqlParameter("@PalletNum",SqlNull(wCSTask.PalletNum)),
                    new SqlParameter("@TaskPri",wCSTask.TaskPri),
                    new SqlParameter("@FmLocation",SqlNull(wCSTask.FmLocation)),
                    new SqlParameter("@ToLocation",SqlNull(wCSTask.ToLocation)),
                    new SqlParameter("@TaskStep",SqlNull(wCSTask.TaskStep)),
                    new SqlParameter("@TaskStatus",SqlNull(wCSTask.TaskStatus)),
                    new SqlParameter("@StartTime",SqlNull(wCSTask.StartTime)),
                    new SqlParameter("@GaoDiBZ",SqlNull(wCSTask.GaoDiBZ)),
                    new SqlParameter("@Floor",SqlNull(wCSTask.Floor)),
                    //new SqlParameter("@WMSTaskTolocation",SqlNull(wCSTask.WMSTaskTolocation)),
                    new SqlParameter("@WMSTaskFmlocation",SqlNull(wCSTask.WMSTaskFmlocation)),
                    

                };

                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[WCSTaskList]" +
                    "([Warehouse],[WMSSeqID],[WCSSeqID],[PalletNum],[FmLocation],[ToLocation],[CreateTime],[StartTime],[DeviceID],[TaskType],[TaskStatus],[TaskPri],[TaskStep],[GaoDiBZ],[Floor],[WMSTaskFmlocation]) VALUES　" +
                    "(@Warehouse,@WMSSeqID, @WCSSeqID, @PalletNum, @FmLocation, @ToLocation, @CreateTime, @StartTime, @DeviceID, @TaskType, @TaskStatus, @TaskPri, @TaskStep,@GaoDiBZ,@Floor,@WMSTaskFmlocation)", sqlParameters);
                WCSTaskList.Add(wCSTask);
                OnWCSTaskAdded(wCSTask);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("AddWCSTask 写入出错!", ex);
            }
        }
        public void DeleteWCSTasksAtWMSSeq(string wMSSeqID)
        {
            if (string.IsNullOrEmpty(wMSSeqID)) return;
            List<WCSTask> wCSTasks = WCSTaskList.FindAll(x => x.WMSSeqID == wMSSeqID);
            if (wCSTasks == null || wCSTasks.Count < 1) return;
            foreach(WCSTask wCSTask in wCSTasks)
            {
                DeleteWCSTask(wCSTask);
            }
        }
        public void DeleteWCSTask(string wcs_seq_id)
        {
            if (string.IsNullOrEmpty(wcs_seq_id)) return;
            WCSTask wCSTask = WCSTaskList.Find(x => x.WCSSeqID == wcs_seq_id);
            if (wCSTask == null) return;
            DeleteWCSTask(wCSTask);
        }
        public void DeleteWCSTask(WCSTask wCSTask)
        {
            try
            {
                if (wCSTask == null) return;
                WCSTaskList.Remove(wCSTask);
                SQLServerHelper.DataBaseExecute(string.Format("DELETE FROM [dbo].[WCSTaskList] WHERE WCSSeqID='{0}'", wCSTask.WCSSeqID));
                OnWCSTaskDeleted(wCSTask);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DeleteWCSTask 写入出错!", ex);
            }
        }
        public void UpdateWCSTaskStatus(WCSTask wCSTask, WCSTaskStatus wCSTaskStatus)
        {
            try
            {
                if (wCSTask == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@TaskStatus",SqlNull( wCSTaskStatus)),
                    new SqlParameter("@WCSSeqID",SqlNull(wCSTask.WCSSeqID))
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[WCSTaskList] SET [TaskStatus] = @TaskStatus WHERE WCSSeqID=@WCSSeqID", sqlParameters);
                wCSTask.TaskStatus = wCSTaskStatus;
                OnWCSTaskChanged(wCSTask);
                OnWCSTaskStatusChanged(wCSTask);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateWCSTaskStatus 写入出错!", ex);
            }
        }

        private void OnWCSTaskStatusChanged(WCSTask wCSTask)
        {
            if (wCSTask.TaskStatus == WCSTaskStatus.Done)
            {
                if (wCSTask.TaskType == WCSTaskTypes.DDJStack)
                {
                    GoodsLocationManager.Instance.UpdateGoodLocationPalletNum(wCSTask.ToLocation, wCSTask.PalletNum, wCSTask.Warehouse);
                }
                else if (wCSTask.TaskType == WCSTaskTypes.DDJUnstack)
                {
                    GoodsLocationManager.Instance.UpdateGoodLocationPalletNum(wCSTask.FmLocation, null, wCSTask.Warehouse);
                }
                else if (wCSTask.TaskType == WCSTaskTypes.DDJStackMove)
                {
                    GoodsLocationManager.Instance.UpdateGoodLocationPalletNum(wCSTask.FmLocation, null, wCSTask.Warehouse);
                    GoodsLocationManager.Instance.UpdateGoodLocationPalletNum(wCSTask.ToLocation, wCSTask.PalletNum, wCSTask.Warehouse);
                }
                else if (wCSTask.TaskType == WCSTaskTypes.SSJInbound)
                {
                    SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == wCSTask.DeviceID);
                    if (sSJDevice != null)
                    {
                        SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == wCSTask.ToLocation&&x.SystemType==DeviceSystemType.InboundFinish&&x.Floor==wCSTask.Floor);
                        //if (sSJDeviceBlock != null)
                            //WMSTasksManager.Instance.UpdateWMSTaskToLocation(wCSTask.WMSSeqID,sSJDeviceBlock.Position);
                    }
                }
                WCSTask nextwCSTask = WCSTaskList.Find(x => x.WMSSeqID == wCSTask.WMSSeqID && x.TaskStep == (wCSTask.TaskStep + 1));
                if (nextwCSTask == null)
                {
                    WMSTasksManager.Instance.FinishWMSTask(wCSTask.WMSSeqID);
                    List<WCSTask> wCSTasks = WCSTaskList.FindAll(x => x.WMSSeqID == wCSTask.WMSSeqID);
                    WCSTask nextwCSTask2 = WCSTaskList.Find(x => x.PalletNum == wCSTask.PalletNum && x.TaskStep == (wCSTask.TaskStep + 1));
                    if (nextwCSTask2 != null)
                    {
                        if (nextwCSTask2.TaskStatus != WCSTaskStatus.Waiting)
                        {
                            UpdateWCSTaskStatus(nextwCSTask2, WCSTaskStatus.Waiting);
                            if (wCSTasks != null && wCSTasks.Count > 0)
                            {
                                foreach (WCSTask task in wCSTasks)
                                {
                                    DeleteWCSTask(task);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (wCSTasks != null && wCSTasks.Count > 0)
                        {
                            foreach (WCSTask task in wCSTasks)
                            {
                                DeleteWCSTask(task);
                            }
                        }
                    }
                }
                else
                {
                    if (nextwCSTask.TaskStatus != WCSTaskStatus.Waiting)
                    {
                        WMSTask wMSTask=WMSTasksManager.Instance.WMSTaskList.Find(x => x.WMSSeqID==wCSTask.WMSSeqID&&x.TaskType== WMSTaskType.InToOut);
                        //if (wMSTask != null&&wCSTask.TaskType== WCSTaskTypes.SSJInbound)
                            //UpdateWCSTaskFloor(nextwCSTask, wCSTask.Floor);  //针对堆垛机直出任务 输送机入库完成 更新堆垛机任务楼层为输送机入库完成楼层
                        UpdateWCSTaskStatus(nextwCSTask, WCSTaskStatus.Waiting);
                    }
                }
                return;
            }
            else if(wCSTask.TaskStatus == WCSTaskStatus.DDJPicked && (wCSTask.TaskType== WCSTaskTypes.DDJUnstack||wCSTask.TaskType== WCSTaskTypes.DDJStackMove||wCSTask.TaskType== WCSTaskTypes.DDJStack||wCSTask.TaskType== WCSTaskTypes.DDJDirect))
            {
                WMSTasksManager.Instance.UpdateWMSTaskStatus(wCSTask.WMSSeqID, WMSTaskStatus.TaskDoinging);
            }
            else if (wCSTask.TaskStatus == WCSTaskStatus.StackDoubleConfirm)
            {
                WMSTasksManager.Instance.InboundTwiceComfirm(wCSTask.WMSSeqID);
            }
            else if (wCSTask.TaskStatus == WCSTaskStatus.UnStackEmptyConfirm)
            {
                WMSTasksManager.Instance.EmptyUnStackConfirmWMSTask(wCSTask.WMSSeqID);
                GoodsLocationManager.Instance.UpdateGoodLocationPalletNum(wCSTask.FmLocation, null, wCSTask.Warehouse);
                List<WCSTask> wCSTasks = WCSTaskList.FindAll(x => x.WMSSeqID == wCSTask.WMSSeqID);
                if (wCSTasks != null && wCSTasks.Count > 0)
                {
                    foreach (WCSTask task in wCSTasks)
                    {
                        DeleteWCSTask(task);
                    }
                }
            }
            else if (wCSTask.TaskStatus == WCSTaskStatus.FarOutboundClearHaveConfirm)
            {
                WMSTasksManager.Instance.FarOutboundClearHaveConfirm(wCSTask.WMSSeqID);
            }
            else if (wCSTask.TaskStatus == WCSTaskStatus.Doing)
            {
                WMSTasksManager.Instance.UpdateWMSTaskStatus(wCSTask.WMSSeqID, WMSTaskStatus.TaskDoing);
            }
            else if (wCSTask.TaskStatus == WCSTaskStatus.LightLED)
            {
                WMSTasksManager.Instance.UpdateWMSTaskStatus(wCSTask.WMSSeqID, WMSTaskStatus.LightLED);            
            }
            //else if(wCSTask.TaskStatus== WCSTaskStatus.SSJDirectDone)
            //{
            //    if (wCSTask.TaskStatus != WCSTaskStatus.Waiting)
            //        UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Waiting);
            //}
        }
        #region WCSTask事件定义
        private void OnWCSTaskAdded(WCSTask wCSTask)
        {
            WCSTaskAdded?.Invoke(this, new WCSTaskEventArgs(wCSTask));
        }

        private void OnWCSTaskDeleted(WCSTask wCSTask)
        {
            WCSTaskDeleted?.Invoke(this, new WCSTaskEventArgs(wCSTask));
        }

        private void OnWCSTaskChanged(WCSTask wCSTask)
        {
            WCSTaskChanged?.Invoke(this, new WCSTaskEventArgs(wCSTask));
        }

        #endregion
        public object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;
            return obj;
        }

        internal void UpdateWCSTaskFmLocation(WCSTask wCSTask, string fmLocation)
        {
            //throw new NotImplementedException();
            try
            {
                if (wCSTask == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@FmLocation",SqlNull( fmLocation)),
                    new SqlParameter("@WCSSeqID",SqlNull(wCSTask.WCSSeqID))
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[WCSTaskList] SET [FmLocation] = @FmLocation WHERE WCSSeqID=@WCSSeqID", sqlParameters);
                wCSTask.FmLocation = fmLocation;
                OnWCSTaskChanged(wCSTask);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateWCSTaskStatus 写入出错!", ex);
            }
        }
        internal void UpdateWCSTaskFloor(WCSTask wCSTask, int floor)
        {
            //throw new NotImplementedException();
            try
            {
                if (wCSTask == null) return;
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@Floor",SqlNull( floor)),
                    new SqlParameter("@WCSSeqID",SqlNull(wCSTask.WCSSeqID))
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[WCSTaskList] SET [Floor] = @Floor WHERE WCSSeqID=@WCSSeqID", sqlParameters);
                wCSTask.Floor = floor;
                OnWCSTaskChanged(wCSTask);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateWCSTaskStatus 写入出错!", ex);
            }
        }
    }
    public class WCSTaskEventArgs : EventArgs
    {
        public WCSTaskEventArgs(WCSTask wCSTask)
        {
            WCSTask = wCSTask;
        }
        public WCSTask WCSTask { get; set; }
    }
}
