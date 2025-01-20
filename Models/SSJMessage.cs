using System;

namespace BMHRI.WCS.Server.Models
{
    public class SSJMessage
    {
        public const int TatolLength = 50;//ssj电报长度变为50，38
        public const int MessageLength = 34;
        public const int HeaderLength = 2;
        public const int PalletNumLength = 12;//托盘号长度变为12
        public const int OldPalletNumLength = 8;//原先是8位
        public const int FmLocationLength = 4;
        public const int HeightLength = 2;
        public const int ToLocationLength = 4;
        public const int VerificationLength = 2;
        public const int ErrorCodeLength = 4;
        public const int WeightNumLength = 4;
        public const int GaoDiLength = 1;
        public const int DBB20Length = 1;

        public string Message;
        public const string UnreadVerificationCodeDB4 = "01";
        public const string ReadedVerificationCodeDB4 = "00";

        public const string UnreadVerificationCodeDB5 = "01";
        public const string ReadedVerificationCodeDB5 = "10";

        public const string ZeroPalletNum = "000000000000";//也改成12位
        public const string NineZero = "000000000";
        public const string EightZero = "00000000";

        public string MsgSeqID { get; set; }
        public int SendPriority { get; set; }
        public DateTime Tkdat { get; set; }
        public string PLCID { get; set; }
        public SSJMsgDirection Direction { get; set; }
        public string MsgParse { get; set; }
        private string trans;
        public string Trans
        {
            get
            {
                if (string.IsNullOrEmpty(trans))
                    trans = GetTransDB4();
                return trans;
            }
            set
            {
                trans = value;
            }
        }
        private string fmLocation;
        public string FmLocation
        {
            get
            {
                if (string.IsNullOrEmpty(fmLocation))
                    fmLocation = GetFmLocation();
                return fmLocation;
            }
            set
            {
                fmLocation = value;
            }
        }

        private string toLocation;
        public string ToLocation
        {
            get
            {
                if (string.IsNullOrEmpty(toLocation))
                    toLocation = GetToLocation();
                return toLocation;
            }
            set
            {
                toLocation = value;
            }
        }
        private string palletNum;
        public string PalletNum
        {
            get
            {
                if (string.IsNullOrEmpty(palletNum))
                    if (messageType != "0X" && messageType != "0Y")
                        palletNum = "";
                    else
                    {
                        palletNum = GetPalletNum(MessageType);//palletNum = "";//GetPalletNum(MessageType);
                    }
                if(palletNum == "")
                {
                    palletNum = Message.Substring(0, 12);
                    if (palletNum.Contains("**") && palletNum.Length == PalletNumLength)
                        palletNum  = palletNum.Substring(0, 10);                  
                    
                }
                return palletNum;
            }
            set
            {
                palletNum = value;
            }
        }
        private string weightNum;
        public string WeightNum
        {
            get
            {
                if (string.IsNullOrEmpty(weightNum))
                    weightNum = GetWeightNum();
                return weightNum;
            }
            set
            {
                weightNum = value;
            }
        }
        private string gaoDi;
        public string GaoDi
        {
            get
            {
                if (string.IsNullOrEmpty(gaoDi))
                    gaoDi = GetGaoDi();
                return gaoDi;
            }
            set
            {
                gaoDi = value;
            }
        }
        private string messageType;
        public string MessageType
        {
            get
            {
                if (string.IsNullOrEmpty(messageType))
                    messageType = GetMessageType();
                return messageType;
            }
            set
            {
                messageType = value;
            }
        }

        public SSJMessage(string PLC_ID)
        {
            PLCID = PLC_ID;
            Tkdat = DateTime.Now;
            MsgSeqID = Guid.NewGuid().ToString("N");
            MsgParse = "";
        }

        public SSJMessage(string trans, string plc_id, SSJMsgDirection direction)
        {
            MsgSeqID = Guid.NewGuid().ToString("N");
            Trans = trans;
            Tkdat = DateTime.Now;
            PLCID = plc_id;
            MessageType = trans.Substring(0, HeaderLength);
            Message = trans.Substring(HeaderLength, MessageLength);
            Direction = direction;
        }
        public SSJMessage(string trans, string plc_id, SSJMsgDirection direction, string msg_parse)
        {
            MsgSeqID = Guid.NewGuid().ToString("N");
            Trans = trans;
            Tkdat = DateTime.Now;
            PLCID = plc_id;
            MessageType = trans.Substring(0, HeaderLength);
            Message = trans.Substring(HeaderLength, MessageLength);
            Direction = direction;
            MsgParse = msg_parse;
        }

