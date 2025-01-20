using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BMHRI.WCS.Server.Models
{
    public class XZTDevice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string PLCID { get; set; }
        private XZTDeviceStatus xzt_status;
        public XZTDeviceStatus XZTStatus
        {
            get { return xzt_status; }
            set
            {
                if (xzt_status != value)
                {
                    xzt_status = value;
                    Notify("XZTStatus");
                }
            }
        }
        
        private bool is_occupying;
        public bool IsOccupying
        {
            get { return is_occupying; }
            set
            {
                if (is_occupying != value)
                {
                    is_occupying = value;
                    Notify("IsOccupying");
                }
            }
        }
        private string pallet_num;
        public string PalletNum
        {
            get { return pallet_num; }
            set
            {
                if (pallet_num != value)
                {
                    pallet_num = value;
                    Notify("PalletNum");
                }
            }
        }
        private int xzt_direction;
        public int XZTDirection
        {
            get { return xzt_direction; }
            set
            {
                xzt_direction = value;
                Notify("XZTDirection");
            }
        }
        
        private string fault_code;
        public string FaultCode
        {
            get { return fault_code; }
            set
            {
                if (fault_code != value)
                {
                    fault_code = value;
                    Notify("FaultCode");
                }
            }
        }
        
        public string IsOccupyingMB { get; set; }
        public string CurrentDirectionMB { get; set; }
        public string PalletNumMB { get; set; }
        public string StatusMB { get; set; }
        public string FaultCodeMB { get; set; }
        public string DeviceID { get; set; }
        private List<DataItem> dataItemList;
        public List<DataItem> DataItemList
        {
            get
            {
                if (dataItemList == null)
                {
                    dataItemList = new List<DataItem>();
                    DataItem IsOccupyingDT = DataItem.FromAddress(IsOccupyingMB);
                    if (IsOccupyingDT != null)
                    {
                        //IsOccupyingDT.VarType = S7.Net.VarType.String;
                        //IsOccupyingDT.Count = 1;
                        dataItemList.Add(IsOccupyingDT);
                    }
                    DataItem XZTDirection = DataItem.FromAddress(CurrentDirectionMB);
                    if (XZTDirection != null)
                        dataItemList.Add(XZTDirection);
                    DataItem PalletNumDT = DataItem.FromAddress(PalletNumMB);
                    if (PalletNumDT != null)
                    {
                        PalletNumDT.VarType = S7.Net.VarType.String;
                        PalletNumDT.Count = 8;
                        dataItemList.Add(PalletNumDT);
                    }
                    DataItem XZTStatusDT = DataItem.FromAddress(StatusMB);
                    if (XZTStatusDT != null)
                    {
                        //RGVStatusDT.VarType = S7.Net.VarType.String;
                        //RGVStatusDT.Count = 1;
                        dataItemList.Add(XZTStatusDT);
                    }
                    DataItem FaultCodeDT = DataItem.FromAddress(FaultCodeMB);
                    if (FaultCodeDT != null)
                        dataItemList.Add(FaultCodeDT);
                }
                return dataItemList;
            }
        }

        public void UpdateXZTStatuss()
        {
            for (int i = 0; i < dataItemList.Count; i++)
            {
                DataItem dataItem = dataItemList[i];
                if (dataItem.Value == null) continue;
                int index = dataItemList.IndexOf(dataItem);
                // System.Diagnostics.Debug.WriteLine(index + "--" + dataItem.Value.ToString());
                switch (i)
                {
                    case 0:
                        IsOccupying = dataItem.Value.ToString() == "1";
                        break;
                    case 1:
                        if (int.TryParse(dataItem.Value.ToString(), out int ppp))
                            XZTDirection = ppp;
                        break;
                    case 2:
                        string code = dataItem.Value.ToString().Trim();
                        if (string.IsNullOrEmpty(code) || code.Length != 8) continue;
                        PalletNum = code.Substring(0, 8);
                        break;
                    case 3:
                        if (int.TryParse(dataItem.Value.ToString(), out int dd))
                        {
                            XZTStatus = (XZTDeviceStatus)dd;
                        }
                        break;
                    case 4:
                        FaultCode = dataItem.Value.ToString();
                        break;
                    default:
                        break;
                }
            }
        }
        
    }
    public enum XZTDeviceStatus
    {
        //None = -1,
        //Initia,
        //Standby,
        //Working,
        //Avoided,
        None,
        Auto,
        Manul,
        Stop,
        Fault,
        Maintenance,
    }
}
