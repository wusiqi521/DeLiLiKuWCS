using System;

namespace BMHRI.WCS.Server.SSJProtocol
{
    public class SSJMessage
    {
        public const int TatolLength = 40;
        public const int MessageLength = 36;
        public const int HeaderLength = 2;
        public const int PalletNumLength = 10;
        public const int FmLocationLength = 4;
        public const int ToLocationLength = 4;
        public const int VerificationLength = 2;
        public const int ErrorCodeLength = 4;

        public string? Message;
        public const string UnreadVerificationCodeDB4 = "01";
        public const string ReadedVerificationCodeDB4 = "00";

        public const string UnreadVerificationCodeDB5 = "01";
        public const string ReadedVerificationCodeDB5 = "10";

        public const string ZeroPalletNum = "0000000000";


        public string? MsgSeqID { get; set; }
        public int SendPriority { get; set; }
        public DateTime Tkdat { get; set; }
        public string? PLCID { get; set; }
        public SSJMsgDirection Direction { get; set; }
        private string? trans;
        public string? Trans
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
        private string? fmLocation;
        public string? FmLocation
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

        private string? toLocation;
        public string? ToLocation
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
        private string? palletNum;
        public string? PalletNum
        {
            get
            {
                if (string.IsNullOrEmpty(palletNum))
                    palletNum = GetPalletNum();
                return palletNum;
            }
            set
            {
                palletNum = value;
            }
        }
        private string? messageType;
        public string? MessageType
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

        public string GetTransDB4()
        {
            return MessageType + Message + UnreadVerificationCodeDB4;
        }

        public string? GetPalletNum()
        {
            if (string.IsNullOrEmpty(Trans)) return null;
            if (Trans.Length < TatolLength) return null;
            string pn = Trans.Substring(HeaderLength, PalletNumLength);
            if (pn.Equals(ZeroPalletNum))
                return Trans.Substring(HeaderLength + PalletNumLength + FmLocationLength + ToLocationLength + 1, PalletNumLength);
            else return pn;
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


        public string? GetMessageType()
        {
            if (string.IsNullOrEmpty(Trans)) return null;
            if (Trans.Length < TatolLength) return null;
            return Trans.Substring(0, HeaderLength);
        }

        public string? GetFmLocation()
        {
            if (string.IsNullOrEmpty(Trans) || Trans.Length != TatolLength) return null;
            return Trans.Substring(HeaderLength + PalletNumLength, FmLocationLength);
        }
        private string? GetToLocation()
        {
            if (string.IsNullOrEmpty(Trans) || Trans.Length != TatolLength) return null;
            return Trans.Substring(HeaderLength + PalletNumLength + FmLocationLength, ToLocationLength);
        }
        public string? GetErrorCode()
        {
            if (string.IsNullOrEmpty(Trans)) return null;
            if (Trans.Length != TatolLength) return null;
            return Trans.Substring(HeaderLength + PalletNumLength, ErrorCodeLength);
        }
        public string? GetErrorDeviceID()
        {
            if (string.IsNullOrEmpty(Trans)) return null;
            if (Trans.Length != TatolLength) return null;
            return Trans.Substring(HeaderLength + PalletNumLength + ErrorCodeLength, FmLocationLength);
        }
        public string? GetEmptyPalletStackFlag()
        {
            //0XP000000325800000k0000000000000000001
            if (string.IsNullOrEmpty(Trans)) return null;
            if (Trans.Length != TatolLength) return null;
            return Trans.Substring(18,1);
        }
        public string? GetFaultContent()
        {
            string reStr = "未知故障";
            string? error_code = GetErrorCode();
            switch (error_code)
            {
                case "0001":
                    reStr = "传送超时";
                    break;
                case "0002":
                    reStr = "托盘停偏";
                    break;
                case "0006":
                    reStr = "出库托盘无信息";
                    break;
                case "0003":
                    reStr = "小车故障";
                    break;
                default:
                    break;
            }
            return reStr;
        }

        public void Set0YMessage(string pallet_num, string fm_location, string to_location)
        {
            Direction = 0;
            MessageType = "0Y";
            SendPriority = 2;
            Message = pallet_num + fm_location.Trim() + to_location.Trim();
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void Set0DYMessage(string pallet_num1, string fm_location1, string to_location1, string pallet_num2, string fm_location2, string to_location2)
        {
            Direction = 0;
            MessageType = "0Y";
            SendPriority = 2;
            Message = pallet_num1 + fm_location1.Trim() + to_location1.Trim()+ pallet_num2 + fm_location2.Trim() + to_location2.Trim(); ;
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void Set0bMessage(string pallet_num, string fm_location, string to_location,int start_position)
        {
            Direction = 0;
            MessageType = "0b";
            SendPriority = 2;
            if (start_position == 1)
                Message = pallet_num + fm_location.Trim() + to_location.Trim();
            else if (start_position == 2)
                Message = "00000000" + "00000000" + "0" + pallet_num + fm_location.Trim() + to_location.Trim();
            else return;
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void Set0DbMessage(string pallet_num1, string fm_location1, string to_location1, string pallet_num2, string fm_location2, string to_location2)
        {
            Direction = 0;
            MessageType = "0b";
            SendPriority = 2;
            Message = pallet_num1 + fm_location1.Trim() + to_location1.Trim() + "0" + pallet_num2 + fm_location2.Trim() + to_location2.Trim();
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void Set0AMessage()
        {
            Direction = 0;
            MessageType = "0A";
            SendPriority = 1;
            Message = "";
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0BMessage()
        {
            Direction = 0;
            SendPriority = 1;
            Message = "";
            MessageType = "0B";
            while (Message.Length < MessageLength)
                Message += "0";
        }
        public void Set0LMessage()
        {
            Direction = 0;
            SendPriority = 1;
            MessageType = "0L";
            Message = "";
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
        public void Set0QMessage(string pallet_num, string tunel, int i)
        {
            Direction = 0;
            MessageType = "0Q";
            SendPriority = 2;
            if (i == 2)
                Message = zero_eight + zero_eight + "0" + pallet_num + tunel;
            else if (i == 1)
                Message = pallet_num + tunel;
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
