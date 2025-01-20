using System;

namespace BMHRI.WCS.Server.DDJProtocol
{
    public class DDJMessage
    {
        public const int TatolLength=42;//改到42
        public int MsgTypeIndex=0;
        public int MsgTypeLength=2;

        public int ErrorCodeIndex = 18;
        public int ErrorCodeLength = 4;

        public int StartRowIndex=10;
        public int StartRowLength=2;
        public int StartRankIndex=12;
        public int StartRankLength=4;
        public int StartLayerIndex=16;
        public int StartLayerLength=2;

        public int EndRowIndex=18;
        public int EndRowLength=2;
        public int EndRankIndex=20;
        public int EndRankLength=4;
        public int EndLayerIndex=24;
        public int EndLayerLength=2;
        public int SublayerIndex=26;
        public int SublayerLength=2;

        public int OldPalletNumIndex = 2;
        public int OldPalletNumLength = 8;
        public const string EightZero = "00000000";
        public const string TwelveZero = "000000000000";
        public int PalletNumIndex = 30;
        public int PalletNumLength = 12;


        public int LocationLength = 8;
        public int PortIndex = 18;
        public int PortLength = 4;

        public string Row;
        public string Rank;
        public string Layer;
        public string Sublayer;

        public string PalletNum;
        public string PortAddress;
        public string ErrorCode;

        public string MessageType;
        public string Message;
        public int MessageLength = 26;
        
        public string UnreadVerificationCodeDB4 = "01";
        public string ReadedVerificationCodeDB4 = "00";

        public string UnreadVerificationCodeDB5 = "01";
        public string ReadedVerificationCodeDB5 = "10";


        public DDJMessageDirection MsgDir { get; set; }
        public DDJMessageDirection Direction { get; set; }

        public string PLCID { get; set; }
        public DateTime Tkdat { get; set; }
        public int SendPriority { get; set; }
        public string MsgSeqID { get; set; }
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
            set { trans = value; }
        }

        public DDJMessage()
        {

        }
        public DDJMessage(string trans, string plc_id,DDJMessageDirection msg_dir)
        {
            if (string.IsNullOrEmpty(trans) || trans.Length != TatolLength)
                return;
            Trans = trans;
            Tkdat = DateTime.Now;
            PLCID = plc_id;
            MessageType = trans.Substring(MsgTypeIndex, MsgTypeLength);
            MsgSeqID = Guid.NewGuid().ToString("N");
            MsgDir = msg_dir;
            Direction = msg_dir;   
        }
        public DDJMessage(string trans, string plc_id, DDJMessageDirection msg_dir,string msg_parse)
        {
            if (string.IsNullOrEmpty(trans) || trans.Length != TatolLength)
                return;
            Trans = trans;
            Tkdat = DateTime.Now;
            PLCID = plc_id;
            MessageType = trans.Substring(MsgTypeIndex, MsgTypeLength);
            MsgSeqID = Guid.NewGuid().ToString("N");
            MsgDir = msg_dir;
            Direction = msg_dir;
            MsgParse = msg_parse;
        }
        public DDJMessage(string plcid)
        {
            PLCID = plcid;
            Tkdat = DateTime.Now;
            MsgSeqID = Guid.NewGuid().ToString("N");
            MsgParse = "";
        }

        public string GetTransDB4()
        {
            if (PalletNum == null) { PalletNum = "000000000000"; }
            else if (PalletNum.Length < 12) { PalletNum = PalletNum.PadRight(12, '*'); }
            return MessageType + Message + UnreadVerificationCodeDB4 + PalletNum;
        }

        public string GetLocation()
        {
            return Trans.Substring(StartRowIndex, LocationLength);
        }

        public string GetErrorCode()
        {
            ErrorCode = Trans.Substring(ErrorCodeIndex, ErrorCodeLength);
            return ErrorCode;
        }

        public string GetPalletNum()
        {
            if (string.IsNullOrEmpty(Trans) || Trans.Length != TatolLength) return null;
            if(Trans.Substring(PalletNumIndex, PalletNumLength).Contains("**")) return Trans.Substring(PalletNumIndex, PalletNumLength-2);

            return Trans.Substring(PalletNumIndex, PalletNumLength);
        }

