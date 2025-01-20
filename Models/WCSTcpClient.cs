using System.Net;

namespace BMHRI.WCS.Server.Models
{
    class WCSTcpClient
    {
        public string ClientName { get; set; }
        public string ClientType { get; set; }
        public IPAddress ClientIP { get; set; }
        public string SessionKey { get; set; }
        private int msgid;
        public int MsgID
        {
            get
            {
                if (msgid > 9999)
                    msgid = 0;
                msgid++;
                return msgid;
            }
            set
            {
                msgid = value;
            }
        }
    }
}
