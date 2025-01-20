using System;
using System.ComponentModel;

namespace BMHRI.WCS.Server.Models
{
    public class WMSMessage
	{
        readonly int trans_length = 30;
        public WMSMessageDirection MsgDirection { set; get; }
        public	string TRANS { set; get; }
		public DateTime TKDAT{set;get;}
		public string 	DEMO1{set; get; }
		public string DEMO2{set; get; }
		public string DEMO3{set; get; }
		public string guid { set; get; }
		public string Task_Priority { set; get; }
		public string ReturnMsg { set; get; }
        public string MsgParse { set; get; }
        internal string GetMessageType()
        {
			//H1212 12210 00000 00666 62193 00100
			if (string.IsNullOrEmpty(TRANS)) return null;
			if (TRANS.Length < trans_length) return null;
			return TRANS.Substring(0, 1);
        }

		internal bool IsRightLength()
		{
			return TRANS.Length == trans_length;
		}

        //U77777777080002010001000000100
        //B 99006886 08000611 2001 0000 00002
        readonly int pallet_num_length = 8;
        readonly int pallet_num_start_index= 1;
        readonly int port_length = 4;
        readonly int good_location_length = 8;
        readonly int agv_port_index = 10;
        readonly int good_1location_start_index = 9;
        readonly int good_2location_start_index = 17;
        readonly int port_1start_index = 17;
        readonly int port_2start_index = 21;
        readonly int outbound_type_index = 29;
        readonly int gaodi_bz_length = 1;
        readonly int agv_task_type_length = 1;
        readonly int outbound_type_length = 1;
        readonly int weight_port_index = 9;
        readonly int weight_port_length = 4;
        readonly int weight_index = 13;
        readonly int weight_length = 4;

        internal string GetWeightPort()
        {
            if (IsRightLength())
                return TRANS.Substring(weight_port_index, weight_port_length);
            else return null;
        }
        internal string GetWeigh()
        {
            if (IsRightLength())
                return TRANS.Substring(weight_index, weight_length);
            else return null;
        }
        internal string Get2Port()
        {
            if (IsRightLength())
                return TRANS.Substring(port_2start_index, port_length);
            else return null;
        }
        //A 990068451700000010080000 10000
        //B 000000010400020110010000 00002
        internal string Get1Port()
        {
            if (IsRightLength())
                return TRANS.Substring(port_1start_index, port_length);
            else return null;
        }
        internal string GetAGVPort()
        {
            if (IsRightLength())
                return TRANS.Substring(agv_port_index, port_length);
            else return null;
        }
        internal string GetPalletNum()
        {
            if (IsRightLength())
                return TRANS.Substring(pallet_num_start_index, pallet_num_length);
            else return null;
        }
        internal string GetTaskType()
        {
            if (IsRightLength())
                return TRANS.Substring(good_1location_start_index, agv_task_type_length);
            else return null;
        }
        internal string GetAgvPort()
        {
            if (IsRightLength())
                return TRANS.Substring(10, port_length);
            else return null;
        }

        internal string GetGaoDi()
        {
            if (IsRightLength())
                return TRANS.Substring(27, gaodi_bz_length);
            else return null;
        }

        internal string Get1Goodlocation()
        {
            if (IsRightLength())
                return TRANS.Substring(good_1location_start_index, good_location_length);
            else return null;
        }
        internal string GetOutBoundType()
        {
            if (IsRightLength())
                return TRANS.Substring(outbound_type_index, outbound_type_length);
            else return null;
        }

