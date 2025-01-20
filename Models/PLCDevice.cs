using BMHRI.WCS.Server.Tools;
using S7.Net;
using S7.Net.Types;
using System;
using System.ComponentModel;
using System.Net;

namespace BMHRI.WCS.Server.Models
{
    public class PLCDevice: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region PLCDevice Members
        public Plc? Plc { get; set; }
        public CpuType CpuType { get; set; }
        public string? PLCID { get; set; }
        public string? IP { get; set; }
        public int Port { get; set; }
        public short Slot { get; set; }
        public short Rack { get; set; }
        public string? PLCDecription { get; set; }
        public string? PLCDeviceType { get; set; }

        private ConnectionStates plc_connect_state;
        public ConnectionStates PLCConnectState
        {
            get { return plc_connect_state; }
            set
            {
                if (plc_connect_state != value)
                {
                    plc_connect_state = value;
                    Notify("PLCConnectState");
                }
            }
        }
        #endregion

        #region Constructor
        public PLCDevice(string cpuType, string Ip,string slot,string rack,string decription, string device_type,string plcid)
        {
            try
            {
                int.TryParse(cpuType, out int ct);
                CpuType = (CpuType)ct;
                short.TryParse(slot, out short _slot);
                Slot = _slot;
                short.TryParse(rack, out short _rack);
                Rack = _rack;
                IP = Ip;
                PLCDecription = decription;
                PLCDeviceType = device_type;
                PLCID = plcid;

                if (!IsValidIp(IP))
                {
                    throw new ArgumentException("Ip address is not valid");
                }
                Plc = new Plc(CpuType, IP, Rack, Slot);

                PLCConnectState = ConnectionStates.Disconnected;
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog("创建PLC失败，"+PLCID+ " "+ex.ToString());
            }
        }
        /// <summary>
        /// 德马泰克堆垛机PLC创建
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="port"></param>
        /// <param name="plcid"></param>
        /// <param name="decription"></param>
        /// <param name="slot"></param>
        /// <param name="rack"></param>
        /// <param name="cpuType"></param>
        /// <param name="device_type"></param>
        public PLCDevice(string Ip,int port, string plcid, string decription, string slot, string rack,string cpuType, string device_type)
        {
            try
            {
                int.TryParse(cpuType, out int ct);
                CpuType = (CpuType)ct;
                short.TryParse(slot, out short _slot);
                Slot = _slot;
                short.TryParse(rack, out short _rack);
                Rack = _rack;
                IP = Ip;
                PLCDecription = decription;
                PLCDeviceType = device_type;
                PLCID = plcid;
                Port = port;
                if (!IsValidIp(IP))
                {
                    throw new ArgumentException("Ip address is not valid");
                }
                Plc = new Plc(CpuType, IP, Port, Rack, Slot);
                //Plc = new Plc(CpuType, IP, Rack, Slot);

                PLCConnectState = ConnectionStates.Disconnected;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("创建PLC失败，"+PLCID+ " "+ex.ToString());
            }
        }
        public PLCDevice()
        {

        }
        #endregion
        public void Connect()
        {
            PLCConnectState = ConnectionStates.Connecting;
            if (Plc != null)
            {
                Plc.Open();
                PLCConnectState = ConnectionStates.Connected;
            }
        }
        public void DEMATICConnect()
        {
            PLCConnectState = ConnectionStates.Connecting;
            if (Plc != null)
            {
                Plc.Open();
                PLCConnectState = ConnectionStates.Connected;
            }
        }

        public void Disconnect()
        {
            PLCConnectState = ConnectionStates.Disconnected;
            try
            {
                if (Plc == null) return;
                if (Plc.IsConnected)
                    Plc.Close();
            }
            catch (PlcException ex)
            {
               //LogHelper.WriteLog("PLC断开连接失败，PLCID = " + PLCID + " ErrorCode: " + ex.ErrorCode, ex);
            }
        }

        public bool WritePLCDataItem(DataItem plc_var, object obj)
        {
            bool return_value = false;
            try
            {
                //Plc.Write(plc_var.DataType, plc_var.DB, plc_var.StartByteAdr, obj);
                if (plc_var.VarType == VarType.Bit)
                    Plc.Write(plc_var.DataType, plc_var.DB, plc_var.StartByteAdr, obj, plc_var.BitAdr);
                else
                    Plc.Write(plc_var.DataType, plc_var.DB, plc_var.StartByteAdr, obj, -1);
                return_value = true;
            }
            catch (PlcException ex)
            {
                //LogHelper.WriteLog("写入PLC错误 ", ex);
            }
            return return_value;
        }

        private bool IsValidIp(string addr)
        {
            bool valid = !string.IsNullOrEmpty(addr) && IPAddress.TryParse(addr, out _);
            return valid;
        }

        public enum ConnectionStates
        {
            Disconnected = 0,
            Connecting = 1,
            Connected = 2,
        }
    }
}
