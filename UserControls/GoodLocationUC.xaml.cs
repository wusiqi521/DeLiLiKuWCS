using BMHRI.WCS.Server.Models;
using System;
using System.Linq;
using System.Windows.Controls;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// GoodLocationUC.xaml 的交互逻辑
    /// </summary>
    public partial class GoodLocationUC : UserControl
    {
        public int RowNum;
        public int RankNum;
        public int LayerNum;
        public GoodsLocation goodsLocation;
        private string currentWarehouse;

        //public string PalletNum;
        public event EventHandler<string> GoodLocationChanged;
        public GoodLocationUC()
        {
            InitializeComponent();
            var rows = GoodsLocationManager.GoodsLocationList.GroupBy(x => x.Row).Select(group => new { Row = group.Key, Count = group.Count() }).OrderBy(x=>x.Row);
            RowNumCBB.ItemsSource = rows;
        }

        public string GetGoodLocation()
        {
            if (goodsLocation != null && goodsLocation.Available == true)
            {
                return goodsLocation.Position;
            }
            else return null;
        }

        public string GetWarehouse()
        {
            if (goodsLocation != null && goodsLocation.Available == true)
            {
                return goodsLocation.Warehouse;
            }
            else return null;
        }


        public void GoodLocationEmpty()
        {
            RowNumCBB.SelectedIndex = -1;
            RankNumCBB.SelectedIndex = -1;
            LayerNumCBB.SelectedIndex = -1;
        }

        private void RowNumCBB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RowNumCBB.SelectedIndex == -1) return;

            var selectedRow = ChangeType(RowNumCBB.SelectedValue, new { Row = 0, Count = 0 }).Row;
            RowNum = selectedRow;

            var filteredLocations = string.IsNullOrEmpty(currentWarehouse)
                ? GoodsLocationManager.GoodsLocationList
                : GoodsLocationManager.GoodsLocationList.Where(x => x.Warehouse == currentWarehouse);

            var Ranks = filteredLocations
                .Where(x => x.Row == selectedRow)
                .GroupBy(x => x.Rank)
                .Select(group => new { Rank = group.Key, Count = group.Count() })
                .OrderBy(x => x.Rank);

            RankNumCBB.ItemsSource = Ranks;
            RankNumCBB.SelectedIndex = -1;
            LayerNumCBB.ItemsSource = null;
            LayerNumCBB.SelectedIndex = -1;
        }






        private void RankNumCBB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RankNumCBB.SelectedIndex == -1) return;

            var selectedRank = ChangeType(RankNumCBB.SelectedValue, new { Rank = 0, Count = 0 }).Rank;
            RankNum = selectedRank;

            var filteredLocations = string.IsNullOrEmpty(currentWarehouse)
                ? GoodsLocationManager.GoodsLocationList
                : GoodsLocationManager.GoodsLocationList.Where(x => x.Warehouse == currentWarehouse);

            var Layers = filteredLocations
                .Where(x => x.Rank == selectedRank)
                .GroupBy(x => x.Layer)
                .Select(group => new { Layer = group.Key, Count = group.Count() })
                .OrderBy(x => x.Layer);

            LayerNumCBB.ItemsSource = Layers;
            LayerNumCBB.SelectedIndex = -1;
        }


        private void LayerNumCBB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 判断是否已选择RankNum和LayerNum
            if (RankNumCBB.SelectedIndex == -1 || LayerNumCBB.SelectedIndex == -1)
                return;

            // 读取选中的Layer
            var layerValue = ChangeType(LayerNumCBB.SelectedValue, new { Layer = 0, Count = 0 });
            int selectedLayer = layerValue.Layer;
            LayerNum = selectedLayer;

            // 在GoodsLocationList中，基于Row、Rank、Layer以及currentWarehouse进行过滤查找
            goodsLocation = GoodsLocationManager.GoodsLocationList
                .FirstOrDefault(x =>
                    x.Layer == selectedLayer
                    && x.Rank == RankNum
                    && x.Row == RowNum
                    && (string.IsNullOrEmpty(currentWarehouse) || x.Warehouse == currentWarehouse));

            // 如果查找不到对应的货位，就返回空字符串
            if (goodsLocation == null)
                OnGoodLocationChanged("");
            else
                OnGoodLocationChanged(goodsLocation.PalletNum);
        }


        private T ChangeType<T>(object obj, T t)
        {
            return (T)obj;
        }

        private void OnGoodLocationChanged(string pallet_num)
        {
            GoodLocationChanged?.Invoke(this, pallet_num);
        }
        public void UpdateRowNumCBBRows(string tunnel)
        {
            if (string.IsNullOrEmpty(tunnel)) return;
            var rows = GoodsLocationManager.GoodsLocationList.FindAll(x => x.Tunnel == tunnel).GroupBy(x => x.Row).Select(group => new { Row = group.Key, Count = group.Count() }).OrderBy(x => x.Row);
            RowNumCBB.ItemsSource = null;
            RowNumCBB.ItemsSource = rows;
        }
        public void FilterByWarehouse(string warehouse)
        {
            currentWarehouse = warehouse;

            if (string.IsNullOrEmpty(warehouse))
            {
                RowNumCBB.ItemsSource = GoodsLocationManager.GoodsLocationList
                    .GroupBy(x => x.Row)
                    .Select(group => new { Row = group.Key, Count = group.Count() })
                    .OrderBy(x => x.Row);
            }
            else
            {
                var filteredLocations = GoodsLocationManager.GoodsLocationList
                    .Where(x => x.Warehouse == warehouse)
                    .ToList();

                var rows = filteredLocations
                    .GroupBy(x => x.Row)
                    .Select(group => new { Row = group.Key, Count = group.Count() })
                    .OrderBy(x => x.Row);

                RowNumCBB.ItemsSource = rows;
            }

            GoodLocationEmpty();
        }


    }
}
