using System.ComponentModel;

namespace BMHRI.WCS.Server.Models
{
    public class SSJDeviceBlock : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public int PalletNumDBAddr { get; set; }
        public int IsOccupyingMBAddr { get; set; }
        public int IsOccupyingMBAddr2 { get; set; }
        public int AllowUnloadingMBAddr { get; set; }
        public int AllowUnloadingMBAddr2 { get; set; }
        public int TPHorizonMBAddr {  get; set; }
        public int TPHorizonMBAddr2 { get; set; }
        public int InCircleMBAddr { get; set; }
        public string WareHouse { get; set; }
        public int InCircleMBAddr2 { get; set; }
        public int FarCircleMBAddr { get; set; }
        public int FarCircleMBAddr2 { get; set; }
        public int InAndOutMBAddr { get; set; }
        public int InAndOutMBAddr2 { get; set; }
        public int LightMBAddr { get; set; }
        public int LightMBAddr2 { get; set; }
        public int StatusDBAddr { get; set; }
        public string? Tunnel { get; set; }
        public string DDJID { get; set; }
        public string PortNum { get; set; }
        public int Floor { get; set; }
        public string AGVToPosition { get; set; }
        public bool AGVGetPoint { get; set; }
        public bool AGVSetPoint { get; set; }
        public int InboundTrigerAddr { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public int MotionQAddr { get; set; }
        public int QAddrBit1 { get; set; }
        public int QAddrBit2 { get; set; }
        public bool InCircle { get; set; }
        public bool FarCircle { get; set; }
        public bool ClearCircle { get; set; }
        public int Floor2Circle {  get; set; }
        
        public int AGVPutMBAddr { get; set; }
        public int AGVPutMBAddr2 { get; set; }

        public int OccupancyNum { get; set; }
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DeviceBlockMotionDirection motion_direction;
        public DeviceBlockMotionDirection MotionDirection
        {
            get
            {
                return motion_direction;
            }
            set
            {
                motion_direction = value;
                Notify(nameof(MotionDirection));
            }
        }
        private DeviceBlockMotionDirection curr_motion_direction;
        public DeviceBlockMotionDirection CurrMotionDirection
        {
            get
            {
                return curr_motion_direction;
            }
            set
            {
                curr_motion_direction = value;
                Notify(nameof(CurrMotionDirection));
            }
        }

        private EqpType device_type;
        public EqpType DeviceType
        {
            get
            {
                return device_type;
            }
            set
            {
                device_type = value;
                Notify(nameof(DeviceType));
            }
        }
        private AreaType area_type;
        public AreaType AreaType
        {
            get { return area_type; }
            set
            {
                area_type = value;
                Notify(nameof(DeviceType));
            }
        }

        private DeviceSystemType system_type;
        public DeviceSystemType SystemType
        {
            get
            {
                return system_type;
                //string st;
                //switch (system_type)
                //{
                //    case DeviceSystemType.OutboundFinish:
                //        st= "出库口";
                //        break;
                //    case DeviceSystemType.InboundBegin:
                //        st = "入库口";
                //        break;
                //    case DeviceSystemType.InboundFinish:
                //        st = "堆垛机取货点";
                //        break;
                //    case DeviceSystemType.OutboundBegin:
                //        st = "堆垛机卸货点";
                //        break;
                //    case DeviceSystemType.Picking:
                //        st = "拣选点";
                //        break;
                //    default:
                //        st = "其他";
                //        break;
                //}
                //return st;
            }
            set
            {
                system_type = value;
                Notify(nameof(SystemType));
            }
        }
        private int margin;
        public int Margin
        {
            get
            {
                return margin;
            }
            set
            {
                margin = value;
                Notify(nameof(Margin));
            }
        }
        private string load_type;
        public string LoadType
        {
            get
            {
                return load_type;
            }
            set
            {
                load_type = value;
                Notify(nameof(LoadType));
            }
        }
        //BIT1：超高 ；BIT2：左超宽 ；BIT3：右超宽 ；BIT4：前超长 ；BIT5：后超长；BIT6：数据异常；BIT7：读码异常；BIT8：重量异常
        private bool is_super_elevation;
        public bool IsSuperElevation
        {
            get { return is_super_elevation; }
            set
            {
                is_super_elevation = value;
                Notify(nameof(IsSuperElevation));
            }
        }
        private bool is_left_super_wide;
        public bool IsLeftSuperWide
        {
            get { return is_left_super_wide; }
            set
            {
                is_left_super_wide = value;
                Notify(nameof(IsLeftSuperWide));
            }
        }
        private bool is_right_super_wide;
        public bool IsRightSuperWide
        {
            get { return is_right_super_wide; }
            set
            {
                is_right_super_wide = value;
                Notify(nameof(IsRightSuperWide));
            }
        }
        private bool is_front_over_length;
        public bool IsFrontOverLength
        {
            get { return is_front_over_length; }
            set
            {
                is_front_over_length = value;
                Notify(nameof(IsFrontOverLength));
            }
        }
        private bool is_rear_over_length;
        public bool IsRearOverLength
        {
            get { return is_rear_over_length; }
            set
            {
                is_rear_over_length = value;
                Notify(nameof(IsRearOverLength));
            }
        }
        private bool is_abnormal_data;
        public bool IsAbnormalData
        {
            get { return is_abnormal_data; }
            set
            {
                is_abnormal_data = value;
                Notify(nameof(IsAbnormalData));
            }
        }
        private bool is_abnormal_code;
        public bool IsAbnormalCode
        {
            get { return is_abnormal_code; }
            set
            {
                is_abnormal_data = value;
                Notify(nameof(IsAbnormalCode));
            }
        }
        private bool is_abnormal_code_reading;
        public bool IsAbnormalCodeReading
        {
            get { return is_abnormal_code_reading; }
            set
            {
                is_abnormal_code_reading = value;
                Notify(nameof(IsAbnormalCodeReading));
            }
        }
        private bool is_abnormal_weight;
        public bool IsAbnormalWeight
        {
            get { return is_abnormal_weight; }
            set
            {
                is_abnormal_weight = value;
                Notify(nameof(IsAbnormalWeight));
            }
        }

        private string error_code;
        public string ErrorCode
        {
            get
            {
                return error_code;
            }
            set
            {
                error_code = value;
                Notify(nameof(ErrorCode));
            }
        }

        private string faultcontent1;
        public string FaultContent1
        {
            get
            {
                return faultcontent1;
            }
            set
            {
                faultcontent1 = value;
                Notify(nameof(FaultContent1));
            }
        }
        private string faultcontent2;
        public string FaultContent2
        {
            get
            {
                if (string.IsNullOrEmpty(faultcontent2))
                    faultcontent2 = "工作正常";
                return faultcontent2;
            }
            set
            {
                faultcontent2 = value;
                Notify(nameof(FaultContent2));
            }
        }

        private string devicedescription;
        public string DeviceDescription
        {
            get
            {

                return devicedescription;
            }
            set
            {
                if (devicedescription != value)
                {
                    devicedescription = value;
                    Notify(nameof(DeviceDescription));
                }
            }
        }

        private string description;
        public string Description
        {
            get
            {

                return description;
            }
            set
            {
                description = value;
                Notify(nameof(Description));
            }
        }

        private string com_position;
        public string ComPosition
        {
            get
            {
                if (string.IsNullOrEmpty(com_position))
                    GetBlockName();
                return com_position;
            }
            set
            {
                com_position = value;
                Notify("ComPosition");
            }
        }

        private void GetBlockName()
        {
            if (string.IsNullOrEmpty(Position)) return;
            string[] name_strs = Position.Split('-');
            if (name_strs.Length != 4) return;
            ComPosition = Position + "-";
            switch (name_strs[1])
            {
                case "1F":
                    ComPosition += "1楼";
                    break;
                case "2F":
                    ComPosition += "2楼";
                    break;
                case "3F":
                    ComPosition += "3楼";
                    break;
                case "4F":
                    ComPosition += "4楼";
                    break;
                case "FL":
                    ComPosition += "夹层";
                    break;
                default:
                    break;
            }
            switch (name_strs[2])
            {
                case "RK":
                    ComPosition += "入库口";
                    break;
                case "CK":
                    ComPosition += "出库口";
                    break;
                case "CD":
                    ComPosition += "拆垛位";
                    break;
                case "JX":
                    ComPosition += "拣选位";
                    break;
                case "MD":
                    ComPosition += "码垛位";
                    break;
                case "SJ":
                    ComPosition += "空托盘收集点";
                    break;
                case "BL":
                    ComPosition += "不合格整理口";
                    break;
                case "SQ":
                    ComPosition += "空托盘申请点";
                    break;
                default:
                    break;
            }
            ComPosition += name_strs[3];

        }

        private string tolocation;
        public string ToLocation
        {
            get
            {
                return tolocation;
            }
            set
            {
                if (tolocation != value)
                {
                    tolocation = value;
                    Notify(nameof(ToLocation));
                }
            }
        }

        private string fmlocation;
        public string FmLocation
        {
            get
            {
                return fmlocation;
            }
            set
            {
                if (fmlocation != value)
                {
                    fmlocation = value;
                    Notify(nameof(FmLocation));
                }
            }
        }

        private string plcid;
        public string PLCID
        {
            get
            {
                return plcid;
            }
            set
            {
                plcid = value;
                Notify("PLCID");
            }
        }

        private bool available;
        public bool Available
        {
            get
            {
                return available;
            }
            set
            {
                if (available != value)
                {
                    available = value;
                    Notify(nameof(Available));
                }
            }
        }

        private string position;
        public string Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                Notify(nameof(Position));
            }
        }

        private string palletnum;
        public string PalletNum
        {
            get
            {
                return palletnum;
            }
            set
            {
                if (palletnum != value)
                {
                    palletnum = value;
                    Notify(nameof(PalletNum));
                    UpdateDescription();
                }
            }
        }
        private string position_infor;
        public string PositionInfor
        {
            get
            {
                return position_infor;
            }
            set
            {
                if (position_infor != value)
                {
                    position_infor = value;
                    Notify(nameof(PositionInfor));
                }
            }
        }
        private double pallet_weight;
        public double LoadWeight
        {
            get
            {
                return pallet_weight;
            }
            set
            {
                if (pallet_weight != value)
                {
                    pallet_weight = value;
                    Notify(nameof(LoadWeight));
                }
            }
        }

        private bool is_faulty;
        public bool IsFaulty
        {
            get
            {
                return is_faulty;
            }
            set
            {
                if (is_faulty != value)
                {
                    is_faulty = value;
                    Notify(nameof(IsFaulty));
                    UpdateDescription();
                }
            }
        }
        private bool is_occupied;
        public bool IsOccupied
        {
            get
            {
                return is_occupied;
            }
            set
            {
                if (is_occupied != value)
                {
                    is_occupied = value;
                    Notify(nameof(IsOccupied));
                    Notify(nameof(Allow_AGV_Put));
                    UpdateDescription();
                }
            }
        }

        private bool is_loaded;
        public bool IsLoaded
        {
            get
            {
                return is_loaded;
            }
            set
            {
                if (is_loaded != value)
                {
                    is_loaded = value;
                    Notify(nameof(IsLoaded));
                }
            }
        }
        private bool allow_unloading;
        public bool AllowUnloading
        {
            get
            {
                return allow_unloading;
            }
            set
            {
                if (allow_unloading != value)
                {
                    allow_unloading = value;
                    Notify("AllowUnloading");
                    UpdateDescription();
                }
            }
        }
        private bool inbound_Light;
        public bool Inbound_Light
        {
            get
            {
                return inbound_Light;
            }
            set
            {
                if (inbound_Light != value)
                {
                    inbound_Light = value;
                    Notify(nameof(Allow_AGV_Put));
                    Notify(nameof(Inbound_Light));
                    //UpdateDescription();
                }
            }
        }
        


        private bool allow_agv_put;
        public bool Allow_AGV_Put
        {
            get
            {
                return !(CurrentMode == DeviceModeType.OutboundMode || CurrentMode == DeviceModeType.None || IsOccupied || Inbound_Light); 
            }
            set
            {
                
            }
        }
        



        private bool tp_horizon;
        public bool TPHorizon
        {
            get
            {
                return tp_horizon;
            }
            set
            {
                if (tp_horizon != value)
                {
                    tp_horizon = value;
                    Notify("TPHorizon");
                }
            }
        }
        private DeviceModeType current_mode = (DeviceModeType)(2);
        public DeviceModeType CurrentMode
        {
            get
            {
                return current_mode;
            }
            set
            {
                if (current_mode != value)
                {
                    current_mode = value;
                    Notify(nameof(CurrentMode));
                    Notify(nameof(Allow_AGV_Put));
                    //  UpdateWMSPortMode();
                }
            }
        }
        private SSJDeviceBlockStatus block_status;
        public SSJDeviceBlockStatus BlockStatus
        {
            get
            {
                return block_status;
            }
            set
            {
                if (block_status != value)
                {
                    block_status = value;
                    Notify(nameof(BlockStatus));
                }
            }
        }

        private bool canNotWrite;

        public bool CanNotWrite
        {
            get
            {
                return canNotWrite;
            }
            set
            {
                if (canNotWrite != value)
                {
                    canNotWrite = value;
                    Notify(nameof(CanNotWrite));
                }
            }
        }

        private void UpdateDescription()
        {
            DeviceDescription = "设备编号:" + Position + "," + "设备类型:" + DeviceType + ",";
            if (IsOccupied || IsLoaded)
            {
                devicedescription += "托盘号:" + PalletNum + "," + "起始地址:" + FmLocation + "," + "目的地址:" + ToLocation + ",";
            }
            if (IsFaulty)
            {
                devicedescription += "故障代码:" + ErrorCode + "," + "故障内容:" + FaultContent1 + ",";
            }
            if (!AllowUnloading)
            {
                devicedescription += "不允许卸货";
            }
        }

        public void UpdateWMSPortMode()
        {
            WMSTask wMSTask;
            if(position=="1310" || position == "1410" || position == "1510" || position == "1320" || position == "1420" || position == "1520" || position == "1330" || position == "1430" || position == "1530")
            {
                if (CurrentMode == DeviceModeType.InboundMode)
            {
                wMSTask = new WMSTask();
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.Warehouse = WareHouse;
                wMSTask.PalletNum = position;
                wMSTask.ToLocation = "A";//    A表示输送机入库模式
                wMSTask.TaskSource = "WMS";
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
            else if (CurrentMode == DeviceModeType.OutboundMode)
            {
                wMSTask = new WMSTask();
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.Warehouse = WareHouse;
                wMSTask.PalletNum = position;
                wMSTask.ToLocation = "B";//    B表示输送机出库模式
                wMSTask.TaskSource = "WMS";
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
            }
            
        }
    }
    public enum DeviceBlockMotionDirection
    {
        None = 0,
        Left = 1,
        Up = 2,
        Right = 3,
        Down = 4,
    }
    public enum DeviceSystemType
    {
        [Description("未知")]
        None,
        [Description("入库申请口")]
        InboundBegin,
        [Description("入库完成口")]
        InboundFinish,
        [Description("出库开始口")]
        OutboundBegin,
        [Description("出库完成口")]
        OutboundFinish,
        [Description("拣选口")]
        Picking,
        Unpacking,
        Packing,
        [Description("入库完成或出库开始口")]
        InboundFinishOrOutboundBegin,
        [Description("输送机二次申请口")]
        InboundSecondBegin,
        [Description("出入库口")]
        TotalPort
        //AGVGet, 存在部分点位既是入库口也是AGV点位，故不能增加
        //AGVSet,
        //AGVGetSet
    }
    public enum DeviceModeType
    {
        OutboundMode,
        InboundMode,
        None,
    }
    //0：无数据； 1：自动中 ；2：手动中；3：停止中 ；4：异常中 ；5：维护中；
    public enum SSJDeviceBlockStatus
    {
        None,
        Auto,
        Manul,
        Stop,
        Fault,
        Maintenance,
    }
    public enum EqpType
    {
        [Description("未知")]
        None,
        [Description("链条机")]
        ChainMachine,
        [Description("辊子机")]
        RollerMachine,
        [Description("提升机")]
        LiftDevice,
        [Description("翻转机")]
        TurningDevice,
        [Description("拆盘机")]
        DischargerDevice,
        [Description("叠盘机")]
        LoadDevice,
        [Description("旋转台")]
        TurnTable
    }
    public enum AreaType
    {
        None,
        RollProcess,//滚加工
        NetStove,//网带炉
        ShotBlasting,//抛丸区
        ColdProcess  //冷制区
    }
}
