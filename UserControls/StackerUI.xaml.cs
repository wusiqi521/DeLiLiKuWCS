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
    /// StackerUI.xaml 的交互逻辑
    /// </summary>
    public partial class StackerUI : UserControl
    {
        public BaseModel BaseMode;
        public DDJDevice DDJDevice;
        //public SocketManager SocketManager;
        
        public StackerUI(BaseModel baseModel)
        {
            InitializeComponent(); 
            BaseMode = baseModel;
            if (baseModel == null) return;
            DDJDevice = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == baseModel.PLCID);
            if (DDJDevice == null) return;
            DataContext = DDJDevice;

            //SocketManager = SocketManager.Instance.SocketManagerList.Find(x => x.PLCID == baseModel.ModelID);
            //if (SocketManager == null) return;
            DataContext = DDJDevice;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DDJDevice == null) return;
            DDJDetailWindow dDJDetailForm = new DDJDetailWindow(DDJDevice);
            dDJDetailForm.ShowDialog();
        }
    }
}
