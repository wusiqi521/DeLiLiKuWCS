using BMHRI.WCS.Server.Tools;
using S7.Net.Types;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using BMHRI.WCS.Server.DDJProtocol;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Windows;

namespace BMHRI.WCS.Server.Models
{
    public class DeMaticDDJManager : INotifyPropertyChanged
    {
        private static readonly Lazy<DeMaticDDJManager> lazy = new Lazy<DeMaticDDJManager>(() => new DeMaticDDJManager());
        public static DeMaticDDJManager Instance { get { return lazy.Value; } }
        public static string PLCConfigfilePathName = Environment.CurrentDirectory + "\\Configure" + "\\DEMATIC.config";
        public event PropertyChangedEventHandler? PropertyChanged;
        
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public List<DeMaticDDJ> DeMaticDDJList;
        private DeMaticDDJManager()
        {
            CreateDeMaticDDJList();
        }
        public void CreateDeMaticDDJList()
        {
            DeMaticDDJList = new List<DeMaticDDJ>();
            if (File.Exists(PLCConfigfilePathName))
            {
                try
                {
                    XDocument xDocument = XDocument.Load(PLCConfigfilePathName);
                    foreach (XElement item in xDocument.Root.Descendants("DDJ"))
                    {
                        DeMaticDDJ deMaticDDJ = new DeMaticDDJ(item.Element("CpuType").Value, item.Element("IP").Value, item.Element("Port").Value, item.Element("Slot").Value, item.Element("Rack").Value, item.Element("Decription").Value, "DDJ", item.Element("PLCID").Value, item.Element("Available").Value);

                        if (deMaticDDJ != null)
                        {
                            DeMaticDDJList.Add(deMaticDDJ);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("堆垛机配置文件读取异常", ex);
                }
            }
        }
    }

    public class DeMaticDDJ : PLCDevice
    {
        public List<DematicTask> DDJTaskList;
        public event EventHandler<DematicTaskEventArgs> DDJTaskChanged;
        public event EventHandler<DematicTaskEventArgs> DDJTaskAdded;
        public event EventHandler<DematicTaskEventArgs> DDJTaskDeleted;
        #region  DDJ状态属性
        private string tunnel;
        public string Tunnel
        {
            get
            {
                if (string.IsNullOrEmpty(tunnel))
                {
                    if (!string.IsNullOrEmpty(PLCID))
                        tunnel = string.Concat("000", PLCID.AsSpan(4, 1));
                }
                return tunnel;
            }
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
        //private bool dDJBusy;
        //public bool DDJBusy
        //{
        //    get { return dDJBusy; }
        //    set
        //    {
        //        if (dDJBusy != value)
        //        {
        //            dDJBusy = value;
        //            Notify(nameof(DDJBusy));
        //        }
        //    }
        //}
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
        private int motionPosition;
        public int MotionPosition
        {
            get { return motionPosition; }
            set
            {
                if (motionPosition != value)
                {
                    motionPosition = value;
                    Notify(nameof(MotionPosition));
                }
            }
        }
        private int liftingPosition;
        public int LiftingPosition
        {
            get { return liftingPosition; }
            set
            {
                if (liftingPosition != value)
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
                if (forkPosition != value)
                {
                    forkPosition = value;
                    Notify(nameof(ForkPosition));
                }
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
                }
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
        private string wcs_to_dematic;
        public string WCSTODeMatic
        {
            get { return wcs_to_dematic; }
            set
            {
                if (wcs_to_dematic != value)
                {
                    wcs_to_dematic = value;
                    Notify(nameof(WCSTODeMatic));
                }
            }
        }
        private string dematic_to_wcs;
        public string DeMaticTOWCS
        {
            get { return dematic_to_wcs; }
            set
            {
                if (dematic_to_wcs != value)
                {
                    dematic_to_wcs = value;
                    Notify(nameof(DeMaticTOWCS));
                }
            }
        }
        #endregion
        public TcpClient WCSTcpClient;
        private System.Threading.Timer SendLiveToDEMATICTimer;
        public List<DataItem> DDJStatusDataItemList;
        private NetworkStream networkStream = null;
        int PortNum;
        public readonly string[] leftPai = new string[] { "01", "02", "05", "06", "09", "10", "13", "14", "17", "18", "21", "22", "25", "26", "29", "30", "33", "34" };
        public readonly string[] rightPai = new string[] { "03", "04", "07", "08", "11", "12", "15", "16", "19", "20", "23", "24", "27", "28", "31", "32", "35", "36" };
        public readonly string[] clearPai = new string[] { "02", "03", "06", "07", "10", "11", "14", "15", "18", "19", "22", "23", "26", "27", "30", "31", "34", "35" };
        public readonly string[] farPai = new string[] { "01", "04", "05", "08", "09", "12", "13", "16", "17", "20", "21", "24", "25", "28", "29", "32", "33", "36" };
        public readonly string[] ClearLocationSmallPai = new string[] { "02", "06", "10", "14", "18", "22", "26", "30", "34"};
        public readonly string[] ClearLocationBigPai = new string[] { "03", "07", "11", "15", "19", "23", "27", "31", "35"};
        public readonly string[] FarLocationSmallPai = new string[] { "01", "05", "09", "13", "17", "21", "25", "29", "33"};
        public readonly string[] FarLocationBigPai = new string[] { "04", "08", "12", "16", "20", "24", "28", "32", "36"};

        #region 更新WMS堆垛机状态
        private void UpdateWMSDBDDJStatus()
        {
            WMSTask wMSTask;
            if (!Available || DDJWorkState == DDJDeviceWorkState.Offline||DDJWorkState== DDJDeviceWorkState.NoMode||DDJWorkState== DDJDeviceWorkState.None)     //!WCSEnable
            {
                wMSTask = new WMSTask();
                wMSTask.PalletNum = PLCID.Substring(4, 1);
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.TaskSource = "WMS";
                wMSTask.ToLocation = "500";    //B
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTask wMSTask1 = WMSTasksManager.Instance.WMSTaskList.Find(x => x.CreateTime == wMSTask.CreateTime);
                if (wMSTask1 != null)
                {
                    wMSTask.CreateTime = System.DateTime.Now.AddMilliseconds(10).ToString("yyyy-MM-dd HH:mm:ss.ffff");
                }
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
            else if (DDJWorkState == DDJDeviceWorkState.Standby)   //&& WCSEnable
            {
                wMSTask = new WMSTask();
                wMSTask.PalletNum = PLCID.Substring(4, 1);
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.TaskSource = "WMS";
                wMSTask.ToLocation = "100";    //D
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTask wMSTask1 = WMSTasksManager.Instance.WMSTaskList.Find(x => x.CreateTime == wMSTask.CreateTime);
                if (wMSTask1 != null)
                {
                    wMSTask.CreateTime = System.DateTime.Now.AddMilliseconds(10).ToString("yyyy-MM-dd HH:mm:ss.ffff");
                }
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
            else if (DDJWorkState == DDJDeviceWorkState.Fault)
            {
                wMSTask = new WMSTask();
                wMSTask.PalletNum = PLCID.Substring(4, 1);
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.TaskSource = "WMS";
                wMSTask.ToLocation = "999";    //@
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
        }
        public void AvailabilityBT_Click(string PLCID,string status)
        {
            string receiver = "UL" + PLCID.Substring(3, 2);
            DeMaticDDJ deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == PLCID);
            if (deMaticDDJ != null)
            {
                //deMaticDDJ.DDJWorkState = DeMaticDDJ.DDJDeviceWorkState.Standby;
                WMSTask wMSTask;
                wMSTask = new WMSTask();
                wMSTask.PalletNum = PLCID.Substring(4, 1);
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.TaskSource = "WMS";
                wMSTask.ToLocation = "100";
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
        }

        public void UnableBT_Click(string PLCID, string status)
        {
            WMSTask wMSTask;
            wMSTask = new WMSTask();
            wMSTask.PalletNum = PLCID.Substring(4, 1);
            wMSTask.TaskType = WMSTaskType.DeviceMsg;
            wMSTask.TaskSource = "WMS";
            wMSTask.ToLocation = "500";
            wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
        }
        #endregion
        public DeMaticDDJ(string cpuType, string Ip, string Port, string slot, string rack, string description, string device_type, string plc_id, string avi) : base(cpuType, Ip, slot, rack, description, device_type, plc_id)
        {
            PLCDecription = description;
            PLCID = plc_id;
            Available = avi == "1";
            WCSEnable = true;
            ReadDEMATICDataConfig();
            if (int.TryParse(Port, out int port_num))
            {
                PortNum = port_num;
            }
            //DEMATICConnect();
            //Plc.ReadMultipleVars(DDJStatusDataItemList);
            //ProceDDJStatus();

            //SendLiveToDEMATICTimer = new System.Threading.Timer(new TimerCallback(SendLiveToDEMATIC), null, 30000, 30000);
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(30000);
                SendLiveToDEMATIC();
            });
            Task.Factory.StartNew(() => Receive(), TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => WCSTaskProcess(PLCID), TaskCreationOptions.LongRunning);
        }
        private void ReadDEMATICDataConfig()
        {
            if (DDJStatusDataItemList == null) DDJStatusDataItemList = new List<DataItem>();
            if (File.Exists(Environment.CurrentDirectory + "\\Configure" + "\\DEMATIC_Data.config"))
            {
                try
                {
                    XElement xDoc = XElement.Load(Environment.CurrentDirectory + "\\Configure" + "\\DEMATIC_Data.config");
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
                            if (dNode.Attribute("Name").Value.Contains("HasPallet"))
                                data_item.BitAdr = (byte)int.Parse(dNode.Attribute("BitAdr").Value);
                            if (data_item != null)
                                DDJStatusDataItemList.Add(data_item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("DEMATIC_Data配置文件读取异常", ex);
                }
            }
        }
        public void ConnectToDematicTcpServer()
        {
            try
            {
                if (WCSTcpClient != null)
                {
                    WCSTcpClient.Close();
                    WCSTcpClient.Dispose();
                }
                DEMATICConnect();
                //// 设置连接超时时间
                //WCSTcpClient = new TcpClient();
                //IPAddress ip = IPAddress.Parse(IP);
                //IPEndPoint point = new IPEndPoint(ip, PortNum);
                //WCSTcpClient.ReceiveTimeout = 3000; // 5秒超时
                //WCSTcpClient.SendTimeout = 3000; // 5秒超时
                //// 异步连接
                //await ConnectAsync(WCSTcpClient,point);
                //// 检查连接状态
                //CheckConnectionStatus(WCSTcpClient);

                //WCSTcpClient = new TcpClient();
                //IPAddress ip = IPAddress.Parse(IP);
                //IPEndPoint point = new IPEndPoint(ip, PortNum);
                //WCSTcpClient.Connect(point);

                //创建负责通信的Socket
                WCSTcpClient = new TcpClient();
                //WCSTcpClient = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(IP);
                //定义IP地址和端口号
                IPEndPoint point = new IPEndPoint(ip, PortNum);
                //获得要连接的远程服务器应用程序的IP地址和端口号/socket.Connect(point):
                // 异步方式进行连接的远程服务器的P地址和端口号
                IAsyncResult result = WCSTcpClient.BeginConnect(ip, PortNum, null, null);
                result.AsyncWaitHandle.WaitOne(500);


                networkStream = WCSTcpClient.GetStream();
                //PLCConnectState = ConnectionStates.Connected;
                //DEMATICConnect();
                SendSTRQToDEMATIC();
            }
            catch (Exception ex)
            {
                DDJWorkState = DDJDeviceWorkState.None;
                PLCConnectState = ConnectionStates.Disconnected;
                LogHelper.WriteLog("WCS连接德马泰克堆垛机失败:" + PLCID, ex);
                Task.Delay(3000).Wait();
            }
        }
        #region 异步处理socket连接
        static async Task ConnectAsync(TcpClient wCSTcpClient, IPEndPoint point)
        {
            try
            {
                await wCSTcpClient.ConnectAsync(point);
                Console.WriteLine("连接成功");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"连接失败: {ex.Message}");
            }
        }

        static void CheckConnectionStatus(TcpClient wCSTcpClient)
        {
            System.DateTime startTime = System.DateTime.Now;
            TimeSpan timeout = TimeSpan.FromSeconds(3); // 设置超时时间为5秒

            while (!wCSTcpClient.Connected)
            {
                if (System.DateTime.Now - startTime > timeout)
                {
                    Console.WriteLine("连接超时");
                    break;
                }

                // 等待一段时间后再次尝试连接
                Thread.Sleep(1000);
            }
        }
        #endregion

        //internal void UpdateDematicBusyState(string deviceID, bool v)
        //{
        //    DeMaticDDJ deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == deviceID);
        //    if (deMaticDDJ != null)
        //    {
        //        deMaticDDJ.DDJBusy = v;
        //    }
        //}

        string ReceiveBuffer;
        private void Receive()
        {
            while (true)   
            {
                try
                {
                    if (WCSTcpClient == null || !WCSTcpClient.Connected || networkStream == null)
                    {
                        ConnectToDematicTcpServer();
                    }
                    //continue;
                    if (PLCConnectState == ConnectionStates.Connected)
                    {
                        Plc.ReadMultipleVars(DDJStatusDataItemList);
                        ProceDDJStatus();
                        int bufSize = WCSTcpClient.ReceiveBufferSize;
                        byte[] buffer = new byte[1024 * 1024 * 3];
                        int count = networkStream.Read(buffer, 0, bufSize);
                        string str = Encoding.ASCII.GetString(buffer, 0, count);
                        ReceiveBuffer += str;
                        DEMATICTOWCSMessageParse();
                        Task.Delay(300).Wait();
                    }
                }
                catch (Exception ex)
                {
                    DDJWorkState = DDJDeviceWorkState.None;
                    PLCConnectState = ConnectionStates.Disconnected;
                    LogHelper.WriteLog(PLCID + "接收服务端消息异常", ex);
                    Task.Delay(3000).Wait();
                    ConnectToDematicTcpServer();
                }
            }
        }

        private void DEMATICTOWCSMessageParse()
        {
            int start_index = ReceiveBuffer.IndexOf('/');
            if (start_index < 0)
            { 
                ReceiveBuffer = null;
                return;
            }
            ReceiveBuffer = ReceiveBuffer.Substring(start_index);
            start_index = ReceiveBuffer.IndexOf('/');
            int end_index = ReceiveBuffer.IndexOf("##");
            if(end_index<0)
            {
                return;
            }
            ParseMsgStr(ReceiveBuffer.Substring(start_index, end_index+2));
            ReceiveBuffer =ReceiveBuffer[(end_index + 2)..];
            DEMATICTOWCSMessageParse();
        }

        private void ParseMsgStr(string str)
        {
            //throw new NotImplementedException();
            //Debug.WriteLine(str);
            DeMaticTOWCS = str;
            DEMATICMessage dEMATIC_Message = null;
            string WCSTODEMATICMessage = "";
            try
            {
                dEMATIC_Message = new DEMATICMessage(PLCID, str, DEMATICMessageDirection.Receive);
                //if(dEMATIC_Message.MsgType!="/RLIVE"&& dEMATIC_Message.MsgType != "/ALIVE"&&dEMATIC_Message.MsgType!= "/RSTAX")
                if (dEMATIC_Message.MsgType != "/RLIVE" && dEMATIC_Message.MsgType != "/ALIVE")
                    InsertIntoDEMATIC_Send_Buffer(dEMATIC_Message);
                if (str.Length >= 6)
                {
                    DEMATICMessage dEMATICMessage = new DEMATICMessage(PLCID);
                    switch (dEMATIC_Message.MsgType)
                    {
                        case "/RSTAT":       //SRM状态询问
                            string state = str.Substring(dEMATIC_Message.MsgDDJWorkStateIndex, dEMATIC_Message.MsgDDJWorkStateLength);
                            if (state == "AU")
                                UpdateDDJStatuss(DDJDeviceWorkState.Standby);
                            else if (state == "MA")
                            {
                                UpdateDDJStatus(DDJDeviceWorkState.Offline);
                            }
                            else if (state == "FL")
                                UpdateDDJStatus(DDJDeviceWorkState.Fault);
                            else if (state == "OF")
                                UpdateDDJStatus(DDJDeviceWorkState.NoMode);
                            //UpdateWMSDBDDJStatus();
                            dEMATICMessage.SetSTATMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/RSTAX":       //SRM设备运行状态报告     /ASTAXWCS1UL080644OK00NG0030##
                            string faultCode = str.Substring(dEMATIC_Message.MsgAllErrorIndex, dEMATIC_Message.MsgAllErrorLength);
                            SetDDJFaultWithoutPalno(faultCode);
                            dEMATICMessage.SetSTAXMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/RSTEN":       //状态询问结束         ASTENWCS1UL080645OK00NG0030##
                            dEMATICMessage.SetSTENMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/RLIVE":      // 心跳信号                      /ALIVEWCS1UL080004OK00NG0030##
                            dEMATICMessage.SetLIVEMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/ALIVE":      // 心跳信号                      /ALIVEWCS1UL080004OK00NG0030##
                            WCSTODEMATICMessage = "";
                            break;
                        case "/RTUNO":     //入库  出库 倒库 取货成功         /ATUNOWCS1UL080576OK00NG0030##
                            dEMATICMessage.SetTUNOMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.PalletNum == str.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength));
                            if (wCSTask != null)
                            {
                                if(wCSTask.TaskType== WCSTaskTypes.DDJDirect||wCSTask.TaskType== WCSTaskTypes.DDJStack&& GetDematicLocation(wCSTask.ToLocation, PLCID) == str.Substring(dEMATIC_Message.MsgToLocationIndex, dEMATIC_Message.MsgToLocationLength))
                                {
                                    //WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.DDJPicked);
                                    SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == string.Concat("SSJ0",wCSTask.Floor));
                                    if (sSJDevice != null)
                                        sSJDevice.ClearOcupty(wCSTask.PalletNum);
                                }
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.DDJPicked);
                            }
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/RTURP":       //入库 出库 倒库 放货成功   以及位置移动成功 /ATURPWCS1UL080578OK00NG0030##
                            dEMATICMessage.SetTURPMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            //入库完成
                            WCSTask inTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.TaskType == WCSTaskTypes.DDJStack && x.PalletNum == str.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength) && GetDematicLocation(x.ToLocation,PLCID) == str.Substring(dEMATIC_Message.MsgToLocationIndex, dEMATIC_Message.MsgToLocationLength));
                            if (inTask != null)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(inTask, WCSTaskStatus.Done);
                                List<WCSTask> wCSTaskNextOutboundTask = WCSTaskManager.Instance.WCSTaskList.FindAll(
                                        x => x.DeviceID == PLCID &&
                                        x.TaskType == WCSTaskTypes.DDJUnstack&&
                                        x.TaskStatus== WCSTaskStatus.Waiting)
                                        .OrderByDescending(x => x.TaskPri)
                                        .ThenBy(x => x.CreateTime).ToList();
                                if (wCSTaskNextOutboundTask != null && wCSTaskNextOutboundTask.Count > 0)
                                    WCSTaskManager.Instance.UpdateWCSTaskPri(wCSTaskNextOutboundTask[0], 10);
                                UpdateDDJStatus(DDJDeviceWorkState.Standby);
                            }
                            //出库完成
                            WCSTask outTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.TaskType == WCSTaskTypes.DDJUnstack && x.PalletNum == str.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength));
                            if (outTask != null)
                            {
                                WMSTask wMSTask1 = WMSTasksManager.Instance.WMSTaskList.Find(x => (x.TaskType == WMSTaskType.Outbound || x.TaskType == WMSTaskType.Picking||x.TaskType== WMSTaskType.MovingToOutbound) && x.PalletNum == str.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength));
                                if (wMSTask1 != null)
                                {
                                    WCSTaskManager.Instance.UpdateWCSTaskStatus(outTask, WCSTaskStatus.Done);
                                }
                                UpdateDDJStatus(DDJDeviceWorkState.Standby);
                            }
                            //倒库完成
                            WCSTask moveTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.TaskType == WCSTaskTypes.DDJStackMove && x.PalletNum == str.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength) && GetDematicLocation(x.ToLocation, PLCID) == str.Substring(dEMATIC_Message.MsgToLocationIndex, dEMATIC_Message.MsgToLocationLength));
                            if (moveTask != null)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(moveTask, WCSTaskStatus.Done);
                                UpdateDDJStatus(DDJDeviceWorkState.Standby);
                            }
                            //直出完成
                            WCSTask directTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.TaskType == WCSTaskTypes.DDJDirect && x.PalletNum == str.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength) );
                            if (directTask != null)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(directTask, WCSTaskStatus.Done);
                                UpdateDDJStatus(DDJDeviceWorkState.Standby);
                            }
                            WCSEnable = true;
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            PalletNum = "";
                            FmLocation = "";
                            ToLocation = "";
                            break;
                        case "/RTUEX":      //SRM发空出报警BE  双重入库BO   出深浅有SN   入深浅有DN    /ATUEXWCS1UL080258OK00NG0030##
                            FaultCode = str.Substring(dEMATIC_Message.MsgErrorIndex, dEMATIC_Message.MsgErrorLength);
                            SetDDJFault(FaultCode, str.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength));
                            PalletNum = str.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength);
                            dEMATICMessage.SetTUEXMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/RTUCA":     //SRM取消任务完成       /ATUCAWCS1UL080259OK00NG0030##
                            WCSTask EmptyUnStackTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.PalletNum == str.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength) && x.TaskType == WCSTaskTypes.DDJUnstack && x.TaskStatus == WCSTaskStatus.UnStackEmpty);
                            if (EmptyUnStackTask != null)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(EmptyUnStackTask, WCSTaskStatus.UnStackEmptyConfirm);
                                WCSEnable = true;   //空出取消完成可以立即恢复堆垛机工作状态，双重不可以，需等待重新分配任务
                                PalletNum = "";   
                            }
                            WCSTask StackDoubleTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.PalletNum == str.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength) && x.TaskType == WCSTaskTypes.DDJStack && x.TaskStatus == WCSTaskStatus.StackDouble);
                            if (StackDoubleTask != null)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(StackDoubleTask, WCSTaskStatus.StackDoubleConfirm);
                            }
                            dEMATICMessage.SetTUCAMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);

                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            FmLocation = "";
                            ToLocation = "";
                            break;
                        case "/ATUMI":   //入库 出库 倒库任务反馈
                            WCSTODEMATICMessage = "";
                            break;
                        case "/ATUMC":     //取消任务反馈
                            WCSTODEMATICMessage = "";
                            break;
                        default:
                            WCSTODEMATICMessage = "Message-Type-Error:" + str;
                            break;
                    }
                }
                else
                {
                    WCSTODEMATICMessage = "Message-Length-Error:" + str;
                }
                SendToDematic(WCSTODEMATICMessage);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DEMATICTOWCSMessageParse 报文处理异常 " + PLCID, ex);
            }
        }
        public bool SendToDematic(string msg)
        {
            bool is_send = false;
            try
            {
                if (WCSTcpClient == null || !WCSTcpClient.Connected)
                   ConnectToDematicTcpServer();
                if (msg.Length>=8&&msg.Substring(0, 8) == "Message-")
                {
                    LogHelper.WriteLog("DEMATICToWCSMessage 异常 " + PLCID + " " + msg);
                    return is_send;
                }
                if(string.IsNullOrEmpty(msg)) return is_send;
                WCSTODeMatic = msg;
                if (networkStream == null)
                    return is_send;
                byte[] buffer = Encoding.ASCII.GetBytes(msg);
                networkStream.Write(buffer, 0, buffer.Length);
                networkStream.Flush();
                DEMATICMessage dEMATICMessage = new DEMATICMessage(PLCID);
                dEMATICMessage.Trans = msg;
                dEMATICMessage.MsgSeqID = msg.Substring(14, 4);
                dEMATICMessage.Direction = DEMATICMessageDirection.Send;
                //if (msg.Substring(0, 6) != "/RLIVE" && msg.Substring(0, 6) != "/ALIVE"&&msg.Substring(0,6)!="/ASTAX")
                if (msg.Substring(0, 6) != "/RLIVE" && msg.Substring(0, 6) != "/ALIVE")
                    InsertIntoDEMATIC_Send_Buffer(dEMATICMessage);
                is_send = true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(PLCID + "发送消息异常", ex);                
            }
            return is_send; 
        }
        private void SendLiveToDEMATIC(object state)
        {
            string msgReceiver = "UL" + PLCID.Substring(3, 2);
            DEMATICMessage dEMATICMessage = new DEMATICMessage(PLCID);
            dEMATICMessage.SetActiveLiveMessage(msgReceiver);
            SendToDematic(dEMATICMessage.Trans);
        }
        private void SendLiveToDEMATIC()
        {
            string msgReceiver = "UL" + PLCID.Substring(3, 2);
            DEMATICMessage dEMATICMessage = new DEMATICMessage(PLCID);
            dEMATICMessage.SetActiveLiveMessage(msgReceiver);
            SendToDematic(dEMATICMessage.Trans);
        }
        private void SendSTRQToDEMATIC()
        {
            string msgReceiver = "UL" + PLCID.Substring(3, 2);
            DEMATICMessage dEMATICMessage = new DEMATICMessage(PLCID);
            dEMATICMessage.SetSTRQMessage(msgReceiver);
            SendToDematic(dEMATICMessage.Trans);
        }
        private void ProceDDJStatus()
        {
            try
            {
                //if (DDJStatusDataItemList.Count < 3) return;
                if (DDJStatusDataItemList[2].Value != null)
                    HasPallet = ((bool)DDJStatusDataItemList[2].Value);
                if (DDJStatusDataItemList[0].Value != null)
                {
                    //int a = (int)(DDJStatusDataItemList[0].Value);
                    MotionPosition = (int)DDJStatusDataItemList[0].Value;
                }
                if (DDJStatusDataItemList[1].Value != null)
                    LiftingPosition = (int)DDJStatusDataItemList[1].Value;

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("RefreshDDJStatus 刷新堆垛机" + PLCID, ex);
            }
        }
        private void WCSTaskProcess(string plcid)
        {
            while (true)
            {
                try
                {
                    if (DDJWorkState != DDJDeviceWorkState.Standby || !WCSEnable) continue;
                    if (HasPallet)
                    {
                        if (!DDJPalletOnStandyProcess())
                        {
                            continue;
                        }
                    }
                    TaskProcess(plcid);
                    Task.Delay(300).Wait();
                }
                catch(Exception ex)
                {
                    LogHelper.WriteLog("WCSTaskProcess " + plcid, ex);
                    Task.Delay(300).Wait();
                }
            }
        }
        private bool DDJPalletOnStandyProcess()
        {
            try
            {
                WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x != null && x.DeviceID != null && x.PalletNum == PalletNum && x.DeviceID == PLCID && (x.TaskStatus == WCSTaskStatus.Waiting || x.TaskStatus == WCSTaskStatus.Doing || x.TaskStatus == WCSTaskStatus.Fault || x.TaskStatus == WCSTaskStatus.DDJPicked || x.TaskStatus == WCSTaskStatus.StackChged));
                if (wCSTask == null)
                {
                    return false;
                }
                WCSTaskManager.Instance.UpdateWCSTaskPriAndStatus(wCSTask, 9, WCSTaskStatus.Waiting);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DDJPalletOnStandyProcess " + PLCID, ex);
                Task.Delay(300).Wait();
                return false;
            }
        }
        private void TaskProcess(string plcid)
        {
            try
            {
                List<WCSTask> wCSTasks;
                if (HasPallet)
                {
                    wCSTasks = WCSTaskManager.Instance.WCSTaskList.FindAll(
                        x => !string.IsNullOrEmpty(x.DeviceID) && x.DeviceID == plcid &&
                        x.TaskStatus == WCSTaskStatus.Waiting && x.PalletNum == PalletNum)
                        .OrderByDescending(x => x.TaskPri)
                        .ThenBy(x => x.CreateTime).ToList();
                }
                else
                {
                    wCSTasks = WCSTaskManager.Instance.WCSTaskList.FindAll(
                        x => !string.IsNullOrEmpty(x.DeviceID) && x.DeviceID == plcid &&
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
                #region
                //switch (wCSTasks[0].TaskType)
                //{
                //    case WCSTaskTypes.DDJStack:
                //        if (DDJStackTaskCanDo(wCSTasks[0]))
                //            dDJTasks.Add(wCSTasks[0]);
                //        break;
                //    case WCSTaskTypes.DDJDirect:
                //        if (DDJDirecTaskCanDo(wCSTasks[0]))
                //            dDJTasks.Add(wCSTasks[0]);
                //        break;
                //    case WCSTaskTypes.DDJStackMove:
                //        if (DDJMoveTaskCanDo(wCSTasks[0]))
                //            dDJTasks.Add(wCSTasks[0]);
                //        break;
                //    case WCSTaskTypes.DDJUnstack:
                //        if (DDJUnStackTaskCanDo(wCSTasks[0]))
                //            dDJTasks.Add(wCSTasks[0]);
                //        break;
                //    default:
                //        break;
                //}
                #endregion
                if (dDJTasks.Count > 0)
                {
                    string WCSTODEMATICMessage = "";
                    string receiver = "";
                    string fmlocation = "";
                    string tolocation = "";
                    DeMaticDDJ deMaticDDJ;
                    DDJMessage ddj_message = new DDJMessage(plcid);
                    switch (dDJTasks[0].TaskType)
                    {
                        case WCSTaskTypes.DDJStack:
                            WMSTask wMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.WMSSeqID == dDJTasks[0].WMSSeqID);
                            if (wMSTask != null)
                            {
                                receiver = "UL" + dDJTasks[0].DeviceID.Substring(3, 2);
                                deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == dDJTasks[0].DeviceID);
                                if (deMaticDDJ != null)
                                {
                                    if (!HasPallet)
                                    {
                                        fmlocation = GetDematicInTaskFmLocation(dDJTasks[0].FmLocation, dDJTasks[0].DeviceID);
                                    }
                                    else
                                    {
                                        fmlocation = string.Concat("ULAI", dDJTasks[0].DeviceID.Substring(3, 2), "SR01LH11");   //载货台地址   ULAI08SR01LH11
                                    }
                                    DEMATICMessage dEMATICMessage = new DEMATICMessage(dDJTasks[0].DeviceID);
                                    dEMATICMessage.SetTUMIIntaskMessage(receiver, fmlocation, GetDematicLocation(dDJTasks[0].ToLocation, dDJTasks[0].DeviceID), dDJTasks[0].PalletNum);
                                    WCSTODEMATICMessage = dEMATICMessage.Trans;
                                    SendToDematic(WCSTODEMATICMessage);
                                    UpdateDDJStatuss(DDJDeviceWorkState.Working);
                                    WCSEnable = false;
                                    PalletNum = dDJTasks[0].PalletNum;
                                    FmLocation = dDJTasks[0].FmLocation;
                                    ToLocation = dDJTasks[0].ToLocation;
                                }
                            }
                            break;
                        case WCSTaskTypes.DDJUnstack:
                            tolocation = GetDematicOutTaskToLoction(dDJTasks[0].ToLocation, dDJTasks[0].DeviceID);
                            receiver = "UL" + dDJTasks[0].DeviceID.Substring(3, 2);
                            deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == dDJTasks[0].DeviceID);
                            if (deMaticDDJ != null)
                            {
                                if (!HasPallet)
                                {
                                    fmlocation = GetDematicLocation(dDJTasks[0].FmLocation, dDJTasks[0].DeviceID);
                                }
                                else
                                {
                                    fmlocation = string.Concat("ULAI", dDJTasks[0].DeviceID.Substring(3, 2), "SR01LH11");   //针对双重无可用货位，WCS将任务更新成出库任务 起始地为载货台地址
                                }
                                DEMATICMessage dEMATICMessage = new DEMATICMessage(dDJTasks[0].DeviceID);
                                dEMATICMessage.SetTUMIOutTaskMessage(receiver, fmlocation, tolocation, dDJTasks[0].PalletNum);
                                WCSTODEMATICMessage = dEMATICMessage.Trans;
                                SendToDematic(WCSTODEMATICMessage);
                                UpdateDDJStatuss(DDJDeviceWorkState.Working);
                                WCSEnable = false;
                                PalletNum = dDJTasks[0].PalletNum;
                                FmLocation = dDJTasks[0].FmLocation;
                                ToLocation = dDJTasks[0].ToLocation;
                            }
                            break;
                        case WCSTaskTypes.DDJDirect:
                            fmlocation= GetDematicInTaskFmLocation(dDJTasks[0].FmLocation, dDJTasks[0].DeviceID);
                            tolocation = GetDematicOutTaskToLoction(dDJTasks[0].ToLocation, dDJTasks[0].DeviceID);
                            receiver = "UL" + dDJTasks[0].DeviceID.Substring(3, 2);
                            deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == dDJTasks[0].DeviceID);
                            if (deMaticDDJ != null)
                            {
                                DEMATICMessage dEMATICMessage = new DEMATICMessage(dDJTasks[0].DeviceID);
                                dEMATICMessage.SetTUMIDirectTaskMessage(receiver, fmlocation, tolocation, dDJTasks[0].PalletNum);
                                WCSTODEMATICMessage = dEMATICMessage.Trans;
                                SendToDematic(WCSTODEMATICMessage);
                                UpdateDDJStatuss(DDJDeviceWorkState.Working);
                                WCSEnable = false;
                                PalletNum = dDJTasks[0].PalletNum;
                                FmLocation = dDJTasks[0].FmLocation;
                                ToLocation = dDJTasks[0].ToLocation;
                            }
                            break;
                        case WCSTaskTypes.DDJStackMove:
                            receiver = "UL" + dDJTasks[0].DeviceID.Substring(3, 2);
                            deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == dDJTasks[0].DeviceID);
                            if (deMaticDDJ != null)
                            {
                                DEMATICMessage dEMATICMessage = new DEMATICMessage(dDJTasks[0].DeviceID);
                                dEMATICMessage.SetTUMIMoveTaskMessage(receiver, GetDematicLocation(dDJTasks[0].FmLocation, dDJTasks[0].DeviceID), GetDematicLocation(dDJTasks[0].ToLocation, dDJTasks[0].DeviceID), dDJTasks[0].PalletNum);
                                WCSTODEMATICMessage = dEMATICMessage.Trans;
                                SendToDematic(WCSTODEMATICMessage);
                                UpdateDDJStatuss(DDJDeviceWorkState.Working);
                                WCSEnable = false;
                                PalletNum = dDJTasks[0].PalletNum;
                                FmLocation = dDJTasks[0].FmLocation;
                                ToLocation = dDJTasks[0].ToLocation;
                            }
                            break;
                        default:
                            break;
                    }
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(dDJTasks[0], WCSTaskStatus.Doing);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("WCSTaskProcess " + PLCID, ex);
            }
        }
        #region 堆垛机任务执行判断
        private string GetDematicInTaskFmLocation(string fm_location, string device_id)
        {
            //ULAI08CL01PS10
            string dematic_fm_location = "";
            dematic_fm_location = "ULAI" + device_id.Substring(3, 2);
            if (fm_location == "000a")
            {
                if(device_id.Substring(3, 2) == "03" || device_id.Substring(3, 2) == "07")
                    dematic_fm_location += "CL";
                else
                    dematic_fm_location += "CR";
                dematic_fm_location += "01" + "PS10";
            }
            else
            {
                //二楼待看是CL还是CR----------------
                if (device_id.Substring(3, 2) == "03" || device_id.Substring(3, 2) == "07")
                    dematic_fm_location += "CL";
                else
                    dematic_fm_location += "CR";
                dematic_fm_location += "02" + "PS10";
            }
            return dematic_fm_location;
        }
        private string GetDematicOutTaskToLoction(string to_location, string device_id)
        {
            string dematic_to_location = "";
            dematic_to_location = "ULAI" + device_id.Substring(3, 2);
            if (to_location == "000a")
            {
                if (device_id.Substring(3, 2) == "03" || device_id.Substring(3, 2) == "07")
                    dematic_to_location += "CR";
                else
                    dematic_to_location += "CL";
                dematic_to_location += "01" + "DS10";
            }
            else
            {
                //二楼待看是CL还是CR----------------
                if (device_id.Substring(3, 2) == "03" || device_id.Substring(3, 2) == "07")
                    dematic_to_location += "CR";
                else
                    dematic_to_location += "CL";
                dematic_to_location += "02" + "DS10";
            }
            return dematic_to_location;
        }

        private bool DDJUnStackTaskCanDo(WCSTask wCSTask)
        {
            if (wCSTask == null || wCSTask.TaskType != WCSTaskTypes.DDJUnstack) return false;
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PortNum == wCSTask.ToLocation);
            if (sSJDevice == null || sSJDevice.SSJWorkState == SSJDeviceWorkState.Offline || sSJDevice.SSJWorkState == SSJDeviceWorkState.None || sSJDevice.SSJWorkState == SSJDeviceWorkState.Manual) return false;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == Tunnel && x.SystemType == DeviceSystemType.OutboundBegin && x.Floor == wCSTask.Floor);
            if (sSJDeviceBlock == null) return false;
            //判断允许出库标志位
            if (!sSJDeviceBlock.AllowUnloading) return false;

            string fm_location = wCSTask.FmLocation;
            WCSTask wCSMoveOrOutboundTask = WCSTaskManager.Instance.WCSTaskList.Find(x => (x.TaskType == WCSTaskTypes.DDJStackMove || x.TaskType == WCSTaskTypes.DDJUnstack) && x.DeviceID == wCSTask.DeviceID && x.FmLocation == GetClearLocation(fm_location)&&x.TaskStatus== WCSTaskStatus.Waiting);
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
                case "05":
                case "09":
                case "13":
                case "17":
                case "21":
                case "25":
                case "29":
                case "33":
                    clear_location=string.Concat(string.Format("{0:D2}", Convert.ToInt32(pai) + 1), far_location.AsSpan(2, 6));
                    break;
                case "04":
                case "08":
                case "12":
                case "16":
                case "20":
                case "24":
                case "28":
                case "32":
                case "36":
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
            GoodsLocation fm_goodlocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == wCSTask.FmLocation);
            GoodsLocation to_goodlocation = GoodsLocationManager.GoodsLocationList.Find(x => x.Position == wCSTask.ToLocation);
            if (fm_goodlocation == null || to_goodlocation == null) return false;
            if (!fm_goodlocation.Available || !to_goodlocation.Available) return false;
            return true;
        }

        private bool DDJDirecTaskCanDo(WCSTask wCSTask)
        {
            if (wCSTask == null || wCSTask.TaskType != WCSTaskTypes.DDJDirect) return false;
            SSJDevice sSJDevice1 = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PortNum == wCSTask.FmLocation);
            if (sSJDevice1 == null || sSJDevice1.SSJWorkState == SSJDeviceWorkState.Offline || sSJDevice1.SSJWorkState == SSJDeviceWorkState.None || sSJDevice1.SSJWorkState == SSJDeviceWorkState.Manual) return false;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice1.DeviceBlockList.Find(x => x.Tunnel == Tunnel && x.SystemType == DeviceSystemType.InboundFinish && x.Floor == wCSTask.Floor);
            if (sSJDeviceBlock == null) return false;
            if (!sSJDeviceBlock.TPHorizon) return false;
            if (!HasPallet)   //有托盘，则不需要看入库口占位等信息
            {
                if (!sSJDeviceBlock.IsOccupied || sSJDeviceBlock.PalletNum != wCSTask.PalletNum) return false;
            }

            SSJDevice sSJDevice2 = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PortNum == wCSTask.ToLocation);
            //SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(wCSTask.FmLocation));
            if (sSJDevice2 == null || sSJDevice2.SSJWorkState == SSJDeviceWorkState.Offline || sSJDevice2.SSJWorkState == SSJDeviceWorkState.None || sSJDevice2.SSJWorkState == SSJDeviceWorkState.Manual) return false;
            SSJDeviceBlock sSJDeviceBlock2 = sSJDevice2.DeviceBlockList.Find(x => x.Tunnel == Tunnel && x.SystemType == DeviceSystemType.OutboundBegin && x.Floor != wCSTask.Floor);
            if (sSJDeviceBlock2 == null) return false;
            //判断允许出库标志位
            if (!sSJDeviceBlock2.AllowUnloading) return false;
            return true;
        }

        private bool DDJStackTaskCanDo(WCSTask wCSTask)
        {
            if (wCSTask == null || wCSTask.TaskType != WCSTaskTypes.DDJStack) return false;
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PortNum == wCSTask.FmLocation);
            //SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(wCSTask.ToLocation));
            if (sSJDevice == null || sSJDevice.SSJWorkState == SSJDeviceWorkState.Offline || sSJDevice.SSJWorkState == SSJDeviceWorkState.None || sSJDevice.SSJWorkState == SSJDeviceWorkState.Manual) return false;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == Tunnel && x.SystemType == DeviceSystemType.InboundFinish && x.Floor == wCSTask.Floor);
            if (sSJDeviceBlock == null) return false;
            if(!sSJDeviceBlock.TPHorizon) return false;
            if (!HasPallet)   //有托盘，则不需要看入库口占位等信息
            {
                if (!sSJDeviceBlock.IsOccupied || sSJDeviceBlock.PalletNum != wCSTask.PalletNum) return false;
            }
            string clear_location_pai = wCSTask.ToLocation.Substring(0, 2);
            if (ClearLocationSmallPai.Contains(clear_location_pai) || ClearLocationBigPai.Contains(clear_location_pai))
            {
                if (HaveFarOutbound(wCSTask, clear_location_pai)) return false;
            }
            return true;
        }
        private bool HaveFarOutbound(WCSTask wCSTask, string pai)
        {
            string clear_location_pai = pai;
            string far_location = "";
            switch (clear_location_pai)
            {
                case "02":
                case "06":
                case "10":
                case "14":
                case "18":
                case "22":
                case "26":
                case "30":
                case "34":
                    far_location = string.Concat(string.Format("{0:D2}", Convert.ToInt32(clear_location_pai) - 1), wCSTask.ToLocation.AsSpan(2, 6));
                    break;
                case "03":
                case "07":
                case "11":
                case "15":
                case "19":
                case "23":
                case "27":
                case "31":
                case "35":
                    far_location= string.Concat(string.Format("{0:D2}", Convert.ToInt32(clear_location_pai) + 1), wCSTask.ToLocation.AsSpan(2, 6));
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
        private bool CalculateCircleNum(SSJDevice sSJDevice)
        {
            List<SSJDeviceBlock> sSJDeviceBlocks = sSJDevice.DeviceBlockList.FindAll(x => x.InCircle==true&&x.IsOccupied==true);
            if (sSJDeviceBlocks.Count < 32)
                return true;
            else
                return false;
        }
        #endregion
        public void UpdateDDJStatus(DDJDeviceWorkState device_state)
        {
            if (DDJWorkState == device_state) return;
            DDJWorkState = device_state;
            //DDJStatusChanged?.Invoke(this, new DDJStatusEventArgs(this));
        }
        public void UpdateDDJStatuss(DDJDeviceWorkState device_state)
        {
            if (DDJWorkState == device_state) return;
            DDJWorkState = device_state;
            FaultCode = "";
            faultContent = "";
            //DDJStatusChanged?.Invoke(this, new DDJStatusEventArgs(this));
        }
        public void InsertIntoDEMATIC_Send_Buffer(DEMATICMessage dEMATICMessage)
        {
            if (dEMATICMessage != null)
            {
                SqlParameter[] sqlParameters = new SqlParameter[] {
                new SqlParameter("@trans", dEMATICMessage.Trans),
                new SqlParameter("@PlcId", dEMATICMessage.PLCID),
                new SqlParameter("@MsgSeqID", dEMATICMessage.MsgSeqID),
                new SqlParameter("@Tkdat",dEMATICMessage.Tkdat ),
                new SqlParameter("@MsgParse",dEMATICMessage.MsgParse),
                new SqlParameter("@Direction",dEMATICMessage.Direction)
                };
                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[DEMATIC_Send_Buffer]([PLCID],[Trans],[Tkdat],[MsgSeqID],[MsgParse],[Direction]) VALUES (@PlcId,@trans,@Tkdat,@MsgSeqID,@MsgParse,@Direction)", sqlParameters);
            }
        }
        public string GetDematicLocation(string location,string device_id)  //13000202    UL081032030211
        {
            if (location.Length == 4) return null;
            int location_pai = int.Parse(location[..2]);
            string left_right = "";
            string deep = "";
            if (leftPai.Contains(location[..2]))
                left_right = "1";
            if (rightPai.Contains(location[..2]))
                left_right = "2";
            if (clearPai.Contains(location[..2]))
                deep = "01";
            if (farPai.Contains(location[..2]))
                deep = "02";
            string dematic_location = "UL" + device_id.Substring(3, 2) + left_right + location.Substring(3, 3) + location.Substring(6, 2) + deep + "11";
            return dematic_location;
        }
        public string GetSSJPLCID(string toLocation)
        {
            string ssj_PLCID = "";
            int pai = Convert.ToInt32(toLocation[..2]);
            if (pai > 0 && pai <= 8)
            {
                ssj_PLCID = "SSJ01";
            }
            else if (pai > 8 && pai <= 12)
            {
                ssj_PLCID = "SSJ02";
            }
            else if (pai > 12 && pai <= 18)
            {
                ssj_PLCID = "SSJ03";
            }
            return ssj_PLCID;
        }
        private void SetDDJFault(string fault_code, string pallet_num)
        {
            if (string.IsNullOrEmpty(fault_code)) return;
            UpdateDDJStatus(DDJDeviceWorkState.Fault);
            DEMATICFault dDJFault = DEMATICFaultList.Instance.FaultList.Find(x => x.FaultCode == fault_code);
            if (string.IsNullOrEmpty(dDJFault.FaultContent)) return;

            FaultCode = fault_code;
            FaultContent = dDJFault.FaultContent;
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == pallet_num && x.DeviceID == PLCID);
            if (wCSTask == null) return;
            switch (FaultCode)
            {
                case "BE":
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.UnStackEmptyConfirm);  //由于不需要WCS给堆垛机下发取消任务，因此堆垛机发出空出报警 直接变成空出确认
                    WCSEnable = true;
                    PalletNum = "";
                    break;
                case "BO":   //双重入库，设定载货台有货
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.StackDouble);
                    break;
                case "SN":
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.FarOutboundClearHave);
                    break;
                case "DN":
                    break;
                default:
                    break;
            }
        }
        private void SetDDJFaultWithoutPalno(string fault_code)
        {
            if (string.IsNullOrEmpty(fault_code)) return;
            if(fault_code != "000000")
                UpdateDDJStatus(DDJDeviceWorkState.Fault);
            DEMATICFault dDJFault = DEMATICFaultList.Instance.FaultList.Find(x => x.FaultCode == fault_code);
            if (string.IsNullOrEmpty(dDJFault.FaultContent)) return;
            FaultCode = fault_code;
            FaultContent = dDJFault.FaultContent;
        }

        internal void Online()
        {
            throw new NotImplementedException();
        }

        internal void OffLine()
        {
            throw new NotImplementedException();
        }

        internal void RecallInPlace()
        {
            throw new NotImplementedException();
        }
        
        public enum DDJDeviceWorkState
        {
            None,
            NoMode,
            Standby,
            Working,
            Fault,
            Offline
        }
    }
    public class DematicTaskEventArgs : EventArgs
    {
        public DematicTaskEventArgs(DematicTask dDJTask)
        {
            DDJTask = dDJTask;
        }
        public DematicTask DDJTask { get; set; }
    }
    public sealed class DEMATICFaultList
    {
        private static readonly Lazy<DEMATICFaultList> lazy = new Lazy<DEMATICFaultList>(() => new DEMATICFaultList());

        public List<DEMATICFault> FaultList;

        public static DEMATICFaultList Instance { get { return lazy.Value; } }

        DEMATICFaultList()
        {
            FaultList = new List<DEMATICFault>();
            DataTable dt = SQLServerHelper.DataBaseReadToTable("SELECT[故障号]," +
                "[故障名称] FROM[dbo].[DEMATIC_Fault_Code]");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DEMATICFault ddj_fault = new DEMATICFault
                {
                    FaultCode = dt.Rows[i]["故障号"].ToString(),
                    FaultContent = dt.Rows[i]["故障名称"].ToString()
                };
                FaultList.Add(ddj_fault);
            }
        }
    }
    public struct DEMATICFault
    {
        public string FaultCode { get; set; }
        public string FaultContent { get; set; }
    }

}
