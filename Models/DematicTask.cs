using BMHRI.WCS.Server.DDJProtocol;
using System;

namespace BMHRI.WCS.Server.Models
{
    public class DematicTask
    {
        private WCSTask wcs_task;
        public WCSTask WCSTask
        {
            get
            {
                if (wcs_task == null)
                    wcs_task = WCSTaskManager.Instance.WCSTaskList.Find(x => x.WCSSeqID == WCSSeqID);
                return wcs_task;
            }
            set
            {
                wcs_task = value;
            }
        }
        public const string DoubleSame = "S2";
        public const string DoubleDiff = "S3";
        public const string SingleFork = "S1";
        public const string SinglePalt = "00";

        public const string DDJ_Stack = "0a";
        public const string DDJ_Unstack = "0b";
        public const string DDJ_InOut = "0e";
        public const string DDJ_Move = "0m";
        private const string Zero_Str = "0000000000";  //10位托盘号
        private const string Ffff_Str = "FFFFFFFF";
        private const string Zero_four = "0000";

        public string GroupType { get; set; }
        public string TaskType { get; set; }

        public string MsgHeader { get; set; }
        public string MsgType { get; set; }
        public string MsgSender { get; set; }
        public string MsgReceiver { get; set; }
        public string MsgSeqID { get; set; }
        public string DeviceID { get; set; }
        public string MsgEnd { get; set; }
        public string PalletNum { get; set; }
        public string FmLocation { get; set; }
        public string ToLocation { get; set; }

        public string TaskSeqID { get; set; }
        public string CTdate { get; set; }
        public string WCSSeqID { get; set; }

        public DematicTaskStatus TaskStatus { get; set; }
        public DematicTask()      //需要根据实际德玛堆垛机协议修改！！！！
        {
            MsgHeader = "";
            MsgType = "";
            MsgSender = "";
            MsgReceiver = "";
            MsgSeqID = "";
            FmLocation = "";
            ToLocation = "";
            PalletNum = "";
            MsgEnd = "";
            TaskSeqID = Guid.NewGuid().ToString();
            CTdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffff");
            TaskStatus = DematicTaskStatus.Waiting;
        }
        
        public static DematicTask CreateSinglePalletTask(WCSTask wCSTask)
        {
            DematicTask dDJTask = new DematicTask(wCSTask);
            return dDJTask;
        }
        
        private DematicTask(WCSTask wCSTask) : this()
        {
            if (wCSTask == null) return;
            WCSTask = wCSTask;
            DeviceID = wCSTask.DeviceID;
            GroupType = SinglePalt;
            PalletNum = wCSTask.PalletNum;
            WCSSeqID = wCSTask.WCSSeqID;
            //SetDDJTaskType(WCSTask1);
            TaskStatus = DematicTaskStatus.Waiting;
            switch (wCSTask.TaskType)
            {
                case WCSTaskTypes.DDJStack:
                    TaskType = DDJ_Stack;
                    ToLocation = wCSTask.ToLocation;
                    FmLocation = wCSTask.FmLocation;
                    break;
                case WCSTaskTypes.DDJUnstack:
                    TaskType = DDJ_Unstack;
                    FmLocation = wCSTask.FmLocation;
                    ToLocation = wCSTask.ToLocation;
                    break;
                case WCSTaskTypes.DDJStackMove:
                    TaskType = DDJ_Move;
                    FmLocation = wCSTask.FmLocation;
                    ToLocation = wCSTask.ToLocation;
                    break;
                case WCSTaskTypes.DDJDirect:
                    TaskType = DDJ_InOut;
                    PalletNum = wCSTask.PalletNum;
                    ToLocation = wCSTask.ToLocation;
                    FmLocation = wCSTask.FmLocation;
                    break;
                default:
                    TaskType = null;
                    break;
            }
        }

        private void SetDDJTaskType(WCSTask wCSTask)
        {
            switch (wCSTask.TaskType)
            {
                case WCSTaskTypes.DDJStack:
                    TaskType = DDJ_Stack;
                    break;
                case WCSTaskTypes.DDJUnstack:
                    TaskType = DDJ_Unstack;
                    break;
                case WCSTaskTypes.DDJStackMove:
                    TaskType = DDJ_Move;
                    break;
                case WCSTaskTypes.DDJDirect:
                    TaskType = DDJ_InOut;
                    break;
                default:
                    TaskType = null;
                    break;
            }
        }
        public DEMATICMessage GetDDJMessage()
        {
            DEMATICMessage dDJMessage = new DEMATICMessage(DeviceID);
            dDJMessage.MsgType = TaskType;
            dDJMessage.Message = MsgHeader + MsgType + MsgSender + MsgReceiver + MsgSeqID + MsgEnd;
            return dDJMessage;
        }
        public enum DematicTaskStatus
        {
            None,
            Waiting,
            Doing,
            Fault,
            EmptyOut,
            DoubleIn,
            Done
        }
    }
}
