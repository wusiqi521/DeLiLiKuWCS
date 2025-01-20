using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.UserWindows;
using System.Windows;
using System.Windows.Controls;
using System;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// ChainMUI.xaml 的交互逻辑
    /// </summary>
    public partial class ChainMUI : UserControl
    {
        public int GroupID = 0;
        public BaseModel BaseModel;
        public ChainMUI(BaseModel baseModel)
        {
            InitializeComponent();
            BaseModel = baseModel;
            if (BaseModel == null) return;
            if (string.IsNullOrEmpty(BaseModel.ModelID)) return;
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == BaseModel.PLCID);
            if (sSJDevice == null) return;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == BaseModel.ModelID[1..]);
            if (sSJDeviceBlock == null) return;
            DataContext = sSJDeviceBlock;
        }

        private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SSJBlockOptionWindows OptionWindow = new SSJBlockOptionWindows(BaseModel);
            OptionWindow.ShowDialog();
            //if (string.IsNullOrEmpty(BaseModel.ModelID)) return;
            //SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == BaseModel.PLCID);
            //if (sSJDevice == null) return;
            //SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position.Contains(BaseModel.ModelID[1..]));
            //if (sSJDeviceBlock == null) return;
            //DataContext = sSJDeviceBlock;
        }
    }
}
