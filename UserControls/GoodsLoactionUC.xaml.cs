using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// Interaction logic for RackUC.xaml
    /// </summary>
    public partial class GoodsLoactionUC : UserControl
    {
        public string Layer { get; set; }
        public string PalletNum { get; set; }
        public string Rank { get; set; }
        public string Row { get; set; }
        public string Position { get; set; }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (value ==true)
                {
                    RackUCBorder.BorderThickness = new Thickness(0.5);
                    RackUCBorder.BorderBrush = Brushes.Red;
                }
                else
                {
                    RackUCBorder.BorderThickness = new Thickness(0.2);
                    RackUCBorder.BorderBrush = Brushes.Green;
                }
                isSelected = value;
            }
        }

        public GoodsLoactionUC()
        {
            InitializeComponent();
        }
    }
}
