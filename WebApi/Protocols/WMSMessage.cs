using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace BMHRI.WCS.Server.WebApi.Protocols
{
    public class RegisterDeviceReqMsg
    {
        public string username { get; set; }
        public string password { get; set; }
        public RegisterDeviceReqMsg()
        {
            username = "WCS";
            password = "123456";
        }
    }
    #region WCS->WMS
    public class WCSReqWMSMsg//WCS请求WMS
    {
        public string Location { get; set; }
        public string Palno { get; set; }
        public string Port { get; set; }
        public string Deviceno { get; set; }
        public WCSToWMSTaskType TaskType { get; set; }
        public string DevicenoStatu { get; set; }
        public string BeginAddre { get; set; }
        public string EndAddre { get; set; }
        public string Weight { get; set; }
        public string HLAddre { get; set; }
        public WCSReqWMSMsg()
        {
            Location = "";
        }
    }
    public class WMSRspWCSMsg//WMS应答WCS
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public WMSRspWCSData Data { get; set; }
    }
    public class WMSRspWCSData//WMS应答WCS中的DATA
    {
        public string Location { get; set; }
        public string Palno { get; set; }
        public string Port { get; set; }
        public string Deviceno { get; set; }
        public string Floor { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WMSToWCSTaskType TaskType { get; set; }
        public string BeginAddre { get; set; }
        public string EndAddre { get; set; }
        public string EndType { get; set; }
        public string IfDo { get; set; }
        public string Matno { get; set; }
        public string Batch { get; set; }
    }
    #endregion
    #region WMS->WCS
    public class WMSReqWCSMsg//WMS请求WCS
    {
        public string Location { get; set; }
        public string Palno { get; set; }
        public string Port { get; set; }
        public string Deviceno { get; set; }
        public string Floor { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WMSToWCSTaskType TaskType { get; set; }
        public string BeginAddre { get; set; }
        public string EndAddre { get; set; }
        public string EndType { get; set; }
        public string If_do { get; set; }
        public string Matno { get; set; }
        public string Batch { get; set; }
    }
    public class WCSRspWMSMsg//WCS应答WMS
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public WCSRspWMSData Data { get; set; }//这个应答好像是null
        public WCSRspWMSMsg()
        {
            Code = "0";
            Message = "任务下发成功!";
        }
    }
    public class WCSRspWMSData//WCS应答WMS的Data
    {
    }
    #endregion

    public enum WCSToWMSTaskType
    {
        None,
        [Description("入库申请")]
        InboundApply,
        [Description("货位申请")]
        InboundSecondApply,
        [Description("入库完成")]
        InboundFinish,
        [Description("出库完成")]
        OutboundFinish,
        [Description("点亮LED")]
        LedOn,
        [Description("熄灭LED")]
        LedOff,
        [Description("堆垛机状态")]
        DDJStatus,

        [Description("申请空框")]
        EmptyBoxApply,
        [Description("设备故障")]
        DeviceFault,
        [Description("倒库完成")]
        MoveFinish,
        [Description("双重入库")]
        DoubleInbound,
        [Description("操作远端近端有货")]
        FarOutboundNearHave,
        [Description("空出库")]
        UnStack_Empty,
        [Description("允许放货")]
        PutAllowed=24,
        [Description("允许取货")]
        GetAllowed=23
    }
    public enum WMSToWCSTaskType
    {
        None,
        [Description("入库巷道反馈")]
        TunnelFeeback,
        [Description("货位反馈")]
        LocationFeeback,
        [Description("出库任务")]
        OutBoundTask,
        [Description("倒库任务")]
        MoveTask,
        [Description("取消任务")]
        TaskCancel,
        [Description("申请放货")]
        ApplyPut=10,
        [Description("放货完成")]
        PutFinish=11,
        [Description("申请取货")]
        ApplyGet=8,
        [Description("取货完成")]
        GetFinish=9
    }
    public enum MsgDirection
    {
        Output,
        Input,
    }

    //public enum ExceptTypes
    //{
    //    [Description("无")]
    //    None = 0,
    //    [Description("空出库")]
    //    UnStackEmpty = 1,
    //    [Description("远端出库近端有货")]
    //    FarOutboundClearHave = 2,
    //    [Description("双重入库")]
    //    DoubleInbound = 3,
    //}
}
