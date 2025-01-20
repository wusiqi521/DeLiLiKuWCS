using BMHRI.WCS.Server.Tools;
using S7.Net;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BMHRI.WCS.Server.Models
{
    public class FZJDevice : PLCDevice
    {
        #region 变量
        public List<SSJDeviceBlock> DeviceBlockList;
        public List<DataItem> DataItemList;
        private List<DataItem> ModifyDataItemList;
        public List<RGVDevice> FZJDeviceList;
        private readonly bool AutoConnect;
        #endregion
        #region 属性
        private FZJDeviceWorkState fzj_work_state;
        public FZJDeviceWorkState FZJWorkState
        {
            get { return fzj_work_state; }
            set
            {
                if (fzj_work_state != value)
                {
                    fzj_work_state = value;
                    Notify(nameof(FZJWorkState));
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
        private string emergencyStop;
        public string EmergencyStop
        {
            get { return emergencyStop; }
            set
            {
                emergencyStop = value;
                Notify(nameof(EmergencyStop));
            }
        }
        private string manuAuto;
        public string ManuAuto
        {
            get { return manuAuto; }
            set
            {
                manuAuto = value;
                Notify(nameof(ManuAuto));
            }
        }
        private string vertival;
        public string Vertival
        {
            get { return vertival; }
            set
            {
                vertival = value;
                Notify(nameof(Vertival));
            }
        }
        private string horizontal;
        public string Horizontal
        {
            get { return horizontal; }
            set
            {
                horizontal = value;
                Notify(nameof(Horizontal));
            }
        }
        private string putMateriStretch;
        public string PutMateriStretch
        {
            get { return putMateriStretch; }
            set
            {
                putMateriStretch = value;
                Notify(nameof(PutMateriStretch));
            }
        }
        private string putMateriShrink;
        public string PutMateriShrink
        {
            get { return putMateriShrink; }
            set
            {
                putMateriShrink = value;
                Notify(nameof(PutMateriShrink));
            }
        }
        private string palletForward;
        public string PalletForward
        {
            get { return palletForward; }
            set
            {
                palletForward = value;
                Notify(nameof(PalletForward));
            }
        }
        private string palletBack;
        public string PalletBack
        {
            get { return palletBack; }
            set
            {
                palletBack = value;
                Notify(nameof(PalletBack));
            }
        }
        private string cylinderHold;
        public string CylinderHold
        {
            get { return cylinderHold; }
            set
            {
                cylinderHold = value;
                Notify(nameof(CylinderHold));
            }
        }
        private string cylinderRelease;
        public string CylinderRelease
        {
            get { return cylinderRelease; }
            set
            {
                cylinderRelease = value;
                Notify(nameof(CylinderRelease));
            }
        }



        #endregion
        public FZJDevice(string cpuType, string Ip, string slot, string rack, string decription, string device_type, string plcid) : base(cpuType, Ip, slot, rack, decription, device_type, plcid)
        {
            AutoConnect = true;

            ReadFZJDataConfig();

            Task.Factory.StartNew(() => PLCCommunication(), TaskCreationOptions.LongRunning);
        }
        private void ReadFZJDataConfig()
        {
            if (DataItemList == null) DataItemList = new List<DataItem>();
            if (File.Exists(Environment.CurrentDirectory + "\\Configure" + "\\FZJ_Data.config"))
            {
                try
                {
                    XElement xDoc = XElement.Load(Environment.CurrentDirectory + "\\Configure" + "\\FZJ_Data.config");
                    var item = (from ele in xDoc.Elements("DeviceData")
                                where ele.Attribute("DeviceType").Value == "FZJ"
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
                            data_item.BitAdr= (byte)int.Parse(dNode.Attribute("BitAdr").Value);
                            data_item.Count = int.Parse(dNode.Attribute("Count").Value);
                            DataItemList.Add(data_item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("翻转机配置文件读取异常", ex);
                }
            }
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
        private void PLCCommunication()
        {
            while (true)
            {
                try
                {
                    if (PLCConnectState == ConnectionStates.Connected)
                    {
                        Plc.ReadMultipleVars(DataItemList);
                        RefreshFZJStatus();
                    }
                    else
                    {
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
                        UpdateDeviceState(FZJDeviceWorkState.None);
                    }

                    //LogHelper.WriteLog("读取PLC失败，" + PLCID + " " + plc_ex.ToString());
                    Task.Delay(3000).Wait();
                }
            }
        }
        private void RefreshFZJStatus()
        {
            try
            {
                if (DataItemList.Count < 11) return;
                if (DataItemList[0].Value != null)
                    FaultCode = DataItemList[0].Value.ToString();
                if (DataItemList[1].Value != null)
                    EmergencyStop = DataItemList[1].Value.ToString();
                if (DataItemList[2].Value != null)
                    ManuAuto = DataItemList[2].Value.ToString();
                if (DataItemList[3].Value != null)
                    Vertival = DataItemList[3].Value.ToString();
                if (DataItemList[4].Value != null)
                    Horizontal = DataItemList[4].Value.ToString();
                if (DataItemList[5].Value != null)
                    PutMateriStretch = DataItemList[5].Value.ToString();
                if (DataItemList[6].Value != null)
                    PutMateriShrink = DataItemList[6].Value.ToString();
                if (DataItemList[7].Value != null)
                    PalletForward = DataItemList[7].Value.ToString();
                if (DataItemList[8].Value != null)
                    PalletBack = DataItemList[8].Value.ToString();
                if (DataItemList[9].Value != null)
                    CylinderHold = DataItemList[9].Value.ToString();
                if (DataItemList[10].Value != null)
                    CylinderRelease = DataItemList[10].Value.ToString();


                //if (DDJStatusDataItemList[8].Value != null)
                //    MotionRank = DDJStatusDataItemList[8].Value.ToString();
                //if (DDJStatusDataItemList[9].Value != null)
                //    LiftLayer = DDJStatusDataItemList[9].Value.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("RefreshFZJStatus 刷新翻转机" + PLCID, ex);
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
                        UpdatePLCConnectState(ConnectionStates.Disconnected);
                        break;
                    case ErrorCode.ReadData:
                    case ErrorCode.SendData:
                    case ErrorCode.WriteData:
                    case ErrorCode.WrongCPU_Type:
                    case ErrorCode.WrongNumberReceivedBytes:
                    case ErrorCode.WrongVarFormat:
                        UpdateDeviceState(FZJDeviceWorkState.Fault);
                        break;
                    default: break;
                }
            }
            LogHelper.WriteLog("FZJ通讯失败，" + PLCID + " ErrorCode = " + plc_ex.ErrorCode + " " + plc_ex.ToString());
        }

        public void UpdatePLCConnectState(ConnectionStates connectionState)
        {
            PLCConnectState = connectionState;
        }
        public void UpdateDeviceState(FZJDeviceWorkState device_state)
        {
            FZJWorkState = device_state;
        }
    }
    public enum FZJDeviceWorkState
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
