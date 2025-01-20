using BMHRI.WCS.Server.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// Interaction logic for GoodsLocationMap1UC.xaml
    /// </summary>
    public partial class GoodskLocationMapUC : UserControl
    {
        public string RowStr { get; set; }
        public List<GoodsLoactionUC> RowGLUCList;
        public GoodskLocationMapUC()
        {
            InitializeComponent();
            RowGLUCList = new List<GoodsLoactionUC>();
        }
        public void CreateRackLocation(List<string> warehouses, string row_str)
        {
            if (warehouses == null || warehouses.Count == 0 || string.IsNullOrEmpty(row_str)) return;

            RowRackLocationMap.Children.Clear();
            RowGLUCList.Clear();

            // 解析 row_str 为整数
            if (!int.TryParse(row_str, out int rowNumber)) return;

            // 根据仓库和排号过滤货位
            var goodsLocations = GoodsLocationManager.GoodsLocationList
                .Where(x => warehouses.Contains(x.Warehouse) && x.Row == rowNumber)
                .OrderBy(x => x.Rank)
                .ThenByDescending(x => x.Layer)
                .ToList();

            if (goodsLocations == null || goodsLocations.Count == 0) return;

            // 计算列数和层数
            int columnCount = goodsLocations.GroupBy(x => x.Rank).Count();
            int layerCount = goodsLocations.GroupBy(x => x.Layer).Count();

            foreach (var goodsLocation in goodsLocations)
            {
                GoodsLoactionUC goodsLoactionUC = new GoodsLoactionUC
                {
                    Height = ActualHeight / layerCount,
                    Width = ActualWidth / columnCount,
                    Row = goodsLocation.Row.ToString("D2"),
                    Rank = goodsLocation.Rank.ToString(),
                    Layer = goodsLocation.Layer.ToString("D2"),
                    Position = goodsLocation.Position,
                    DataContext = goodsLocation
                };

                RowRackLocationMap.Children.Add(goodsLoactionUC);
                Canvas.SetLeft(goodsLoactionUC, goodsLoactionUC.Width * (goodsLocation.Rank - 1));
                Canvas.SetTop(goodsLoactionUC, goodsLoactionUC.Height * (layerCount - goodsLocation.Layer));
            }
        }
        public void CreateRackLocation(string row_str)
        {
            if (string.IsNullOrEmpty(row_str)) return;
            //RowRackLactionTitlLB.Content = "第" + RowStr + "排货位分布图";
            RowRackLocationMap.Children.Clear();
            int columnCount = GoodsLocationManager.GoodsLocationList.FindAll(x => x.RowStr == row_str).GroupBy(x => x.RankStr).Count();
            int layerCount = GoodsLocationManager.GoodsLocationList.FindAll(x => x.RowStr == row_str).GroupBy(x => x.LayerStr).Count();
            while (RowGLUCList.Count < columnCount * layerCount)
            {
                RowGLUCList.Add(new GoodsLoactionUC());
            }
            RowStr = row_str;
            List<GoodsLocation> goodsLocations = GoodsLocationManager.GoodsLocationList.FindAll(x => x.RowStr == row_str).OrderBy(x => x.RankStr).OrderByDescending(x => x.LayerStr).ToList();
            if (goodsLocations != null && goodsLocations.Count > 0)
            {
                for (int i = 0; i < goodsLocations.Count; i++)
                {
                    RowGLUCList[i].Height = ActualHeight / layerCount;
                    RowGLUCList[i].Width = ActualWidth / columnCount;
                    RowGLUCList[i].Row = goodsLocations[i].RowStr;
                    RowGLUCList[i].Rank = goodsLocations[i].RankStr;
                    RowGLUCList[i].Layer = goodsLocations[i].LayerStr;
                    RowGLUCList[i].Position = goodsLocations[i].Position;
                    RowGLUCList[i].DataContext = goodsLocations[i];

                    if (int.TryParse(goodsLocations[i].RankStr, out int ccount) && int.TryParse(goodsLocations[i].LayerStr, out int lcount))
                    {
                        RowRackLocationMap.Children.Add(RowGLUCList[i]);
                        Canvas.SetLeft(RowGLUCList[i], RowGLUCList[i].Width * (ccount - 1));
                        Canvas.SetTop(RowGLUCList[i], RowGLUCList[i].Height * (layerCount - lcount));
                    }

                }
            }
        }

        public void CreateRackLocation(int row_num)
        {
            //if (string.IsNullOrEmpty(row_num)) return;
            //RowRackLactionTitlLB.Content = "第" + RowStr + "排货位分布图";
            //RowRackLocationMap.Children.Clear();
            int columnCount = GoodsLocationManager.GoodsLocationList.FindAll(x => x.Row == row_num).GroupBy(x => x.RankStr).Count();
            int layerCount = GoodsLocationManager.GoodsLocationList.FindAll(x => x.Row == row_num).GroupBy(x => x.LayerStr).Count();
            foreach (GoodsLocation goodsLocation in GoodsLocationManager.GoodsLocationList.FindAll(x => x.Row == row_num).OrderBy(x => x.Rank).OrderByDescending(x => x.Layer))
            {
                GoodsLoactionUC goodsLoactionUC = new GoodsLoactionUC
                {
                    Height = ActualHeight / layerCount,
                    Width = ActualWidth / columnCount
                };
                goodsLoactionUC.Row = goodsLocation.RowStr;
                goodsLoactionUC.Rank = goodsLocation.RankStr;
                goodsLoactionUC.Layer = goodsLocation.LayerStr;
                goodsLoactionUC.Position = goodsLocation.Position;
                goodsLoactionUC.DataContext = goodsLocation;

                RowRackLocationMap.Children.Add(goodsLoactionUC);
                Canvas.SetLeft(goodsLoactionUC, goodsLoactionUC.Width * (goodsLocation.Rank - 1));
                Canvas.SetTop(goodsLoactionUC, goodsLoactionUC.Height * (layerCount - goodsLocation.Layer));

            }
        }
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //RowRackLactionTitlLB.Content = "第"+ RowStr+"排货位分布图";
            int rowCount = GoodsLocationManager.GoodsLocationList.GroupBy(x => x.Row).Count();
            for(int i=1;i<=rowCount;i++)
            CreateRackLocation(i);
        }

        /// 起始位置
        /// </summary>
        Point startPoint;
        //Rectangle rect = null;
        private void RowRackLocationMap_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(RowRackLocationMap);
            //this.rect = null;

            foreach (UIElement userControl in RowRackLocationMap.Children)
            {
                if (userControl is GoodsLoactionUC goodsLoactionUC)
                {
                    if (Canvas.GetLeft(userControl) + goodsLoactionUC.ActualWidth > startPoint.X
                        && Canvas.GetLeft(userControl) < startPoint.X
                        && Canvas.GetTop(userControl) + goodsLoactionUC.ActualHeight > startPoint.Y
                        && Canvas.GetTop(userControl) < startPoint.Y)
                    {
                        goodsLoactionUC.IsSelected = true;
                    }
                    else if (Keyboard.Modifiers != ModifierKeys.Control)
                        goodsLoactionUC.IsSelected = false;
                }
            }
        }

        private void RowRackLocationMap_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(RowRackLocationMap);
                //if (p != startPoint && rect == null)
                //{
                //    //rect = new Rectangle() { Stroke=Brushes.Red,StrokeThickness=0.5 };
                //    rect = new Rectangle(); ;
                //    RowRackLocationMap.Children.Add(rect);
                //}

                //rect.Width = Math.Abs(p.X - startPoint.X);
                //rect.Height = Math.Abs(p.Y - startPoint.Y);
                //Canvas.SetLeft(rect, Math.Min(p.X, startPoint.X));
                //Canvas.SetTop(rect, Math.Min(p.Y, startPoint.Y));

                foreach (UIElement userControl in RowRackLocationMap.Children)
                {
                    if (userControl is GoodsLoactionUC goodsLoactionUC)
                    {
                        if (Canvas.GetLeft(userControl) + goodsLoactionUC.ActualWidth > startPoint.X
                            && Canvas.GetLeft(userControl) < p.X
                            && Canvas.GetTop(userControl) + goodsLoactionUC.ActualHeight > startPoint.Y
                            && Canvas.GetTop(userControl) < p.Y)
                        {
                            goodsLoactionUC.IsSelected = true;
                        }
                        else if(Keyboard.Modifiers != ModifierKeys.Control)
                            goodsLoactionUC.IsSelected = false;
                    }
                }
            }
        }       

        public List<GoodsLocation> GetSelectedRacks()
        {
            List<GoodsLocation> SelectedList = new List<GoodsLocation>();

            foreach (UIElement userControl in RowRackLocationMap.Children)
            {
                if (userControl is GoodsLoactionUC goodsLoactionUC)
                {
                    GoodsLocation GoodsLocation = null;
                    if (goodsLoactionUC.IsSelected)
                        GoodsLocation =  GoodsLocationManager.GoodsLocationList.Find(x => x.Position == goodsLoactionUC.Position);
                    if (GoodsLocation != null)
                        SelectedList.Add(GoodsLocation);
                }
            }
            return SelectedList;
        }
    }
}