        public string GetTransDB4()
        {
            string trans_1 = MessageType + Message + UnreadVerificationCodeDB4 + PalletNum;
            if (messageType != "0X" && messageType != "0Y") return MessageType + Message + UnreadVerificationCodeDB4+"000000000000";
            else
            {
                if(PalletNum.Length==12) return MessageType + Message + UnreadVerificationCodeDB4 + PalletNum;
                else return MessageType + Message + UnreadVerificationCodeDB4 + PalletNum + "**";
            }
            
        }

        public string GetPalletNum(string messageType)
        {
            string pallet = "";
            if (string.IsNullOrEmpty(Trans)) return null;
            //if (Trans.Length < TatolLength) return null;
            // 托盘号是最后12位
            if (messageType != "0X" && messageType != "0Y")
            {
                if (Trans.Substring(12, 2) == "**")
                    return Trans.Substring(HeaderLength, 10);
                else
                    return Trans.Substring(HeaderLength, PalletNumLength);
            }
            else
            {
                pallet = Trans.Substring(TatolLength - PalletNumLength, PalletNumLength);
                if (pallet.Contains("**") && pallet.Length == PalletNumLength)
                    return pallet.Substring(0, 10);
                else
                    return pallet;
            }
        }

        public string GetWeightNum()
        {
            if (string.IsNullOrEmpty(Trans)) return null;
            if (Trans.Length < TatolLength) return null;
            string weight = Trans.Substring(HeaderLength + OldPalletNumLength + FmLocationLength + HeightLength, WeightNumLength);
            return weight;
        }
        public string GetGaoDi()
        {
            if (string.IsNullOrEmpty(Trans)) return null;
            if (Trans.Length < TatolLength) return null;
            string gaoDi = Trans.Substring(HeaderLength + OldPalletNumLength + FmLocationLength+ToLocationLength, GaoDiLength);
            return gaoDi;
        }

        public int GetPalletStation()
        {
            if (string.IsNullOrEmpty(Trans)) return 0;
            if (Trans.Length < TatolLength) return 0;
            int station = 0;
            string pn = Trans.Substring(HeaderLength, PalletNumLength);
            if (!pn.Equals(ZeroPalletNum))
                station = 1;
            else
            {
                pn = Trans.Substring(HeaderLength + PalletNumLength + FmLocationLength + ToLocationLength + 1, PalletNumLength);
                if (!pn.Equals(ZeroPalletNum))
                    station = 2;
            }
            return station;
        }


        public string GetMessageType()
        {
            if (string.IsNullOrEmpty(Trans)) return null;
            if (Trans.Length < TatolLength) return null;
            return Trans.Substring(0, HeaderLength);
        }

