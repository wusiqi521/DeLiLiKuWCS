using Fleck;
using System;

namespace BMHRI.WCS.Server
{
    public class WCSClient
    {
        public string ClientName { get; set; }
        public string ClientType { get; set; }
        public string ClientIP { get; set; }
        public Guid ClientGuid { get; set; }
        public IWebSocketConnection Socket { get; set; }
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
    public enum ClientType
    {
        WCSClient,
        LCDClient,
        MsgID
    }
}