        internal string Get2Goodlocation()
        {
            if (IsRightLength())
                return TRANS.Substring(good_2location_start_index, good_location_length);
            else return null;
        }
        public void CreateWriteToWMSMessage()
        {
            guid = Guid.NewGuid().ToString("N");
            TKDAT = DateTime.Now;
            MsgDirection = WMSMessageDirection.Output;
        }
        //H 12121221 00000000 6666 2193 00100
        public void CreateHMessageToWMS(string pallet_num,string fm_port,string weight_num,string gaodi_pallet,string auto_in,string pallet_count)
        {
            CreateWriteToWMSMessage();
            //TRANS = "H" + pallet_num + "00000000" + fm_port + weight_num + "00" + pallet_type + pallet_count;
            if (weight_num == null || weight_num.Length != 8)
            {
                weight_num = "0000";
            }
            else if (weight_num.Length == 8)
                weight_num = weight_num.Substring(4, 4);
            if (pallet_count == "" || pallet_count == null)
            {
                if (fm_port == "1013")
                {
                    pallet_count = "6";
                }
                else if (fm_port == "2028")
                {
                    pallet_count = "8";
                }
            }
            TRANS = "H" + pallet_num + "00000000" + fm_port + "0" + weight_num + "0" + gaodi_pallet + auto_in + pallet_count;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
       // D99006587 000000000001000000000
        public void CreateDMessageToWMS(string pallet_num, string fm_port)
        {
            CreateWriteToWMSMessage();
            TRANS = "D" + pallet_num + "00000000" + fm_port ;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        public void CreateEMessageToWMS(string pallet_num, string fm_port,string to_goodlocation)
        {
            CreateWriteToWMSMessage();
            TRANS = "E" + pallet_num + fm_port+ to_goodlocation;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        //F99008261130013082004000000000
        public void CreateFMessageToWMS(string pallet_num, string to_port, string fm_goodlocation)
        {
            CreateWriteToWMSMessage();
            TRANS = "F" + pallet_num + fm_goodlocation + to_port;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        public void CreateIMessageToWMS(string pallet_num, string to_port, string fm_goodlocation)
        {
            CreateWriteToWMSMessage();
            TRANS = "I" + pallet_num + fm_goodlocation + to_port;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        public void CreateXMessageToWMS(string pallet_num, string to_port)
        {
            CreateWriteToWMSMessage();
            TRANS = "X" + pallet_num + "00000000" + to_port;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        public void CreateNMessageToWMS(string pallet_num, string task_type)
        {
            CreateWriteToWMSMessage();
            TRANS = "N" + pallet_num  + task_type;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        public void CreateNMessageToWMS(string pallet_num, string task_type,string ssj_port)
        {
            CreateWriteToWMSMessage();
            TRANS = "N" + pallet_num + task_type + ssj_port;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        public void CreateZMessageToWMS(string pallet_num, string fm_location,string to_location)
        {
            CreateWriteToWMSMessage();
            TRANS = "Z" + pallet_num + fm_location + to_location;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        public void CreateGMessageToWMS(string pallet_num, string fm_location,string type)
        {
            CreateWriteToWMSMessage();
            TRANS = "G" + pallet_num + fm_location + type;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        public void CreateJMessageToWMS(string pallet_num, string fm_location, string err_code)
        {
            CreateWriteToWMSMessage();
            TRANS = "J" + pallet_num + fm_location + err_code;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        public void CreateMMessageToWMS(string pallet_num, string to_goodlocation, string fm_goodlocation)
        {
            CreateWriteToWMSMessage();
            TRANS = "M" + pallet_num + fm_goodlocation + to_goodlocation;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        //S 0000 0000 0000 0000 0000 W00100000
        public void CreateSMessageToWMS(string ddj_num, string error_code)
        {
            CreateWriteToWMSMessage();
            TRANS = "S00000000000000000000" + error_code + "00" + ddj_num;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        public void CreateSSJSMessageToWMS(string ssj_num, string error_code,string ssj_block,string status)
        {
            CreateWriteToWMSMessage();
            TRANS = "S0000000000000000" + ssj_block + status + "00" + ssj_num + error_code;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        //P00000000000000003085000000000
        public void CreatePMessageToWMS(string fm_port,string box_type)
        {
            CreateWriteToWMSMessage();
            TRANS = "P0000000000000000" + fm_port + box_type;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
        public void CreateTMessageToWMS(string fm_port, string materia_exist)
        {
            CreateWriteToWMSMessage();
            TRANS = "T0000000000000000" + fm_port + materia_exist;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }

        //Q99008019 14003907 2004 060600000
        public void CreateQMessageToWMS(string palletNum, string toLocation,string fmLocation,string errorCode)
        {
            CreateWriteToWMSMessage();
            TRANS = "Q" + palletNum+toLocation+fmLocation+errorCode;
            while (TRANS.Length < trans_length)
                TRANS += "0";
        }
    }
  public enum WMSMessageDirection
    {
        [Description("发送")]
        Output,
        [Description("接收")]
        Input
    }
}
