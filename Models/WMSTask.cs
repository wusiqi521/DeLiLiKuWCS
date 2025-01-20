using System;
using System.ComponentModel;

namespace BMHRI.WCS.Server.Models
{
    public class WMSTask
    {
        public WMSTaskType TaskType { get; set; }
        //public string WMSTaskID { get; set; }
        public string PalletNum { get; set; }
        public string FmLocation { get; set; }
        public string ToLocation { get; set; }
        public int Priority { get; set; }
        public WMSTaskStatus TaskStatus { get; set; }
        public string CreateTime { get; set; }
        public string WMSSeqID { get; set; }
        public string TaskSource { get; set; }
        public string WeightNum { get; set; }
        public WMSGaoDiBZ GaoDiBZ { get; set; }
        public int DiePanJiCount { get; set; }
        public string Message { get; set; }
        public string TotalNum { get; set; }
        public string PickNum { get; set; }
        public string Sku { get; set; }
        public string MateralName { get; set; }
        public string OrderNum { get; set; }
        public string Destination { get; set; }
        public int Floor { get; set; }
        public string udf01 { get; set; }
        public string udf02 { get; set; }
        public string udf03 { get; set; }
        public string udf04 { get; set; }
        public string udf056 { get; set; }
        public string udf06 { get; set; }
        public string taskId { get; set; }
        public string WMSLocation { get; set; }
        public string WMSLocation2 { get; set; }
        public string Warehouse { get; set; }
        public string LedMessage { get; set; }
        public string Direction {  get; set; }
        public string SourceLocation {  get; set; }

        private static object obj = new object();
        public WMSTask()
        {
            lock (obj)
            {
                try
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff");
                    WMSSeqID = Guid.NewGuid().ToString();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
        }
    }
    public enum WMSTaskType
    {
        [Description("未知任务")]
        None,
        [Description("输送机入库任务")]
        Inbound,
        [Description("上架任务")]
        Stacking,
        [Description("移库任务")]
        Moving,
        [Description("整托出库")]
        Outbound,
        [Description("移库转出库")]
        MovingToOutbound,
        [Description("拣选出库")]
        Picking,
        [Description("堆垛机搬运任务")]//得力项目暂且改一描述
        InToOut,
        [Description("提升机换层直出任务")]
        Directe,
        [Description("空托申请")]
        EmptyWanted,
        [Description("设备消息")]
        DeviceMsg,
        [Description("无任务")]
        NoTaskQuit,
        [Description("LED展示")]
        LedDisplay,
        [Description("输送机出库")]
        SSJOutbound,
        [Description("输送机to1026")]
        SSJTo1026,
        [Description("联机入库任务")]
        AutoInbound,
        [Description("双重入库无货位转出")]
        DoubleWithoutLocation,
        [Description("AGV搬运任务")]
        AGVMove,
        [Description("点位释放")]
        AGVPositionRelease,
        [Description("申请取货")]
        AGVApplyGet,
        [Description("申请放货")]
        AGVApplyInput
    }
    public enum WMSTaskStatus
    {
        None,
        [Description("SSJ申请入库")]
        SSJ_APP_IN, //申请入库
        [Description("SSJ二次申请入库")]
        SSJ_APP_IN_Again,
        [Description("WMS任务初始化")]
        TaskAssigned,
        [Description("WMS任务下发完成")]
        TaskDoing,
        [Description("WMS正在执行中")]
        TaskDoinging,
        [Description("WMS正在执行中确认")]
        TaskDoingingConfirm,
        [Description("WMS任务完成")]
        TaskDone,
        [Description("WMS任务反馈失败")]
        TaskFeedBackFailed,
        [Description("SSJ空托申请")]
        SSJ_APP_EM,
        [Description("碟盘机数量更新")]
        EmptyCountUpdate,
        [Description("双重入库")]
        Double_Inbound, 
        [Description("空出库")]
        UnStack_Empty, 
        [Description("远端入库近端有货")]
        Put_Down_Stop,
        [Description("远端出库近端有货")]
        Pick_Up_Stop, 

        [Description("入库起始地址错误")]
        Inb_Fm_Wrong,  
        [Description("入库目的地址错误")]
        Inb_To_Wrong,  
        [Description("出库起始地址错误")]
        Out_Fm_Wrong,  
        [Description("出库目的地址错误")]
        Out_To_Wrong,  
        [Description("移库起始地址错误")]
        Mov_Fm_Wrong,  
        [Description("移库目的地址错误")]
        Mov_To_Wrong,  
        [Description("直出目的地址错误")]
        IO_To_Wrong, 
        [Description("直出起始地址错误")]
        IO_Fm_Wrong,   
        [Description("托盘号错误")]
        TrayID_Wrong,  
        [Description("DDJ不可用")]
        DDJUnReady,   
        [Description("60列不可达")]
        Rank60Unreach,   
        [Description("二次申请失败")]
        ReApplyFailed,  
        [Description("二次申请")]
        ApplyGOAgein,  
        [Description("目标货位改址")]
        GoodLocChg,   
        [Description("申请入库失败")]
        ApplyFailed,   
        [Description("设备状态变化")]
        DeviceStatusChg,   
        [Description("点亮LED")]
        LightLED,
        [Description("熄灭LED")]
        UnLightLED,
        [Description("AGV允许取货")]
        AGVAllowedGet,
        [Description("AGV允许放货")]
        AGVAllowedSet,
        [Description("AGV不允许取货")]
        AGVNotAllowedGet,
        [Description("AGV不允许放货")]
        AGVNotAllowedSet,
        [Description("DDJ开始工作")]
        DDJTaskStart,
        [Description("SSJ开始工作")]
        SSJTaskStart,
        [Description("SSJ入库模式")]
        SSJPortModeIn,
        [Description("SSJ出库模式")]
        SSJPortModeOut,
        [Description("SSJ1026到位")]
        SSJ1026Arrive,
        [Description("SSJ直出到位")]
        SSJDirectDone,
        [Description("SSJ异常到位")]
        SSJAlarm,
        [Description("SSJ物料信号")]
        SSJMateriExist,
        [Description("SSJ中途到位")]
        SSJMidArrive,
        [Description("SSJ开始倒料")]
        SSJPutMaterial,
        [Description("巷道不可达")]
        TunnelCannotArrive,
        [Description("AGV点位变化")]
        AGVPositionChg,
        [Description("AGV申请取货")]
        AGVApplyGet,
        [Description("AGV申请放货")]
        AGVApplyInput,
        [Description("出库任务改址")]
        ChangeOutBound,
    }
    public enum WMSGaoDiBZ
    {
        [Description("低托盘")]
        Low,
        [Description("中托盘")]
        Middle,
        [Description("高托盘")]
        Height,
    }
}
