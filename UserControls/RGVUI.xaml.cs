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
    /// RGVUC.xaml 的交互逻辑
    /// </summary>
    public partial class RGVUI : UserControl
    {
        public int GroupID = 0;
        public BaseModel BaseModel;
        public RGVUI(BaseModel baseModel)
        {
            InitializeComponent();
            BaseModel = baseModel;
            if (BaseModel == null) return;
            if (string.IsNullOrEmpty(BaseModel.ModelID)) return;

            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == BaseModel.PLCID);
            if (sSJDevice == null) return;
            RGVDevice rGVDevice = sSJDevice.RGVDeviceList.Find(x => x.DeviceID == baseModel.ModelID);
            if (rGVDevice == null) return;
            DataContext = rGVDevice;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RGVBlockOptionWindows OptionWindow = new RGVBlockOptionWindows(BaseModel);
            OptionWindow.ShowDialog();
            if (string.IsNullOrEmpty(BaseModel.ModelID)) return;
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == BaseModel.PLCID);
            if (sSJDevice == null) return;
            RGVDevice rGVDevice = sSJDevice.RGVDeviceList.Find(x => x.DeviceID == BaseModel.ModelID);
            if (rGVDevice == null) return;
            DataContext = rGVDevice;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
