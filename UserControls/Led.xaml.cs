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
    /// Led.xaml 的交互逻辑
    /// </summary>
    public partial class Led : UserControl
    {
        public Led(BaseModel baseModel)
        {
            InitializeComponent();
            BaseModel = baseModel;
            if (BaseModel == null) return;
            if (string.IsNullOrEmpty(BaseModel.ModelID)) return;
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == BaseModel.PLCID);
            if (sSJDevice == null) return;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == BaseModel.ModelID.Substring(1,4));
            if (sSJDeviceBlock == null) return;
            DataContext = sSJDeviceBlock;
        }
       
        public BaseModel BaseModel;
        

    }
}
