using BMHRI.WCS.Server.WCSProtocol;
using BMHRI.WCS.Server.Tools;
using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static BMHRI.WCS.Server.Models.GoodsLocationManager;

namespace BMHRI.WCS.Server.Models
{
    public class WebSockectServer
    {
        private static readonly Lazy<WebSockectServer> lazy = new Lazy<WebSockectServer>(() => new WebSockectServer());
        public static WebSockectServer Instance { get { return lazy.Value; } }

        public List<IWebSocketConnection> AllSockets = new List<IWebSocketConnection>();
        public List<WCSClient> wCSClients = new List<WCSClient>();
        public WebSocketServer WebSocketServer;
        public string ServerURI = "ws://0.0.0.0:8080";
        public string SeverName = "BQY";
        //public Timer RefreshTimer;
        private WebSockectServer()
        {
            FleckLog.Level = LogLevel.Debug;
            AllSockets = new List<IWebSocketConnection>();
            WMSTasksManager.Instance.WMSTaskAdded += Instance_WMSTaskAdded;
            WMSTasksManager.Instance.WMSTaskChanged += Instance_WMSTaskChanged;
            WMSTasksManager.Instance.WMSTaskDeleted += Instance_WMSTaskDeleted;
            WCSTaskManager.Instance.WCSTaskAdded += Instance_WCSTaskAdded;
            WCSTaskManager.Instance.WCSTaskChanged += Instance_WCSTaskChanged;
            WCSTaskManager.Instance.WCSTaskDeleted += Instance_WCSTaskDeleted;
            GoodsLocationManager.Instance.GoodsLocationChanged += Instance_GoodsLocationChanged;
            //foreach (DDJDevice dDJDevice in PLCDeviceManager.Instance.DDJDeviceList)
            //{
            //    //dDJDevice.DDJTaskAdded += DDJDevice_DDJTaskAdded;
            //    //dDJDevice.DDJTaskChanged += DDJDevice_DDJTaskChanged;
            //    //dDJDevice.DDJTaskDeleted += DDJDevice_DDJTaskDeleted;
            //    dDJDevice.PropertyChanged += DDJDevice_PropertyChanged;
            //}
            foreach (DeMaticDDJ deMaticDDJ in DeMaticDDJManager.Instance.DeMaticDDJList)
            {
                deMaticDDJ.PropertyChanged += DeMaticDDJDevice_PropertyChanged;
            }

            foreach (SSJDevice sSJDevice in PLCDeviceManager.Instance.SSJDeviceList)
            {
                sSJDevice.PropertyChanged += SSJDevice_PropertyChanged;
                foreach (SSJDeviceBlock sSJDeviceBlock in sSJDevice.DeviceBlockList)
                {
                    sSJDeviceBlock.PropertyChanged += SSJDeviceBlock_PropertyChanged;
                }
                //foreach (RGVDevice rGVDevice in sSJDevice.RGVDeviceList)
                //{
                //    rGVDevice.PropertyChanged += RGVDevice_PropertyChanged;
                //}

            }
        }

        private void Instance_GoodsLocationChanged(object sender, EventArgs e)
        {
            if (!(e is GoodsLocationEventArgs goodsLocationEventArgs)) return;
            SysMessage sysMessage = new SysMessage(SeverName);
            sysMessage.CreateGoodsLocationUpdateMessage(goodsLocationEventArgs.GoodsLocation);
            SendToWCSClients(sysMessage);
        }

        //private void RGVDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (!(sender is RGVDevice rGVDevice)) return;
        //    SysMessage sysMessage = new SysMessage(SeverName);
        //    sysMessage.CreateRGVDeviceBlockMessage(rGVDevice);
        //    SendToWCSClients(sysMessage);
        //}
        //private void TSJDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (!(sender is TSJDevice tSJDevice)) return;
        //    SysMessage sysMessage = new SysMessage(SeverName);
        //    sysMessage.CreateTSJDeviceBlockMessage(tSJDevice);
        //    SendToWCSClients(sysMessage);
        //}
        //private void CDJDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (!(sender is CDJDevice cDJDevice)) return;
        //    SysMessage sysMessage = new SysMessage(SeverName);
        //    sysMessage.CreateCDJDeviceBlockMessage(cDJDevice);
        //    SendToWCSClients(sysMessage);
        //}
        //private void XZTDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (!(sender is XZTDevice xZTDevice)) return;
        //    SysMessage sysMessage = new SysMessage(SeverName);
        //    sysMessage.CreateXZTDeviceBlockMessage(xZTDevice);
        //    SendToWCSClients(sysMessage);
        //}

