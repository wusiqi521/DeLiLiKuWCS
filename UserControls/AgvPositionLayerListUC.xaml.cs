using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.UserWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// AgvPositionLayerListUC.xaml 的交互逻辑
    /// </summary>
    public partial class AgvPositionLayerListUC : UserControl
    {
        public string Title {  get; set; }
        public int GroupID { get; set; }
        public ObservableCollection<AgvPosition> AgvPositionLCollection;
        public AgvPositionLayerListUC()
        {
            InitializeComponent();
            Gp.Header = Title;
            AgvPositionLCollection = new ObservableCollection<AgvPosition>(AgvManager.Instance.AgvPositionList.FindAll(x => x.GroupID == GroupID));
            AgvPositionLLV.ItemsSource = AgvPositionLCollection;
        }
        public AgvPositionLayerListUC(string title,int goupid)
        {
            InitializeComponent();
            Title = title;
            GroupID = goupid;
            Gp.Header = title;
            AgvPositionLCollection = new ObservableCollection<AgvPosition>(AgvManager.Instance.AgvPositionList.FindAll(x => x.GroupID == goupid));
            AgvPositionLLV.ItemsSource = AgvPositionLCollection;
        }
        private void WMSTaskLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem lv = (ListViewItem)sender;
            if (lv is null) return;
            if (lv.DataContext is AgvPosition agvPosition)
            {
                AgvPositionOptionWin agvPositionOptionWin = new AgvPositionOptionWin(agvPosition);
                agvPositionOptionWin.ShowDialog();
            }
            AgvPositionLCollection = new ObservableCollection<AgvPosition>(AgvManager.Instance.AgvPositionList.FindAll(x => x.GroupID == GroupID));
            AgvPositionLLV.ItemsSource = AgvPositionLCollection;

        }
    }
}
