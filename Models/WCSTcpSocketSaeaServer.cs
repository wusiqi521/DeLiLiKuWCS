using BMHRI.WCS.Server.WCSProtocol;
using Cowboy.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BMHRI.WCS.Server.Models
{
    //class WCSTcpSocketSaeaServer : ITcpSocketSaeaServerMessageDispatcher, INotifyPropertyChanged
    //{
    //    public List<TcpSocketSaeaSession> MyTcpSocketSaeaSessions;
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    public void Notify(string propertyName)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //    public EventHandler<string> SaeaServerMessageRecved;
    //    public EventHandler<string> SaeaServerMessageSended;
    //    public EventHandler<TcpSocketSaeaSession> SaeaServerSessionClosed;
    //    public EventHandler<TcpSocketSaeaSession> SaeaServerSessionStarted;
    //    public string SeverName = "BHMRI";
    //    public List<TcpSocketSaeaSession> TcpSocketSaeaSessionList;
    //    public List<WCSTcpClient> WCSClientList;
    //    public bool IsListening
    //    {
    //        get { return MyTcpSocketSaeaServer.IsListening; }
    //    }
    //    private string ip;
    //    public string IP
    //    {
    //        set
    //        {
    //            if (!ip.Equals(value))
    //            {
    //                ip = value;
    //                Notify("IP");
    //            }
    //        }
    //        get
    //        {
    //            return ip;
    //        }
    //    }
    //    private int port;
    //    public int Port
    //    {
    //        set
    //        {
    //            if (!port.Equals(value))
    //            {
    //                port = value;
    //                Notify("Port");
    //            }
    //        }
    //        get
    //        {
    //            return port;
    //        }
    //    }
    //    private static readonly Lazy<WCSTcpSocketSaeaServer> lazy = new Lazy<WCSTcpSocketSaeaServer>(() => new WCSTcpSocketSaeaServer());
    //    public static WCSTcpSocketSaeaServer Instance { get { return lazy.Value; } }
    //    private WCSTcpSocketSaeaServer()
    //    {
    //        var config = new TcpSocketSaeaServerConfiguration
    //        {
    //            //config.UseSsl = true;
    //            //config.SslServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(@"D:\\Cowboy.pfx", "Cowboy");
    //            //config.SslPolicyErrorsBypassed = false;

    //            //config.FrameBuilder = new FixedLengthFrameBuilder(20000);
    //            FrameBuilder = new RawBufferFrameBuilder()
    //            //config.FrameBuilder = new LineBasedFrameBuilder();
    //            //config.FrameBuilder = new LengthPrefixedFrameBuilder();
    //            //config.FrameBuilder = new LengthFieldBasedFrameBuilder();
    //        };
    //        WCSClientList = new List<WCSTcpClient>();
    //        WMSTasksManager.Instance.WMSTaskAdded += Instance_WMSTaskAdded;
    //        WMSTasksManager.Instance.WMSTaskChanged += Instance_WMSTaskChanged;
    //        WMSTasksManager.Instance.WMSTaskDeleted += Instance_WMSTaskDeleted;
    //        WCSTaskManager.Instance.WCSTaskAdded += Instance_WCSTaskAdded;
    //        WCSTaskManager.Instance.WCSTaskChanged += Instance_WCSTaskChanged;
    //        WCSTaskManager.Instance.WCSTaskDeleted += Instance_WCSTaskDeleted;
    //        foreach (DeMaticDDJ deMaticDDJ in DeMaticDDJManager.Instance.DeMaticDDJList)
    //        {
    //            deMaticDDJ.DDJTaskAdded += DDJDevice_DDJTaskAdded;
    //            deMaticDDJ.DDJTaskChanged += DDJDevice_DDJTaskChanged;
    //            deMaticDDJ.DDJTaskDeleted += DDJDevice_DDJTaskDeleted;
    //            deMaticDDJ.PropertyChanged += DDJDevice_PropertyChanged;
    //        }
    //        foreach (SSJDevice sSJDevice in PLCDeviceManager.Instance.SSJDeviceList)
    //        {
    //            sSJDevice.PropertyChanged += SSJDevice_PropertyChanged;
    //            foreach (SSJDeviceBlock sSJDeviceBlock in sSJDevice.DeviceBlockList)
    //            {
    //                sSJDeviceBlock.PropertyChanged += SSJDeviceBlock_PropertyChanged;
    //            }
    //            foreach (RGVDevice rGVDevice in sSJDevice.RGVDeviceList)
    //            {
    //                rGVDevice.PropertyChanged += RGVDevice_PropertyChanged;
    //            }
    //        }
    //        MyTcpSocketSaeaServer = new TcpSocketSaeaServer(IPAddress.Parse("0.0.0.0"), 60000, this, config);
    //        MyTcpSocketSaeaSessions = new List<TcpSocketSaeaSession>();
    //    }
    //    public void Start()
    //    {
    //        if (MyTcpSocketSaeaServer == null) return;
    //        MyTcpSocketSaeaServer.Listen();
    //    }
    //    public void Stop()
    //    {
    //        if (MyTcpSocketSaeaServer == null) return;
    //        MyTcpSocketSaeaServer.Shutdown();
    //    }
    //    public void BroadcastAsync(string str)
    //    {
    //        byte[] data = Encoding.Default.GetBytes(str);
    //        MyTcpSocketSaeaServer.BroadcastAsync(data);
    //        SaeaServerMessageSended?.Invoke(this, str);
    //    }

    //    public void SendToAsync(string sessionkey, string str)
    //    {
    //        byte[] byteArray = Encoding.Default.GetBytes(str);
    //        MyTcpSocketSaeaServer.SendToAsync(sessionkey, byteArray);
    //        SaeaServerMessageSended?.Invoke(this, str);
    //    }
    //    private readonly TcpSocketSaeaServer MyTcpSocketSaeaServer;
    //    public Task OnSessionClosed(TcpSocketSaeaSession session)
    //    {
    //        return Task.Factory.StartNew(() => ProcessSessionClossed(session));
    //    }
    //    public Task OnSessionDataReceived(TcpSocketSaeaSession session, byte[] data, int offset, int count)
    //    {
    //        string text = Encoding.UTF8.GetString(data, offset, count);
    //        //MyTcpSocketSaeaServer.SendToAsync(session.SessionKey, data);
    //        return Task.Factory.StartNew(() => ProcessReceivedData(session, text));
    //    }
    //    public Task OnSessionStarted(TcpSocketSaeaSession session)
    //    {
    //        return Task.Factory.StartNew(() => ProcessSessionStarted(session));
    //    }
    //    private SysMessage CreateWCSMessageFromStr(string msg)
    //    {
    //        SysMessage wCSMessage = null;
    //        try
    //        {
    //            wCSMessage = JsonConvert.DeserializeObject<SysMessage>(msg);
    //        }
    //        catch (Exception ex)
    //        {
    //            //"Unable to find a constructor to use for type BMHRI.WCS.Server.PLCProtocol.WCSProtocol.SysMessage." +
    //            //   " A class should either have a default constructor, one constructor with arguments " +
    //            //   "or a constructor marked with the JsonConstructor attribute." +
    //            //   " Path 'MsgType', line 1, position 11."
    //            ex.ToString();
    //            return wCSMessage;
    //        }
    //        return wCSMessage;
    //    }
    //    private void MsgProcess(TcpSocketSaeaSession session, SysMessage sysMessage)
    //    {
    //        if (sysMessage == null) return;

    //        SysMessage sysmsg = null;
    //        switch (sysMessage.MsgType)
    //        {
    //            case MsgType.HeartBeatAsk:
    //                sysmsg = new SysMessage(MsgType.HeartBeatAns, SeverName, sysMessage.Sender, sysMessage.MsgID++, "IAOK", null, null);
    //                SendToAsync(session.SessionKey, JsonConvert.SerializeObject(sysmsg));
    //                break;
    //            case MsgType.Hello:
    //                WCSTcpClient wCSClient = WCSClientList.Find(x => x.SessionKey == session.SessionKey);
    //                if (wCSClient == null)
    //                {
    //                    wCSClient = new WCSTcpClient();
    //                    wCSClient.SessionKey = session.SessionKey;
    //                    wCSClient.ClientIP = session.RemoteEndPoint.Address;
    //                    wCSClient.ClientType = sysMessage.Sender;
    //                    wCSClient.ClientName = sysMessage.Sender;
    //                    WCSClientList.Add(wCSClient);
    //                }
    //                sysmsg = new SysMessage(MsgType.HeartBeatAns, SeverName, sysMessage.Sender, wCSClient.MsgID, "IAOK", null, null);
    //                SendToAsync(session.SessionKey, JsonConvert.SerializeObject(sysmsg));
    //                SendDeviceStatusToClientOnHello(wCSClient);
    //                break;
    //            case MsgType.WCSTaskMsg:
    //                ProcessWCSTaskMsg(session, sysMessage);
    //                break;
    //            case MsgType.WMSTaskMsg:
    //                ProcessWMSTaskMsg(session, sysMessage);
    //                break;
    //            case MsgType.DDJOptMsg:
    //                ProcessDDJOptMsg(session, sysMessage);
    //                break;
    //            case MsgType.SSJOptMsg:
    //                ProcessSSJOptMsg(session, sysMessage);
    //                break;
    //            case MsgType.DDJTaskMsg:
    //                ProcessDDJTaskMsg(session, sysMessage);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    private void ProcessSessionStarted(TcpSocketSaeaSession session)
    //    {
    //        MyTcpSocketSaeaSessions.Add(session);
    //        SaeaServerSessionStarted?.Invoke(this, session);
    //    }
    //    private void ProcessReceivedData(TcpSocketSaeaSession session, string text)
    //    {
    //        SysMessage wCSMessage = CreateWCSMessageFromStr(text);
    //        MsgProcess(session, wCSMessage);
    //        SaeaServerMessageRecved?.Invoke(this, text);
    //    }
    //    private void ProcessSessionClossed(TcpSocketSaeaSession session)
    //    {
    //        MyTcpSocketSaeaSessions.Remove(session);
    //        SaeaServerSessionClosed?.Invoke(this, session);
    //    }
    //    private void RGVDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //    {
    //        if (!(sender is RGVDevice rGVDevice)) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateRGVDeviceBlockMessage(rGVDevice);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void SSJDeviceBlock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //    {
    //        if (!(sender is SSJDeviceBlock sSJDeviceBlock)) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateSSJDeviceBlockMessage(sSJDeviceBlock);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void SendDeviceStatusToClientOnHello(WCSTcpClient wCSClient)
    //    {
    //        foreach (DDJDevice dDJDevice in PLCDeviceManager.Instance.DDJDeviceList)
    //        {
    //            if (dDJDevice == null) continue;
    //            SysMessage sysMessage = new SysMessage(SeverName);
    //            sysMessage.CreateDDJDeviceMessage(dDJDevice);
    //            SendToWCSClients(sysMessage);
    //        }
    //        foreach (SSJDevice sSJDevice in PLCDeviceManager.Instance.SSJDeviceList)
    //        {
    //            if (sSJDevice == null) continue;
    //            SysMessage sysMessage = new SysMessage(SeverName);
    //            sysMessage.CreateSSJDeviceMessage(sSJDevice);
    //            SendToWCSClients(sysMessage);
    //            foreach (SSJDeviceBlock sSJDeviceBlock in sSJDevice.DeviceBlockList)
    //            {
    //                SysMessage sysMessage1 = new SysMessage(SeverName);
    //                sysMessage1.CreateSSJDeviceBlockMessage(sSJDeviceBlock);
    //                SendToWCSClients(sysMessage1);
    //            }
    //            foreach (RGVDevice rGVDevice in sSJDevice.RGVDeviceList)
    //            {
    //                SysMessage sysMessage1 = new SysMessage(SeverName);
    //                sysMessage1.CreateRGVDeviceBlockMessage(rGVDevice);
    //                SendToWCSClients(sysMessage1);
    //            }
    //        }
    //    }
    //    private void Instance_WCSTaskDeleted(object sender, EventArgs e)
    //    {
    //        if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateWCSTaskDeletedMessage(wCSTaskEventArgs.WCSTask);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void Instance_WCSTaskChanged(object sender, EventArgs e)
    //    {
    //        if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateWCSTaskChangedMessage(wCSTaskEventArgs.WCSTask);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void Instance_WCSTaskAdded(object sender, EventArgs e)
    //    {
    //        if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateWCSTaskAddedMessage(wCSTaskEventArgs.WCSTask);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void Instance_WMSTaskDeleted(object sender, EventArgs e)
    //    {
    //        if (!(e is WMSTaskEventArgs wMSTaskEventArgs)) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateWMSTaskDeletedMessage(wMSTaskEventArgs.WMSTask);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void Instance_WMSTaskChanged(object sender, EventArgs e)
    //    {
    //        if (!(e is WMSTaskEventArgs wMSTaskEventArgs)) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateWMSTaskChangedMessage(wMSTaskEventArgs.WMSTask);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void Instance_WMSTaskAdded(object sender, EventArgs e)
    //    {
    //        if (!(e is WMSTaskEventArgs wMSTaskEventArgs)) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateWMSTaskAddedMessage(wMSTaskEventArgs.WMSTask);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void DDJDevice_DDJTaskDeleted(object sender, DDJTaskEventArgs e)
    //    {
    //        if (!(e is DDJTaskEventArgs dDJTaskEventArgs)) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateDDJTaskDeletedMessage(dDJTaskEventArgs.DDJTask);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void DDJDevice_DDJTaskChanged(object sender, DDJTaskEventArgs e)
    //    {
    //        if (!(e is DDJTaskEventArgs dDJTaskEventArgs)) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateDDJTaskChangedMessage(dDJTaskEventArgs.DDJTask);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void DDJDevice_DDJTaskAdded(object sender, DDJTaskEventArgs e)
    //    {
    //        if (!(e is DDJTaskEventArgs dDJTaskEventArgs)) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateDDJTaskAddedMessage(dDJTaskEventArgs.DDJTask);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void SSJDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //    {
    //        if (e.PropertyName == "SSJWorkState")
    //        {
    //            SSJDevice sSJDevice = sender as SSJDevice;
    //            if (sSJDevice == null) return;
    //            SysMessage sysMessage = new SysMessage(SeverName);
    //            sysMessage.CreateSSJDeviceMessage(sSJDevice);
    //            SendToWCSClients(sysMessage);
    //        }
    //    }
    //    private void DDJDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //    {
    //        DDJDevice sDJDevice = sender as DDJDevice;
    //        if (sDJDevice == null) return;
    //        SysMessage sysMessage = new SysMessage(SeverName);
    //        sysMessage.CreateDDJDeviceMessage(sDJDevice);
    //        SendToWCSClients(sysMessage);
    //    }
    //    private void ProcessSSJOptMsg(TcpSocketSaeaSession socket, SysMessage sysMessage)
    //    {
    //        WCSTcpClient wCSClient = WCSClientList.Find(x => x.SessionKey == socket.SessionKey);
    //        if (wCSClient == null) return;
    //        SSJOptMsg deviceOptMsg = JsonConvert.DeserializeObject<SSJOptMsg>(sysMessage.MsgBody);
    //        SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == deviceOptMsg.DeviceID);
    //        if (sSJDevice == null) return;
    //        switch (deviceOptMsg.OptType)
    //        {
    //            case SSJOptType.ClearOpty:
    //                string palletNum = deviceOptMsg.Var.ToString();
    //                if (string.IsNullOrEmpty(palletNum)) return;
    //                sSJDevice.ClearOcupty(palletNum);
    //                break;
    //            case SSJOptType.Online:
    //                sSJDevice.Online();
    //                break;
    //            case SSJOptType.Offline:
    //                sSJDevice.OffLine();
    //                break;
    //            case SSJOptType.Disable:
    //                break;
    //            case SSJOptType.Disconnect:
    //                sSJDevice.Disconnect();
    //                break;
    //            case SSJOptType.Connect:
    //                sSJDevice.Connect();
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    private void ProcessDDJOptMsg(TcpSocketSaeaSession socket, SysMessage wCSMessage)
    //    {
    //        WCSTcpClient wCSClient = WCSClientList.Find(x => x.SessionKey == socket.SessionKey);
    //        if (wCSClient == null) return;
    //        DDJOptMsg deviceOptMsg = JsonConvert.DeserializeObject<DDJOptMsg>(wCSMessage.MsgBody);
    //        DDJDevice dDJDevice = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == deviceOptMsg.DeviceID);
    //        if (dDJDevice == null) return;
    //        switch (deviceOptMsg.OptType)
    //        {
    //            case DDJOptType.RecallIn:
    //                string place = deviceOptMsg.Var.ToString();
    //                if (string.IsNullOrEmpty(place)) return;
    //                dDJDevice.RecallInPlace(place);
    //                break;
    //            case DDJOptType.Online:
    //                dDJDevice.Online();
    //                break;
    //            case DDJOptType.Offline:
    //                dDJDevice.OffLine();
    //                break;
    //            case DDJOptType.Disable:
    //                break;
    //            case DDJOptType.Disconnect:
    //                dDJDevice.Disconnect();
    //                break;
    //            case DDJOptType.Connect:
    //                dDJDevice.Connect();
    //                break;
    //            case DDJOptType.DDJDoubleUp:
    //                if (deviceOptMsg.Var == null) return;
    //                dDJDevice.DoubleInboundConfirm((int)deviceOptMsg.Var);
    //                break;
    //            case DDJOptType.DDJEmptyOutBound:
    //                if (deviceOptMsg.Var == null) return;
    //                dDJDevice.EmtpyOutboundConfirm((int)deviceOptMsg.Var);
    //                break;
    //            case DDJOptType.DDJEmptyPick:
    //                if (deviceOptMsg.Var == null) return;
    //                dDJDevice.DoubleInboundConfirm((int)deviceOptMsg.Var);
    //                break;
    //            case DDJOptType.DDJUnreach:
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    private void ProcessWMSTaskMsg(TcpSocketSaeaSession socket, SysMessage sysMessage)
    //    {
    //        WCSTcpClient wCSClient = WCSClientList.Find(x => x.SessionKey == socket.SessionKey);
    //        if (wCSClient == null) return;
    //        WMSTaskMsg wMSTaskMsg = JsonConvert.DeserializeObject<WMSTaskMsg>(sysMessage.MsgBody);
    //        switch (wMSTaskMsg.TaskOptType)
    //        {
    //            case TaskOptType.Added:
    //                WMSTasksManager.Instance.AddWMSTask(wMSTaskMsg.WMSTask);
    //                break;
    //            case TaskOptType.Changed:
    //                WMSTasksManager.Instance.UpdateWMSTask(wMSTaskMsg.WMSTask);
    //                break;
    //            case TaskOptType.Deleted:
    //                WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTaskMsg.WMSTask);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    private void ProcessWCSTaskMsg(TcpSocketSaeaSession socket, SysMessage sysMessage)
    //    {
    //        WCSTcpClient wCSClient = WCSClientList.Find(x => x.SessionKey == socket.SessionKey);
    //        if (wCSClient == null) return;
    //        WCSTaskMsg wCSTaskMsg = JsonConvert.DeserializeObject<WCSTaskMsg>(sysMessage.MsgBody);
    //        switch (wCSTaskMsg.TaskOptType)
    //        {
    //            case TaskOptType.Added:
    //                WCSTaskManager.Instance.AddWCSTask(wCSTaskMsg.WCSTask);
    //                break;
    //            case TaskOptType.Changed:
    //                WCSTaskManager.Instance.UpdateWCSTask(wCSTaskMsg.WCSTask);
    //                break;
    //            case TaskOptType.Deleted:
    //                WCSTaskManager.Instance.DeleteWCSTask(wCSTaskMsg.WCSTask.WCSSeqID);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    private void ProcessDDJTaskMsg(TcpSocketSaeaSession socket, SysMessage sysMessage)
    //    {
    //        WCSTcpClient wCSClient = WCSClientList.Find(x => x.SessionKey == socket.SessionKey);
    //        if (wCSClient == null) return;
    //        DDJTaskMsg dDJTaskMsg = JsonConvert.DeserializeObject<DDJTaskMsg>(sysMessage.MsgBody);
    //        DDJDevice dDJDevice = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == dDJTaskMsg.DDJTask.DeviceID);
    //        if (dDJDevice == null) return;
    //        switch (dDJTaskMsg.TaskOptType)
    //        {
    //            case TaskOptType.Added:
    //                dDJDevice.AddDDJTask(dDJTaskMsg.DDJTask);
    //                break;
    //            case TaskOptType.Changed:
    //                break;
    //            case TaskOptType.Deleted:
    //                dDJDevice.DeleteDDJTask(dDJTaskMsg.DDJTask);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    private void SendToWCSClients(SysMessage sysMessage)
    //    {
    //        BroadcastAsync(JsonConvert.SerializeObject(sysMessage));
    //    }
    //}
}