        private void SSJDeviceBlock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is SSJDeviceBlock sSJDeviceBlock)) return;
            SysMessage sysMessage = new SysMessage(SeverName);
            sysMessage.CreateSSJDeviceBlockMessage(sSJDeviceBlock);
            SendToWCSClients(sysMessage);
        }

        public void Start()
        {
            if (WebSocketServer == null)
                WebSocketServer = new WebSocketServer(ServerURI);
            WebSocketServer.RestartAfterListenError = true;
            WebSocketServer.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    AllSockets.Add(socket);
                    WCSClient wCSClient = new WCSClient();
                    NotifyOpenEvent(socket);
                };
                socket.OnClose = () =>
                {
                    AllSockets.Remove(socket);
                    WCSClient wCSClient = wCSClients.Find(x => x.ClientGuid == socket.ConnectionInfo.Id);
                    if (wCSClient != null)
                        wCSClients.Remove(wCSClient);
                    NotifyCloseEvent(socket);
                };

                socket.OnMessage = message =>
                {
                    //AllSockets.ToList().ForEach(s => s.Send(message));       //处理客户端发来的消息？message时客户端发来的Json消息
                    SysMessage wCSMessage = CreateWCSMessageFromStr(message);
                    MsgProcess(socket, wCSMessage);
                    NotifyRecvEvent(socket);
                };
                socket.OnBinary = data =>
                {
                    string msg = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
                    SysMessage wCSMessage = CreateWCSMessageFromStr(msg);
                    MsgProcess(socket, wCSMessage);
                };

            });
        }
        public void Stop()
        {
            if (WebSocketServer != null)
            {
                AllSockets.ToList().ForEach(s => s.Close());
                WebSocketServer.Dispose();
                WebSocketServer = null;
            }
        }
        private void SendMsgOnHello()
        {
            if (wCSClients == null) return;
            if (wCSClients.Count < 1) return;
            foreach (WCSClient wCSClient in wCSClients)
            {
                SendDeviceStatusToClientOnHello(wCSClient);
            }
        }
        private void SendDeviceStatusToClientOnHello(WCSClient wCSClient)
        {
            foreach (DDJDevice dDJDevice in PLCDeviceManager.Instance.DDJDeviceList)
            {
                if (dDJDevice == null) continue;
                SysMessage sysMessage = new SysMessage(SeverName);
                sysMessage.CreateDDJDeviceMessage(dDJDevice);
                SendToWCSClients(sysMessage);
            }
            foreach (DeMaticDDJ deMaticDDJ in DeMaticDDJManager.Instance.DeMaticDDJList)
            {
                if (deMaticDDJ == null) return;
                SysMessage sysMessage = new SysMessage(SeverName);
                sysMessage.CreateDeMaticDDJDeviceMessage(deMaticDDJ);
                SendToWCSClients(sysMessage);
            }
            //foreach (FZJDevice fZJDevice in PLCDeviceManager.Instance.FZJDeviceList)
            //{
            //    if (fZJDevice == null) return;
            //    SysMessage sysMessage = new SysMessage(SeverName);
            //    sysMessage.CreateFZJDeviceMessage(fZJDevice);
            //    SendToWCSClients(sysMessage);
            //}
            foreach (SSJDevice sSJDevice in PLCDeviceManager.Instance.SSJDeviceList)
            {
                if (sSJDevice == null) continue;
                SysMessage sysMessage = new SysMessage(SeverName);
                sysMessage.CreateSSJDeviceMessage(sSJDevice);
                SendToWCSClients(sysMessage);
                foreach (SSJDeviceBlock sSJDeviceBlock in sSJDevice.DeviceBlockList)
                {
                    SysMessage sysMessage1 = new SysMessage(SeverName);
                    sysMessage1.CreateSSJDeviceBlockMessage(sSJDeviceBlock);
                    SendToWCSClients(sysMessage1);
                }
                //foreach (RGVDevice rGVDevice in sSJDevice.RGVDeviceList)
                //{
                //    SysMessage sysMessage1 = new SysMessage(SeverName);
                //    sysMessage1.CreateRGVDeviceBlockMessage(rGVDevice);
                //    SendToWCSClients(sysMessage1);
                //}

            }
        }
        private void Instance_WCSTaskDeleted(object sender, EventArgs e)   //服务端任务删除时，给客户端发删除消息
        {
            if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
            SysMessage sysMessage = new SysMessage(SeverName);
            sysMessage.CreateWCSTaskDeletedMessage(wCSTaskEventArgs.WCSTask);
            SendToWCSClients(sysMessage);
        }
        private void Instance_WCSTaskChanged(object sender, EventArgs e)
        {
            if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
            SysMessage sysMessage = new SysMessage(SeverName);
            sysMessage.CreateWCSTaskChangedMessage(wCSTaskEventArgs.WCSTask);
            SendToWCSClients(sysMessage);
        }
        private void Instance_WCSTaskAdded(object sender, EventArgs e)
        {
            if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
            SysMessage sysMessage = new SysMessage(SeverName);
            sysMessage.CreateWCSTaskAddedMessage(wCSTaskEventArgs.WCSTask);
            SendToWCSClients(sysMessage);
        }
        private void Instance_WMSTaskDeleted(object sender, EventArgs e)
        {
            if (!(e is WMSTaskEventArgs wMSTaskEventArgs)) return;
            SysMessage sysMessage = new SysMessage(SeverName);
            sysMessage.CreateWMSTaskDeletedMessage(wMSTaskEventArgs.WMSTask);
            SendToWCSClients(sysMessage);
        }
        private void Instance_WMSTaskChanged(object sender, EventArgs e)
        {
            if (!(e is WMSTaskEventArgs wMSTaskEventArgs)) return;
            SysMessage sysMessage = new SysMessage(SeverName);
            sysMessage.CreateWMSTaskChangedMessage(wMSTaskEventArgs.WMSTask);
            SendToWCSClients(sysMessage);
        }
        private void Instance_WMSTaskAdded(object sender, EventArgs e)
        {
            if (!(e is WMSTaskEventArgs wMSTaskEventArgs)) return;
            SysMessage sysMessage = new SysMessage(SeverName);
            sysMessage.CreateWMSTaskAddedMessage(wMSTaskEventArgs.WMSTask);
            SendToWCSClients(sysMessage);
        }
        //private void DDJDevice_DDJTaskDeleted(object sender, DDJTaskEventArgs e)   //良信项目单独有个堆垛机任务表，所以堆垛机任务发生改变也需要通知客户端
        //{
        //    if (!(e is DDJTaskEventArgs dDJTaskEventArgs)) return;
        //    SysMessage sysMessage = new SysMessage(SeverName);
        //    sysMessage.CreateDDJTaskDeletedMessage(dDJTaskEventArgs.DDJTask);
        //    SendToWCSClients(sysMessage);
        //}
        //private void DDJDevice_DDJTaskChanged(object sender, DDJTaskEventArgs e)
        //{
        //    if (!(e is DDJTaskEventArgs dDJTaskEventArgs)) return;
        //    SysMessage sysMessage = new SysMessage(SeverName);
        //    sysMessage.CreateDDJTaskChangedMessage(dDJTaskEventArgs.DDJTask);
        //    SendToWCSClients(sysMessage);
        //}
        //private void DDJDevice_DDJTaskAdded(object sender, DDJTaskEventArgs e)
        //{
        //    if (!(e is DDJTaskEventArgs dDJTaskEventArgs)) return;
        //    SysMessage sysMessage = new SysMessage(SeverName);
        //    sysMessage.CreateDDJTaskAddedMessage(dDJTaskEventArgs.DDJTask);
        //    SendToWCSClients(sysMessage);
        //}
        private void SSJDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "SSJWorkState"||e.PropertyName== "PLCConnectState")
            //{
                SSJDevice sSJDevice = sender as SSJDevice;
                if (sSJDevice == null) return;
                SysMessage sysMessage = new SysMessage(SeverName);
                sysMessage.CreateSSJDeviceMessage(sSJDevice);
                SendToWCSClients(sysMessage);
            //}
        }
        private void DDJDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DDJDevice sDJDevice = sender as DDJDevice;
            if (sDJDevice == null) return;
            SysMessage sysMessage = new SysMessage(SeverName);
            sysMessage.CreateDDJDeviceMessage(sDJDevice);
            SendToWCSClients(sysMessage);
        }
        private void DeMaticDDJDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DeMaticDDJ deMaticDDJ = sender as DeMaticDDJ;
            if (deMaticDDJ == null) return;
            SysMessage sysMessage = new SysMessage(SeverName);
            sysMessage.CreateDeMaticDDJDeviceMessage(deMaticDDJ);
            SendToWCSClients(sysMessage);
        }
        //private void FZJDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    FZJDevice fZJDevice = sender as FZJDevice;
        //    if (fZJDevice == null) return;
        //    SysMessage sysMessage = new SysMessage(SeverName);
        //    sysMessage.CreateFZJDeviceMessage(fZJDevice);
        //    SendToWCSClients(sysMessage);
        //}
        private void SendToWCSClients(SysMessage sysMessage)
        {
            try
            {
                foreach (WCSClient wCSClient in wCSClients)
                {
                    if ((sysMessage.MsgType == MsgType.WCSTaskMsg || sysMessage.MsgType == MsgType.WMSTaskMsg ||
                        sysMessage.MsgType == MsgType.DDJTaskMsg) && wCSClient.ClientName == "SerDataCenter") continue;
                    sysMessage.MsgID = wCSClient.MsgID;
                    sysMessage.Recver = wCSClient.ClientName;
                    wCSClient.Socket.Send(JsonConvert.SerializeObject(sysMessage));
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SendToWCSClients ", ex);
            }
        }
        public void SendToLCD(string taskInforToLCD)
        {
            SysMessage wCSMsg = new SysMessage(MsgType.LCDMessage, SeverName, "MsgLCD", 12, taskInforToLCD, null, null);
            Broadcast(wCSMsg);
        }

        public void RefreshLCD()
        {
            SysMessage wCSMsg = new SysMessage(MsgType.LCDMessage, SeverName, "MsgLCD", 12, "RefreshLCD", null, null);
            Broadcast(wCSMsg);
        }
        private SysMessage CreateWCSMessageFromStr(string msg)
        {
            SysMessage wCSMessage = null;
            try
            {
                wCSMessage = JsonConvert.DeserializeObject<SysMessage>(msg);
            }
            catch (Exception ex)
            {
                //"Unable to find a constructor to use for type BMHRI.WCS.Server.PLCProtocol.WCSProtocol.SysMessage." +
                //   " A class should either have a default constructor, one constructor with arguments " +
                //   "or a constructor marked with the JsonConstructor attribute." +
                //   " Path 'MsgType', line 1, position 11."
                ex.ToString();
                return wCSMessage;
            }
            return wCSMessage;
        }

        private void MsgProcess(IWebSocketConnection socket, SysMessage sysMessage)    //处理客户端发来的消息？
        {
            if (sysMessage == null) return;

            SysMessage sysmsg = null;
            switch (sysMessage.MsgType)
            {
                case MsgType.HeartBeatAsk:    //客户端每隔10s发送一次  
                    sysmsg = new SysMessage(MsgType.HeartBeatAns, SeverName, sysMessage.Sender, sysMessage.MsgID++, "IAOK", null, null);
                    socket.Send(JsonConvert.SerializeObject(sysmsg));
                    break;
                case MsgType.Hello:          //客户端开启瞬间发送一次  服务端先给客户端发送一次设备状态
                    WCSClient wCSClient = wCSClients.Find(x => x.ClientGuid == socket.ConnectionInfo.Id);
                    if (wCSClient == null)
                    {
                        wCSClient = new WCSClient();
                        wCSClient.ClientGuid = socket.ConnectionInfo.Id;
                        wCSClient.ClientIP = socket.ConnectionInfo.ClientIpAddress;
                        wCSClient.ClientType = sysMessage.Sender;
                        wCSClient.ClientName = sysMessage.Sender;
                        wCSClient.Socket = socket;
                        wCSClients.Add(wCSClient);
                    }
                    sysmsg = new SysMessage(MsgType.HeartBeatAns, SeverName, sysMessage.Sender, wCSClient.MsgID, "IAOK", null, null);
                    socket.Send(JsonConvert.SerializeObject(sysmsg));
                    SendDeviceStatusToClientOnHello(wCSClient);
                    break;
                case MsgType.WCSTaskMsg:
                    ProcessWCSTaskMsg(socket, sysMessage);
                    break;
                case MsgType.WMSTaskMsg:
                    ProcessWMSTaskMsg(socket, sysMessage);
                    break;
                case MsgType.DDJOptMsg:
                    ProcessDDJOptMsg(socket, sysMessage);
                    break;
                case MsgType.SSJOptMsg:
                    ProcessSSJOptMsg(socket, sysMessage);
                    break;
                //case MsgType.DDJTaskMsg:
                //    ProcessDDJTaskMsg(socket, sysMessage);
                //    break;
                default:
                    break;
            }
        }
        private void LogReceiveMsg(WCSClient wCSClient, SysMessage sysMessage)
        {
            if (wCSClient == null || sysMessage == null) return;
        }
        private void ProcessSSJOptMsg(IWebSocketConnection socket, SysMessage sysMessage)
        {
            WCSClient wCSClient = wCSClients.Find(x => x.ClientGuid == socket.ConnectionInfo.Id);
            if (wCSClient == null) return;
            SSJOptMsg deviceOptMsg = JsonConvert.DeserializeObject<SSJOptMsg>(sysMessage.MsgBody);
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == deviceOptMsg.DeviceID);
            if (sSJDevice == null) return;
            switch (deviceOptMsg.OptType)
            {
                case SSJOptType.ClearOpty:
                    string palletNum = deviceOptMsg.Var.ToString();
                    if (string.IsNullOrEmpty(palletNum)) return;
                    sSJDevice.ClearOcupty(palletNum);
                    break;
                case SSJOptType.Online:
                    sSJDevice.Online();
                    break;
                case SSJOptType.Offline:
                    sSJDevice.OffLine();
                    break;
                case SSJOptType.Disable:
                    break;
                case SSJOptType.Disconnect:
                    sSJDevice.Disconnect();
                    break;
                case SSJOptType.Connect:
                    sSJDevice.Connect();
                    break;
                default:
                    break;
            }
        }
        private void ProcessDDJOptMsg(IWebSocketConnection socket, SysMessage wCSMessage)
        {
            WCSClient wCSClient = wCSClients.Find(x => x.ClientGuid == socket.ConnectionInfo.Id);
            if (wCSClient == null) return;
            DDJOptMsg deviceOptMsg = JsonConvert.DeserializeObject<DDJOptMsg>(wCSMessage.MsgBody);
            DeMaticDDJ dDJDevice = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == deviceOptMsg.DeviceID);
            if (dDJDevice == null) return;
            switch (deviceOptMsg.OptType)
            {
                //case DDJOptType.RecallIn:
                //    string place = deviceOptMsg.Var.ToString();
                //    if (string.IsNullOrEmpty(place)) return;
                //    dDJDevice.RecallInPlace(place);
                //    break;
                case DDJOptType.Online:
                    dDJDevice.AvailabilityBT_Click(deviceOptMsg.DeviceID, "");
                    break;
                case DDJOptType.Offline:
                    dDJDevice.UnableBT_Click(deviceOptMsg.DeviceID, "");
                    break;
                //case DDJOptType.Disable:
                //    break;
                //case DDJOptType.Disconnect:
                //    dDJDevice.Disconnect();
                //    break;
                //case DDJOptType.Connect:
                //    dDJDevice.Connect();
                //    break;
                //case DDJOptType.DDJDoubleUp:
                //    if (deviceOptMsg.Var == null) return;
                //    dDJDevice.DoubleInboundConfirm((int)deviceOptMsg.Var);
                //    break;
                //case DDJOptType.DDJEmptyOutBound:
                //    if (deviceOptMsg.Var == null) return;
                //    dDJDevice.EmtpyOutboundConfirm((int)deviceOptMsg.Var);
                //    break;
                //case DDJOptType.DDJEmptyPick:
                //    if (deviceOptMsg.Var == null) return;
                //    dDJDevice.DoubleInboundConfirm((int)deviceOptMsg.Var);
                //    break;
                //case DDJOptType.DDJUnreach:
                //    break;
                //case DDJOptType.SPalletStack:
                //    if (deviceOptMsg.Var == null) return;
                //    dDJDevice.CreateSinglePalletStack(deviceOptMsg.Var.ToString());
                //    break;
                case DDJOptType.WCSEnableReset:
                    dDJDevice.WCSEnable = true;
                    break;
                default:
                    break;
            }
        }
        private void ProcessWMSTaskMsg(IWebSocketConnection socket, SysMessage sysMessage)
        {
            WCSClient wCSClient = wCSClients.Find(x => x.ClientGuid == socket.ConnectionInfo.Id);
            if (wCSClient == null) return;
            WMSTaskMsg wMSTaskMsg = JsonConvert.DeserializeObject<WMSTaskMsg>(sysMessage.MsgBody);
            switch (wMSTaskMsg.TaskOptType)
            {
                case TaskOptType.Added:
                    WMSTasksManager.Instance.AddWMSTask(wMSTaskMsg.WMSTask);
                    break;
                case TaskOptType.Changed:
                    WMSTasksManager.Instance.UpdateWMSTask(wMSTaskMsg.WMSTask);
                    break;
                case TaskOptType.Deleted:
                    WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTaskMsg.WMSTask.WMSSeqID);
                    break;
                default:
                    break;
            }
        }
        private void ProcessWCSTaskMsg(IWebSocketConnection socket, SysMessage sysMessage)
        {
            WCSClient wCSClient = wCSClients.Find(x => x.ClientGuid == socket.ConnectionInfo.Id);
            if (wCSClient == null) return;
            WCSTaskMsg wCSTaskMsg = JsonConvert.DeserializeObject<WCSTaskMsg>(sysMessage.MsgBody);
            switch (wCSTaskMsg.TaskOptType)
            {
                case TaskOptType.Added:
                    WCSTaskManager.Instance.AddWCSTask(wCSTaskMsg.WCSTask);
                    break;
                case TaskOptType.Changed:
                    WCSTaskManager.Instance.UpdateWCSTask(wCSTaskMsg.WCSTask);
                    if (wCSTaskMsg.WCSTask.TaskStatus == WCSTaskStatus.Done && (wCSTaskMsg.WCSTask.TaskType == WCSTaskTypes.DDJStack || wCSTaskMsg.WCSTask.TaskType == WCSTaskTypes.DDJUnstack || wCSTaskMsg.WCSTask.TaskType == WCSTaskTypes.DDJStackMove || wCSTaskMsg.WCSTask.TaskType == WCSTaskTypes.DDJDirect))
                    {
                        DeMaticDDJ deMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == wCSTaskMsg.WCSTask.DeviceID);
                        if (deMaticDDJ != null)
                        {
                            deMaticDDJ.WCSEnable = true;
                        }
                    }
                    break;
                case TaskOptType.Deleted:
                    WCSTaskManager.Instance.DeleteWCSTask(wCSTaskMsg.WCSTask.WCSSeqID);
                    break;
                default:
                    break;
            }
        }
        //private void ProcessDDJTaskMsg(IWebSocketConnection socket, SysMessage sysMessage)
        //{
        //    WCSClient wCSClient = wCSClients.Find(x => x.ClientGuid == socket.ConnectionInfo.Id);
        //    if (wCSClient == null) return;
        //    DDJTaskMsg dDJTaskMsg = JsonConvert.DeserializeObject<DDJTaskMsg>(sysMessage.MsgBody);
        //    DDJDevice dDJDevice = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == dDJTaskMsg.DDJTask.DeviceID);
        //    if (dDJDevice == null) return;
        //    switch (dDJTaskMsg.TaskOptType)
        //    {
        //        case TaskOptType.Added:
        //            dDJDevice.AddDDJTask(dDJTaskMsg.DDJTask);
        //            break;
        //        case TaskOptType.Changed:
        //            break;
        //        case TaskOptType.Deleted:
        //            dDJDevice.DeleteDDJTask(dDJTaskMsg.DDJTask);
        //            break;
        //        default:
        //            break;
        //    }
        //}
        public void Broadcast(SysMessage sysMessage)
        {
            if (sysMessage is null) return;

            foreach (var socket in AllSockets.ToList())
            {
                socket.Send(JsonConvert.SerializeObject(sysMessage));
                NotifySendEvent();
            }
        }

        public void Broadcast(string msgStr)
        {
            foreach (var socket in AllSockets.ToList())
            {
                socket.Send(msgStr);
                NotifySendEvent();
            }
        }

        private void NotifySendEvent()
        {

        }

        private void NotifyOpenEvent(IWebSocketConnection socket)
        {

        }

        private void NotifyCloseEvent(IWebSocketConnection socket)
        {

        }

        private void NotifyRecvEvent(IWebSocketConnection socket)
        {

        }
    }
}
