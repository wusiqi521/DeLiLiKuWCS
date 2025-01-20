using System;
using System.ComponentModel;
using System.Text;

namespace BMHRI.WCS.Server.Models
{
    public class WCSTask
    {
        public string WMSSeqID { set; get; }
        public string WCSSeqID { set; get; }
        public string PalletNum { set; get; }
        public string FmLocation { set; get; }
        public string ToLocation { set; get; }
        public string CreateTime { get; set; }
        public string StartTime { get; set; }
        public string FinishTime { get; set; }
        public string DeviceID { get; set; }
        public WCSGaoDiBZ GaoDiBZ { get; set; }
        public WCSTaskTypes TaskType { get; set; }
        public WCSTaskStatus TaskStatus { get; set; }
        public int TaskPri { get; set; }
        public int TaskStep { get; set; }
        public int Floor { get; set; }
        public string WMSTaskFmlocation { get; set; }//用于堆垛机上架任务存储巷道口输送机position
       // public string WMSTaskTolocation { get; set; }//用于堆垛机下架任务存储最终地址（通常是出库口、拣选口等）输送机position
        public string Warehouse { get; set; }
        //public string SourceLocation { get; set; } //得力项目wms拆分堆垛机上架任务的时候，需要将实际申请的position传给该属性，找到相应的ssj（得力项目三层是一个plc）
        public WCSTask()
        {
            //2021-07-22 15:49:28.8456876
            WCSSeqID = Guid.NewGuid().ToString().Substring(0, 8);//GetWCSSeqID(8);
            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffff");
            TaskStep = 1;
            TaskPri = 1;
        }
        private string GetWCSSeqID(int j)
        {
            byte[] r = new byte[8];
            Random rand = new Random((int)(DateTime.Now.Ticks % 1000000));
            int ran;
            //生成8字节原始数据
            for (int i = 0; i < j; i++)
                //while循环剔除非字母和数字的随机数
                do
                {
                    //数字范围是ASCII码中字母数字和一些符号
                    ran = rand.Next(48, 122);
                    r[i] = Convert.ToByte(ran);
                } while ((ran >= 58 && ran <= 64) || (ran >= 91 && ran <= 96));
            //转换成8位String类型               
            return Encoding.ASCII.GetString(r);
        }
    }

    public enum WCSTaskTypes
    {
        [Description("输送机入库")]
        SSJInbound,
        [Description("堆垛机上架")]
        DDJStack,
        [Description("堆垛机下架")]
        DDJUnstack,
        [Description("堆垛机移库")]
        DDJStackMove,
        [Description("堆垛机直出")]
        DDJDirect,
        [Description("输送机出库")]
        SSJOutbound,
        [Description("输送机拣选出库")]
        SSJPickUpOutbound,
        [Description("输送机直行")]
        SSJDirect,
        [Description("Agv搬运")]
        AgvBound,
        [Description("Agv出库搬运")]
        AgvOubound,
        [Description("Agv入库搬运")]
        AgvInbound,
        [Description("Agv车间任务")]
        AgvToWorkShop,
        [Description("Agv货位任务")]
        AgvToGoodLoca,
    }

    public enum WCSTaskStatus
    {
        [Description("无法执行")]
        Cannot,
        [Description("等待执行")]
        Waiting,
        [Description("正在执行")]
        Doing,
        [Description("不合格整理")]
        UnPass,
        [Description("托盘到位")]
        SSJInPlace,
        [Description("托盘取走")]
        SSJPicked,
        [Description("托盘取走")]
        DDJPicked,
        [Description("执行完毕")]
        Done,
        [Description("执行异常")]
        Fault,
        [Description("目标不可达")]
        Unreachable,
        [Description("小车改址")]
        DFCarryChged,
        [Description("入库改址")]
        StackChged,
        [Description("空出库")]
        UnStackEmpty,
        [Description("空出库确认")]
        UnStackEmptyConfirm,
        [Description("空取货")]
        PickEmpty,
        [Description("空取确认")]
        PickEmptyConfirm,
        [Description("双重入库")]
        StackDouble,
        [Description("双重确认")]
        StackDoubleConfirm,
        [Description("小车卸货完成")]
        CarryDone,
        [Description("等待申请")]
        WaitingForApply,
        [Description("点亮LED")]
        LightLED,
        [Description("熄灭LED")]
        UnLightLED,
        [Description("远端出库近端有货")]
        FarOutboundClearHave,
        [Description("远端出库近端有货确认")]
        FarOutboundClearHaveConfirm,
        [Description("SSJ直出到位")]
        SSJDirectDone,
        [Description("SSJ已进圈")]
        SSJInCircle,
        [Description("进圈队列中")]
        InCircleQueue,
        [Description("AGV取货完成")]
        AGVPickUped,
        [Description("AGV放货完成")]
        AGVPutDowned,
        [Description("AGV请求取货")]
        AGVApplyPick,
        [Description("AGV请求放货")]
        AGVApplyPut,
        [Description("AGV允许放货")]
        AGVAllowPut,
        [Description("AGV允许取货")]
        AGVAllowPick
    }
    public enum WCSGaoDiBZ
    {
        [Description("未知")]
        None,
        [Description("低托盘")]
        Low,
        [Description("高托盘")]
        Hight,
    }
}
