using BMHRI.WCS.Server.Tools;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMHRI.WCS.Server.Models
{
    public class GoodsLocationManager
    {
        private static readonly Lazy<GoodsLocationManager> lazy = new Lazy<GoodsLocationManager>(() => new GoodsLocationManager());
        public static GoodsLocationManager Instance { get { return lazy.Value; } }
        public event EventHandler<EventArgs> GoodsLocationChanged;

        private static List<GoodsLocation>? _goods_location_list;
        public static List<GoodsLocation>? GoodsLocationList
        {
            get
            {
                if (_goods_location_list == null)
                    CreateGoodsLocationList();
                return _goods_location_list;
            }
        }
        private static void CreateGoodsLocationList()
        {
            try
            {
                _goods_location_list = MyDataTableExtensions.ToList<GoodsLocation>(SQLServerHelper.DataBaseReadToTable("SELECT * FROM [dbo].[GoodsLocation]"));

                if (_goods_location_list == null)
                {
                    _goods_location_list = new List<GoodsLocation>();
                }
            }
            catch (Exception ex)
            {
               LogHelper.WriteLog("货位数据库读取异常", ex);
            }
        }

        internal void UpdateGoodLocationPalletNum(string position, string pallet_num,string warehouse)
        {
            try
            {
                if (position.Length < 8) return;
                GoodsLocation goodsLocation = GoodsLocationList.Find(x => x.Position == position.Substring(0, 8) && x.Warehouse == warehouse);
                if (goodsLocation == null) return;
                goodsLocation.PalletNum = pallet_num;
                goodsLocation.TaskType = 0;
                goodsLocation.UpdatePalletNumTime = DateTime.Now;

                SqlParameter[] sqlParameters = new SqlParameter[]
               {
                    new SqlParameter("@PalletNum",SqlNull(goodsLocation.PalletNum)),
                    new SqlParameter("@TaskType",SqlNull(goodsLocation.TaskType)),
                    new SqlParameter("@UpdatePalletNumTime",SqlNull(goodsLocation.UpdatePalletNumTime)),
                    new SqlParameter("@Position",SqlNull(goodsLocation.Position)),
                    new SqlParameter("@Warehouse",SqlNull(goodsLocation.Warehouse)),
               };
                string sqlstr = string.Format("UPDATE [dbo].[GoodsLocation] SET [PalletNum] =@PalletNum,[TaskType] =@TaskType,[UpdatePalletNumTime]=@UpdatePalletNumTime WHERE Position=@Position AND Warehouse=@Warehouse");
                SQLServerHelper.ExeSQLStringWithParam(sqlstr, sqlParameters);
                OnGoodsLocationChanged(goodsLocation);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UpdateGoodLocationPalletNum 货位数据库修改异常", ex);
            }
        }
        private void OnGoodsLocationChanged(GoodsLocation goodsLocation)
        {
            GoodsLocationChanged?.Invoke(this, new GoodsLocationEventArgs(goodsLocation));
        }
        public object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;

            return obj;
        }
        public class GoodsLocationEventArgs : EventArgs
        {
            public GoodsLocationEventArgs(GoodsLocation goodsLocation)
            {
                GoodsLocation = goodsLocation;
            }
            public GoodsLocation GoodsLocation { get; set; }
        }
    }
}
