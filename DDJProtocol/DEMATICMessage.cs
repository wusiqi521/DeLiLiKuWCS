using System;
using System.ComponentModel;
using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.Models;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data;

namespace BMHRI.WCS.Server.DDJProtocol
{
    public class DEMATICMessage
    {
        public int MsgTypeIndex=0;
        public int MsgTypeLength = 6;

        public int MsgSenderIndex = 6;
        public int MsgSenderLength = 4;

        public int MsgReceiverIndex = 10;
        public int MsgReceiverLength = 4;

        public int MsgSeqIDIndex = 14;
        public int MsgSeqIDLength = 4;

        public int ReturnCodeIndex = 18;
        public int ReturnCodeLength = 4;

        public int MsgLengthIndex = 24;
        public int MsgLength = 4;

        public int MsgDDJWorkStateIndex = 42;
        public int MsgDDJWorkStateLength = 2;

        public int MsgToLocationIndex = 56;
        public int MsgToLocationLength = 14;

        public int MsgPalletIndex = 70;
        public int MsgPalletLength = 10;

        public int MsgSTATLength = 46;
        public int MsgSTAXLength = 306;
        public int MsgSTENLength = 30;
        public int MsgLIVELength = 30;
        public int MsgTUMILength = 30;
        public int MsgTUNOLength = 124;
        public int MsgTURPLength = 124;
        public int MsgTUEXLength = 124;
        public int MsgTUMCLength = 30;
        public int MsgTUCALength = 124;

        public int MsgErrorIndex = 116;
        public int MsgErrorLength = 2;

        public int MsgAllErrorIndex = 42;
        public int MsgAllErrorLength = 6;

        //报体

        public string UnMessageBlockType = "NG";
        public string MessageBlockType = "LG";

        public string MsgType;
        public string Message;
        public string MsgHeader;
        public string MsgSender;
        public string MsgReceiver;
        public string MsgEnd;
        public MsgReturnCodes ReturnCodes { get; set; }
        public DEMATICMessageDirection Direction { get; set; } 
        

        public string PLCID { get; set; }
        public DateTime Tkdat { get; set; }
        private int msgid;
        public int MsgID
        {
            get
            {
                if (msgid >= 9999)
                {
                    msgid = 0;
                }
                msgid++;
                return msgid;
            }
            set
            {
                MsgID = value;
            }
        }
        private string msgseqid;
        public string MsgSeqID
        {
            get 
            {
                return msgseqid; 
            }
            set { msgseqid = value; }
        }
        public string MsgParse { get; set; }

        private string trans;
        public string Trans
        {
            get
            {
                if (string.IsNullOrEmpty(trans))
                {
                    trans = GetSendTrans();
                }
                return trans;
            }
            set { trans = value; }
        }
        

        public DEMATICMessage(string plcid,string message,DEMATICMessageDirection msg_dir)
        {
            PLCID = plcid;
            Tkdat = DateTime.Now;
            MsgType = message.Substring(MsgTypeIndex, MsgTypeLength);
            MsgSeqID = message.Substring(MsgSeqIDIndex, MsgSeqIDLength);
            MsgSender = message.Substring(MsgSenderIndex, MsgSenderLength);
            MsgReceiver = message.Substring(MsgReceiverIndex, MsgReceiverLength);
            MsgParse = "";
            Direction = msg_dir;
            MsgEnd = message.Substring(18);
        }
        public DEMATICMessage(string plcid)
        {
            PLCID = plcid;
            Tkdat = DateTime.Now;
            MsgParse = "";
        }
        public DEMATICMessage(string plcid,string positive_send)
        {
            PLCID = plcid;
            Tkdat = DateTime.Now;
            //MsgSeqID = SocketManager.Instance.GetMaxID();
            MsgParse = "";
        }
        public DEMATICMessage()
        {

        }
        
        /// <summary>
        /// 类似于联机申请 WCS主动发送
        /// /.STRQWCS1UL087158OK01LG0044 ALL...........##
        /// </summary>
        public void SetSTRQMessage(string receiver)
        {
            MsgHeader = "/.";
            MsgType = "STRQ";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = GetMsgSeqID(PLCID);
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK01LG0044ALL...........##";
        }

