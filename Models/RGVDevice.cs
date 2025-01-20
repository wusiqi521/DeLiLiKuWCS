using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BMHRI.WCS.Server.Models
{
    public class RGVDevice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string PLCID { get; set; }
        private RGVDeviceStatus rgv_status;
        public RGVDeviceStatus RGVStatus
        {
            get { return rgv_status; }
            set
            {
                if (rgv_status != value)
                {
                    rgv_status = value;
                    Notify("RGVStatus");
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
        private int rgv_position_y;
        public int RGVPostionY
        {
            get { return rgv_position_y; }
            set
            {
                if (Math.Abs(rgv_position_y - value) > 10)
                {
                    rgv_position_y = value;
                    Notify("RGVPostionY");
                }
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
        private string rDeviceID;
        public string RDeviceID
        {
            get { return rDeviceID; }
            set
            {
                if (rDeviceID != value)
                {
                    rDeviceID = value;
                    Notify("RDeviceID");
                }
            }
        }
        private string sDeviceID;
        public string SDeviceID
        {
            get { return sDeviceID; }
            set
            {
                if (sDeviceID != value)
                {
                    sDeviceID = value;
                    Notify("SDeviceID");
                }
            }
        }
        private string Cecv_id;
        public string CDeviceID
        {
            get { return Cecv_id; }
            set
            {
                if (Cecv_id != value)
                {
                    Cecv_id = value;
                    Notify("CDeviceID");
                }
            }
        }
        public string IsOccupyingMB { get; set; }
        public string PositionMB { get; set; }
        public string PalletNumMB { get; set; }
        public string StatusMB { get; set; }
        public string FaultCodeMB { get; set; }
        public string SDeviceIDMB { get; set; }
        public string RDeviceIDMB { get; set; }
        public string CDeviceIDMB { get; set; }
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
                    DataItem RGVPostionYDT = DataItem.FromAddress(PositionMB);
                    if (RGVPostionYDT != null)
                        dataItemList.Add(RGVPostionYDT);
                    DataItem PalletNumDT = DataItem.FromAddress(PalletNumMB);
                    if (PalletNumDT != null)
                    {
                        PalletNumDT.VarType = S7.Net.VarType.String;
                        PalletNumDT.Count = 8;
                        dataItemList.Add(PalletNumDT);
                    }
                    DataItem RGVStatusDT = DataItem.FromAddress(StatusMB);
                    if (RGVStatusDT != null)
                    {
                        //RGVStatusDT.VarType = S7.Net.VarType.String;
                        //RGVStatusDT.Count = 1;
                        dataItemList.Add(RGVStatusDT);
                    }
                    DataItem FaultCodeDT = DataItem.FromAddress(FaultCodeMB);
                    if (FaultCodeDT != null)
                        dataItemList.Add(FaultCodeDT);
                    //DataItem SDeviceIDDT = DataItem.FromAddress(SDeviceIDMB);
                    //if (SDeviceIDDT != null)
                    //{
                    //    SDeviceIDDT.VarType = S7.Net.VarType.String;
                    //    SDeviceIDDT.Count = 4;
                    //    dataItemList.Add(SDeviceIDDT);
                    //}
                    //DataItem RDeviceIDDT = DataItem.FromAddress(RDeviceIDMB);
                    //if (RDeviceIDDT != null)
                    //{
                    //    RDeviceIDDT.VarType = S7.Net.VarType.String;
                    //    RDeviceIDDT.Count = 4;
                    //    dataItemList.Add(RDeviceIDDT);
                    //}
                    //DataItem CDeviceIDDT = DataItem.FromAddress(CDeviceIDMB);
                    //if (CDeviceIDDT != null)
                    //{
                    //    CDeviceIDDT.VarType = S7.Net.VarType.String;
                    //    CDeviceIDDT.Count = 4;
                    //    dataItemList.Add(CDeviceIDDT);
                    //}
                }
                return dataItemList;
            }
        }
        public void UpdateRGVStatus()
        {
            foreach (DataItem dataItem in DataItemList)
            {
                if (dataItem.Value == null) continue;
                int index = dataItemList.IndexOf(dataItem);
                System.Diagnostics.Debug.WriteLine(index + "--" + dataItem.Value.ToString());
                switch (index)
                {
                    case 0:
                        IsOccupying = dataItem.Value.ToString() == "1";
                        break;
                    case 1:
                        if (int.TryParse(dataItem.Value.ToString(), out int ppp))
                            RGVPostionY = ppp;
                        break;
                    case 2:
                        string code = dataItem.Value.ToString();
                        if (string.IsNullOrEmpty(code) || code.Length != 16) continue;
                        PalletNum = code.Substring(0, 8);
                        break;
                    case 3:
                        if (int.TryParse(dataItem.Value.ToString(), out int dd))
                        {
                            RGVStatus = (RGVDeviceStatus)dd;
                        }
                        break;
                    case 4:
                        FaultCode = dataItem.Value.ToString();
                        break;
                    case 5:
                        SDeviceID = dataItem.Value.ToString();
                        break;
                    case 6:
                        RDeviceID = dataItem.Value.ToString();
                        break;
                    case 7:
                        CDeviceID = dataItem.Value.ToString();
                        break;
                    default:
                        break;
                }
            }
        }

        public void UpdateRGVStatuss()
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
                            RGVPostionY = ppp;
                        break;
                    case 2:
                        string code = dataItem.Value.ToString().Trim();
                        if (string.IsNullOrEmpty(code) || code.Length != 8) continue;
                        PalletNum = code.Substring(0, 8);
                        break;
                    case 3:
                        if (int.TryParse(dataItem.Value.ToString(), out int dd))
                        {
                            RGVStatus = (RGVDeviceStatus)dd;
                        }
                        break;
                    case 4:
                        FaultCode = dataItem.Value.ToString();
                        break;
                    //case 5:
                    //    RDeviceID = dataItem.Value.ToString();
                    //    break;
                    //case 6:
                    //    SDeviceID = dataItem.Value.ToString();
                    //    break;
                    //case 7:
                    //    CDeviceID = dataItem.Value.ToString();
                    //    break;
                    default:
                        break;
                }
            }
        }
        
    }
    public enum RGVDeviceStatus
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