        public string GetFmLocation()
        {
            if (string.IsNullOrEmpty(Trans) || (Trans.Length != TatolLength && Trans.Length != 38) || string.IsNullOrEmpty(messageType)) return null;
            if (messageType != "0X" && messageType != "0Y")
            {
                return Trans.Substring(HeaderLength + PalletNumLength, FmLocationLength);
            }
            else
            {
                return Trans.Substring(HeaderLength + OldPalletNumLength, FmLocationLength);
            }
        }
        private string GetToLocation()
        {
            if (string.IsNullOrEmpty(Trans) || (Trans.Length != TatolLength && Trans.Length != 38)) return null;
            if (messageType != "0X" && messageType != "0Y")
            {
                return Trans.Substring(HeaderLength + PalletNumLength + FmLocationLength, ToLocationLength);
            }
            else
            {
                return Trans.Substring(HeaderLength + OldPalletNumLength + FmLocationLength, ToLocationLength);
            }
            
        }
        public string GetErrorCode()
        {
            if (string.IsNullOrEmpty(Trans)) return null;
            if (Trans.Length != TatolLength) return null;
            //return Trans.Substring(26, 2);
            return Trans.Substring(HeaderLength + OldPalletNumLength, ErrorCodeLength);
        }
        public string GetErrorDeviceID()
        {
            if (string.IsNullOrEmpty(Trans)) return null;
            if (Trans.Length != TatolLength) return null;
            //return Trans.Substring(HeaderLength + OldPalletNumLength + 4, FmLocationLength);
            return Trans.Substring(HeaderLength + OldPalletNumLength + ErrorCodeLength, FmLocationLength);
        }
        public string GetEmptyPalletStackFlag()
        {
            //0XP000000325800000k0000000000000000001
            if (string.IsNullOrEmpty(Trans)) return null;
            if (Trans.Length != TatolLength) return null;
            return Trans.Substring(18, 1);
        }
        public string GetFaultContents(string error_code)
        {
            string reStr = "未知故障";
            switch (error_code)
            {
                case "0":
                    reStr = "";
                    break;
                case "1":
                    reStr = "安全异常";
                    break;
                case "2":
                    reStr = "变频器异常";
                    break;
                case "3":
                    reStr = "马达过载";
                    break;
                case "4":
                    reStr = "气压低下报警";
                    break;
                case "5":
                    reStr = "火警异常";
                    break;
                case "6":
                    reStr = "物料异常";
                    break;
                case "7":
                    reStr = "快速门异常";
                    break;
                case "8":
                    reStr = "设备在荷数据异常";
                    break;
                case "9":
                    reStr = "托盘动作超时";
                    break;
                case "10":
                    reStr = "设备动作超时";
                    break;
                case "11":
                    reStr = "提升机升降超时";
                    break;
                case "12":
                    reStr = "提升机上升极限";
                    break;
                case "13":
                    reStr = "提升机下降极限";
                    break;
                case "14":
                    reStr = "提升机断链";
                    break;
                case "15":
                    reStr = "设备不在位置异常";
                    break;
                case "16":
                    reStr = "隔离开关异常";
                    break;
                case "17":
                    reStr = "移载机不在位置";
                    break;
                case "18":
                    reStr = "拆码盘机不在位置";
                    break;
                case "19":
                    reStr = "拆码盘机夹爪不在位置";
                    break;
                case "20":
                    reStr = "活动挡板不在位置";
                    break;
                case "21":
                    reStr = "露出异常";
                    break;
                case "22":
                    reStr = "RGV冲出异常";
                    break;
                case "23":
                    reStr = "旋转台不在位置";
                    break;
                case "24":
                    reStr = "自走台车不在位置";
                    break;
                case "25":
                    reStr = "WCS通讯异常";
                    break;
                case "26":
                    reStr = "第三方设备通信中断";
                    break;
                case "27":
                    reStr = "托盘连续投放异常";
                    break;
                case "28":
                    reStr = "货型异常";
                    break;
                case "29":
                    reStr = "子托条码异常";
                    break;
                case "30":
                    reStr = "母托条码异常";
                    break;
                case "31":
                    reStr = "母托盘校验异常";
                    break;
                case "32":
                    reStr = "重量异常";
                    break;
                default:
                    reStr = "未知故障";
                    break;
            }
            return reStr;
        }
        public string GetFaultContent()
        {
            string reStr = "未知故障";
            string error_code = GetErrorCode();
            switch (error_code)
            {
                case "00":
                    reStr = "";
                    break;
                case "01":
                    reStr = "超时";
                    break;
                case "02":
                    reStr = "停偏";
                    break;
                case "03":
                    reStr = "提升门故障";
                    break;
                case "04":
                    reStr = "前方光电遮挡";
                    break;
                case "05":
                    reStr = "";
                    break;
                case "06":
                    reStr = "出库托盘无信息";
                    break;
                case "07":
                    reStr = "";
                    break;
                case "08":
                    reStr = "";
                    break;
                case "09":
                    reStr = "人员进入工作区域";
                    break;
                case "10":
                    reStr = "";
                    break;
                case "11":
                    reStr = "";
                    break;
                case "12":
                    reStr = "";
                    break;
                default:
                    reStr = "未知故障";
                    break;
            }
            return reStr;
        }

