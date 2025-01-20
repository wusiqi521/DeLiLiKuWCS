using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.Models;
using System.Windows.Media;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// WinRackLocationManager.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsLocationManagerUC : System.Windows.Controls.UserControl
    {
        public GoodsLocationManagerUC()
        {
            InitializeComponent();
            GoodsLocationRowTV.Loaded += GoodsLocationRowTV_Loaded;
        }

        private void GoodsLocationTV_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedItem = GoodsLocationRowTV.SelectedItem as TreeViewItem;
            GoodsLocationLV.ItemsSource = null;

            if (selectedItem == null) return;

            // 检查是否选择的是排节点（有父节点）
            TreeViewItem parentWarehouse = GetParentTreeViewItem(selectedItem);
            if (parentWarehouse == null)
            {
                // 选择的是根节点（仓库），不处理
                return;
            }

            string warehouseGroup = parentWarehouse.Header.ToString();
            string rowStr = "";

            if (selectedItem.Header.ToString().StartsWith("第"))
            {
                rowStr = selectedItem.Header.ToString().Substring(1, 2);
            }
            else
            {
                return;
            }

            // 根据仓库组确定需要过滤的 Warehouse ID
            List<string> warehouses = new List<string>();
            if (warehouseGroup == "北起院立库")
            {
                warehouses.Add("1503");
                warehouses.Add("1504");
            }
            else if (warehouseGroup == "北起院模具库")
            {
                warehouses.Add("1519");
            }

            if (string.IsNullOrEmpty(rowStr) || warehouses.Count == 0)
            {
                return;
            }

            // 设置标题
            RowGoodsLocationMapTitleLB.Content = $"第{rowStr}排货位分布图";
            RowGoodsLocationMap.Visibility = Visibility.Visible;
            RowGoodsLocationGrid.Visibility = Visibility.Visible;

            // 更新货位分布图
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                RowGoodsLocationMap.CreateRackLocation(warehouses, rowStr);
            });

            // 更新统计信息
            var filteredList = GoodsLocationManager.GoodsLocationList
                .Where(x => warehouses.Contains(x.Warehouse) && x.Row.ToString("D2") == rowStr)
                .ToList();

            TotalGoodsLB.Content = $"当前货位总数为：{filteredList.Count}";
            UsedGoodsLB.Content = $"已占用货位数：{filteredList.Count(x => !string.IsNullOrEmpty(x.PalletNum))}";
            UnavailableGoodsLB.Content = $"不可用货位数：{filteredList.Count(x => !x.Available)}";
            RemainAvailableGoodsLB.Content = $"剩余可用货位数：{filteredList.Count(x => x.Available && string.IsNullOrEmpty(x.PalletNum))}";
        }

        private void GoodsLocationRowTV_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeTreeView();
            TreeViewItem storageRoot = GoodsLocationRowTV.Items.OfType<TreeViewItem>().FirstOrDefault();
            if (storageRoot != null && storageRoot.Items.Count > 0)
            {
                TreeViewItem firstRowItem = storageRoot.Items.OfType<TreeViewItem>().FirstOrDefault();
                if (firstRowItem != null)
                {
                    firstRowItem.IsSelected = true;
                    // 手动触发选中事件逻辑
                    GoodsLocationTV_SelectedItemChanged(GoodsLocationRowTV, null);
                }
            }
        }

        private void InitializeTreeView()
        {
            try
            {
                if (GoodsLocationRowTV.Items.Count > 0) return;

                // 创建根节点
                TreeViewItem storageRoot = new TreeViewItem { Header = "北起院立库" };
                TreeViewItem moldRoot = new TreeViewItem { Header = "北起院模具库" };

                // 定义各仓库对应的组
                var storageWarehouses = new List<string> { "1503", "1504" };
                var moldWarehouse = "1519";

                // 获取各组对应的货位数据
                var storageGoodsLocations = GoodsLocationManager.GoodsLocationList
                    .Where(x => storageWarehouses.Contains(x.Warehouse))
                    .ToList();

                var moldGoodsLocations = GoodsLocationManager.GoodsLocationList
                    .Where(x => x.Warehouse == moldWarehouse)
                    .ToList();

                // 获取北起院立库的排
                var storageRows = storageGoodsLocations
                    .Select(x => x.Row)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                foreach (var row in storageRows)
                {
                    TreeViewItem rowItem = new TreeViewItem
                    {
                        Header = $"第{row:D2}排"
                    };
                    storageRoot.Items.Add(rowItem);
                }

                // 获取北起院模具库的排
                var moldRows = moldGoodsLocations
                    .Select(x => x.Row)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                foreach (var row in moldRows)
                {
                    TreeViewItem rowItem = new TreeViewItem
                    {
                        Header = $"第{row:D2}排"
                    };
                    moldRoot.Items.Add(rowItem);
                }

                // 将根节点添加到 TreeView
                GoodsLocationRowTV.Items.Add(storageRoot);
                GoodsLocationRowTV.Items.Add(moldRoot);

                // 展开根节点
                storageRoot.IsExpanded = true;
                moldRoot.IsExpanded = true;

                // 默认选择第一个排
                if (storageRoot.Items.Count > 0)
                {
                    ((TreeViewItem)storageRoot.Items[0]).IsSelected = true;
                }

                // 订阅鼠标左键点击事件
                RowGoodsLocationMap.MouseLeftButtonUp += RowRackLocationMap_MouseLeftButtonUp;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化货位树结构时发生错误: {ex.Message}");
            }
        }
        private TreeViewItem GetParentTreeViewItem(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (parent != null && !(parent is TreeViewItem))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as TreeViewItem;
        }


        private void RowRackLocationMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                GoodsLocationLV.ItemsSource = null;
                GoodsLocationLV.ItemsSource = RowGoodsLocationMap.GetSelectedRacks();
                RKLVRowCount.Content = GoodsLocationLV.Items.Count;
            });
        }

        private void RackNumQueryBT_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(RackQueryRowNumTB.Text) ||
                string.IsNullOrEmpty(RackQueryyRankNumTB.Text) ||
                string.IsNullOrEmpty(RackQueryLayerNumTB.Text))
            {
                return;
            }
            else
            {
                TreeViewItem selectedItem = GoodsLocationRowTV.SelectedItem as TreeViewItem;
                TreeViewItem parentWarehouse = GetParentTreeViewItem(selectedItem);
                if (parentWarehouse == null) return;

                string warehouseGroup = parentWarehouse.Header.ToString();
                List<string> warehouses = new List<string>();
                if (warehouseGroup == "北起院立库")
                {
                    warehouses.Add("1503");
                    warehouses.Add("1504");
                }
                else if (warehouseGroup == "北起院模具库")
                {
                    warehouses.Add("1519");
                }

                string palletNum = GetPalletNumString(warehouses,
                    RackQueryRowNumTB.Text.Trim(),
                    RackQueryyRankNumTB.Text.Trim(),
                    RackQueryLayerNumTB.Text.Trim());

                RackQueryPalletNumTB.Text = palletNum;
            }
        }
        private string GetPalletNumString(List<string> warehouses, string rowNums, string colNums, string layNums)
        {
            int rowNum = int.Parse(rowNums);
            int colNum = int.Parse(colNums);
            int layNum = int.Parse(layNums);

            string RackNum = GetRackNum(rowNum, colNum, layNum);

            // 在多个仓库中查找 Position
            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList
                .FirstOrDefault(data => warehouses.Contains(data.Warehouse) && data.Position == RackNum);

            return goodsLocation?.PalletNum ?? "";
        }

        private string GetPalletNumString(string rowNums, string colNums, string layNums)
        {

            int rowNum = int.Parse(rowNums);
            int colNum = int.Parse(colNums);
            int layNum = int.Parse(layNums);

            string RackNum = GetRackNum(rowNum, colNum, layNum);

            GoodsLocation goodsLocation = GoodsLocationManager.GoodsLocationList.Find(data => data.Position == RackNum);
            if (goodsLocation != null)
                return goodsLocation.PalletNum;
            else return "";
        }
        private string GetRackNum(int row, int column, int layer)
        {
            string RackNum = "";

            RackNum += row < 10 ? $"0{row}" : row.ToString();
            RackNum += column < 10 ? $"000{column}" : column < 100 ? $"00{column}" : column < 1000 ? $"0{column}" : column.ToString();
            RackNum += layer < 10 ? $"0{layer}" : layer.ToString();

            return RackNum;
        }
        // 文件: GoodsLocationManagerUC.xaml.cs

        private void PalletNumQueryBT_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(RackQueryPalletNumTB.Text.Trim())) return;

            TreeViewItem selectedItem = GoodsLocationRowTV.SelectedItem as TreeViewItem;
            TreeViewItem parentWarehouse = GetParentTreeViewItem(selectedItem);
            if (parentWarehouse == null) return;

            string warehouseGroup = parentWarehouse.Header.ToString();
            List<string> warehouses = new List<string>();
            if (warehouseGroup == "北起院立库")
            {
                warehouses.Add("1503");
                warehouses.Add("1504");
            }
            else if (warehouseGroup == "北起院模具库")
            {
                warehouses.Add("1519");
            }

            string pallet_num = RackQueryPalletNumTB.Text.Trim();
            if (string.IsNullOrEmpty(pallet_num)) return;

            GoodsLocationLV.ItemsSource = GoodsLocationManager.GoodsLocationList
                .Where(x => warehouses.Contains(x.Warehouse) &&
                            !string.IsNullOrEmpty(x.PalletNum) &&
                            x.PalletNum.Contains(pallet_num))
                .ToList();
        }


        private void TextBox_PrviewExecuted(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void OnlyNumNeeded_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9) ||
               e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)
            {
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void ExportRacksBT_Click(object sender, RoutedEventArgs e)
        {
            // 设置实际的文件夹路径，例如选择桌面
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"Choho-Ware-Location-List-{DateTime.Now:yyyyMMddHHmmssfff}.xls";

            // 查询所有货位，包含 Warehouse
            DataTable? dt = SQLServerHelper.DataBaseReadToTable("SELECT Warehouse, Position AS 货位, PalletNum AS 托盘号 FROM dbo.GoodsLocation");

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("导出数据为空或读取失败。");
                return;
            }

            // 异步导出到 Excel
            Task.Run(() =>
            {
                ExcelHelper.ExportToExcel(dt, System.IO.Path.Combine(folderPath, fileName), "sheet1");
                MessageBox.Show("导出完成！");
            });
        }

        private void RefreshRacksBT_Click(object sender, RoutedEventArgs e)
        {
            // 假设 GoodsLocationManager 有刷新方法
           // GoodsLocationManager.RefreshGoodsLocationList();

            TreeViewItem selectedItem = GoodsLocationRowTV.SelectedItem as TreeViewItem;
            GoodsLocationLV.ItemsSource = null;

            if (selectedItem == null) return;

            // 检查是否选择的是排节点
            TreeViewItem parentWarehouse = GetParentTreeViewItem(selectedItem);
            if (parentWarehouse == null)
            {
                // 选择的是根节点，不做操作
                return;
            }

            string warehouseGroup = parentWarehouse.Header.ToString();
            string rowStr = "";

            if (selectedItem.Header.ToString().StartsWith("第"))
            {
                rowStr = selectedItem.Header.ToString().Substring(1, 2);
            }

            // 确定需要过滤的仓库
            List<string> warehouses = new List<string>();
            if (warehouseGroup == "北起院立库")
            {
                warehouses.Add("1503");
                warehouses.Add("1504");
            }
            else if (warehouseGroup == "北起院模具库")
            {
                warehouses.Add("1519");
            }

            if (string.IsNullOrEmpty(rowStr) || warehouses.Count == 0)
            {
                return;
            }

            // 更新货位分布图
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                RowGoodsLocationMap.CreateRackLocation(warehouses, rowStr);
            });
        }

        private void RackQueryExportBT_Click(object sender, RoutedEventArgs e)
        {
            // 设置实际的文件夹路径，例如选择桌面
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"Choho-Selected-Ware-Location-List-{DateTime.Now:yyyyMMddHHmmssfff}.xls";

            List<GoodsLocation> goodsLocations = GoodsLocationLV.Items.Cast<GoodsLocation>().ToList();

            if (goodsLocations.Count == 0)
            {
                MessageBox.Show("没有选择的货位进行导出。");
                return;
            }

            // 异步导出到 Excel
            Task.Run(() =>
            {
                DataTable dt = ToDataTable(goodsLocations);
                ExcelHelper.ExportToExcel(dt, System.IO.Path.Combine(folderPath, fileName), "sheet1");
                MessageBox.Show("导出完成！");
            });
        }

        public  DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

        private void RackLocationLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GoodsLocationLV.SelectedItems.Count != 1) return;
            if (!(GoodsLocationLV.SelectedItem is GoodsLocation goodsLocation)) return;
            //UserForm.RackLoactionModifyForm rackLoactionModifyForm = new UserForm.RackLoactionModifyForm
            //{
            //    DataContext = goodsLocation
            //};
            //rackLoactionModifyForm.ShowDialog();
        }
    }    
}
