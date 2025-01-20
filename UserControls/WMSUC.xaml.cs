using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.UserWindows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// WMSUC.xaml 的交互逻辑
    /// </summary>
    public partial class WMSUC : UserControl
    {
        public WMSUC()
        {
            InitializeComponent();
        }
        public int SystemNum { get; set; }
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Systems system = SystemManager.Instance.SystemList.Find(x => x.ID == SystemNum);
            if (system != null)
            {
                DataContext = system;
            }
        }
    }
}
