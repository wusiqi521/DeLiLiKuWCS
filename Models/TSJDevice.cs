using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BMHRI.WCS.Server.Models
{
    public class TSJDevice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string PLCID { get; set; }
        
        private TSJDeviceStatus tsj_status;
        public TSJDeviceStatus TSJStatus
        {
            get { return tsj_status; }
            set
            {
                if (tsj_status != value)
                {
                    tsj_status = value;
                    Notify("TSJStatus");
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
        
        private int tsj_position;
        public int TSJPostion
        {
            get { return tsj_position; }
            set
            {
                tsj_position = value;
                Notify("TSJPostion");
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
        public string PositionMB { get; set; }
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
                    DataItem TSJPostionDT = DataItem.FromAddress(PositionMB);
                    if (TSJPostionDT != null)
                        dataItemList.Add(TSJPostionDT);
                    DataItem PalletNumDT = DataItem.FromAddress(PalletNumMB);
                    if (PalletNumDT != null)
                    {
                        PalletNumDT.VarType = S7.Net.VarType.String;
                        PalletNumDT.Count = 8;
                        dataItemList.Add(PalletNumDT);
                    }
                    DataItem TSJtatusDT = DataItem.FromAddress(StatusMB);
                    if (TSJtatusDT != null)
                    {
                        //TSJtatusDT.VarType = S7.Net.VarType.String;
                        //TSJtatusDT.Count = 1;
                        dataItemList.Add(TSJtatusDT);
                    }
                    DataItem FaultCodeDT = DataItem.FromAddress(FaultCodeMB);
                    if (FaultCodeDT != null)
                        dataItemList.Add(FaultCodeDT);
                }
                return dataItemList;
            }
        }
        
        public void UpdateTSJStatuss()
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
                            TSJPostion = ppp;
                        break;
                    case 2:
                        string code = dataItem.Value.ToString().Trim();
                        if (string.IsNullOrEmpty(code) || code.Length != 8) continue;
                        PalletNum = code.Substring(0, 8);
                        break;
                    case 3:
                        if (int.TryParse(dataItem.Value.ToString(), out int dd))
                        {
                            TSJStatus = (TSJDeviceStatus)dd;
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
    
    public enum TSJDeviceStatus
    {
        None,
        Auto,
        Manul,
        Stop,
        Fault,
        Maintenance,
    }
}
