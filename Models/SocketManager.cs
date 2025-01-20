using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using BMHRI.WCS.Server.DDJProtocol;
using System.Data;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.SqlClient;
using S7.Net.Types;
using Timer = System.Threading.Timer;
using System.Linq;
using S7.Net;

namespace BMHRI.WCS.Server.Models
{
    //class DeMaticDDJManager {
    //    private static readonly Lazy<DeMaticDDJManager> lazy = new Lazy<DeMaticDDJManager>(() => new DeMaticDDJManager());
    //    public static DeMaticDDJManager Instance { get { return lazy.Value; } }
    //    List<DeMaticDDJ> DeMaticDDJS;

    //}

    //class DeMaticDDJ : PLCDevice
    //{ 
    //    private void WCSTaskProcess()
    //    {
    //        while (true)
    //        {

    //        }
    //    }
    //}


    public class SocketManager:PLCDevice
    {
        private static readonly Lazy<SocketManager> lazy = new Lazy<SocketManager>(() => new SocketManager());
        public static SocketManager Instance { get { return lazy.Value; } }
        public static string PLCConfigfilePathName = Environment.CurrentDirectory + "\\Configure" + "\\DEMATIC.config";
        public List<DEMATICConfig> dEMATICConfigsList;
        public List<DEMATICMessage> dEMATICMessagesList;
        public string WCSTODEMATICMessage = "";
        public List<DataItem> DataItemList;
        public List<DataItem> DDJStatusDataItemList;
        //public string PLCDecription { get; set; }
        //public string PLCID { get; set; }
        #region  DDJ状态变量
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
        private bool dDJBusy;
        public bool DDJBusy
        {
            get { return dDJBusy; }
            set
            {
                if(dDJBusy != value)
                {
                    dDJBusy = value;
                    Notify(nameof(DDJBusy));
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
                motionPosition = value;
                Notify(nameof(MotionPosition));
            }
        }
        private int liftingPosition;   
        public int LiftingPosition
        {
            get { return liftingPosition; }
            set
            {
                
                liftingPosition = value;
                Notify(nameof(LiftingPosition));
            }
        }

        private int forkPosition;
        public int ForkPosition
        {
            get { return forkPosition; }
            set
            {
                forkPosition = value;
                Notify(nameof(ForkPosition));
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
                    //updatewmsdbddjstatus();
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
        #endregion
        public Socket socketSend;
        private Timer SendLiveToDEMATICTimer;
        public SocketManager()
        {

        }
        public Socket GetSocket(string plc_id)
        {
            PLCID = plc_id;
            Socket socket = null;

            socket = SocketsList[plc_id];
            return socket;
        }
        private void CreateSendLiveToDEMATIC()    //Socket socket_send
        {
            SendLiveToDEMATICTimer = new Timer(new TimerCallback(SendLiveToDEMATIC), null, 15000, 15000);
        }
        private bool is_send_to_dematic_live = false;
        private void SendLiveToDEMATIC(object state)
        {
            if(is_send_to_dematic_live) return;
            is_send_to_dematic_live = true;
            string msgReceiver = "UL" + PLCID.Substring(3, 2);
            DEMATICMessage dEMATICMessage = new DEMATICMessage(PLCID);
            string msgseqid = ("000" + dEMATICMessage.MsgID.ToString()).Substring(0, 4);
            dEMATICMessage.SetActiveLiveMessage(msgReceiver);
            WCSTODEMATICMessage = dEMATICMessage.Trans;
            WCSToDEMATICMessageSend(WCSTODEMATICMessage, socketSend);
            is_send_to_dematic_live = false;
        }
        private void CreateSendSTRQToDEMATIC()
        {
            string msgReceiver = "UL" + PLCID.Substring(3, 2);
            DEMATICMessage dEMATICMessage = new DEMATICMessage(PLCID);
            string msgseqid = ("000" + dEMATICMessage.MsgID.ToString()).Substring(0, 4);
            dEMATICMessage.SetSTRQMessage(msgReceiver);
            WCSTODEMATICMessage = dEMATICMessage.Trans;
            WCSToDEMATICMessageSend(WCSTODEMATICMessage, socketSend);
        }
        public Dictionary<string, Socket> SocketsList;
        private SocketManager(string cpuType, string Ip, string slot, string rack, string description, string device_type, string plc_id, string avi) : base(cpuType, Ip, slot, rack, description, device_type, plc_id)
        //private SocketManager(string IP, int port, string plc_id, string description,string slot,string rack,string cpuType,string device_type, string avi):base(IP,port, plc_id, description, slot, rack, cpuType, device_type)
        {
            if (SocketsList == null) SocketsList = new Dictionary<string, Socket>();
            
            ReadDEMATICDataConfig();
            GetTagListFromConfig();
            PLCDecription = description;
            PLCID = plc_id;
            Available = avi == "1";
            Task.Factory.StartNew(() => PLCCommunication(Ip), TaskCreationOptions.LongRunning);
        }
        
        private List<SocketManager> _socketManagerList;
        public List<SocketManager>  SocketManagerList
        {
            get
            {
                if (_socketManagerList == null)
                    CreateDDJeviceList();
                return _socketManagerList;
            }
        }

        public void SocketStart()
        {
            //GetTagListFromConfig();

        }
        
        List<Socket> socketsSendList;
        public void ConnectToDEMATIC()
        {
            GetTagListFromConfig();
            socketsSendList = new List<Socket>();
            try
            {
                for (int i = 0; i < dEMATICConfigsList.Count; i++)
                {
                    //客户端创建一个用于通讯的socket，与远程服务器连接
                    //socketsSendList.Add( new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
                    //IPAddress ip = IPAddress.Parse(dEMATICConfigsList[i].IP);
                    //IPEndPoint point = new IPEndPoint(ip, dEMATICConfigsList[i].Port);
                    //socketsSendList[i].Connect(point);
                    //Task.Factory.StartNew(() => Receive(socketsSendList[i]), TaskCreationOptions.LongRunning);
                }
                //Thread th = new Thread(Receive);
                //th.IsBackground = true;
                //th.Start();
                
            }
            catch(Exception ex)
            {
                LogHelper.WriteLog("创建通讯SoctekSend失败",ex);
            }
        }
        Dictionary<string,string> msg=new Dictionary<string, string>(); 
        void Receive(Socket socketSend)
        {
            while (true)
            {
                try
                {
                    //for (int i = 0; i < socketsSendList.Count; i++)
                    //{
                        byte[] buffer = new byte[1024 * 1024 * 3];
                        //实际上接收到的有效字节数
                        int r = socketSend.Receive(buffer);
                        //服务端若不send数据，客户端则无法receice到数据，此时r根本就没有值，故i永远也跳转不到2.故造成的后果是必须等
                        //等服务端1send后，服务端2才能send，从而客户端才能receive到数据。故必须采用一个堆垛机服务端一个线程的方式，独立工作
                        if (r == 0)
                        {
                            break;
                        }
                    //从服务端接收到的消息
                        string receive = Encoding.Default.GetString(buffer, 1, r - 1);
                        //string MsgSeqID = receive.Substring(14, 4);
                        //msg.Add(MsgSeqID, receive);
                        DEMATICTOWCSMessageParse(receive, socketSend);
                    //}

                }
                catch(Exception ex)
                {
                    LogHelper.WriteLog("接收服务端消息异常", ex);
                }
            }
        }
        private void Receive()
        {
            while (!cancelTokenSource.IsCancellationRequested)   //!cancelTokenSource.IsCancellationRequested
            {
                try
                {
                    Task.Delay(300).Wait();//观察是否有必要 针对开机第一条状态报文接收不到
                    //for (int i = 0; i < socketsSendList.Count; i++)
                    //{
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    //实际上接收到的有效字节数
                    int r = socketSend.Receive(buffer);
                    //服务端若不send数据，客户端则无法receice到数据，此时r根本就没有值，故i永远也跳转不到2.故造成的后果是必须等
                    //等服务端1send后，服务端2才能send，从而客户端才能receive到数据。故必须采用一个堆垛机服务端一个线程的方式，独立工作
                    if (r == 0)
                    {
                        break;
                    }
                    //从服务端接收到的消息
                    string receive = Encoding.Default.GetString(buffer, 0, r);
                    //string MsgSeqID = receive.Substring(14, 4);
                    //msg.Add(MsgSeqID, receive);
                    DEMATICTOWCSMessageParse(receive, socketSend);
                    //}

                }
                catch (Exception ex)
                {
                    cancelTokenSource.Cancel();
                    is_connect_to_dematic = false;
                    is_send_to_dematic_live = true;
                    DDJWorkState = DDJDeviceWorkState.Offline;
                    PLCConnectState = ConnectionStates.Disconnected;
                    LogHelper.WriteLog(PLCID+"接收服务端消息异常", ex);
                    return;
                }
            }
        }
        public void UpdateDematicBusyState(string plcid,bool ddj_busy)
        {
            SocketManager socketManager = SocketManagerList.Find(x => x.PLCID == plcid);
            if (socketManager != null)
            {
                socketManager.DDJBusy = ddj_busy;
            }
        }
        private void PLCCommunication(string Ip)
        {
            while (true)
            {
                try
                {
                    if (dEMATICConfigsList == null || dEMATICConfigsList.Count == 0) continue;
                    DEMATICConfig dEMATICConfig = dEMATICConfigsList.Find(x => x.IP == Ip);
                    if (dEMATICConfig == null) continue;
                    if (PLCConnectState == ConnectionStates.Connected)
                    {
                        //若读取失败，则进入catch,plcconnection恢复成 Disconnected。若不是西门子PLC则无法读取，plc断开后，怎么恢复成disconnected?
                        //Plc.ReadMultipleVars(DDJStatusDataItemList);
                        //ProceDDJStatus();
                    }
                    else if(PLCConnectState== ConnectionStates.Disconnected)
                    {
                        PLCConnectState = ConnectionStates.Connecting;
                        TcpClient client = new TcpClient();
                        IPAddress ip = IPAddress.Parse(Ip);
                        try
                        {
                            client.Connect(Ip, dEMATICConfig.Port);
                            PLCConnectState = ConnectionStates.Connected;
                            //DEMATICConnect();
                        }
                        catch(Exception ex)
                        {
                            PLCConnectState = ConnectionStates.Disconnected;
                            LogHelper.WriteLog("连接DEMATIC Ip,端口异常!" + PLCID + " " + ex.ToString());
                        }
                        client.Close();
                        client.Dispose();
                    }
                    Task.Delay(300).Wait();
                }
                catch(PlcException ex)
                {
                    PlcExceptionParse(ex);
                    if (!Plc.IsConnected)
                    {
                        PLCConnectState = ConnectionStates.Disconnected;
                        UpdateDDJStatus(DDJDeviceWorkState.None);
                    }
                    LogHelper.WriteLog("连接DEMATIC Ip,端口异常!!" + PLCID + " " + ex.ToString());
                    Task.Delay(1000).Wait();
                }
            }
        }
        private void WCSTaskProcess()
        {
            DDJDevice dDJDevice = new DDJDevice(PLCID);
            while (true)
            {
                try
                {
                    if (WCSEnable&&!DDJBusy)
                    {
                        if (DDJWorkState != DDJDeviceWorkState.Standby)
                        {
                            Task.Delay(300).Wait();
                            continue;
                        }
                        dDJDevice.WCSTaskProcess();
                    }
                }
                catch(Exception ex)
                {
                    LogHelper.WriteLog("WCSTaskProcess " + PLCID, ex);
                }
            }
        }
        public void DisConnectToDematic(string plcid)
        {
            if (SocketsList[plcid] != null)
            {
                SocketsList[plcid].Shutdown(SocketShutdown.Send);
                DDJWorkState = DDJDeviceWorkState.Offline;
            }
        }
        private bool is_connect_to_dematic = false;
        static CancellationTokenSource cancelTokenSource=new CancellationTokenSource();
        public void ConnectToDematic(string IP,int port,string plcid)
        {
            is_connect_to_dematic = false;

            try
            {
                if (PLCConnectState != ConnectionStates.Connected) return;
                if (is_connect_to_dematic) return;
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(IP);
                IPEndPoint point = new IPEndPoint(ip, port);

                if (SocketsList.Count != 0)
                {
                    SocketsList[plcid].Shutdown(SocketShutdown.Send);
                    //SocketsList[plcid].Close();
                    SocketsList.Clear();
                }
                socketSend.Connect(point);
                SocketsList.Add(plcid, socketSend);



                is_connect_to_dematic = true;
                is_send_to_dematic_live = false;
                CreateSendSTRQToDEMATIC();
                CreateSendLiveToDEMATIC();
                DDJWorkState = DDJDeviceWorkState.Online;
                PLCConnectState = ConnectionStates.Connected;
                if (cancelTokenSource != null) cancelTokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(() => Receive(), cancelTokenSource.Token);     //cancelTokenSource.Token
                Task.Factory.StartNew(() => WCSTaskProcess(), TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                DDJWorkState = DDJDeviceWorkState.Offline;
                PLCConnectState = ConnectionStates.Disconnected;
                LogHelper.WriteLog("WCS连接德马泰克堆垛机失败:" + PLCID, ex);
                return;
            }
        }
        private void GetTagListFromConfig()
        {
            dEMATICConfigsList = new List<DEMATICConfig>();
            if (File.Exists(Environment.CurrentDirectory + "\\Configure" + "\\DEMATIC.config"))
            {
                try
                {
                    XDocument xDocument = XDocument.Load(PLCConfigfilePathName);
                    foreach (XElement item in xDocument.Root.Descendants("DDJ"))
                    {
                        DEMATICConfig data_item = new DEMATICConfig();
                        data_item.IP = item.Element("IP").Value;
                        data_item.Port = int.Parse(item.Element("Port").Value);
                        dEMATICConfigsList.Add(data_item);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("堆垛机配置文件读取异常", ex);
                }
            }
        }
        public void CreateDDJeviceList()
        {
            _socketManagerList = new List<SocketManager>();
            if (File.Exists(PLCConfigfilePathName))
            {
                try
                {
                    XDocument xDocument = XDocument.Load(PLCConfigfilePathName);
                    foreach (XElement item in xDocument.Root.Descendants("DDJ"))
                    {
                        //SocketManager socketManager = new SocketManager(item.Element("IP").Value, int.Parse(item.Element("Port").Value),item.Element("PLCID").Value,item.Element("Decription").Value,item.Element("Slot").Value,item.Element("Rack").Value,item.Element("CpuType").Value,"DDJ",item.Element("Available").Value);
                        SocketManager socketManager = new SocketManager(item.Element("CpuType").Value, item.Element("IP").Value, item.Element("Slot").Value, item.Element("Rack").Value, item.Element("Decription").Value, "DDJ", item.Element("PLCID").Value, item.Element("Available").Value);

                        if (socketManager != null)
                        {
                            _socketManagerList.Add(socketManager);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("堆垛机配置文件读取异常", ex);
                }
            }
        }
        private void ReadDEMATICDataConfig()
        {
            if (DataItemList == null) DataItemList = new List<DataItem>();
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
                            if (data_item != null)
                                DataItemList.Add(data_item);
                        }
                    }
                    DDJStatusDataItemList = new List<DataItem>();
                    DDJStatusDataItemList = DataItemList.FindAll(x => x.DataType == DataType.Memory && (x.DB == 0));
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("DEMATIC_Data配置文件读取异常", ex);
                }
            }
        }

        private void DEMATICTOWCSMessageParse(string message,Socket socketSend)
        {
            DEMATICMessage dEMATIC_Message = null;
            //string WCSTODEMATICMessage = "";
            try
            {
                dEMATIC_Message = new DEMATICMessage(PLCID,message,DEMATICMessageDirection.Receive);
                InsertIntoDEMATIC_Send_Buffer(dEMATIC_Message);
                if (message.Length >= 6)
                {
                    DEMATICMessage dEMATICMessage = new DEMATICMessage(PLCID);
                    switch (dEMATIC_Message.MsgType)
                    {
                        case "/RSTAT":       //SRM状态询问
                            string state = message.Substring(dEMATIC_Message.MsgDDJWorkStateIndex, dEMATIC_Message.MsgDDJWorkStateLength);
                            if (state == "AU")
                                UpdateDDJStatus(DDJDeviceWorkState.Standby);
                            else if (state == "MA")
                            {
                                WCSEnable = false;
                                UpdateDDJStatus(DDJDeviceWorkState.Offline);
                            }
                            else if (state == "FL")
                                UpdateDDJStatus(DDJDeviceWorkState.Fault);
                            else if (state == "OF")
                                UpdateDDJStatus(DDJDeviceWorkState.None);
                            dEMATICMessage.SetSTATMessage(dEMATIC_Message.MsgSender,dEMATIC_Message.MsgSeqID);
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/RSTAX":       //SRM设备运行状态报告     /ASTAXWCS1UL080644OK00NG0030##
                            dEMATICMessage.SetSTAXMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/RSTEN":       //状态询问结束         ASTENWCS1UL080645OK00NG0030##
                            dEMATICMessage.SetSTENMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/RLIVE":      // 心跳信号                      /ALIVEWCS1UL080004OK00NG0030##
                            dEMATICMessage.SetLIVEMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSEnable = true;
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/ALIVE":      // 心跳信号                      /ALIVEWCS1UL080004OK00NG0030##
                            //dEMATICMessage.SetLIVEMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSEnable = true;
                            WCSTODEMATICMessage = "";
                            break;
                        case "/RTUNO":     //入库  出库 倒库 取货成功         /ATUNOWCS1UL080576OK00NG0030##
                            dEMATICMessage.SetTUNOMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID &&x.TaskType== WCSTaskTypes.DDJStack&& x.PalletNum==message.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength) &&GetDematicLocation(x.ToLocation) == message.Substring(dEMATIC_Message.MsgToLocationIndex, dEMATIC_Message.MsgToLocationLength));
                            if (wCSTask != null)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.DDJPicked);
                                //SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PortNum == wCSTask.FmLocation);
                                SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJPLCID(wCSTask.ToLocation));
                                if (sSJDevice != null)
                                    sSJDevice.ClearOcupty(wCSTask.PalletNum);
                            }
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/RTURP":       //入库 出库 倒库 放货成功   以及位置移动成功 /ATURPWCS1UL080578OK00NG0030##
                            dEMATICMessage.SetTURPMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            //入库完成
                            WCSTask inTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.TaskType == WCSTaskTypes.DDJStack &&x.PalletNum== message.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength) && GetDematicLocation(x.ToLocation) == message.Substring(dEMATIC_Message.MsgToLocationIndex, dEMATIC_Message.MsgToLocationLength));
                            if(inTask != null)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(inTask, WCSTaskStatus.Done);
                            }
                            //出库完成
                            WCSTask outTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.TaskType == WCSTaskTypes.DDJUnstack && x.PalletNum == message.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength));
                            if (outTask != null)
                            {
                                WMSTask wMSTask1 = WMSTasksManager.Instance.WMSTaskList.Find(x => (x.TaskType == WMSTaskType.Outbound || x.TaskType == WMSTaskType.Picking) && x.PalletNum == message.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength));
                                if(wMSTask1 != null)
                                {
                                    WCSTaskManager.Instance.UpdateWCSTaskStatus(outTask, WCSTaskStatus.Done);
                                    WMSTask wMSTask = new WMSTask();
                                    wMSTask.PalletNum = message.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength);
                                    wMSTask.TaskType = WMSTaskType.DeviceMsg;
                                    wMSTask.TaskSource = wMSTask1.TaskSource;
                                    wMSTask.TaskStatus = WMSTaskStatus.SSJTaskStart;
                                    WMSTasksManager.Instance.AddWMSTask(wMSTask);
                                }
                            }
                            WCSTask moveTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.TaskType == WCSTaskTypes.DDJStackMove && x.PalletNum == message.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength) && GetDematicLocation(x.ToLocation) == message.Substring(dEMATIC_Message.MsgToLocationIndex, dEMATIC_Message.MsgToLocationLength));
                            if (moveTask != null)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(moveTask, WCSTaskStatus.Done);
                            }
                            UpdateDematicBusyState(PLCID, false);
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/RTUEX":      //SRM发空出报警BE  双重入库BO   出深浅有SN   入深浅有DN    /ATUEXWCS1UL080258OK00NG0030##
                            FaultCode = message.Substring(dEMATIC_Message.MsgErrorIndex, dEMATIC_Message.MsgErrorLength);
                            SetDDJFault(FaultCode, message.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength));
                            
                            dEMATICMessage.SetTUEXMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/RTUCA":     //SRM取消任务完成       /ATUCAWCS1UL080259OK00NG0030##
                            WCSTask EmptyUnStackTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.PalletNum == message.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength) && x.TaskType == WCSTaskTypes.DDJUnstack && x.TaskStatus == WCSTaskStatus.UnStackEmpty);
                            if (EmptyUnStackTask != null)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(EmptyUnStackTask, WCSTaskStatus.UnStackEmptyConfirm);
                                UpdateDematicBusyState(PLCID, false);   //空出取消完成可以立即恢复堆垛机工作状态，双重不可以，需等待重新分配任务
                            }
                            WCSTask StackDoubleTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.DeviceID == PLCID && x.PalletNum == message.Substring(dEMATIC_Message.MsgPalletIndex, dEMATIC_Message.MsgPalletLength) && x.TaskType == WCSTaskTypes.DDJStack && x.TaskStatus == WCSTaskStatus.StackDouble);
                            if (StackDoubleTask != null)
                            {
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(StackDoubleTask, WCSTaskStatus.StackDoubleConfirm);
                            }
                            dEMATICMessage.SetTUCAMessage(dEMATIC_Message.MsgSender, dEMATIC_Message.MsgSeqID);
                            
                            WCSTODEMATICMessage = dEMATICMessage.Trans;
                            break;
                        case "/ATUMI":   //入库 出库 倒库任务反馈
                            WCSTODEMATICMessage = "";
                            break;
                        case "/TUMC":     //取消任务反馈
                            WCSTODEMATICMessage = "";
                            break;

                        default:
                            WCSTODEMATICMessage = "Message-Type-Error:" + message;
                            break;
                    }
                    //WCSTODEMATICMessage = dEMATICMessage.Trans;
                }
                else
                {
                    WCSTODEMATICMessage = "Message-Length-Error:" + message;
                }
                
                WCSToDEMATICMessageSend(WCSTODEMATICMessage, socketSend);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DEMATICTOWCSMessageParse 报文处理异常 "+PLCID, ex);
            }
        }
        public void WCSToDEMATICMessageSend(string wCSToDEMATICMessage,Socket socketSend)
        {
            try
            {
                
                if (wCSToDEMATICMessage == "" || wCSToDEMATICMessage == null) return;
                if (wCSToDEMATICMessage.Substring(0, 8) == "Message-")
                {
                    LogHelper.WriteLog("DEMATICToWCSMessage 异常 " + PLCID + " " + wCSToDEMATICMessage);
                    return;
                }
                string send = wCSToDEMATICMessage;
                byte[] buffer = Encoding.Default.GetBytes(send);
                socketSend.Send(buffer);
                
                //if (wCSToDEMATICMessage.Substring(2, 4) == "STEN")   //状态询问结束后才能发心跳信号
                //{
                //    CreateSendLiveToDEMATIC();
                //}
                DEMATICMessage dEMATICMessage = new DEMATICMessage(PLCID);
                dEMATICMessage.Trans = wCSToDEMATICMessage;
                dEMATICMessage.MsgSeqID = wCSToDEMATICMessage.Substring(14, 4);
                dEMATICMessage.Direction = DEMATICMessageDirection.Send;
                InsertIntoDEMATIC_Send_Buffer(dEMATICMessage);
            }
            catch(Exception ex)
            {
                LogHelper.WriteLog("WCSToDEMATICMessageSend 异常 "+PLCID, ex);
            }
        }
        private void ProceDDJStatus()
        {
            try
            {
                if (DDJStatusDataItemList.Count < 4) return;
                if (DDJStatusDataItemList[3].Value != null)
                    HasPallet = ((byte)DDJStatusDataItemList[3].Value).SelectBit(4);
                if (DDJStatusDataItemList[0].Value != null)
                {
                    //int a = (int)(DDJStatusDataItemList[0].Value);
                    MotionPosition = (int)(uint)DDJStatusDataItemList[0].Value;
                }
                if (DDJStatusDataItemList[1].Value != null)
                    LiftingPosition = (int)(uint)DDJStatusDataItemList[1].Value;
                if (DDJStatusDataItemList[2].Value != null)
                    ForkPosition = (int)(uint)DDJStatusDataItemList[2].Value;

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("RefreshDDJStatus 刷新堆垛机" + PLCID, ex);
            }
        }
        public string GetMaxID()
        {
            string MsgSeqID = "";
            int ID = 0;
            DEMATICMessage dEMATICMessage = new DEMATICMessage();
            dEMATICMessagesList = new List<DEMATICMessage>();
            DataTable dt = SQLServerHelper.DataBaseReadToTable("select max(MsgSeqID) as ID from [dbo].[DEMATIC_Send_Buffer]");
            if (string.IsNullOrEmpty(dt.Rows[0]["ID"].ToString()))
            {
                MsgSeqID = dEMATICMessage.ConvertMsgIDToString(1);
            }
            else
            {
                ID = int.Parse(dt.Rows[0]["ID"].ToString()) + 1;
                if (ID > 9999)
                {
                    SQLServerHelper.DataBaseExecute("delete from [dbo].[DEMATIC_Send_Buffer]");
                    ID = 1;
                }
                MsgSeqID = dEMATICMessage.ConvertMsgIDToString(ID);
            }
            return MsgSeqID;
        }
        public void UpdateDDJStatus(DDJDeviceWorkState device_state)
        {
            if (DDJWorkState == device_state) return;
            DDJWorkState = device_state;
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
        /// <summary>
        /// UL081032030111
        ///UL08第8巷道，1左伸，032列，03层，01深度，11最后默认
        ///UL082032030211
        ///UL08第8巷道，2右伸，032列，03层，02深度，11最后默认
        ///location=13000202     13 14 15 16 17 18
        ///                      5     6     7
        /// </summary>
        public string GetDematicLocation(string location)
        {
            if (location.Length == 4) return null;
            int location_pai = int.Parse(location.Substring(0, 2));
            string left_right = "";
            switch (int.Parse(location.Substring(0, 2))%2)
            {
                case 0:
                    left_right = "2";
                    break;
                case 1:
                    left_right = "1";
                    break;
            }
            string dematic_location = "UL0" + (Math.Ceiling((location_pai - 4) / 2.0)).ToString() + left_right + location.Substring(3, 3) + location.Substring(6, 2) + "0111";
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
        private void UpdateWMSDBDDJStatus()
        {
            WMSTask wMSTask;
            if (!Available || DDJWorkState == DDJDeviceWorkState.Offline)     //!WCSEnable
            {
                wMSTask = new WMSTask();
                wMSTask.PalletNum = PLCID.Substring(4, 1);
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.TaskSource = "WMS";
                wMSTask.ToLocation = "B";
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTask wMSTask1 = WMSTasksManager.Instance.WMSTaskList.Find(x => x.CreateTime == wMSTask.CreateTime);
                if (wMSTask1 != null)
                {
                    wMSTask.CreateTime = System.DateTime.Now.AddMilliseconds(10).ToString("yyyy-MM-dd HH:mm:ss.ffff");
                }
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
            else if (DDJWorkState == DDJDeviceWorkState.Standby && WCSEnable)
            {
                wMSTask = new WMSTask();
                wMSTask.PalletNum = PLCID.Substring(4, 1);
                wMSTask.TaskType = WMSTaskType.DeviceMsg;
                wMSTask.TaskSource = "WMS";
                wMSTask.ToLocation = "D";
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
                wMSTask.ToLocation = "@";
                wMSTask.TaskStatus = WMSTaskStatus.DeviceStatusChg;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
        }
        private void SetDDJFault(string fault_code, string pallet_num)
        {
            if (string.IsNullOrEmpty(fault_code)) return;
            UpdateDDJStatus(DDJDeviceWorkState.Fault);
            //DDJFault dDJFault = DDJFaultList.Instance.FaultList.Find(x => x.FaultCode == fault_code);
            //if (string.IsNullOrEmpty(dDJFault.FaultContent)) return;

            FaultCode = fault_code;
            //FaultContent = dDJFault.FaultContent;
            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == pallet_num && x.DeviceID == PLCID);
            if (wCSTask == null) return;
            switch (FaultCode)
            {
                case "BE":
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.UnStackEmpty);
                    break;
                case "BO":   //双重入库，设定载货台有货
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.StackDouble);
                    HasPallet = true;
                    break;
                case "SN":
                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.FarOutboundClearHave);
                    break;
                case "DN":
                    break;
            }
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
                        //UpdateWMSDBDDJStatus();
                        break;
                    default: break;
                }
            }
            //LogHelper.WriteLog("PLC通讯失败，" + PLCID + " ErrorCode = " + plc_ex.ErrorCode + " " + plc_ex.ToString());
        }

        public enum DDJDeviceWorkState
        {
            None,
            Online,
            Standby,
            Working,
            Fault,
            Offline,
            Manual,
        }
    }
}
