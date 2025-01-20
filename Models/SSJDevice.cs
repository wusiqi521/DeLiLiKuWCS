using BMHRI.WCS.Server.DDJProtocol;
using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.UserControls;
using NPOI.SS.Formula.Functions;
using S7.Net;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BMHRI.WCS.Server.Models
{
    public class SSJDevice : PLCDevice
    {
        #region 变量
        public List<SSJDeviceBlock> DeviceBlockList;
        public List<DataItem> DataItemList;
        public List<SSJMessage> WCSTOPLCDB4MessageList;
        public List<SSJMessage> PLCTOWCSDB5MessageList;
        
        public List<DataItem> DB45DataItemList;
        //private List<DataItem> DB10DataItemList;
        //private List<DataItem> DB8DataItemList;
        private List<DataItem> DB10DataItemList;
        private List<DataItem> InputDataItemList;
        public List<DataItem> MBInAndOutDataItemList;
        public List<DataItem> MBLightDataItemList;
        private List<DataItem> MBOccupyDataItemList;
        private List<DataItem> MBAllowOutDataItemList;
        private List<DataItem> MBTPHorizonDataItemList;
        private List<DataItem> MBInCircleDataItemList;
        private List<DataItem> MBFarawayCircleDataItemList;
        public List<DataItem> OutTaskLimitDataItemList;
        public List<DataItem> CircleModeDataItemList;
        private List<DataItem> RGVDataItemList;
        public List<DataItem> ModifyDataItemList;
        public List<RGVDevice> RGVDeviceList;
        public List<TSJDevice> TSJDeviceList;
        private readonly bool AutoConnect;
        private readonly int[] DBAddressOffsets = new int[] { 50, 52, 54, 58, 78, 82, 84, 86, 88, 90, 92, 94, 96, 98, 100, 102, 104, 106 };
        private readonly int[] DBCounts = new int[] { 2, 2, 4, 10, 4, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        private readonly string[] PropertyNames = new string[] {
            nameof(SSJDeviceBlock.Position),
            nameof(SSJDeviceBlock.ToLocation),
            null,
            nameof(SSJDeviceBlock.PalletNum) ,
            nameof(SSJDeviceBlock.LoadWeight),
            nameof(SSJDeviceBlock.LoadType),
            null,
            null,
            nameof(SSJDeviceBlock.CurrentMode),
            nameof(SSJDeviceBlock.PositionInfor),
            nameof(SSJDeviceBlock.IsLoaded),
            nameof(SSJDeviceBlock.IsOccupied),
            nameof(SSJDeviceBlock.BlockStatus),
            nameof(SSJDeviceBlock.ErrorCode),
            nameof(SSJDeviceBlock.FaultContent1),
            nameof(SSJDeviceBlock.FaultContent2) };
        #endregion
        #region 属性
        private SSJDeviceWorkState ssj_work_state;
        public SSJDeviceWorkState SSJWorkState
        {
            get { return ssj_work_state; }
            set
            {
                if (ssj_work_state != value)
                {
                    ssj_work_state = value;
                    Notify(nameof(SSJWorkState));
                }
            }
        }

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

        private string faultCode;
        public string FaultCode
        {
            get { return faultCode; }
            set
            {
                faultCode = value;
                Notify(nameof(FaultCode));
            }
        }

        private string faultContent;
        public string FaultContent
        {
            get { return faultContent; }
            set
            {
                faultContent = value;
                Notify("FaultContent");
            }
        }
        private int inCircleNum;
        public int InCircleNum 
        { 
            get { return inCircleNum; } 
            set 
            { 
                inCircleNum = value;
                Notify("InCircleNum");
            } 
        }
        private int inCircleNum2;
        public int InCircleNum2
        {
            get { return inCircleNum2; }
            set
            {
                inCircleNum2 = value;
                Notify("InCircleNum2");
            }
        }
        private int inCircleNum3;
        public int InCircleNum3
        {
            get { return inCircleNum3; }
            set
            {
                inCircleNum3 = value;
                Notify("InCircleNum3");
            }
        }
        private int circleMode;
        public int CircleMode
        {
            get { return circleMode; }
            set
            {
                if (circleMode != value)
                {
                    circleMode = value;
                    Notify("CircleMode");
                }
            }
        }

        private string port_num;
        public string PortNum
        {
            get
            {
                //if (string.IsNullOrEmpty(port_num))
                    //switch (PLCID.Substring(4, 1))
                    //{
                    //    case "1":
                    //        port_num = "000a";
                    //        break;
                    //    case "2":
                    //        port_num = "000c";
                    //        break;
                    //    case "3":
                    //        port_num = "000b";
                    //        break;
                    //    case "4":
                    //        port_num = "000d";
                    //        break;
                    //    case "5":
                    //        port_num = "000b";
                    //        break;
                    //    default:
                            port_num = null;
                    //        break;
                    //}
                return port_num;
            }
        }

        #endregion
        public SSJDevice(string cpuType, string Ip, string slot, string rack, string decription, string device_type, string plcid) : base(cpuType, Ip, slot, rack, decription, device_type, plcid)
        {
            AutoConnect = true;
            
            DeviceBlockList = MyDataTableExtensions.ToList<SSJDeviceBlock>
                (SQLServerHelper.DataBaseReadToTable(
                    "SELECT [Position]" +
                    ",[PLCID]" +
                    ",[SystemType]" +
                    ",[DeviceType]" +
                    ",[Available]" +
                    ",[Description]" +
                    ",[PalletNumDBAddr]" +
                    ",[IsOccupyingMBAddr]" +
                    ",[IsOccupyingMBAddr2]" +
                    ",[AllowUnloadingMBAddr]" +
                    ",[AllowUnloadingMBAddr2]" +
                    ",[TPHorizonMBAddr]"+
                    ",[TPHorizonMBAddr2]"+
                    ",[InCircleMBAddr]" +
                    ",[InCircleMBAddr2]" +
                    ",[FarCircleMBAddr]" +
                    ",[FarCircleMBAddr2]" +
                    ",[StatusDBAddr]" +
                    ",[Tunnel]" +
                    ",[MotionDirection]" +
                    ",[DDJID]" +
                    ",[PortNum]" +
                    ",[Floor]" +
                    ",[AGVToPosition]" +
                    ",[AGVGetPoint]" +
                    ",[AGVSetPoint]" +
                    ",[AreaType]" +
                    ",[InCircle]" +
                    ",[ClearCircle]" +
                    ",[FarCircle]"+
                    ",[Floor2Circle]" +
                     ",[Warehouse]" +
                     ",[InAndOutMBAddr]" +
                     ",[InAndOutMBAddr2]" +
                     ",[LightMBAddr]" +
                     ",[LightMBAddr2]" +
                   " FROM[dbo].[ConveyerBlocks] where PLCID='" + plcid + "'"));
            CreateTagList();//从表里读标志
            DB45DataItemList = DataItemList.FindAll(x => x.DataType == DataType.DataBlock && (x.DB == 4 || x.DB == 5));//从配置文件里
            DB10DataItemList = DataItemList.FindAll(x => x.DataType == DataType.DataBlock && x.DB == 10);
            // OutTaskLimitDataItemList = DataItemList.FindAll(x => x.DataType == DataType.Memory && x.StartByteAdr == 15 && x.BitAdr == 1);
            // MBInAndOutDataItemList = DataItemList.FindAll(x => x.DataType == DataType.Memory && x.StartByteAdr==172 && x.VarType == VarType.Bit);
            InputDataItemList = DataItemList.FindAll(x => x.DataType == DataType.Input && x.StartByteAdr == 13);
            WCSTOPLCDB4MessageList = new List<SSJMessage>();
            PLCTOWCSDB5MessageList = new List<SSJMessage>();
            ModifyDataItemList = new List<DataItem>();

            CreateRGVDataItemList();

            Task.Factory.StartNew(() => PLCCommunication(), TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => ProcessSSJTask(), TaskCreationOptions.LongRunning);
        }
        private void CreateRGVDataItemList()
        {
            RGVDeviceList = MyDataTableExtensions.ToList<RGVDevice>(SQLServerHelper.DataBaseReadToTable("SELECT[DeviceID],[StatusMB],[IsOccupyingMB],[PalletNumMB],[PositionMB],[FaultCodeMB],[PLCID] FROM[dbo].[RGVList] where PLCID='" + PLCID + "'"));
            if (RGVDeviceList == null) RGVDeviceList = new List<RGVDevice>();
        }

        private void AutoConnectPLC()
        {
            if (AutoConnect)
            {
                if (PLCConnectState == ConnectionStates.Disconnected)
                {
                    Connect();
                }
            }
        }
        public void ReadMulipleVars(List<DataItem> dataItems)
        {
            int localIndex = 0;
            int count = dataItems.Count;   //db块的数量
            int step = 6;
            while (count > 0)
            {
                var maxToWrite = Math.Min(count, step);
                List<DataItem> subdataItems = dataItems.GetRange(localIndex, maxToWrite);   //一次性读取6个db块，不是6个字节
                Plc.ReadMultipleVars(subdataItems);
                count -= maxToWrite;
                localIndex += maxToWrite;
                //System.Diagnostics.Debug.WriteLine(subdataItems[0].StartByteAdr);
            }
        }
        private void PLCCommunication()//ReadMulipleVars利用该方法从plc中读取参数获取的值并赋值给参数列表中的数据项
        {
            while (true)
            {
                try
                {
                    if (PLCConnectState == ConnectionStates.Connected)
                    {
                        Plc.ReadMultipleVars(DB45DataItemList);
                        ProcessPLCTOWCSDB5();
                        ReadMulipleVars(DB10DataItemList);
                        ProcessWCSTOPLCDB4();
                        ReadMulipleVars(MBAllowOutDataItemList);
                        ReadMulipleVars(MBOccupyDataItemList);
                        ReadMulipleVars(InputDataItemList);
                        ReadMulipleVars(MBLightDataItemList);
                        
                        if (OutTaskLimitDataItemList != null && OutTaskLimitDataItemList.Count > 0)
                        {
                            ReadMulipleVars(OutTaskLimitDataItemList);
                        }
                        if (MBInAndOutDataItemList != null && MBInAndOutDataItemList.Count > 0)
                        {
                            Plc.ReadMultipleVars(MBInAndOutDataItemList);
                        }
                        UpdateDeviceBlocksStatus();
                        FindDoorStatus();
                        //UpdateRGVDeviceStatus();

                        WriteToPLCFromDataItemList();

                    }
                    else
                    {
                      //   UpdateDeviceBlocksStatus();
                        AutoConnectPLC();
                       
                    }
                    Task.Delay(300).Wait();
                }
                catch (PlcException plc_ex)
                {
                    PlcExceptionParse(plc_ex);
                    if (!Plc.IsConnected)
                    {
                        PLCConnectState = ConnectionStates.Disconnected;
                        UpdateDeviceState(SSJDeviceWorkState.None);
                    }

                    //LogHelper.WriteLog("读取PLC失败，" + PLCID + " " + plc_ex.ToString());
                    Task.Delay(3000).Wait();
                }
            }
        }
        internal void WriteDataItemToPLC(DataItem dataItem)
        {
            if (dataItem == null) return;
            lock (ModifyDataItemList)
            {
                ModifyDataItemList.Add(dataItem);
            }
        }

        private void FindDoorStatus()
        {
            
            if (InputDataItemList.Count > 0)//只有SSJ02有
            {
                DataItem door_tunnel1 = InputDataItemList.Find(x => x.BitAdr==2);
                DataItem door_tunnel2 = InputDataItemList.Find(x => x.BitAdr == 6);
                if (door_tunnel1 == null || door_tunnel2 == null) return;
                DDJDevice dDJ01 = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == "DDJ01");
                DDJDevice dDJ02 = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == "DDJ02");
                if (dDJ01 == null || dDJ02 == null) return;
                dDJ01.IsOpenDoor = Convert.ToInt32(door_tunnel1.Value) == 1;
                dDJ02.IsOpenDoor = Convert.ToInt32(door_tunnel2.Value) == 1;

            }
        }
        private void UpdateDeviceBlocksStatus()
        {
            foreach (SSJDeviceBlock sSJDeviceBlock in DeviceBlockList)
            {
                if (sSJDeviceBlock == null) continue;
                DataItem dataItem = DataItemList.Find(x => x.DB == 10 && x.StartByteAdr == sSJDeviceBlock.PalletNumDBAddr);
                DataItem occupy_dataItem = MBOccupyDataItemList.Find(x => x.DataType == DataType.Memory && x.StartByteAdr == sSJDeviceBlock.IsOccupyingMBAddr && x.BitAdr == sSJDeviceBlock.IsOccupyingMBAddr2);
                
                if (dataItem == null || occupy_dataItem == null) continue;
                byte[] bytes = (byte[])dataItem.Value;
                if (bytes == null || bytes.Length != 30) continue;
                string pallet_num = S7.Net.Types.String.FromByteArray(bytes.Skip(0).Take(12).ToArray());
                pallet_num = pallet_num.TrimEnd('\0', '\u0003', ' ');
                string fm_position = S7.Net.Types.String.FromByteArray(bytes.Skip(12).Take(4).ToArray());
                string to_position = S7.Net.Types.String.FromByteArray(bytes.Skip(16).Take(4).ToArray());
                if (!string.IsNullOrEmpty(pallet_num) && pallet_num.Length >= 10)
                {                  
                    if (pallet_num.Contains("**"))
                    {
                        sSJDeviceBlock.PalletNum = pallet_num.Substring(0, 10);
                    }
                    else
                    {
                        sSJDeviceBlock.PalletNum = pallet_num;
                    }
                }
                else
                {
                    sSJDeviceBlock.PalletNum = null;
                } 
                sSJDeviceBlock.FmLocation = fm_position;
                sSJDeviceBlock.ToLocation = to_position;               
                int is_occupied = Convert.ToInt32(occupy_dataItem.Value);
                sSJDeviceBlock.IsOccupied = is_occupied == 1;
                DataItem Light_dataItem = MBLightDataItemList.Find(x => x.DataType == DataType.Input && x.StartByteAdr == sSJDeviceBlock.LightMBAddr && x.BitAdr == sSJDeviceBlock.LightMBAddr2);
                int NotAGVAllowUnloading = 0;
                if (Light_dataItem == null)
                    NotAGVAllowUnloading = 0;
                else
                    NotAGVAllowUnloading = Convert.ToInt32(Light_dataItem.Value);
                sSJDeviceBlock.Inbound_Light = NotAGVAllowUnloading == 1;
                DataItem allowunloading_dataItem = MBAllowOutDataItemList.Find(x => x.DataType == DataType.Memory && x.StartByteAdr == sSJDeviceBlock.AllowUnloadingMBAddr && x.BitAdr == sSJDeviceBlock.AllowUnloadingMBAddr2);
                int allow_unloading = 0;
                if (allowunloading_dataItem == null)
                    allow_unloading = 0;
                else
                    allow_unloading = Convert.ToInt32(allowunloading_dataItem.Value);
                sSJDeviceBlock.AllowUnloading = allow_unloading == 1;

                DataItem inandout_dataItem = MBInAndOutDataItemList.Find(x => x.DataType == DataType.Memory && x.StartByteAdr == sSJDeviceBlock.InAndOutMBAddr && x.BitAdr == sSJDeviceBlock.InAndOutMBAddr2);
                if (inandout_dataItem == null)
                    sSJDeviceBlock.CurrentMode = (DeviceModeType)(2);//所有设备块的出入库模式都是-1，数据库中如果没有写该设备块内存地址信息，该设备块的信息仍旧赋值-1，避免通知。
                else
                    sSJDeviceBlock.CurrentMode = (DeviceModeType)Convert.ToInt32(inandout_dataItem.Value);//400.0是1是入库模式 400.0是0是出库模式
            }
        }
        private void ReduceCircleNum(List<DataItem> dataItem)
        {
            if (PLCID == "SSJ01")
            {
                for (int i = 0; i < dataItem.Count; i++)
                {
                    if (Convert.ToInt32(dataItem[i].Value) == 1)
                    {
                        InCircleNum--;
                        if (InCircleNum < 0)
                            InCircleNum = 0;
                        WritePLCDataItem(dataItem[i], 0);

                    }
                }
            }
            else
            {
                for (int i = 0; i < dataItem.Count; i++)
                {
                    if (Convert.ToInt32(dataItem[i].Value) == 1)
                    {
                        SSJDeviceBlock sSJDeviceBlock = DeviceBlockList.Find(x => x.FarCircle == true && x.FarCircleMBAddr == dataItem[i].StartByteAdr && x.FarCircleMBAddr2 == dataItem[i].BitAdr);
                        if (sSJDeviceBlock == null) return;
                        if (sSJDeviceBlock.Floor2Circle == 1)
                        {
                            InCircleNum--;
                            if (InCircleNum < 0)
                                InCircleNum = 0;
                        }
                        else if (sSJDeviceBlock.Floor2Circle == 2)
                        {
                            InCircleNum2--;
                            if (InCircleNum2 < 0)
                                InCircleNum2 = 0;
                        }
                        else
                        {
                            InCircleNum3--;
                            if (InCircleNum3 < 0)
                                InCircleNum3 = 0;
                        }
                        WritePLCDataItem(dataItem[i], 0);
                    }
                }
            }
        }
        private void CircleModeTrans(List<DataItem> dataItems)
        {
            if(dataItems==null|| dataItems.Count==0) return;
            for(int i = 0;i< dataItems.Count;i++)
            {
                if (Convert.ToInt32(dataItems[i].Value) == 1)
                    CircleMode = i + 1;
            }
        }
        public void UpdateRGVDeviceStatus()
        {
            foreach (RGVDevice rGVDevice in RGVDeviceList)
            {
                if (rGVDevice != null && rGVDevice.DataItemList.Count > 0)
                {
                    Plc.ReadMultipleVars(rGVDevice.DataItemList);
                    rGVDevice.UpdateRGVStatuss();
                }
            }
        }

        //public void UpdateWMSPortMode(SSJDeviceBlock sSJDeviceBlock)
        //{
        //    WMSTask wMSTask;
        //    if (sSJDeviceBlock.Position == "1001" || sSJDeviceBlock.Position == "1026")
        //    {
        //        if (sSJDeviceBlock.CurrentMode == DeviceModeType.InboundMode)
        //        {
        //            wMSTask = new WMSTask();
        //            wMSTask.TaskType = WMSTaskType.DeviceMsg;
        //            wMSTask.PalletNum = "00000000";
        //            wMSTask.TaskSource = "WMS";
        //            wMSTask.ToLocation = sSJDeviceBlock.Position;
        //            wMSTask.TaskStatus = WMSTaskStatus.SSJPortModeIn;
        //            WMSTasksManager.Instance.AddWMSTask(wMSTask);
        //        }
        //        else if (sSJDeviceBlock.CurrentMode == DeviceModeType.OutboundMode)
        //        {
        //            wMSTask = new WMSTask();
        //            wMSTask.TaskType = WMSTaskType.DeviceMsg;
        //            wMSTask.PalletNum = "00000000";
        //            wMSTask.TaskSource = "WMS";
        //            wMSTask.ToLocation = sSJDeviceBlock.Position;
        //            wMSTask.TaskStatus = WMSTaskStatus.SSJPortModeOut;
        //            WMSTasksManager.Instance.AddWMSTask(wMSTask);
        //        }
        //    }
        //}

        public void XApplyInbound(SSJDeviceBlock sSJDeviceBlock)
        {
            if (sSJDeviceBlock is null) return;
            if (sSJDeviceBlock.InboundTrigerAddr < 1) return;
            DataItem db_data = new DataItem
            {
                DB = 0,
                StartByteAdr = sSJDeviceBlock.InboundTrigerAddr,
                VarType = VarType.Bit,
                Count = 1,
                DataType = DataType.Input,
                BitAdr = 1,
                Value = true
            };
            lock (ModifyDataItemList)
            {
                ModifyDataItemList.Add(db_data);
            }
        }
        public void OutTaskLimitDataItem(int StartByteAdr, byte BitAdr, object value)
        {
            DataItem db_data1 = new DataItem
            {
                DB = 0,
                StartByteAdr = StartByteAdr,
                VarType = VarType.Bit,
                Count = 1,
                DataType = DataType.Memory,
                BitAdr = BitAdr,
                Value = value
            };
            lock (ModifyDataItemList)
            {
                ModifyDataItemList.Add(db_data1);
            }
        }
        public bool UpdateDeviceBlockList(SSJDeviceBlock ssjdevblock)
        {
            if (ssjdevblock == null) return false;
            SqlParameter[] sqlParameters = new SqlParameter[] {
                    new SqlParameter("@id", ssjdevblock.ID),
                    new SqlParameter("@Position", ssjdevblock.Position),
                    new SqlParameter("@plcid", ssjdevblock.PLCID),
                    new SqlParameter("@SystemType", ssjdevblock.SystemType),
                    new SqlParameter("@DeviceType", ssjdevblock.DeviceType),
                    new SqlParameter("@Avaiable", ssjdevblock.Available),
                    new SqlParameter("@PalletNumDBAddr", ssjdevblock.PalletNumDBAddr),
                    new SqlParameter("@StatusDBAddr", ssjdevblock.StatusDBAddr),
                    new SqlParameter("@ReturnValue", SqlDbType.Int) };
            int rv = (int)SQLServerHelper.ExeProcedure("UpdateNewConveyBlock", sqlParameters);
            if (rv > 0)
            {
                SSJDeviceBlock ssjdb = DeviceBlockList.Find(x => x.ID == ssjdevblock.ID);
                if (ssjdb != null)
                    DeviceBlockList.Remove(ssjdb);
                DeviceBlockList.Add(ssjdevblock);
                return true;
            }
            else return false;
        }
        private void CreateTagList()
        {
            if (DataItemList == null)
                DataItemList = new List<DataItem>();
            if (MBOccupyDataItemList == null)
                MBOccupyDataItemList = new List<DataItem>();
            if (MBAllowOutDataItemList == null)
                MBAllowOutDataItemList = new List<DataItem>();
            if (OutTaskLimitDataItemList == null)
                OutTaskLimitDataItemList = new List<DataItem>();
            if (MBInAndOutDataItemList == null)
                MBInAndOutDataItemList = new List<DataItem>();
            if (MBLightDataItemList == null)
                MBLightDataItemList = new List<DataItem>();
            foreach (SSJDeviceBlock sSJDeviceBlock in DeviceBlockList)
            {
                if (sSJDeviceBlock.ID < 0) continue;
                if (sSJDeviceBlock.PalletNumDBAddr < 0 || sSJDeviceBlock.IsOccupyingMBAddr < 0 || sSJDeviceBlock.IsOccupyingMBAddr2 < 0) continue;

                //for (int i = 0; i < DBAddressOffsets.Length; i++)
                //{
                //    string addressString = "DB110.DBW" + sSJDeviceBlock.ID + DBAddressOffsets[i];
                //    DataItem dataItem = DataItem.FromAddress(addressString);
                //    if (dataItem != null)
                //    {
                //        dataItem.Count = DBCounts[i];
                //        DataItemList.Add(dataItem);
                //    }
                //}
                DataItem dataItem = new DataItem();
                dataItem.DataType = DataType.DataBlock;
                dataItem.VarType = VarType.Byte;
                dataItem.DB = 10;
                dataItem.StartByteAdr = sSJDeviceBlock.PalletNumDBAddr;
                dataItem.Count = 30;
                DataItemList.Add(dataItem);

                DataItem occupy_dataItem = new DataItem();
                occupy_dataItem.DataType = DataType.Memory;
                occupy_dataItem.VarType = VarType.Bit;
                occupy_dataItem.DB = 0;
                occupy_dataItem.StartByteAdr = sSJDeviceBlock.IsOccupyingMBAddr;
                occupy_dataItem.BitAdr = (byte)sSJDeviceBlock.IsOccupyingMBAddr2;
                occupy_dataItem.Count = 1;
                MBOccupyDataItemList.Add(occupy_dataItem);

                DataItem allowloading_dataItem = new DataItem();
                allowloading_dataItem.DataType = DataType.Memory;
                allowloading_dataItem.VarType = VarType.Bit;
                allowloading_dataItem.DB = 0;
                allowloading_dataItem.StartByteAdr = sSJDeviceBlock.AllowUnloadingMBAddr;
                allowloading_dataItem.BitAdr = (byte)sSJDeviceBlock.AllowUnloadingMBAddr2;
                allowloading_dataItem.Count = 1;
                if (sSJDeviceBlock.AllowUnloadingMBAddr > 0)
                    MBAllowOutDataItemList.Add(allowloading_dataItem);

                DataItem inandout_dataItem = new DataItem();
                inandout_dataItem.DataType = DataType.Memory;
                inandout_dataItem.VarType = VarType.Bit;
                inandout_dataItem.DB = 0;
                inandout_dataItem.StartByteAdr = sSJDeviceBlock.InAndOutMBAddr;
                inandout_dataItem.BitAdr = (byte)sSJDeviceBlock.InAndOutMBAddr2;
                inandout_dataItem.Count = 1;
                if (sSJDeviceBlock.InAndOutMBAddr > 0)
                    MBInAndOutDataItemList.Add(inandout_dataItem);

                DataItem Light_dataItem = new DataItem();
                Light_dataItem.DataType = DataType.Input;
                Light_dataItem.VarType = VarType.Bit;
                Light_dataItem.DB = 0;
                Light_dataItem.StartByteAdr = sSJDeviceBlock.LightMBAddr;
                Light_dataItem.BitAdr = (byte)sSJDeviceBlock.LightMBAddr2;
                Light_dataItem.Count = 1;
                if (sSJDeviceBlock.LightMBAddr > 0)
                    MBLightDataItemList.Add(Light_dataItem);

            }
            GetTagListFromConfig();
        }
        private void ProcessPLCTOWCSDB5()
        {
            // 从 DataItem 列表中找到 DB=5（输送机向WCS发送的电报），将其 Value 转成字符串
            PLCTOWCSDB5 = DB45DataItemList.Find(x => x.DB == 5).Value.ToString();
            if (PLCTOWCSDB5.Substring(0, 2) == "0F")
            {
                int a = 5;
            }
            if (PLCTOWCSDB5.Length == 38|| PLCTOWCSDB5.Length == 50)
            {
                if (PLCTOWCSDB5.Substring(36, 2) == "01")
                {
                    if(PLCTOWCSDB5.Substring(0, 2) !="0X" && PLCTOWCSDB5.Substring(0, 2) != "0Y")
                    {
                        PLCTOWCSDB5 = PLCTOWCSDB5.Substring(0, 38);
                    }
                    if (WritePLCDataItem(DB45DataItemList.Find(x => x.DB == 5), PLCTOWCSDB5.Substring(0, 36) + "10"))
                    {
                        Task.Factory.StartNew(() => PLCTOWCSMessageParse(PLCTOWCSDB5));//只解析将01—>10的
                    }
                }
            }
            //else
            //{

            //    SSJMessage ssj_message = new SSJMessage(PLCTOWCSDB5, PLCID, SSJMsgDirection.Receive);
            //    if (ssj_message == null) return;
            //    InsertIntoSSJMsgLog(ssj_message);
            //}
            if (PLCTOWCSDB5MessageList.Count > 0)
            {
                if (WritePLCDataItem(DB45DataItemList.Find(x => x.DB == 5), PLCTOWCSDB5MessageList[0].GetTransDB4()))
                    PLCTOWCSDB5MessageList.RemoveAt(0);
            }
        }
        private void ProcessWCSTOPLCDB4()
        {
            WCSTOPLCDB4 = DB45DataItemList.Find(x => x.DB == 4).Value.ToString();
            if (WCSTOPLCDB4.Length != 38 && WCSTOPLCDB4.Length != 50 || WCSTOPLCDB4.Substring(37, 1) == "1") return;
            //if (WCSTOPLCDB4.Length < 38 || WCSTOPLCDB4.Substring(37, 1) == "1") return;
            if (WCSTOPLCDB4MessageList.Count < 1) return;
            if (PLCConnectState != ConnectionStates.Connected) return;

            WCSTOPLCDB4MessageList = WCSTOPLCDB4MessageList.OrderBy(x => x.SendPriority).ThenBy(x => x.Tkdat).ToList();

            if (SSJWorkState == SSJDeviceWorkState.Manual)
                return;
            if (SSJWorkState == SSJDeviceWorkState.Offline || SSJWorkState == SSJDeviceWorkState.None)
                if (WCSTOPLCDB4MessageList[0].MessageType != "00" && WCSTOPLCDB4MessageList[0].MessageType != "0L" && WCSTOPLCDB4MessageList[0].MessageType != "0B")
                    return;

            if (WCSTOPLCDB4MessageList[0] != null)
            {
                string WcsToPLCDB4_50 = WCSTOPLCDB4MessageList[0].GetTransDB4();
                if (WcsToPLCDB4_50.Length < 50)
                {
                    WcsToPLCDB4_50 = WcsToPLCDB4_50.PadRight(50, '0');
                }

                if (WritePLCDataItem(DB45DataItemList.Find(x => x.DB == 4), WcsToPLCDB4_50))
                {
                    WCSTOPLCDB4MessageTransfor(WCSTOPLCDB4MessageList[0]);
                    DeleteSSJSendList(WCSTOPLCDB4MessageList[0]);
                }

            }
        }
        private void WCSTOPLCDB4MessageTransfor(SSJMessage sSJMessage)
        {
            string wCSTOPLCDB4Message = "";
            if (sSJMessage == null) return;
            switch (sSJMessage.MessageType)
            {
                case "0Y":
                    wCSTOPLCDB4Message = "WCS向输送机回复入库应答 托盘号" + sSJMessage.GetPalletNum(sSJMessage.MessageType) + "起始地" + sSJMessage.FmLocation + "目的地" + sSJMessage.ToLocation;
                    break;
                case "0b":
                    wCSTOPLCDB4Message = "WCS向输送机发送出库任务 托盘号" + sSJMessage.GetPalletNum(sSJMessage.MessageType) + "起始地" + sSJMessage.FmLocation + "目的地" + sSJMessage.ToLocation;
                    break;
                case "0E":
                    wCSTOPLCDB4Message = "WCS向输送机回复入库完成确认 托盘号" + sSJMessage.GetPalletNum(sSJMessage.MessageType) + "起始地" + sSJMessage.FmLocation + "目的地" + sSJMessage.ToLocation;
                    break;
                case "0Q":
                    wCSTOPLCDB4Message = "WCS向输送机发送清除占位信息 托盘号" + sSJMessage.GetPalletNum(sSJMessage.MessageType) + "地址" + sSJMessage.FmLocation;
                    break;
                case "0S":
                    wCSTOPLCDB4Message = "WCS向输送机申请补空托盘至1026 托盘号" + sSJMessage.GetPalletNum(sSJMessage.MessageType) + "起始地" + sSJMessage.FmLocation + "目的地" + sSJMessage.ToLocation;
                    break;
                case "0L":
                    wCSTOPLCDB4Message = "WCS向输送机发送联机申请";
                    break;
                case "0B":
                    wCSTOPLCDB4Message = "WCS向输送机发送脱机信号";
                    break;
                case "0A":
                    wCSTOPLCDB4Message = "WCS向输送机发送联机应答信号";
                    break;
                case "0G":
                    wCSTOPLCDB4Message = "WCS向输送机发送直行信号 托盘号" + sSJMessage.GetPalletNum(sSJMessage.MessageType) + "起始地" + sSJMessage.FmLocation + "目的地" + sSJMessage.ToLocation;
                    break;
                case "0K":
                    wCSTOPLCDB4Message = "WCS向输送机发送重量 托盘号" + sSJMessage.GetPalletNum(sSJMessage.MessageType) + "口地址" + sSJMessage.FmLocation + "重量" + sSJMessage.Trans.Substring(23, 4);
                    break;
                default:
                    wCSTOPLCDB4Message = "";
                    break;
            }
            sSJMessage.MsgParse = wCSTOPLCDB4Message;
            UpdateIntoSSJSendListDB(sSJMessage);
        }
        private void PlcExceptionParse(PlcException plc_ex)
        {
            if (plc_ex.ErrorCode != ErrorCode.NoError)
            {
                switch (plc_ex.ErrorCode)
                {
                    case ErrorCode.ConnectionError:
                    case ErrorCode.IPAddressNotAvailable:
                        UpdatePLCConnectState(ConnectionStates.Disconnected);
                        break;
                    case ErrorCode.ReadData:
                    case ErrorCode.SendData:
                    case ErrorCode.WriteData:
                    case ErrorCode.WrongCPU_Type:
                    case ErrorCode.WrongNumberReceivedBytes:
                    case ErrorCode.WrongVarFormat:
                        UpdateDeviceState(SSJDeviceWorkState.Fault);
                        break;
                    default: break;
                }
            }
            LogHelper.WriteLog("PLC通讯失败，" + PLCID + " ErrorCode = " + plc_ex.ErrorCode + " " + plc_ex.ToString());
        }
        private void ProcessSSJTask()
        {
            while (true)
            {
                try
                {
                    List<WCSTask> wCSTasks0b = WCSTaskManager.Instance.WCSTaskList.FindAll(x => x.DeviceID == PLCID && (x.TaskStatus == WCSTaskStatus.Waiting) && (x.TaskType == WCSTaskTypes.SSJOutbound || x.TaskType == WCSTaskTypes.SSJPickUpOutbound));
                    List<WCSTask> wCSTasks0Y = WCSTaskManager.Instance.WCSTaskList.FindAll(x => x.DeviceID == PLCID && (x.TaskStatus == WCSTaskStatus.Waiting) && x.TaskType == WCSTaskTypes.SSJInbound);

                    for (int i = 0; i < wCSTasks0b.Count; i++)
                    {
                        SSJDeviceBlock to_blockb = DeviceBlockList.Find(x => x.Position == wCSTasks0b[i].ToLocation && (x.SystemType == DeviceSystemType.OutboundFinish || x.SystemType == DeviceSystemType.Picking || x.SystemType == DeviceSystemType.TotalPort));

                        SSJDeviceBlock fm_blockb = DeviceBlockList.Find(x => x.Tunnel == wCSTasks0b[i].FmLocation &&
                        (x.SystemType == DeviceSystemType.OutboundBegin || x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.TotalPort || x.SystemType == DeviceSystemType.Picking) && x.Floor == to_blockb.Floor);

                        if (fm_blockb == null || to_blockb == null)
                        {
                            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasks0b[i], WCSTaskStatus.Unreachable);
                            continue;
                        }

                        if ("000"+ to_blockb.Position.Substring(2, 1) != wCSTasks0b[i].FmLocation)//确保出库的起始地址巷道和目的地的巷道一致（针对堆垛机搬运任务第三步）
                        {
                            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasks0b[i], WCSTaskStatus.Unreachable);
                            continue;
                        }
                        SSJMessage ssj_0b = new SSJMessage(PLCID);
                        if ((SSJWorkState == SSJDeviceWorkState.Online || SSJWorkState == SSJDeviceWorkState.Fault ) && !fm_blockb.CanNotWrite)
                        {
                            if (wCSTasks0b[i].TaskType == WCSTaskTypes.SSJOutbound)
                            {
                                ssj_0b.Set0bMessage(wCSTasks0b[i].PalletNum, fm_blockb.Position, wCSTasks0b[i].ToLocation, "0");
                            }
                            else if (wCSTasks0b[i].TaskType == WCSTaskTypes.SSJPickUpOutbound)
                            {
                                ssj_0b.Set0bMessage(wCSTasks0b[i].PalletNum, fm_blockb.Position, wCSTasks0b[i].ToLocation, "1");
                            }
                            InsertIntoSSJSendList(ssj_0b);
                            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasks0b[i], WCSTaskStatus.Doing);
                        }
                    }
                    for (int i = 0; i < wCSTasks0Y.Count; i++)
                    {
                        SSJDeviceBlock fm_blocky = DeviceBlockList.Find(x => x.Position == wCSTasks0Y[i].FmLocation);
                        if(fm_blocky!=null&& (fm_blocky.Position=="1125"|| fm_blocky.Position == "1115"))
                        {
                            SSJDeviceBlock fm_blocky_xx = DeviceBlockList.Find(x => x.Position == fm_blocky.Position+"XX");
                            fm_blocky.PalletNum = fm_blocky_xx.PalletNum;

                        }

                       // if (fm_blocky == null || (fm_blocky.PalletNum != wCSTasks0Y[i].PalletNum))
                        if (fm_blocky == null)
                        {                       
                            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasks0Y[i], WCSTaskStatus.Cannot);
                            continue;
                        }
                        if (fm_blocky.Position == "2116" || fm_blocky.Position == "2126" || fm_blocky.Position == "3116" || fm_blocky.Position == "3126")
                        {
                            string fm_blocky_apply_position = (int.Parse(fm_blocky.Position) + 2).ToString();
                            SSJDeviceBlock fm_blocky_apply = DeviceBlockList.Find(x => x.Position == fm_blocky_apply_position);
                            if (fm_blocky_apply.PalletNum != wCSTasks0Y[i].PalletNum)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasks0Y[i], WCSTaskStatus.Cannot);
                                continue;
                            }
                        }
                        else if (fm_blocky.Position == "2216" || fm_blocky.Position == "2226" || fm_blocky.Position == "3216" || fm_blocky.Position == "3226")
                        {
                            string fm_blocky_apply_position = (int.Parse(fm_blocky.Position) + 1).ToString();
                            SSJDeviceBlock fm_blocky_apply = DeviceBlockList.Find(x => x.Position == fm_blocky_apply_position);
                            if (fm_blocky_apply.PalletNum != wCSTasks0Y[i].PalletNum)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasks0Y[i], WCSTaskStatus.Cannot);
                                continue;
                            }
                        }
                        else
                        {
                            if (fm_blocky.PalletNum != wCSTasks0Y[i].PalletNum)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasks0Y[i], WCSTaskStatus.Cannot);
                                continue;
                            }
                        }
                        //  if(fm_blocky.Position == "2116" || fm_blocky.Position == "212sss6" || )

                        SSJDeviceBlock to_blocky;
                           if (fm_blocky.Floor==1 && (fm_blocky.SystemType == DeviceSystemType.Picking || fm_blocky.SystemType == DeviceSystemType.TotalPort))
                        {
                            to_blocky = DeviceBlockList.Find(x => x.Tunnel == wCSTasks0Y[i].ToLocation && (x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.TotalPort) && x.Floor == fm_blocky.Floor);
                        }
                        else
                        {
                            to_blocky = DeviceBlockList.Find(x => x.Tunnel == wCSTasks0Y[i].ToLocation && (x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.InboundFinish ) && x.Floor == fm_blocky.Floor);
                        }
                      //  List<SSJDeviceBlock> to_blockys = DeviceBlockList.FindAll(x => x.Tunnel == wCSTasks0Y[i].ToLocation && (x.SystemType == DeviceSystemType.InboundFinish || x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.TotalPort) && x.Floor == fm_blocky.Floor);
                        if (to_blocky == null && wCSTasks0Y[i].ToLocation != "0000"&& wCSTasks0Y[i].ToLocation.Substring(0,2)!="15"&& wCSTasks0Y[i].ToLocation.Substring(0, 2) != "14"&&wCSTasks0Y[i].ToLocation.Substring(0, 2) != "13")//确保是换层任务时，目的地可以找到
                        {
                            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasks0Y[i], WCSTaskStatus.Unreachable);
                            continue;
                        }
                        //if (SSJWorkState == SSJDeviceWorkState.Online || SSJWorkState == SSJDeviceWorkState.Fault || SSJWorkState == SSJDeviceWorkState.None)
                        if(SSJWorkState == SSJDeviceWorkState.Online || SSJWorkState == SSJDeviceWorkState.Fault)
                        { 
                            SSJMessage ssj_0y = new SSJMessage(PLCID);
                            if (wCSTasks0Y[i].ToLocation == "0000" || wCSTasks0Y[i].ToLocation.Substring(0, 2) == "15" || wCSTasks0Y[i].ToLocation.Substring(0, 2) == "14" || wCSTasks0Y[i].ToLocation.Substring(0, 2) == "13")
                            {
                                ssj_0y.Set0YMessage(wCSTasks0Y[i].PalletNum, fm_blocky.Position, wCSTasks0Y[i].ToLocation);
                                InsertIntoSSJSendList(ssj_0y);
                                if (wCSTasks0Y[i].ToLocation == "0000")
                                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasks0Y[i], WCSTaskStatus.Done);
                                else
                                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasks0Y[i], WCSTaskStatus.Doing);
                            }
                            else
                            {
                                ssj_0y.Set0YMessage(wCSTasks0Y[i].PalletNum, fm_blocky.Position, to_blocky.Position);
                                InsertIntoSSJSendList(ssj_0y);
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasks0Y[i], WCSTaskStatus.Doing);
                            }
                        }
                    }
                    Task.Delay(300).Wait();
                    
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("ProcessSSJTask ", ex);
                }
            }
        }
        public void AddCircle(SSJDeviceBlock sSJDeviceBlock, int i)
        {
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => (x.TaskType == WCSTaskTypes.SSJInbound || x.TaskType == WCSTaskTypes.SSJOutbound) && x.PalletNum == sSJDeviceBlock.PalletNum && x.TaskStatus == WCSTaskStatus.Doing);
            if (wCSTask != null)
            {
                DataItem item = new DataItem();
                item = MBInCircleDataItemList.Find(x => x.StartByteAdr == sSJDeviceBlock.InCircleMBAddr && x.BitAdr == sSJDeviceBlock.InCircleMBAddr2);
                if (item != null)
                {
                    WritePLCDataItem(item, 1);
                }
                if (i == 1)
                    InCircleNum++;
                else if (i == 2)
                    InCircleNum2++;
                else
                    InCircleNum3++;
                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.SSJInCircle);
            }
        }
        public void AddCircle(List<SSJDeviceBlock> sSJDeviceBlocks,int limitNum)
        {
            List<CircleQueue> circleQueues = CircleQueueManager.Instance.CircleQueueList.FindAll(x=>x.Locked== false).OrderBy(x=>x.CircleQueueTime).ToList();
            if (circleQueues == null || circleQueues.Count <= 0) return;
            if (InCircleNum < limitNum)
            {
                SSJDeviceBlock sSJDeviceBlockIncircle = sSJDeviceBlocks.Find(x => x.Position == circleQueues[0].Position);
                if (sSJDeviceBlockIncircle == null) return;
                DataItem item = new DataItem();
                item = MBInCircleDataItemList.Find(x => x.StartByteAdr == sSJDeviceBlockIncircle.InCircleMBAddr && x.BitAdr == sSJDeviceBlockIncircle.InCircleMBAddr2);
                if (item != null)
                {
                    WritePLCDataItem(item, 1);
                }
                InCircleNum++;
                WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == circleQueues[0].PalletNum && (x.TaskType == WCSTaskTypes.SSJInbound || x.TaskType == WCSTaskTypes.SSJOutbound) && x.TaskStatus == WCSTaskStatus.InCircleQueue);
                if (wCSTask != null)
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.SSJInCircle);
                
                List<CircleQueue> circleQueuesUnSameTaskType = CircleQueueManager.Instance.CircleQueueList.FindAll(x => x.TaskType != circleQueues[0].TaskType);
                List<CircleQueue> circleQueuesSameTaskType = CircleQueueManager.Instance.CircleQueueList.FindAll(x => x.TaskType == circleQueues[0].TaskType && x.Position != circleQueues[0].Position);
                if (circleQueuesUnSameTaskType != null&& circleQueuesUnSameTaskType.Count>0)  // 入库/出库任务进圈后，得先判断有没有出库/入库任务队列。如果有，把不同任务类型队列解锁，相同任务类型锁住；若无，则不需要锁住同类型任务，否则无法解锁
                {
                    if (circleQueuesSameTaskType != null&& circleQueuesSameTaskType.Count>0)  
                    {
                        for (int i = 0; i < circleQueuesSameTaskType.Count; i++)
                        {
                            CircleQueueManager.Instance.UpdateCircleQueueLocked(circleQueuesSameTaskType[i], true);
                        }
                    }
                    for (int i = 0; i < circleQueuesUnSameTaskType.Count; i++)
                    {
                        CircleQueueManager.Instance.UpdateCircleQueueLocked(circleQueuesUnSameTaskType[i], false);
                    }
                }
                CircleQueueManager.Instance.DeleteCircleQueueTask(circleQueues[0]);
            }

        }
        private void WriteToPLCFromDataItemList()
        {
            if (ModifyDataItemList != null && ModifyDataItemList.Count > 0)
            {
                for (int i = 0; i < ModifyDataItemList.Count; i++)
                {
                    WritePLCDataItem(ModifyDataItemList[i], ModifyDataItemList[i].Value);
                }

                lock (ModifyDataItemList)
                    ModifyDataItemList.Clear();
            }
        }

        public void PLCTOWCSMessageParse(string SSJReceMsg)
        {
            SSJMessage ssj_message = new SSJMessage(SSJReceMsg, PLCID, SSJMsgDirection.Receive);
            if (ssj_message == null) return;

            try
            {
                switch (ssj_message.MessageType)
                {
                    case "0A":
                        ssj_message.MsgParse = "输送机向WCS回复联机";
                        //if (SSJWorkState == SSJDeviceWorkState.None || SSJWorkState == SSJDeviceWorkState.Offline)
                        SSJ_0AParse(ssj_message);
                        break;
                    case "0B":
                        ssj_message.MsgParse = "输送机向WCS反馈脱机";
                        SSJ_0BParse(ssj_message);
                        break;
                    case "0X":
                        ssj_message.MsgParse = "输送机向WCS申请入库 托盘号" + ssj_message.PalletNum + ",起始地" + ssj_message.FmLocation;
                        SSJ_0XParse(ssj_message);
                        break;
                    case "0@":
                        SSJ_EEParse(ssj_message);
                        ssj_message.MsgParse = "输送机向WCS反馈故障 故障内容" + ssj_message.GetFaultContent();
                        break;
                    case "0H":
                        SSJ_0HParse(ssj_message);
                        ssj_message.MsgParse = "输送机向WCS反馈故障解除";
                        break;
                    case "0E":
                        ssj_message.MsgParse = "输送机向WCS回复入库完成 托盘号" + ssj_message.PalletNum + ",起始地" + ssj_message.FmLocation + ",目的地" + ssj_message.ToLocation;
                        SSJ_0EParse(ssj_message);
                        break;
                    case "0F":
                        ssj_message.MsgParse = "输送机向WCS回复出库完成 托盘号" + ssj_message.PalletNum + ",起始地" + ssj_message.FmLocation + ",目的地" + ssj_message.ToLocation;
                        SSJ_0FParse(ssj_message);
                        break;
                    case "0f":
                        SSJ_ffParse(ssj_message);
                        break;
                    case "0i":
                        ssj_message.MsgParse = "输送机向WCS反馈熄灭LED 托盘号" + ssj_message.PalletNum + ",目的地" + ssj_message.FmLocation;
                        SSJ_0iParse(ssj_message);
                        break;
                    case "0R":
                        ssj_message.MsgParse = "输送机向WCS反馈点亮LED 托盘号" + ssj_message.PalletNum + ",目的地" + ssj_message.FmLocation;
                        SSJ_0RParse(ssj_message);
                        break;
                    case "0Z":
                        ssj_message.MsgParse = "输送机向WCS反馈直出完成 托盘号" + ssj_message.PalletNum + ",目的地" + ssj_message.ToLocation;
                        SSJ_0ZParse(ssj_message);
                        break;
                    case "0P":
                        ssj_message.MsgParse = "输送机向WCS申请空托盘垛 托盘号" + ssj_message.PalletNum + ",目的地" + ssj_message.FmLocation;
                        SSJ_0PParse(ssj_message);
                        break;
                    case "0d":
                        ssj_message.MsgParse = "输送机向WCS发送允许AGV放货  托盘号" + ssj_message.PalletNum + ",目的地" + ssj_message.FmLocation;
                        SSJ_0dParse(ssj_message);
                        break;
                    default:
                        ssj_message.MsgParse = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("PLCTOWCSMessageParse " + PLCID, ex);
            }
            InsertIntoSSJMsgLog(ssj_message);
        }
        private bool InsertIntoSSJMsgLog(SSJMessage ssj_message)
        {
            if (ssj_message != null)
            {
                SqlParameter[] sqlParameters = new SqlParameter[] {
                new SqlParameter("@trans", ssj_message.Trans),
                new SqlParameter("@PlcId", ssj_message.PLCID),
                new SqlParameter("@Direction",ssj_message.Direction ),
                new SqlParameter("@MsgSeqID",ssj_message.MsgSeqID ),
                new SqlParameter("@Tkdat",ssj_message.Tkdat ),
                new SqlParameter("@MsgParse",ssj_message.MsgParse )
                };
                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[PLC_Message_Log]([PLCID],[Trans],[Direction],[Tkdat],[MsgSeqID],[MsgParse]) VALUES (@PlcId,@trans ,@Direction,@Tkdat,@MsgSeqID,@MsgParse)", sqlParameters);
            }
            return true;
        }
        public bool InsertIntoSSJSendList(SSJMessage ssj_message)
        {
            if (ssj_message != null)
            {
                WCSTOPLCDB4MessageList.Add(ssj_message);

                SqlParameter[] sqlParameters = new SqlParameter[] {
                    new SqlParameter("@trans", ssj_message.Trans),
                    new SqlParameter("@PlcId", ssj_message.PLCID),
                    new SqlParameter("@priority", ssj_message.SendPriority),
                    new SqlParameter("@MsgSeqID", ssj_message.MsgSeqID),
                    new SqlParameter("@Tkdat", ssj_message.Tkdat),
                    new SqlParameter("@MsgParse", ssj_message.MsgParse)
                };
                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[SSJ_Send_Buffer]([PLCID],[Trans],[SendPriority],[Tkdat],[MsgSeqID],[MsgParse]) VALUES (@PlcId,@trans ,@priority,@Tkdat,@MsgSeqID,@MsgParse)", sqlParameters);
            }
            return true;
        }
        public bool UpdateIntoSSJSendListDB(SSJMessage ssj_message)
        {
            if (ssj_message != null)
            {
                SqlParameter[] sqlParameters = new SqlParameter[] {
                    new SqlParameter("@trans", ssj_message.Trans),
                    new SqlParameter("@PlcId", ssj_message.PLCID),
                    new SqlParameter("@priority", ssj_message.SendPriority),
                    new SqlParameter("@MsgSeqID", ssj_message.MsgSeqID),
                    new SqlParameter("@Tkdat", ssj_message.Tkdat),
                    new SqlParameter("@MsgParse", ssj_message.MsgParse)
                };
                SQLServerHelper.ExeSQLStringWithParam("UPDATE [dbo].[SSJ_Send_Buffer]SET" +
                    " [PLCID] = @PLCID" +
                    ",[Trans] = @Trans" +
                    ",[SendPriority] = @priority" +
                    ",[Tkdat] = @Tkdat" +
                    ",[MsgParse] = @MsgParse" +
                    " WHERE [MsgSeqID] = @MsgSeqID ", sqlParameters);
            }
            return true;
        }
        private bool DeleteSSJSendList(SSJMessage ssj_message)
        {
            try
            {
                if (ssj_message != null)
                {
                    WCSTOPLCDB4MessageList.Remove(ssj_message);
                    SqlParameter[] sqlParameters = new SqlParameter[] {
                    new SqlParameter("@trans", ssj_message.Trans),
                    new SqlParameter("@PlcId", ssj_message.PLCID),
                    new SqlParameter("@priority", ssj_message.SendPriority),
                    new SqlParameter("@MsgSeqID", ssj_message.MsgSeqID),
                    new SqlParameter("@Tkdat", ssj_message.Tkdat),
                    new SqlParameter("@MsgParse", ssj_message.MsgParse)
                    };
                    SQLServerHelper.ExeSQLStringWithParam("DELETE [dbo].[SSJ_Send_Buffer] WHERE MsgSeqID = @MsgSeqID", sqlParameters);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("删除输送机发送信息异常 DeleteSSJSendBuffer，" + PLCID + " " + ex.ToString());
            }
            return true;
        }
        #region 设备操作--上线--脱机--复位--急停
        public void Online()
        {
            if (PLCConnectState == ConnectionStates.Connected)
            {
                SSJMessage ssj_0l = new SSJMessage(PLCID);
                ssj_0l.Set0LMessage();
                _ = InsertIntoSSJSendList(ssj_0l);
                SSJWorkState = SSJDeviceWorkState.Online;
            }
        }
        public bool GetAllowUnload(string pLCID)
        {
            SSJDeviceBlock sSJDeviceBlock = DeviceBlockList.Find(x => (x.SystemType == DeviceSystemType.OutboundBegin || x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin) && x.DDJID == pLCID);
            if (sSJDeviceBlock == null) return false;
            return sSJDeviceBlock.IsLoaded;
        }

        public void OffLine()
        {
            if (PLCConnectState == ConnectionStates.Connected)
            {
                SSJMessage ssj_0B = new SSJMessage(PLCID);
                ssj_0B.Set0BMessage();
                _ = InsertIntoSSJSendList(ssj_0B);
                SSJWorkState = SSJDeviceWorkState.Offline;
            }
        }

        public void DB5ClearZero()
        {
            if (PLCConnectState == ConnectionStates.Connected)
            {
                SSJMessage ssj_00db5 = new SSJMessage(PLCID);
                ssj_00db5.SetClearDB5Message();
                PLCTOWCSDB5MessageList.Add(ssj_00db5);
            }
        }

        public void DB4ClearZero_01()//db4清零置01
        {
            string trans = "00000000000000000000000000000000000010";
            if (WritePLCDataItem(DB45DataItemList.Find(x => x.DB == 4), trans))
            {
              int a=3;
            }
        }

        public void DB4ClearZero()
        {
            if (PLCConnectState == ConnectionStates.Connected)
            {
                SSJMessage ssj_00db4 = new SSJMessage(PLCID);
                ssj_00db4.SetClearDB4Message();
                _ = InsertIntoSSJSendList(ssj_00db4);
            }
        }

        public void RecallInPlace()
        {
            if (PLCConnectState == ConnectionStates.Connected && SSJWorkState == SSJDeviceWorkState.Online)
            {
                SSJMessage ssj_0d = new SSJMessage(PLCID);
                ssj_0d.Set0AMessage();
                _ = InsertIntoSSJSendList(ssj_0d);
            }
        }
        public void ClearOcupty(string palletNum)
        {
            SSJDeviceBlock sSJDeviceBlock = DeviceBlockList.Find(x => x.PalletNum == palletNum && x.IsOccupied && (x.SystemType == DeviceSystemType.InboundFinish || x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.OutboundFinish || x.SystemType == DeviceSystemType.Picking || x.AGVGetPoint || x.SystemType == DeviceSystemType.TotalPort));    //|| x.AGVSetPoint
            if (sSJDeviceBlock == null) return;
            SSJMessage sSJMessage = new SSJMessage(PLCID);
            sSJMessage.Set0QMessage(palletNum, sSJDeviceBlock.Position);
            InsertIntoSSJSendList(sSJMessage);
        }
        public void ApplyPut(string palletNum,string position)
        {
            SSJMessage sSJMessage = new SSJMessage(PLCID);
            sSJMessage.Set0dMessage(palletNum, position);
            InsertIntoSSJSendList(sSJMessage);
        }
        public void PutDone(string palletNum, string position)
        {
            SSJMessage sSJMessage = new SSJMessage(PLCID);
            sSJMessage.Set0aMessage(palletNum, position);
            InsertIntoSSJSendList(sSJMessage);
        }
        public void GetDone(string palletNum, string position)
        {
            SSJMessage sSJMessage = new SSJMessage(PLCID);
            sSJMessage.Set0qMessage(palletNum, position);
            InsertIntoSSJSendList(sSJMessage);
        }
        private string GetStationPosition(string to_location)
        {
            string stationPosition = "";
            if (to_location.Substring(0, 2) == "44")
                stationPosition = "3292";
            else if (to_location.Substring(0, 2) == "43")
                stationPosition = "3293";
            else if (to_location.Substring(0, 2) == "42")
                stationPosition = "3294";
            else if (to_location.Substring(0, 2) == "41")
                stationPosition = "3295";
            else
                stationPosition = to_location;
            return stationPosition;
        }
        #endregion
        private void CreateTagListFromDB()
        {
            if (DataItemList == null)
                DataItemList = new List<DataItem>();

            DataTable dt = SQLServerHelper.DataBaseReadToTable(string.Format("SELECT [PLCID],[Name],[DataType],[VarType],[DB],[StartByteAdr],[BitAdr],[Count] FROM[dbo].[Tag_Table] where PLCID = '{0}'", PLCID));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataItem data_item = new DataItem();
                data_item.DataType = (DataType)Enum.Parse(typeof(DataType), dt.Rows[i]["DataType"].ToString());
                data_item.VarType = (VarType)Enum.Parse(typeof(VarType), dt.Rows[i]["VarType"].ToString());
                //data_item.Name = dt.Rows[i]["Name"].ToString();
                data_item.DB = (int)dt.Rows[i]["DB"];
                data_item.StartByteAdr = (int)dt.Rows[i]["StartByteAdr"];
                data_item.Count = (int)dt.Rows[i]["Count"];
                data_item.BitAdr = (byte)dt.Rows[i]["BitAdr"];
                DataItemList.Add(data_item);
            }
        }
        private void GetTagListFromConfig()
        {
            if (File.Exists(Environment.CurrentDirectory + "\\Configure" + "\\SSJ_Block.config"))
            {
                try
                {
                    XElement xDoc = XElement.Load(Environment.CurrentDirectory + "\\Configure" + "\\SSJ_Block.config");
                    var item = (from ele in xDoc.Elements("DeviceData")
                                where ele.Attribute("PLCID").Value == PLCID
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
                            if (dNode.Attribute("Name").Value.Contains("Door"))
                                data_item.BitAdr = (byte)int.Parse(dNode.Attribute("BitAdr").Value);
                            data_item.Value = dNode.Attribute("DefaultValue").Value;
                            DataItemList.Add(data_item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("输送机配置文件读取异常", ex);
                }
            }
        }
        private DataItem GetDataItmeFromDBConfig(XElement item, string var_name)
        {
            DataItem data_item = new DataItem();
            var item_db = (from ele in item.Elements("Data")
                           where ele.Attribute("Name").Value == var_name
                           select ele).SingleOrDefault();

            data_item.DataType = (DataType)Enum.Parse(typeof(DataType), item_db.Attribute("DataType").Value);
            data_item.VarType = (VarType)Enum.Parse(typeof(VarType), item_db.Attribute("VarType").Value);
            data_item.DB = int.Parse(item_db.Attribute("Db").Value);
            data_item.StartByteAdr = int.Parse(item_db.Attribute("StartByteAdr").Value);
            data_item.Count = int.Parse(item_db.Attribute("Count").Value);
            data_item.Value = item_db.Attribute("DefaultValue").Value;
            return data_item;
        }
        public string GetVarName<T>(System.Linq.Expressions.Expression<Func<T, T>> exp)
        {
            return ((System.Linq.Expressions.MemberExpression)exp.Body).Member.Name;
        }
        public void UpdateDeviceState(SSJDeviceWorkState device_state)
        {
            SSJWorkState = device_state;
        }
        public void UpdatePLCConnectState(ConnectionStates connectionState)
        {
            PLCConnectState = connectionState;
        }
        private string ModifyCheckChar(string trans)
        {
            return trans.Substring(0, 30) + 1;
        }

        #region 接收PLC信息处理
        private void SSJ_0AParse(SSJMessage ssj_0A)
        {
            if (ssj_0A == null) return;
            SSJMessage ssjmsg = new SSJMessage(PLCID);
            ssjmsg.Set0AMessage();
            InsertIntoSSJSendList(ssjmsg);
        }
        private void SSJ_0BParse(SSJMessage ssj_0B)
        {
            if (ssj_0B == null) return;
            SSJWorkState = SSJDeviceWorkState.Offline;
        }
        private void SSJ_0HParse(SSJMessage ssj_0h)
        {
            if (ssj_0h.GetErrorDeviceID() == "0000")
            {
                foreach (SSJDeviceBlock ssj_device_block in DeviceBlockList.FindAll(x => x.ErrorCode != "0000" && PLCID == PLCID))
                {
                    ssj_device_block.IsFaulty = false;
                    ssj_device_block.ErrorCode = "";
                    ssj_device_block.FaultContent1 = "";
                }
            }
            else
            {
                SSJDeviceBlock ssj_device_block = DeviceBlockList.Find(x => x.Position == ssj_0h.GetErrorDeviceID() && PLCID == PLCID);
                if (ssj_device_block != null)
                {
                    ssj_device_block.IsFaulty = false;
                    ssj_device_block.ErrorCode = "";
                    ssj_device_block.FaultContent1 = "";
                }
            }
            if (DeviceBlockList.FindAll(x => x.IsFaulty && PLCID == PLCID).Count < 1)
            {
                FaultCode = "";
                FaultContent = "";
                SSJWorkState = SSJDeviceWorkState.Online;
            }
        }
        private void SSJ_0XParse(SSJMessage ssj_0X)
        {
            if (ssj_0X == null) return;
            string pallet_num = ssj_0X.GetPalletNum(ssj_0X.MessageType);
            int len = pallet_num.Length;
            string gao_di;
            gao_di = ssj_0X.GaoDi;
            if (string.IsNullOrEmpty(pallet_num) || pallet_num.Contains("ERROR") || pallet_num.Contains("\0")) //如果0x电报发过来的托盘号错误可能是/0，插入无任务退回的托盘号 000000000000
            {
                SSJMessage ssjmsg = new SSJMessage(PLCID);
                ssjmsg.Set0YMessage("000000000000", ssj_0X.GetFmLocation(), "0000");
                InsertIntoSSJSendList(ssjmsg);
                return;
            }
            SSJDeviceBlock sSJDeviceBlock = DeviceBlockList.Find(x => x.Position == ssj_0X.FmLocation);
            if (sSJDeviceBlock == null) return;
            // 一号提升机自动出库逻辑
            if (WMSTaskCreateUC.Instance.isAutoOutbound1Enabled &&
                (sSJDeviceBlock.Position == "1310" || sSJDeviceBlock.Position == "1320" || sSJDeviceBlock.Position == "1330"))
            {

                // 创建并发送0Y电报
                SSJMessage ssj_y = new SSJMessage(PLCID);
                string toLocation = GetToPosition(PLCID,int.Parse(WMSTaskCreateUC.Instance.selectedOutboundFloor1));
                ssj_y.Set0YMessage(ssj_0X.PalletNum, sSJDeviceBlock.Position, toLocation);
                InsertIntoSSJSendList(ssj_y);
                return;
            }
            if (WMSTaskCreateUC.Instance.isAutoOutbound2Enabled &&
                (sSJDeviceBlock.Position == "1410" || sSJDeviceBlock.Position == "1420" || sSJDeviceBlock.Position == "1430"))
            {
                // 创建并发送0Y电报
                SSJMessage ssj_y = new SSJMessage(PLCID);
                string toLocation = GetToPosition(PLCID, int.Parse(WMSTaskCreateUC.Instance.selectedOutboundFloor2));
                ssj_y.Set0YMessage(ssj_0X.PalletNum, sSJDeviceBlock.Position, toLocation);
                InsertIntoSSJSendList(ssj_y);
                return;
            }
            if (WMSTaskCreateUC.Instance.isAutoOutbound3Enabled &&
    (sSJDeviceBlock.Position == "1510" || sSJDeviceBlock.Position == "1520" || sSJDeviceBlock.Position == "1530"))
            {
                // 创建并发送0Y电报
                SSJMessage ssj_y = new SSJMessage(PLCID);
                string toLocation = GetToPosition(PLCID, int.Parse(WMSTaskCreateUC.Instance.selectedOutboundFloor3));
                ssj_y.Set0YMessage(ssj_0X.PalletNum, sSJDeviceBlock.Position, toLocation);
                InsertIntoSSJSendList(ssj_y);
                return;
            }

            WMSTask wMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == ssj_0X.PalletNum);
            if (wMSTask == null)
            {
                //SSJDeviceBlock sSJDeviceBlock = DeviceBlockList.Find(x => x.Position == ssj_0X.FmLocation);
                //if (sSJDeviceBlock == null) return;
                wMSTask = new WMSTask();
                wMSTask.TaskType = WMSTaskType.Inbound;
                wMSTask.TaskStatus = WMSTaskStatus.SSJ_APP_IN;
                wMSTask.PalletNum = ssj_0X.PalletNum;
                wMSTask.FmLocation = sSJDeviceBlock.Position;
                wMSTask.TaskSource = "WMS";
                wMSTask.Warehouse = sSJDeviceBlock.WareHouse;
                wMSTask.WeightNum = ssj_0X.WeightNum;
                if(sSJDeviceBlock.WareHouse == "1503")
                {
                    wMSTask.GaoDiBZ = WMSGaoDiBZ.Height;
                }
                else
                {
                    if (gao_di == "0")
                        wMSTask.GaoDiBZ = WMSGaoDiBZ.Low;
                    //else if (gao_di == "1")
                    //    wMSTask.GaoDiBZ = WMSGaoDiBZ.Middle;
                    else
                        wMSTask.GaoDiBZ = WMSGaoDiBZ.Height;
                }
                    
                //wMSTask.Direction = direction;
                wMSTask.Floor = sSJDeviceBlock.Floor;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
            //else if (wMSTask.TaskSource == WMSTask.WMS_Task_Source_WCS)
            else
            {
                //SSJDeviceBlock sSJDeviceBlock = DeviceBlockList.Find(x => x.Position == ssj_0X.FmLocation);
                if (sSJDeviceBlock == null) return;
                WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == ssj_0X.PalletNum && (x.TaskType == WCSTaskTypes.SSJInbound || x.TaskType == WCSTaskTypes.DDJDirect));
                if (wCSTask != null && wCSTask.ToLocation != ssj_0X.FmLocation)
                {
                    WCSTaskManager.Instance.UpdateWCSTaskFmLocation(wCSTask, ssj_0X.FmLocation);
                    WCSTaskManager.Instance.UpdateWCSTaskFloor(wCSTask, sSJDeviceBlock.Floor);
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Waiting);
                }
                else
                {  //针对无任务未自动删除，输送机再次入库申请的情况
                    WMSTask noTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == ssj_0X.PalletNum && x.TaskType == WMSTaskType.NoTaskQuit);
                    if (noTask != null && noTask.ToLocation == "0000")
                    {
                        WMSTasksManager.Instance.UpdateWMSTaskStatus(noTask, WMSTaskStatus.TaskAssigned);
                    }
                    else//针对扫码器扫到新托盘的新条码，但是发送的是上一个已经执行上架任务的托盘条码，应该将托盘执行无任务退回
                    {
                        //不创建新的输送机申请入库任务、超时也会自动退回
                    }
                }
            }
        }


        public string GetToPosition(string plcId, int floor)
        {
            // 定义查询语句，使用参数化查询
            string sqlstr = "SELECT Position FROM dbo.ConveyerBlocks WHERE PLCID = @plcId AND Floor = @floor";

            // 定义参数
            var parameters = new List<SqlParameter>
        {
            new SqlParameter("@plcId", plcId),
            new SqlParameter("@floor", floor)
        };

            // 执行查询
            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr, parameters);

            // 检查查询结果
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return "";

            return obj.ToString();
        }

        private void SSJ_EEParse(SSJMessage ssj_ee)
        {
            if (ssj_ee == null) return;
            SSJWorkState = SSJDeviceWorkState.Fault;
            SSJDeviceBlock ssj_device_block = DeviceBlockList.Find(x => x.Position == ssj_ee.GetErrorDeviceID() && x.PLCID == PLCID);
            if (ssj_device_block == null) return;
            ssj_device_block.IsFaulty = true;
            ssj_device_block.ErrorCode = ssj_ee.GetErrorCode();
            ssj_device_block.FaultContent1 = ssj_ee.GetFaultContent();
            FaultCode = ssj_ee.GetErrorCode();
            FaultContent = ssj_ee.GetFaultContent();
        }
        private void SSJ_0EParse(SSJMessage ssj_0e)
        {
            if (ssj_0e == null) return;
            SSJMessage ssjmsg = new SSJMessage(PLCID);
            //ssjmsg.Set0EMessage(ssj_0e.Trans);
            //InsertIntoSSJSendList(ssjmsg);


            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == ssj_0e.PalletNum && x.DeviceID == PLCID && x.TaskType == WCSTaskTypes.SSJInbound);
            if (wCSTask == null) return;
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Done);
        }
        private void SSJ_0FParse(SSJMessage ssj_0f)
        {
            if (ssj_0f == null) return;
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == ssj_0f.PalletNum && x.DeviceID == PLCID && (x.TaskType == WCSTaskTypes.SSJOutbound || x.TaskType == WCSTaskTypes.DDJDirect || x.TaskType == WCSTaskTypes.SSJPickUpOutbound));
            if (wCSTask == null) return;
            SSJDeviceBlock sSJDeviceBlock = DeviceBlockList.Find(x => x.Position == ssj_0f.FmLocation);
            if (sSJDeviceBlock == null) return;
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Done);
            //由于输送机先发0F出库完成，任务消失后无法解析or电报
            WMSTask wMSTaskLed = new WMSTask();
            wMSTaskLed.TaskType = WMSTaskType.LedDisplay;
            wMSTaskLed.Warehouse = sSJDeviceBlock.WareHouse;
            wMSTaskLed.TaskSource = "WMS";
            wMSTaskLed.PalletNum = ssj_0f.PalletNum;
            wMSTaskLed.TaskStatus = WMSTaskStatus.LightLED;
            wMSTaskLed.FmLocation = ssj_0f.FmLocation;
            wMSTaskLed.ToLocation = ssj_0f.ToLocation;//ssj发的起始地址
            WMSTasksManager.Instance.AddWMSTask(wMSTaskLed);
        }
        
        private void SSJ_ffParse(SSJMessage ssj_ff)
        {
            if (ssj_ff == null) return;
        }
        private void SSJ_0iParse(SSJMessage ssj_0i)
        {
            if (ssj_0i == null) return;
            DeleteIntoLogLed(ssj_0i.ToLocation);
            SSJDeviceBlock to_blockb = DeviceBlockList.Find(x => x.Position == ssj_0i.FmLocation);//输送机给我发的起始地址
            if (to_blockb == null) return;
            WMSTask wMSTask = new WMSTask();
            wMSTask.Warehouse = to_blockb.WareHouse;
            wMSTask.TaskType = WMSTaskType.LedDisplay;
            wMSTask.TaskSource = "WMS";
            wMSTask.PalletNum = ssj_0i.GetPalletNum(ssj_0i.MessageType);
            wMSTask.TaskStatus = WMSTaskStatus.UnLightLED;
            wMSTask.ToLocation = ssj_0i.FmLocation;
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
        }
        private void SSJ_0RParse(SSJMessage ssj_0r)
        {
            if (ssj_0r == null) return;
            WMSTask wMSTask1 = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == ssj_0r.PalletNum && (x.TaskType == WMSTaskType.Outbound || x.TaskType == WMSTaskType.Picking)&&x.TaskSource=="WMS");
            if (wMSTask1 == null) return;
            WMSTask wMSTask2 = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == ssj_0r.PalletNum && x.TaskType==WMSTaskType.LedDisplay && x.TaskSource == "WMS");//判断是否重复,如果重复不新建点亮Led任务
            if (wMSTask1 != null) return;
            //InsertIntoLogLed(wMSTask1.LedMessage, ssj_0r.ToLocation);
            SSJDeviceBlock to_blockb = DeviceBlockList.Find(x => x.Position == ssj_0r.FmLocation);
            if (to_blockb == null) return;
            //为防止不报0F，加上如下代码
            //WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == ssj_0r.PalletNum && x.DeviceID == PLCID && (x.TaskType == WCSTaskTypes.SSJOutbound || x.TaskType == WCSTaskTypes.DDJDirect || x.TaskType == WCSTaskTypes.SSJPickUpOutbound));
            //if (wCSTask == null) return;
            //WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Done);
            /////
            WMSTask wMSTask = new WMSTask();
            wMSTask.TaskType = WMSTaskType.LedDisplay;
            wMSTask.Warehouse = to_blockb.WareHouse;
            wMSTask.TaskSource = "WMS";
            wMSTask.PalletNum = ssj_0r.GetPalletNum(ssj_0r.MessageType);
            wMSTask.TaskStatus = WMSTaskStatus.LightLED;
            wMSTask.FmLocation = wMSTask1.FmLocation;
            wMSTask.ToLocation = ssj_0r.FmLocation;//ssj发的起始地址
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
        }
        /// <summary>
        /// 输送机允许卸货
        /// </summary>
        /// <param name="ssj_0d"></param>
        private void SSJ_0dParse(SSJMessage ssj_0d)
        {
            if (ssj_0d == null) return;
            AgvPosition agvPositionTo = AgvManager.Instance.AgvPositionList.Find(x => x.SSJPositon == ssj_0d.FmLocation);
            if (agvPositionTo == null) return;
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == WCSTaskManager.AGVID && x.ToLocation == agvPositionTo.Position);
            if (wCSTask == null) return;
            WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.AGVAllowPut);
        }
        private void SSJ_0PParse(SSJMessage ssj_message)
        {
            SSJDeviceBlock sSJDeviceBlock = DeviceBlockList.Find(x => x.Position == ssj_message.FmLocation);
            if (sSJDeviceBlock == null) return;
            if (sSJDeviceBlock.IsLoaded)
            {
                LogHelper.WriteLog(ssj_message.FmLocation + "申请空托盘垛异常 " + PLCID + " sSJDeviceBlock.IsLoaded" + sSJDeviceBlock.IsLoaded);
                return;
            }
            WMSTask wMSTask = new WMSTask();
            wMSTask.TaskStatus = WMSTaskStatus.SSJ_APP_EM;
            wMSTask.FmLocation = sSJDeviceBlock.Position;
            wMSTask.TaskSource = "WMS";
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
        }
        private void SSJ_0ZParse(SSJMessage ssj_message)
        {
            SSJDeviceBlock sSJDeviceBlock = DeviceBlockList.Find(x => x.Position == ssj_message.FmLocation);
            if (sSJDeviceBlock == null) return;
        }
        public void InsertIntoLogLed(string ledMessage,string port)
        {
            if (ledMessage == null) return;
            string[] message = ledMessage.Split(',');
            if (message.Length == 10)
            {
                try
                {
                    //"comment": "OUBTJ20231206000043,整托下架,311,冷冻带骨绵羊腿,2023-10-03,P323111210,豫GF5067,公司,35箱,HLBU9253663 "
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                    new SqlParameter("@Palno",message[5]),
                    new SqlParameter("@Matno",message[0]),
                    new SqlParameter("@stype",message[1]),
                    new SqlParameter("@Addre",message[3]),
                    new SqlParameter("@mname",message[8]),
                    new SqlParameter("@batch",message[2]),
                    new SqlParameter("@quant0",message[7]),
                    new SqlParameter("@quant",message[7]),
                    new SqlParameter("@to_addre",message[6]),
                    new SqlParameter("@Doors",port),
                    };
                    SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[log_Led] " +
                        "([Palno],[Matno],[stype],[Addre],[mname],[batch],[quant0],[quant],[to_addre],[Doors])VALUES" +
                        "(@Palno, @Matno , @stype ,@Addre,@mname,@batch,@quant0,@quant,@to_addre,@Doors)", sqlParameters);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("InserIntolog_Led ", ex);
                }
            }
        }
        public void DeleteIntoLogLed(string port)
        {
            if (port == null) return;
            try
            {
                //"comment": "OUBTJ20231206000043,整托下架,311,冷冻带骨绵羊腿,2023-10-03,P323111210,豫GF5067,,35箱,HLBU9253663 ",
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                new SqlParameter("@Doors",port),
                };
                SQLServerHelper.ExeSQLStringWithParam("DELETE [dbo].[log_Led] WHERE Doors = @Doors", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DELETElog_Led ", ex);
            }
            
        }
        #endregion
        public void SetAgvOpt2070(bool v)
        {
            DataItem dataItem = DataItem.FromAddress("M73.7");
            if (dataItem == null) return;
            dataItem.VarType = VarType.Bit;
            dataItem.Value = v;
            WriteDataItemToPLC(dataItem);
        }
        public void SetAgvOpt2103(bool v)
        {
            DataItem dataItem = DataItem.FromAddress("M102.7");
            if (dataItem == null) return;
            dataItem.VarType = VarType.Bit;
            dataItem.Value = v;
            WriteDataItemToPLC(dataItem);
        }
    }
    public enum SSJDeviceWorkState
    {
        None = 0,
        Online = 1,
        Standby = 2,
        Working = 3,
        Fault = 4,
        Offline = 5,
        Manual = 6,
    }
}
