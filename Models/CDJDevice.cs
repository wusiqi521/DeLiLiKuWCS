using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BMHRI.WCS.Server.Models
{
    public class CDJDevice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string PLCID { get; set; }

        private CDJDeviceStatus cdj_status;
        public CDJDeviceStatus CDJStatus
        {
            get { return cdj_status; }
            set
            {
                if (cdj_status != value)
                {
                    cdj_status = value;
                    Notify("CDJStatus");
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
        private int pallet_count;
        public int PalletCount
        {
            get { return pallet_count; }
            set
            {
                if (pallet_count != value)
                {
                    pallet_count = value;
                    Notify("PalletCount");
                }
            }
        }

        //private CDJEquType cdj_type;
        //public CDJEquType CDJ_Type
        //{
        //    get { return cdj_type; }
        //    set
        //    {
        //        cdj_type = value;
        //        Notify("CDJ_Type");
        //    }
        //}
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
        public string PalletNumMB { get; set; }
        public string StatusMB { get; set; }
        public string FaultCodeMB { get; set; }
        public string DeviceID { get; set; }
        public string PalletCountMB { get; set; }
        private CDJEquType device_type;
        public CDJEquType DeviceType
        {
            get { return device_type; }
            set
            {
                if (device_type != value)
                {
                    device_type = value;
                    Notify("DeviceType");
                }
            }
        }
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
                    DataItem PalletNumDT = DataItem.FromAddress(PalletNumMB);
                    if (PalletNumDT != null)
                    {
                        PalletNumDT.VarType = S7.Net.VarType.String;
                        PalletNumDT.Count = 8;
                        dataItemList.Add(PalletNumDT);
                    }
                    DataItem CDJtatusDT = DataItem.FromAddress(StatusMB);
                    if (CDJtatusDT != null)
                    {
                        //CDJtatusDT.VarType = S7.Net.VarType.String;
                        //CDJtatusDT.Count = 1;
                        dataItemList.Add(CDJtatusDT);
                    }
                    DataItem FaultCodeDT = DataItem.FromAddress(FaultCodeMB);
                    if (FaultCodeDT != null)
                        dataItemList.Add(FaultCodeDT);
                    if (!string.IsNullOrEmpty(PalletCountMB))
                    {
                        DataItem PalletCountDT = DataItem.FromAddress(PalletCountMB);
                        if (PalletCountDT != null)
                            dataItemList.Add(PalletCountDT);
                    }
                }
                return dataItemList;
            }
        }
        
        public void UpdateCDJStatuss()
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
                        string code = dataItem.Value.ToString().Trim();
                        if (string.IsNullOrEmpty(code) || code.Length != 8) continue;
                        PalletNum = code.Substring(0, 8);
                        break;
                    case 2:
                        if (int.TryParse(dataItem.Value.ToString(), out int dd))
                        {
                            CDJStatus = (CDJDeviceStatus)dd;
                        }
                        break;
                    case 3:
                        FaultCode = dataItem.Value.ToString();
                        break;
                    case 4:
                        if (int.TryParse(dataItem.Value.ToString(), out int d))
                            PalletCount = d;
                        break;
                    default:
                        break;
                }
            }
        }
    }
    
    public enum CDJDeviceStatus
    {
        None,
        Auto,
        Manul,
        Stop,
        Fault,
        Maintenance,
    }
    public enum CDJEquType
    {
        None,
        DischargerDevice,
        LoadDevice,
        DischargerAndLoadDevice
    }
}