        public string GetPortNum(string fm_location)
        {
            string port_num = null;
            switch (fm_location.Substring(0, 1))
            {
                case "1":
                    port_num = "000a";
                    break;
                case "2":
                    port_num = "000b";
                    break;
                case "4":
                    port_num = "000c";
                    break;
                default:
                    break;
            }
            return port_num;

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
        public void Set0BMessage()
        {
            MessageType = "0B";
            Message = "";
            SendPriority = 0;
            MsgDir = DDJMessageDirection.Send;
            while (Message.Length < MessageLength)
                Message = Message + "0";
        }
        public void Set0LMessage()
        {
            MessageType = "0L";
            Message = "";
            SendPriority = 0;
            MsgDir = DDJMessageDirection.Send;
            while (Message.Length < MessageLength)
                Message = Message + "0";
        }
        public void SetAAMessage()
        {
            MessageType = "0A";
            Message = "";
            SendPriority = 0;
            MsgDir = DDJMessageDirection.Send;
            while (Message.Length < MessageLength)
                Message = Message + "0";
        }

        public void Set0aMessage(string palletNum, string fmLocation, string toLocation,string gaoDi)
        {
            MessageType = "0a";
            if (palletNum.Length == 10)
            {
                palletNum = palletNum + "**";
            }
            PalletNum = palletNum;
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            if (toLocation.Length == 10)
            {
                Sublayer = toLocation.Substring(8);
                toLocation = toLocation.Substring(0, 8);
            }
            else if (toLocation.Length == 8) Sublayer = "00";
            //string fmPortNum = GetPortNum(fmLocation);
            ///if (!string.IsNullOrEmpty(fmPortNum))27

            Message = EightZero + toLocation + fmLocation + Sublayer + "00" + gaoDi + "0";
            while (Message.Length < MessageLength)
                Message = Message + "0";
        }



        public void Set0bMessage(string palletNum, string fmLocation, string toLocation)
        {
            MessageType = "0b";
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            if (palletNum.Length == 10)
            {
                palletNum = palletNum + "**";
            }
            PalletNum = palletNum;

            //string toPortNum = GetPortNum(toLocation);
            //if (!string.IsNullOrEmpty(toPortNum))
                Message = EightZero + fmLocation + toLocation;
            while (Message.Length < MessageLength)
                Message = Message + "0";
        }

        public void SetAuto0bMessage(string palletNum, string fmLocation, string toLocation)
        {
            MessageType = "0b";
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            PalletNum = palletNum;
            Message = EightZero + fmLocation + toLocation;
            while (Message.Length < MessageLength)
                Message = Message + "0";
        }

        public void Set0mMessage(string palletNum, string fmLocation, string toLocation)
        {
            MessageType = "0m";
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            if(palletNum.Length== 10)
            {
                palletNum = palletNum + "**";
            }
            PalletNum = palletNum;

            Message = EightZero + fmLocation + toLocation;
            while (Message.Length < MessageLength)
                Message = Message + "0";

        }
        public void Set0eMessage(string palletNum, string fmLocation, string toLocation)
        {
            MessageType = "0e";
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            if (palletNum.Length == 10)
            {
                palletNum = palletNum + "**";
            }
            PalletNum = palletNum;

            string toPortNum = toLocation;
            string fmPortNum = fmLocation;
            if (!string.IsNullOrEmpty(toPortNum) && !string.IsNullOrEmpty(fmPortNum))
                Message = EightZero + EightZero+ fmPortNum + toPortNum;
            while (Message.Length < MessageLength)
                Message = Message + "0";
        }

        public void SetMMMessage(string trans)
        {
            MessageType = "0M";
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            Trans = trans;
        }
        public void SetGGMessage(string trans)
        {
            MessageType = "0G";
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            Trans = trans;
        }
        public void SetEEMessage(string trans)
        {
            MessageType = "0E";
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            Trans = trans;
        }
        public void SetFFMessage(string trans)
        {
            MessageType = "0F";
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            Trans = trans;
        }

        public void Set0SMessage(string trans)
        {
            MessageType = "0S";
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            Trans = MessageType + trans.Substring(MsgTypeLength);
        }

        public void Set0YMessage(string trans)
        {
            MessageType = "0Y";
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            Trans = trans;
        }

        public void Set00Message()
        {
            MessageType = "00";
            SendPriority = 1;
            MsgDir = DDJMessageDirection.Send;
            Message = "";
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void Set0dMessage(string portNum)
        {
            MessageType = "0d";
            SendPriority = 1;
            PalletNum = TwelveZero;
            MsgDir = DDJMessageDirection.Send;
            Message = "0000000000000000"+portNum;
            while (Message.Length < MessageLength)
                Message += "0";
        }

        public void Set0jMessage( )
        {
            MessageType = "0@";
            SendPriority = 1;
            PalletNum = TwelveZero;
            MsgDir = DDJMessageDirection.Send;
            Message = "0000000000000000" ;
            while (Message.Length < MessageLength)
                Message += "0";
        }
    }
    public enum DDJMessageDirection
    {
        [System.ComponentModel.Description("WCS->PLC")]
        Send = 0,
        [System.ComponentModel.Description("PLC->WCS")]
        Receive = 1
    }

}
