using BMHRI.WCS.Server.Models;
using Newtonsoft.Json;
using System;

namespace BMHRI.WCS.Server.WCSProtocol
{
    public class SysMessage
    {
        public MsgType MsgType { get; set; }
        public string Sender { get; set; }
        public string Recver { get; set; }
        public int MsgID { get; set; }
        public string MsgBody { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public SysMessage(MsgType msgtype, string sender, string recver, int msgid, string msgbody, string username, string password)
        {
            MsgType = msgtype;
            Sender = sender;
            Recver = recver;
            MsgID = msgid;
            MsgBody = msgbody;
            UserName = username;
            Password = password;
        }
        public SysMessage()
        {

        }
        public SysMessage(string sender)
        {
            Sender = sender;
        }
        #region WMS WCS消息处理
        public void CreateWCSTaskDeletedMessage(WCSTask wCSTask)
        {
            MsgType = MsgType.WCSTaskMsg;
            WCSTaskMsg wCSTaskMsg = new WCSTaskMsg();
            wCSTaskMsg.TaskOptType = TaskOptType.Deleted;
            wCSTaskMsg.WCSTask = wCSTask;
            MsgBody = JsonConvert.SerializeObject(wCSTaskMsg);
        }
        public void CreateWCSTaskAddedMessage(WCSTask wCSTask)
        {
            MsgType = MsgType.WCSTaskMsg;
            WCSTaskMsg wCSTaskMsg = new WCSTaskMsg();
            wCSTaskMsg.TaskOptType = TaskOptType.Added;
            wCSTaskMsg.WCSTask = wCSTask;
            MsgBody = JsonConvert.SerializeObject(wCSTaskMsg);
        }
        public void CreateWCSTaskChangedMessage(WCSTask wCSTask)
        {
            MsgType = MsgType.WCSTaskMsg;
            WCSTaskMsg wCSTaskMsg = new WCSTaskMsg();
            wCSTaskMsg.TaskOptType = TaskOptType.Changed;
            wCSTaskMsg.WCSTask = wCSTask;
            MsgBody = JsonConvert.SerializeObject(wCSTaskMsg);
        }
        public void CreateWMSTaskAddedMessage(WMSTask wMSTask)
        {
            MsgType = MsgType.WMSTaskMsg;
            WMSTaskMsg wMSTaskMsg = new WMSTaskMsg();
            wMSTaskMsg.TaskOptType = TaskOptType.Added;
            wMSTaskMsg.WMSTask = wMSTask;
            MsgBody = JsonConvert.SerializeObject(wMSTaskMsg);
        }
        public void CreateWMSTaskDeletedMessage(WMSTask wMSTask)
        {
            MsgType = MsgType.WMSTaskMsg;
            WMSTaskMsg wMSTaskMsg = new WMSTaskMsg();
            wMSTaskMsg.TaskOptType = TaskOptType.Deleted;
            wMSTaskMsg.WMSTask = wMSTask;
            MsgBody = JsonConvert.SerializeObject(wMSTaskMsg);
        }
        public void CreateGoodsLocationUpdateMessage(GoodsLocation goodsLocation)
        {
            MsgType = MsgType.GoodLocationMsg;
            GoodsLocationMsg goodsLocationMsg = new GoodsLocationMsg();
            goodsLocationMsg.GoodLocation = goodsLocation;
            MsgBody = JsonConvert.SerializeObject(goodsLocationMsg);
        }
        #endregion
        public void CreateRGVDeviceBlockMessage(RGVDevice rGVDevice)
        {
            MsgType = MsgType.RGVDeviceMsg;
            RGVDeviceMsg rGVDeviceMsg = new RGVDeviceMsg();
            rGVDeviceMsg.DeviceID = rGVDevice.DeviceID;
            //if (!string.IsNullOrEmpty(rGVDevice.CDeviceID))
            //    rGVDeviceMsg.CDeviceID = rGVDevice.CDeviceID.Trim((char)0x00);
            //if (!string.IsNullOrEmpty(rGVDevice.RDeviceID))
            //    rGVDeviceMsg.RDeviceID = rGVDevice.RDeviceID.Trim((char)0x00);
            //if (!string.IsNullOrEmpty(rGVDevice.SDeviceID))
            //    rGVDeviceMsg.SDeviceID = rGVDevice.SDeviceID.Trim((char)0x00);
            rGVDeviceMsg.RGVStatus = rGVDevice.RGVStatus;
            rGVDeviceMsg.RGVPositionY = rGVDevice.RGVPostionY;
            if (string.IsNullOrEmpty(rGVDevice.PalletNum))
                rGVDeviceMsg.PalletNum = null;
            else rGVDeviceMsg.PalletNum = rGVDevice.PalletNum.Trim((char)0x00);
            //rGVDeviceMsg.FaultContent = rGVDevice.FaultCode;
            rGVDeviceMsg.FaultCode = rGVDevice.FaultCode;
            rGVDeviceMsg.IsOccupied = rGVDevice.IsOccupying;
            //rGVDeviceMsg.PLCID = rGVDevice.PLCID;
            //if (!string.IsNullOrEmpty(rGVDevice.PalletNum)&&rGVDevice.PalletNum.Contains("\0"))
            //    rGVDevice.PalletNum.ToLower();
            //if (rGVDevice.DeviceID == "1320")
            //    rGVDevice.DeviceID.ToLower();
            MsgBody = JsonConvert.SerializeObject(rGVDeviceMsg);
        }
        //public void CreateTSJDeviceBlockMessage(TSJDevice tSJDevice)
        //{
        //    MsgType = MsgType.LiftDeviceMsg;
        //    TSJDeviceMsg tSJDeviceMsg = new TSJDeviceMsg();
        //    tSJDeviceMsg.DeviceID = tSJDevice.DeviceID;
        //    tSJDeviceMsg.Status = tSJDevice.TSJStatus;
        //    tSJDeviceMsg.CurrentFloor = tSJDevice.TSJPostion;
        //    if (string.IsNullOrEmpty(tSJDevice.PalletNum))
        //        tSJDeviceMsg.PalletNum = null;
        //    else tSJDeviceMsg.PalletNum = tSJDevice.PalletNum.Trim((char)0x00);
        //    //rGVDeviceMsg.FaultContent = rGVDevice.FaultCode;
        //    tSJDeviceMsg.FaultCode = tSJDevice.FaultCode;
        //    tSJDeviceMsg.IsOccupied = tSJDevice.IsOccupying;
        //    //rGVDeviceMsg.PLCID = rGVDevice.PLCID;
        //    //if (!string.IsNullOrEmpty(rGVDevice.PalletNum)&&rGVDevice.PalletNum.Contains("\0"))
        //    //    rGVDevice.PalletNum.ToLower();
        //    MsgBody = JsonConvert.SerializeObject(tSJDeviceMsg);
        //}
        //public void CreateCDJDeviceBlockMessage(CDJDevice cDJDevice)
        //{
        //    MsgType = MsgType.DischargerAndLoadDevice;
        //    CDJDeviceMsg cDJDeviceMsg = new CDJDeviceMsg();
        //    cDJDeviceMsg.DeviceID = cDJDevice.DeviceID;
        //    cDJDeviceMsg.Status = cDJDevice.CDJStatus;
        //    if (string.IsNullOrEmpty(cDJDevice.PalletNum))
        //        cDJDeviceMsg.PalletNum = null;
        //    else cDJDeviceMsg.PalletNum = cDJDevice.PalletNum.Trim((char)0x00);
        //    //rGVDeviceMsg.FaultContent = rGVDevice.FaultCode;
        //    cDJDeviceMsg.FaultCode = cDJDevice.FaultCode;
        //    cDJDeviceMsg.IsOccupied = cDJDevice.IsOccupying;
        //    cDJDeviceMsg.DeviceType = cDJDevice.DeviceType;
        //    if ((int)cDJDeviceMsg.DeviceType == 2)  //叠盘机
        //        cDJDeviceMsg.PalletCount = cDJDevice.PalletCount;
        //    else
        //    {
        //        if (cDJDeviceMsg.DeviceID.Substring(0, 1) == "1")
        //            cDJDeviceMsg.PalletCount = 6;
        //        else
        //            cDJDeviceMsg.PalletCount = 8;
        //    }
        //    MsgBody = JsonConvert.SerializeObject(cDJDeviceMsg);
        //}
        //public void CreateXZTDeviceBlockMessage(XZTDevice xZTDevice)
        //{
        //    MsgType = MsgType.TurnTableMsg;
        //    XZTDeviceMsg xZTDeviceMsg = new XZTDeviceMsg();
        //    xZTDeviceMsg.DeviceID = xZTDevice.DeviceID;
        //    xZTDeviceMsg.Status = xZTDevice.XZTStatus;
        //    xZTDeviceMsg.CurrentDirection = xZTDevice.XZTDirection;
        //    if (string.IsNullOrEmpty(xZTDevice.PalletNum))
        //        xZTDeviceMsg.PalletNum = null;
        //    else xZTDeviceMsg.PalletNum = xZTDevice.PalletNum.Trim((char)0x00);
        //    //rGVDeviceMsg.FaultContent = rGVDevice.FaultCode;
        //    xZTDeviceMsg.FaultCode = xZTDevice.FaultCode;
        //    xZTDeviceMsg.IsOccupied = xZTDevice.IsOccupying;
        //    //rGVDeviceMsg.PLCID = rGVDevice.PLCID;
        //    //if (!string.IsNullOrEmpty(rGVDevice.PalletNum)&&rGVDevice.PalletNum.Contains("\0"))
        //    //    rGVDevice.PalletNum.ToLower();
        //    MsgBody = JsonConvert.SerializeObject(xZTDeviceMsg);
        //}

        public void CreateSSJDeviceBlockMessage(SSJDeviceBlock sSJDeviceBlock)
        {
            MsgType = MsgType.SSJBlockMsg;
            SSJBlockMsg sSJBlockMsg = new SSJBlockMsg();
            sSJBlockMsg.Position = sSJDeviceBlock.Position;
            sSJBlockMsg.PLCID = sSJDeviceBlock.PLCID;
            sSJBlockMsg.DeviceType = sSJDeviceBlock.DeviceType;
            sSJBlockMsg.SystemType = sSJDeviceBlock.SystemType;

            sSJBlockMsg.FaultContent = sSJDeviceBlock.FaultContent1;
            sSJBlockMsg.ErrorCode = sSJDeviceBlock.ErrorCode;
            sSJBlockMsg.IsOccupied = sSJDeviceBlock.IsOccupied;
            if (string.IsNullOrEmpty(sSJDeviceBlock.PalletNum))
                sSJBlockMsg.PalletNum = null;
            else
                //sSJBlockMsg.PalletNum = sSJDeviceBlock.PalletNum.Replace("\u0000", string.Empty);
                sSJBlockMsg.PalletNum = sSJDeviceBlock.PalletNum;
            //sSJBlockMsg.MotionDirection = sSJDeviceBlock.MotionDirection;
            //sSJBlockMsg.CurrMotionDirection = sSJDeviceBlock.CurrMotionDirection;
            sSJBlockMsg.IsFaulty = sSJDeviceBlock.IsFaulty;
            sSJBlockMsg.AllowUnload = sSJDeviceBlock.AllowUnloading;
            sSJBlockMsg.TPHorizon = sSJDeviceBlock.TPHorizon;
            //sSJBlockMsg.BlockStatus = (int)sSJDeviceBlock.BlockStatus;
            sSJBlockMsg.FmLocation = sSJDeviceBlock.FmLocation;
            sSJBlockMsg.ToLocation = sSJDeviceBlock.ToLocation;
            MsgBody = JsonConvert.SerializeObject(sSJBlockMsg);
        }

        public void CreateWMSTaskChangedMessage(WMSTask wMSTask)
        {
            MsgType = MsgType.WMSTaskMsg;
            WMSTaskMsg wMSTaskMsg = new WMSTaskMsg();
            wMSTaskMsg.TaskOptType = TaskOptType.Changed;
            wMSTaskMsg.WMSTask = wMSTask;
            MsgBody = JsonConvert.SerializeObject(wMSTaskMsg);
        }

        //public void CreateDDJTaskAddedMessage(DDJTask dDJTask)
        //{
        //    MsgType = MsgType.DDJTaskMsg;
        //    DDJTaskMsg dDJTaskMsg = new DDJTaskMsg();
        //    dDJTaskMsg.TaskOptType = TaskOptType.Added;
        //    dDJTaskMsg.DDJTask = dDJTask;
        //    MsgBody = JsonConvert.SerializeObject(dDJTaskMsg);
        //}

        //public void CreateDDJTaskChangedMessage(DDJTask dDJTask)
        //{
        //    MsgType = MsgType.DDJTaskMsg;
        //    DDJTaskMsg dDJTaskMsg = new DDJTaskMsg();
        //    dDJTaskMsg.TaskOptType = TaskOptType.Changed;
        //    dDJTaskMsg.DDJTask = dDJTask;
        //    MsgBody = JsonConvert.SerializeObject(dDJTaskMsg);
        //}

        //public void CreateDDJTaskDeletedMessage(DDJTask dDJTask)
        //{
        //    MsgType = MsgType.DDJTaskMsg;
        //    DDJTaskMsg dDJTaskMsg = new DDJTaskMsg();
        //    dDJTaskMsg.TaskOptType = TaskOptType.Deleted;
        //    dDJTaskMsg.DDJTask = dDJTask;
        //    MsgBody = JsonConvert.SerializeObject(dDJTaskMsg);
        //}

        public void CreateSSJDeviceMessage(SSJDevice sSJDevice)
        {
            MsgType = MsgType.SSJStatusMsg;
            SSJStatusMsg deviceStatusMsg = new SSJStatusMsg();
            deviceStatusMsg.DeviceID = sSJDevice.PLCID;
            deviceStatusMsg.Avalible = true;
            deviceStatusMsg.WorkState = (int)sSJDevice.SSJWorkState;
            deviceStatusMsg.ConnectState = (int)sSJDevice.PLCConnectState;
            deviceStatusMsg.WCSTOPLCDB4 = sSJDevice.WCSTOPLCDB4;
            deviceStatusMsg.PLCTOWCSDB5 = sSJDevice.PLCTOWCSDB5;
            deviceStatusMsg.FaultCode = sSJDevice.FaultCode;
            deviceStatusMsg.FaultContent = sSJDevice.FaultContent;
            deviceStatusMsg.CircleMode = sSJDevice.CircleMode;
            deviceStatusMsg.InCircleNum = sSJDevice.InCircleNum;
            deviceStatusMsg.InCircleNum2 = sSJDevice.InCircleNum2;
            deviceStatusMsg.InCircleNum3 = sSJDevice.InCircleNum3;
            MsgBody = JsonConvert.SerializeObject(deviceStatusMsg);
        }

        public void CreateDDJDeviceMessage(DDJDevice dDJDevice)
        {
            MsgType = MsgType.DDJStatusMsg;
            DDJStatusMsg deviceStatusMsg = new DDJStatusMsg();
            deviceStatusMsg.DeviceID = dDJDevice.PLCID;
            deviceStatusMsg.Available = dDJDevice.Available;
            deviceStatusMsg.WorkState = (int)dDJDevice.DDJWorkState;
            deviceStatusMsg.ConnectState = (int)dDJDevice.PLCConnectState;
            deviceStatusMsg.LiftingPosition = dDJDevice.LiftingPosition;
            deviceStatusMsg.MotionPosition = dDJDevice.MotionPosition;
            deviceStatusMsg.PalletNum = dDJDevice.pallet_num;
            deviceStatusMsg.ForkPosition = dDJDevice.ForkPosition;
            deviceStatusMsg.FaultCode = dDJDevice.FaultCode;
            deviceStatusMsg.FaultContent = dDJDevice.FaultContent;
            deviceStatusMsg.HasPallet = dDJDevice.HasPallet;
            deviceStatusMsg.FmLocation = dDJDevice.FmLocation;
            deviceStatusMsg.ToLocation = dDJDevice.ToLocation;
            MsgBody = JsonConvert.SerializeObject(deviceStatusMsg);
        }
        public void CreateDeMaticDDJDeviceMessage(DeMaticDDJ deMaticDDJ)
        {
            MsgType = MsgType.DDJStatusMsg;
            DDJStatusMsg deviceStatusMsg = new DDJStatusMsg();
            deviceStatusMsg.DeviceID = deMaticDDJ.PLCID;
            deviceStatusMsg.Available = deMaticDDJ.Available;
            deviceStatusMsg.WorkState = (int)deMaticDDJ.DDJWorkState;
            deviceStatusMsg.ConnectState = (int)deMaticDDJ.PLCConnectState;
            deviceStatusMsg.LiftingPosition = deMaticDDJ.LiftingPosition;
            deviceStatusMsg.MotionPosition = deMaticDDJ.MotionPosition;
            deviceStatusMsg.PalletNum = deMaticDDJ.pallet_num;
            deviceStatusMsg.ForkPosition = deMaticDDJ.ForkPosition;
            deviceStatusMsg.FaultCode = deMaticDDJ.FaultCode;
            deviceStatusMsg.FaultContent = deMaticDDJ.FaultContent;
            deviceStatusMsg.HasPallet = deMaticDDJ.HasPallet;
            deviceStatusMsg.FmLocation = deMaticDDJ.FmLocation;
            deviceStatusMsg.ToLocation = deMaticDDJ.ToLocation;
            deviceStatusMsg.WCSTODeMatic = deMaticDDJ.WCSTODeMatic;
            deviceStatusMsg.DeMaticTOWCS = deMaticDDJ.DeMaticTOWCS;
            deviceStatusMsg.WCSEnable= deMaticDDJ.WCSEnable;
            MsgBody = JsonConvert.SerializeObject(deviceStatusMsg);
        }
        //public void CreateFZJDeviceMessage(FZJDevice fZJDevice)
        //{
        //    MsgType = MsgType.TurningDeviceMsg;
        //    FZJDeviceMsg deviceStatusMsg = new FZJDeviceMsg();
        //    deviceStatusMsg.DeviceID = fZJDevice.PLCID;
        //    deviceStatusMsg.FaultCode = fZJDevice.FaultCode;
        //    deviceStatusMsg.EmergencyStop = fZJDevice.EmergencyStop;
        //    deviceStatusMsg.ManuAuto = fZJDevice.ManuAuto;
        //    deviceStatusMsg.Vertival = fZJDevice.Vertival;
        //    deviceStatusMsg.Horizontal = fZJDevice.Horizontal;
        //    deviceStatusMsg.PutMateriStretch = fZJDevice.PutMateriStretch;
        //    deviceStatusMsg.PutMateriShrink = fZJDevice.PutMateriShrink;
        //    deviceStatusMsg.PalletForward = fZJDevice.PalletForward;
        //    deviceStatusMsg.PalletBack = fZJDevice.PalletBack;
        //    deviceStatusMsg.CylinderHold = fZJDevice.CylinderHold;
        //    deviceStatusMsg.CylinderRelease = fZJDevice.CylinderRelease;
        //    MsgBody = JsonConvert.SerializeObject(deviceStatusMsg);
        //}
    }
    public enum MsgType
    {
        Hello,
        HeartBeatAsk,
        HeartBeatAns,
        LCDMessage,
        WCSTaskMsg,
        WMSTaskMsg,
        DDJStatusMsg,
        SSJOptMsg,
        SSJStatusMsg,
        DDJOptMsg,
        UserMsg,
        DDJTaskMsg,
        SSJBlockMsg,
        RGVDeviceMsg,
        GoodLocationMsg
    }
    public struct WMSTaskMsg
    {
        public TaskOptType TaskOptType { get; set; }
        public WMSTask WMSTask { get; set; }
    }
    public struct WCSTaskMsg
    {
        public TaskOptType TaskOptType { get; set; }
        public WCSTask WCSTask { get; set; }
    }
    //public struct DDJTaskMsg
    //{
    //    public TaskOptType TaskOptType { get; set; }
    //    public DDJTask DDJTask { get; set; }
    //}
    public struct GoodsLocationMsg
    {
        public GoodsLocation GoodLocation { get; set; }
        //public string Position { set; get; }
        //public string WMSPosition { set; get; }
        //public string Tunnel { set; get; }
        //public string PalletNum { set; get; }
        //public int TaskType { set; get; }
        //public bool Available { set; get; }
        //public string DDJID { set; get; }
        //public string RowStr { set; get; }
        //public string RankStr { set; get; }
        //public string LayerStr {  set; get; }
        //public string Row {  set; get; }
        //public string Rank {  set; get; }
        //public string Layer { set; get; }
        //public DateTime UpdatePalletNumTime {  set; get; }

    }
    public struct SSJBlockMsg
    {
        public string Position { get; set; }
        public string PLCID { get; set; }
        public EqpType DeviceType { get; set; }
        public DeviceSystemType SystemType { get; set; }
        public bool IsOccupied { get; set; }
        public string PalletNum { get; set; }
        public string ErrorCode { get; set; }
        public string FaultContent { get; set; }
        public string FmLocation { get; set; }
        public string ToLocation { get; set; }
        public bool IsFaulty { get; set; }
        public bool AllowUnload { get; set; }
        public bool TPHorizon {  get; set; }
        //public int BlockStatus { get; set; }
        //public DeviceBlockMotionDirection MotionDirection { get; set; }
        //public DeviceBlockMotionDirection CurrMotionDirection { get; set; }
    }
    public struct RGVDeviceMsg
    {
        //public string PLCID { get; set; }
        public string DeviceID { get; set; }
        public bool IsOccupied { get; set; }
        public string PalletNum { get; set; }
        //public string FaultContent { get; set; }
        public int RGVPositionY { get; set; }
        public RGVDeviceStatus RGVStatus { get; set; }
        public string FaultCode { get; set; }
        //public string RDeviceID { get; set; }
        //public string SDeviceID { get; set; }
        //public string CDeviceID { get; set; }
    }
    public struct TSJDeviceMsg
    {
        //public string PLCID { get; set; }
        public string DeviceID { get; set; }
        public bool IsOccupied { get; set; }
        public string PalletNum { get; set; }
        //public string FaultContent { get; set; }
        public int CurrentFloor { get; set; }
        public TSJDeviceStatus Status { get; set; }
        public string FaultCode { get; set; }
    }
    public struct CDJDeviceMsg
    {
        //public string PLCID { get; set; }
        public string DeviceID { get; set; }
        public bool IsOccupied { get; set; }
        public string PalletNum { get; set; }
        //public string FaultContent { get; set; }
        public CDJEquType DeviceType { get; set; }
        public CDJDeviceStatus Status { get; set; }
        public string FaultCode { get; set; }
        public int PalletCount { get; set; }
    }
    public struct XZTDeviceMsg
    {
        //public string PLCID { get; set; }
        public string DeviceID { get; set; }
        public bool IsOccupied { get; set; }
        public string PalletNum { get; set; }
        //public string FaultContent { get; set; }
        public int CurrentDirection { get; set; }
        public XZTDeviceStatus Status { get; set; }
        public string FaultCode { get; set; }
    }
    public struct FZJDeviceMsg
    {
        //public string PLCID { get; set; }
        public string DeviceID { get; set; }
        public string FaultCode { get; set; }
        public string EmergencyStop { get; set; }
        public string ManuAuto { get; set; }
        public string Vertival { get; set; }
        public string Horizontal { get; set; }
        public string PutMateriStretch { get; set; }
        public string PutMateriShrink { get; set; }
        public string PalletForward { get; set; }
        public string PalletBack { get; set; }
        public string CylinderHold { get; set; }
        public string CylinderRelease { get; set; }
    }
    public struct DDJStatusMsg
    {
        public string DeviceID { get; set; }
        public bool Available { get; set; }
        public int WorkState { get; set; }
        public int ConnectState { get; set; }
        public int LiftingPosition { get; set; }
        public int MotionPosition { get; set; }
        public int ForkPosition { get; set; }
        public string PalletNum { get; set; }
        public string FaultCode { get; set; }
        public string FaultContent { get; set; }
        public bool HasPallet { get; set; }
        public string FmLocation { get; set; }
        public string ToLocation { get; set; }
        public string DeMaticTOWCS {  get; set; }
        public string WCSTODeMatic {  get; set; }
        public bool WCSEnable {  get; set; }
    }
    public struct SSJStatusMsg
    {
        public string DeviceID { get; set; }
        public bool Avalible { get; set; }
        public int WorkState { get; set; }
        public int ConnectState { get; set; }
        public string WCSTOPLCDB4 {  get; set; }
        public string PLCTOWCSDB5 {  get; set; }
        public string FaultCode { get; set; }
        public string FaultContent { get; set; }
        public int InCircleNum {  get; set; }
        public int InCircleNum2 {  get; set; }
        public int InCircleNum3 {  get; set; }
        public int CircleMode {  get; set; }
    }
    public struct DDJOptMsg
    {
        public string DeviceID { get; set; }
        public DDJOptType OptType { get; set; }
        public object Var { get; set; }
    }
    public struct SSJOptMsg
    {
        public string DeviceID { get; set; }
        public SSJOptType OptType { get; set; }
        public object Var { get; set; }
    }
    public enum SSJOptType
    {
        Online,
        Offline,
        Disconnect,
        Connect,
        ClearOpty,
        Disable,
    }
    public enum DDJOptType
    {
        Online,
        Offline,
        Disconnect,
        Connect,
        RecallIn,
        Disable,
        DDJEmptyPick,
        DDJDoubleUp,
        DDJEmptyOutBound,
        DDJUnreach,
        SPalletStack,
        ReSendMsg,
        WCSEnableReset
    }
    public enum TaskOptType
    {
        Added,
        Deleted,
        Changed
    }
}