        public void Set0YMessage(string pallet_num, string fm_location, string to_location)
        {
            Direction = 0;
            MessageType = "0Y";
            SendPriority = 2;
            
            if (pallet_num.Length == 10)
                pallet_num = pallet_num + "**";
            PalletNum = pallet_num;
            Message = EightZero + fm_location.Trim() + to_location.Trim();
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0YMessages(string pallet_num, string fm_location, string to_location,string apply)
        {
            Direction = 0;
            MessageType = "0Y";
            SendPriority = 2;
            PalletNum = pallet_num;
            Message = EightZero + fm_location.Trim() + to_location.Trim()+apply;
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0GMessage(string pallet_num, string fm_location, string to_location)
        {
            Direction = 0;
            MessageType = "0G";
            SendPriority = 2;
            PalletNum = pallet_num;
            Message = EightZero + fm_location.Trim() + to_location.Trim();
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void Set0DYMessage(string pallet_num1, string fm_location1, string to_location1, string pallet_num2, string fm_location2, string to_location2)
        {
            Direction = 0;
            MessageType = "0Y";
            SendPriority = 2;
            PalletNum = pallet_num1;
            Message = EightZero + fm_location1.Trim() + to_location1.Trim() + pallet_num2 + fm_location2.Trim() + to_location2.Trim(); ;
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void Set0bMessage(string pallet_num, string fm_location, string to_location, string type)
        {
            Direction = 0;
            MessageType = "0b";
            SendPriority = 2;
            if (pallet_num.Length == 10)
                pallet_num = pallet_num + "**";
            PalletNum = "";
            Message = pallet_num + fm_location.Trim() + to_location.Trim() + type;
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0bMessages(string pallet_num, string fm_location, string to_location, string type, string port)
        {
            Direction = 0;
            MessageType = "0b";
            SendPriority = 2;
            if (to_location.Substring(0, 2) == "44")
                to_location = "3292";
            else if (to_location.Substring(0, 2) == "43")
                to_location = "3293";
            else if (to_location.Substring(0, 2) == "42")
                to_location = "3294";
            else if (to_location.Substring(0, 2) == "41")
                to_location = "3295";
            PalletNum = pallet_num;
            Message = EightZero + fm_location.Trim() + to_location.Trim() + type + port;
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0GMessages(string pallet_num, string fm_location, string to_location)
        {
            Direction = 0;
            MessageType = "0G";
            SendPriority = 2;
            PalletNum = pallet_num;
            Message = EightZero + fm_location.Trim() + to_location.Trim();
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0KMessage(string pallet_num, string fm_location, string weight)
        {
            Direction = 0;
            MessageType = "0K";
            SendPriority = 2;
            PalletNum = pallet_num;
            Message = EightZero + fm_location.Trim() + NineZero + weight.Trim();
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void Set0DbMessage(string pallet_num1, string fm_location1, string to_location1, string pallet_num2, string fm_location2, string to_location2)
        {
            Direction = 0;
            MessageType = "0b";
            SendPriority = 2;
            PalletNum = pallet_num1;
            Message = EightZero + fm_location1.Trim() + to_location1.Trim() + "0" + pallet_num2 + fm_location2.Trim() + to_location2.Trim();
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void Set0AMessage()
        {
            Direction = 0;
            MessageType = "0A";
            SendPriority = 1;
            PalletNum = "";
            Message = "";
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0EMessage(string trans)
        {
            Direction = 0;
            MessageType = "0E";
            SendPriority = 1;
            Message = trans.Substring(HeaderLength, MessageLength);
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0WMessage(string trans, string fm_location, string to_location, string is_error, string port)
        {
            Direction = 0;
            MessageType = "0W";
            SendPriority = 1;
            Message = trans.Substring(HeaderLength, OldPalletNumLength) + fm_location + to_location + is_error + port;
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0cMessage(string pallet_num, string position, string result)
        {
            Direction = 0;
            MessageType = "0c";
            SendPriority = 2;
            PalletNum = pallet_num;
            Message = EightZero + position + "0000" + result;
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0dMessage(string pallet_num, string position, string result)
        {
            Direction = 0;
            MessageType = "0d";
            SendPriority = 2;
            PalletNum = pallet_num;
            Message = EightZero + position + "0000" + result;
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0SMessage(string pallet_num, string fm_location, string to_location)
        {
            Direction = 0;
            MessageType = "0S";
            SendPriority = 2;
            PalletNum = pallet_num;
            Message = EightZero + fm_location.Trim() + to_location.Trim();
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0BMessage()
        {
            Direction = 0;
            SendPriority = 1;
            Message = "";
            MessageType = "0B";
            PalletNum = "";
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0LMessage()
        {
            Direction = 0;
            SendPriority = 0;
            MessageType = "0L";
            Message = "";
            PalletNum = "";
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void Set0dMessage()
        {
            Direction = 0;
            MessageType = "0d";
            SendPriority = 1;
            Message = "";
            while (Message.Length < MessageLength)
                Message += "0";
        }

        string zero_eight = "00000000";
        public void Set0QMessage(string pallet_num, string position)
        {
            Direction = 0;
            MessageType = "0Q";
            SendPriority = 2;
            PalletNum = pallet_num;
            Message = EightZero + position;
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0dMessage(string pallet_num, string position)
        {
            Direction = 0;
            MessageType = "0d";
            SendPriority = 2;
            PalletNum = pallet_num;
            Message = EightZero + position;
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0aMessage(string pallet_num, string position)
        {
            Direction = 0;
            MessageType = "0a";
            SendPriority = 2;
            PalletNum = pallet_num;
            Message = EightZero + position;
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0qMessage(string pallet_num, string position)
        {
            Direction = 0;
            MessageType = "0q";
            SendPriority = 2;
            PalletNum = pallet_num;
            Message = EightZero + position;
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void SetClearDB4Message()
        {
            SendPriority = 0;
            MessageType = "00";
            Message = "";
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void SetClearDB5Message()
        {
            SendPriority = 0;
            MessageType = "00";
            Message = "";
            while (Message.Length < MessageLength)
                Message += "0";
        }
    }

    public enum SSJMsgDirection
    {
        [System.ComponentModel.Description("WCS->PLC")]
        Send = 0,
        [System.ComponentModel.Description("PLC->WCS")]
        Receive = 1
    }
}
