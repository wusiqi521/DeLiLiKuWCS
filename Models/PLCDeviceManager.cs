using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BMHRI.WCS.Server.Models
{
    public sealed class PLCDeviceManager
    {
        private static readonly Lazy<PLCDeviceManager> lazy = new Lazy<PLCDeviceManager>(() => new PLCDeviceManager());

        public static PLCDeviceManager Instance { get { return lazy.Value; } }
        public static string PLCConfigfilePathName = Environment.CurrentDirectory + "\\Configure" + "\\PLC_Set.config";

        private List<SSJDevice> _sSJDeviceList;
        public List<SSJDevice> SSJDeviceList
        {
            get
            {
                if (_sSJDeviceList == null)
                    CreateSSJDeviceList();
                return _sSJDeviceList;
            }
        }
        private List<DDJDevice> _dDJDeviceList;
        public List<DDJDevice> DDJDeviceList
        {
            get
            {
                if (_dDJDeviceList == null)
                    CreateDDJeviceList();
                return _dDJDeviceList;
            }
        }
        private List<FZJDevice> _fZJDeviceList;
        public List<FZJDevice> FZJDeviceList
        {
            get
            {
                if (_fZJDeviceList == null)
                    CreateFZJeviceList();
                return _fZJDeviceList;
            }
        }

        private void CreateSSJDeviceList()
        {
            _sSJDeviceList = new List<SSJDevice>();
            if (File.Exists(PLCConfigfilePathName))
            {
                try
                {
                    XDocument xDocument = XDocument.Load(PLCConfigfilePathName);
                    foreach (XElement item in xDocument.Root.Descendants("SSJ"))
                    {
                        SSJDevice sSjDevice = new SSJDevice(item.Element("CpuType").Value, item.Element("IP").Value, item.Element("Slot").Value, item.Element("Rack").Value, item.Element("Decription").Value, "SSJ", item.Element("PLCID").Value);

                        if (sSjDevice != null)
                        {
                            _sSJDeviceList.Add(sSjDevice);
                        }                     
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("输送机配置文件读取异常", ex);
                }
            }
        }
        private void CreateDDJeviceList()
        {
            _dDJDeviceList = new List<DDJDevice>();
            if (File.Exists(PLCConfigfilePathName))
            {
                try
                {
                    XDocument xDocument = XDocument.Load(PLCConfigfilePathName);
                    foreach (XElement item in xDocument.Root.Descendants("DDJ"))
                    {
                        DDJDevice dDJDevice = new DDJDevice(item.Element("CpuType").Value, item.Element("IP").Value, item.Element("Slot").Value, item.Element("Rack").Value, item.Element("Decription").Value, "DDJ", item.Element("PLCID").Value, item.Element("Available").Value);

                        if (dDJDevice != null)
                        {
                            _dDJDeviceList.Add(dDJDevice);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("堆垛机配置文件读取异常", ex);
                }
            }
        }
        private void CreateFZJeviceList()
        {
            _fZJDeviceList = new List<FZJDevice>();
            if (File.Exists(PLCConfigfilePathName))
            {
                try
                {
                    XDocument xDocument = XDocument.Load(PLCConfigfilePathName);
                    foreach (XElement item in xDocument.Root.Descendants("FZJ"))
                    {
                        FZJDevice fZJDevice = new FZJDevice(item.Element("CpuType").Value, item.Element("IP").Value, item.Element("Slot").Value, item.Element("Rack").Value, item.Element("Decription").Value, "FZJ", item.Element("PLCID").Value);

                        if (fZJDevice != null)
                        {
                            _fZJDeviceList.Add(fZJDevice);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("堆垛机配置文件读取异常", ex);
                }
            }
        }
        public void SavePLCDeviceStatus(string node_name, string plcid, string name, string value)
        {
            try
            {
                XDocument xDocument = XDocument.Load(PLCConfigfilePathName);
                foreach (XElement item in xDocument.Root.Descendants(node_name))
                {
                    if (item.Element("PLCID").Value == plcid)
                    {
                        item.Element(name).Value = value;
                    }
                }
                xDocument.Save(PLCConfigfilePathName);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("堆垛机配置文件读取异常", ex);
            }
        }
        public SSJDevice FindSSJDevice(string position)
        {
            SSJDevice sSJDevice = null;
            string plcid = null;
            string sqlstr = string.Format("select PLCID from dbo.ConveyerBlocks where position='{0}' or WMSPosition='{0}'", position);
            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr);
            if (obj == null) return sSJDevice;
            if (string.IsNullOrEmpty(obj.ToString())) return sSJDevice;
            plcid = obj.ToString();
            sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == plcid);
            return sSJDevice;
        }
    }
}
