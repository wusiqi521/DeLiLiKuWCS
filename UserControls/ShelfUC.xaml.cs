using BMHRI.WCS.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// ShelfUC.xaml 的交互逻辑
    /// </summary>
    public partial class ShelfUC : UserControl
    {
        public ShelfUC()
        {
            InitializeComponent();
        }
        int totalLayer;
        int totalRow;
        int maxRank,maxLayer;
        //double layoutBorderHeight;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                CreateShelfsLayout();
            }));
        }

        private void CreateShelfsLayout()
        {
            totalRow = GoodsLocationManager.GoodsLocationList.GroupBy(x => x.Row).Count();
            maxRank = GoodsLocationManager.GoodsLocationList.Max(x => x.Rank);
            maxLayer = GoodsLocationManager.GoodsLocationList.Max(x => x.Layer);
            for (int i = 0; i < maxRank; i++)
            {
                LayoutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < maxLayer * totalRow; i++)
            {
                LayoutGrid.RowDefinitions.Add(new RowDefinition());
            }
            LayoutGrid.ShowGridLines = true;
            foreach(GoodsLocation goodsLocation in GoodsLocationManager.GoodsLocationList)
            {
                GoodsLoactionUC goodsLoactionUC = new GoodsLoactionUC();               
                goodsLoactionUC.Row = goodsLocation.RowStr;
                goodsLoactionUC.Rank = goodsLocation.RankStr;
                goodsLoactionUC.Layer = goodsLocation.LayerStr;
                goodsLoactionUC.Position = goodsLocation.Position;
                goodsLoactionUC.DataContext = goodsLocation;

                LayoutGrid.Children.Add(goodsLoactionUC);
                Grid.SetRow(goodsLoactionUC, maxLayer * (goodsLocation.Row - 1) + goodsLocation.Layer - 1);
                Grid.SetColumn(goodsLoactionUC, goodsLocation.Rank-1);

            }
        }
    }
}
