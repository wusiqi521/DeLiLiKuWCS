using System;

namespace BMHRI.WCS.Server.Models
{
    public class GoodsLocation
    {
        //public string? FLPosition { set; get; }
        public string? Position { set; get; }
        public string WMSPosition { set; get; }
        public string? Tunnel { set; get; }
        public string PalletNum { set; get; }
        public string Warehouse { set; get; }
        public int TaskType { set; get; }
        public bool Available { set; get; }
        public string? DDJID { set; get; }
        private string rowStr;
        public string RowStr
        {
            get
            {
                if (string.IsNullOrEmpty(rowStr))
                {
                    rowStr= Row.ToString();
                    while (rowStr.Length < 2)
                        rowStr = "0"+rowStr;
                }
               return rowStr;
            }
        }
        private string rankStr;
        public string RankStr
        {
            get
            {
                if (string.IsNullOrEmpty(rankStr))
                {
                    rankStr = Rank.ToString();
                    while (rankStr.Length < 4)
                        rankStr = "0" + rankStr;
                }
                return rankStr;
            }
        }
        private string layerStr;
        public string LayerStr
        {
            get
            {
                if (string.IsNullOrEmpty(layerStr))
                {
                    layerStr = Layer.ToString();
                    while (layerStr.Length < 2)
                        layerStr = "0" + layerStr;
                }
                return layerStr;
            }
        }
        public int Row { set; get; }
        public int Rank { set; get; }
        public int Layer { set; get; }
        public DateTime UpdatePalletNumTime { set; get; }
        private string? goods_location_des;
        public string? GoodsLocationDescribe
        {
            set
            {
                goods_location_des = value;
            }
            get
            {
                GetGoodsLocationDescribe();
                return goods_location_des;
            }
        }
        public void GetGoodsLocationDescribe()
        {
            //GoodsLocationDescribe = string.Format("{0}排 {1}列 {2}层  托盘号:{3}  托盘数:{4}  货位状态:{5}", Row, Rank, Layer, PalletNum, Sublayer, Available);  0304-05-01
            goods_location_des = "";//0352-03-02
            goods_location_des = string.Format("{0}排 {1}列 {2}层 ", RowStr, RankStr, LayerStr);
            //if (FLPosition.Length == 10)
            //{
            //    switch (FLPosition.Substring(8, 2))
            //    {
            //        case "01":
            //            goods_location_des = goods_location_des + "近端";
            //            break;
            //        case "02":
            //            goods_location_des = goods_location_des + "远端";
            //            break;
            //        default:
            //            break;
            //    }
            //}
            if (Available)
                goods_location_des += " 货位可用";
            else goods_location_des += " 货位不可用";
            if (!string.IsNullOrEmpty(PalletNum))
                goods_location_des += string.Format(" 托盘号:{0}", PalletNum);
        }
    }
}