        /// <summary>
        /// 类似于联机应答 WCS被动回复
        /// /ASTATWCS1UL080643OK00NG0030##
        /// </summary>
        /// <returns></returns>
        public void SetSTATMessage(string receiver,string msg_id)
        {
            MsgHeader = "/A";
            MsgType = "STAT";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = msg_id;
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK00NG0030##";
        }
        /// <summary>
        /// 堆垛机位置发生改变 WCS被动做出应答
        /// /ASTAXWCS1UL080644OK00NG0030##
        /// </summary>
        public void SetSTAXMessage(string receiver,string msg_id)
        {
            MsgHeader = "/A";
            MsgType = "STAX";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = msg_id;
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK00NG0030##";
        }
        /// <summary>
        /// 堆垛机状态询问结束 WCS被动做出应答
        /// /ASTENWCS1UL080645OK00NG0030##
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="msg_id"></param>
        public void SetSTENMessage(string receiver,string msg_id)
        {
            MsgHeader = "/A";
            MsgType = "STEN";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = msg_id;
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK00NG0030##";
        }
        /// <summary>
        /// 堆垛机心跳信号 WCS被动做出应答
        /// /ALIVEWCS1UL080004OK00NG0030##
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="msg_id"></param>
        public void SetLIVEMessage(string receiver,string msg_id)
        {
            MsgHeader = "/A";
            MsgType = "LIVE";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = msg_id;
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK00NG0030##";
        }
        /// <summary>
        /// 心跳信号 WCS每隔30s主动发出
        /// /RLIVEWCS1UL080004OK00NG0030##
        /// </summary>
        /// <param name="receiver"></param>
        public void SetActiveLiveMessage(string receiver)
        {
            MsgHeader = "/R";
            MsgType = "LIVE";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = GetMsgSeqID(PLCID);
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK00NG0030##";
        }
        /// <summary>
        /// 入库指令 WCS主动发送     
        /// /RTUMIWCS1UL085739OK01LG0124
        /// CCTA01DP61....ULAI08CL01PS10  UL081032030111 G01-000000000042......IP01000000000000
        /// 00000000OK0100##
        /// </summary>
        public void SetTUMIIntaskMessage(string receiver,string fmlocation,string toLocation,string pallet)
        {
            MsgHeader = "/R";
            MsgType = "TUMI";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = GetMsgSeqID(PLCID);
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK01LG0124" + fmlocation + fmlocation + toLocation + pallet+"............"+"IP01000000000000" + "00000000OK0100##";
        }
        /// <summary>
        /// 入库取货完成反馈   WCS被动做出应答
        /// /ATUNOWCS1UL080576OK00NG0030##
        /// </summary>
        /// <param name="receiver"></param>
        public void SetTUNOMessage(string receiver,string msg_id)
        {
            MsgHeader = "/A";
            MsgType = "TUNO";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = msg_id;
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK00NG0030##";
        }
        /// <summary>
        /// 入库放货完成  WCS被动做出应答
        /// /ATURPWCS1UL080578OK00NG0030##
        /// </summary>
        /// <param name="receiver"></param>
        public void SetTURPMessage(string receiver,string msg_id)
        {
            MsgHeader = "/A";
            MsgType = "TURP";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = msg_id;
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK00NG0030##";
        }
        /// <summary>
        /// 出库指令 WCS主动发送       
        /// /RTUMIWCS1UL087256OK01LG0124
        ///  UL081032030111 UL081032030111 ULAI08CR01DS10 G01-000000000042......IP01000000000000
        ///  00000000OK0100##
        /// </summary>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public void SetTUMIOutTaskMessage(string receiver,string fmLocation,string toLocation,string pallet)
        {
            MsgHeader = "/R";
            MsgType = "TUMI";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = GetMsgSeqID(PLCID); 
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK01LG0124" + fmLocation + fmLocation + toLocation + pallet + "............" + "IP01000000000000" + "00000000OK0100##";
        }
        /// <summary>
        /// 直出指令 WCS主动发送       
        /// /RTUMIWCS1UL087256OK01LG0124
        ///  UL081032030111 UL081032030111 ULAI08CR01DS10 G01-000000000042......IP01000000000000
        ///  00000000OK0100##
        /// </summary>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public void SetTUMIDirectTaskMessage(string receiver, string fmLocation, string toLocation, string pallet)
        {
            MsgHeader = "/R";
            MsgType = "TUMI";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = GetMsgSeqID(PLCID);
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK01LG0124" + fmLocation + fmLocation + toLocation + pallet + "............" + "IP01000000000000" + "00000000OK0100##";
        }
        /// <summary>
        /// 倒库指令 WCS主动发送
        /// /RTUMIWCS1UL080003OK01LG0124
        /// UL081032030111UL081032030111UL081038020211G01-000000000042......IP01000000000000
        /// 00000000OK0100##
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="fmLocation"></param>
        /// <param name="toLocation"></param>
        /// <param name="pallet"></param>
        public void SetTUMIMoveTaskMessage(string receiver, string fmLocation, string toLocation, string pallet)
        {
            MsgHeader = "/R";
            MsgType = "TUMI";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = GetMsgSeqID(PLCID);
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK01LG0124" + fmLocation + fmLocation + toLocation + pallet + "............" + "IP01000000000000" + "00000000OK0100##";
        }
        /// <summary>
        /// 空出库报警 双重入库 出深浅有 入深浅有反馈   WCS被动发送
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="msg_id"></param>
        public void SetTUEXMessage(string receiver,string msg_id)
        {
            MsgHeader = "/A";
            MsgType = "TUEX";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = msg_id;
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK00NG0030##";
        }
        /// <summary>
        /// 堆垛机故障后 DDJ取消任务完成反馈   WCS被动发送
        /// /ATUCAWCS1UL080259OK00NG0030##
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="msg_id"></param>
        public void SetTUCAMessage(string receiver, string msg_id)
        {
            MsgHeader = "/A";
            MsgType = "TUCA";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = msg_id;
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK00NG0030##";
        }
        /// <summary>
        /// 堆垛机故障后，WCS取消任务 WCS主动发送
        /// /RTUMCWCS1UL080004OK01LG0124
        /// UL081032030111UL081032030111UL081038020211G01-000000000042......IP01000000000000
        /// 00000000OK0100##
        /// </summary>
        /// <returns></returns>
        public void SetTUMCMessage(string receiver, string fmLocation, string toLocation, string pallet)
        {
            MsgHeader = "/R";
            MsgType = "TUMC";
            MsgSender = "WCS1";
            MsgReceiver = receiver;
            MsgSeqID = GetMsgSeqID(PLCID);
            Direction = DEMATICMessageDirection.Send;
            MsgEnd = "OK01LG0124" + "CCTA01DP61...." + fmLocation + toLocation + pallet + ".............." + "IP01000000000000" + "00000000OK0100##";
        }
        public string GetSendTrans()
        {
            return MsgHeader + MsgType + MsgSender + MsgReceiver + MsgSeqID + MsgEnd;
        }

