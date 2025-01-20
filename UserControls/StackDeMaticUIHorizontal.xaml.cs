using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.UserWindows;
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
    /// StackUIHorizontal.xaml 的交互逻辑
    /// </summary>
    public partial class StackDeMaticUIHorizontal : UserControl
    {
        public BaseModel BaseMode;
        public DeMaticDDJ DeMaticDDJ;
        public StackDeMaticUIHorizontal(BaseModel baseModel)
        {
            InitializeComponent();
            BaseMode = baseModel;
            if (baseModel == null) return;
            DeMaticDDJ = DeMaticDDJManager.Instance.DeMaticDDJList.Find(x => x.PLCID == baseModel.ModelID);
            if (DeMaticDDJ == null) return;
            DataContext = DeMaticDDJ;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DeMaticDDJ == null) return;
            DmDDJDetailWindow dDJDetailForm = new DmDDJDetailWindow(DeMaticDDJ);
            dDJDetailForm.ShowDialog();
        }
    }
}