        public string ConvertMsgIDToString(int id)
        {
            string MsgID = "000" + id.ToString();
            if (MsgID.Length == 4)
            {
                return MsgID;
            }
            else
            {
                return MsgID.Substring(MsgID.Length - 4, 4);
            }
        }
        public string GetMsgSeqID(string plc_id)
        {
            if (string.IsNullOrEmpty(plc_id))
                return null;
            SqlParameter[] sqlParameters = new SqlParameter[]
               {
                    new SqlParameter("@PLCID",plc_id),
                    new SqlParameter("@ReturnValue",SqlDbType.Int),
               };
            object obj=  SQLServerHelper.ExeProcedure("PR_Get_Dematic_MsgID", sqlParameters);
            return obj.ToString();
        }
    }
    public enum MsgReturnCodes
    {
        [Description("无错误")]
        OK,
        [Description("未知的消息名称")]
        MU,
        [Description("未知的消息发送者")]
        SU,
        [Description("未知的消息接收者")]
        RU,
        [Description("消息长度不合法")]
        LE,
        [Description("接收缓存区容量耗尽")]
        BF,
        [Description("总体协议错误")]   
        GE
    }
    public enum DEMATICMessageDirection
    {
        [Description("WCS->PLC")]
        Send = 0,
        [Description("PLC->WCS")]
        Receive = 1
    }
}
