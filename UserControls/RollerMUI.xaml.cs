using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.UserWindows;
using System.Windows.Controls;
using System;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// RollerMUI.xaml 的交互逻辑
    /// </summary>
    public partial class RollerMUI : UserControl
    {
        public int GroupID = 0;
        public BaseModel BaseModel;
        public RollerMUI(BaseModel baseModel)
        {
            InitializeComponent();
            BaseModel = baseModel;
            if (BaseModel == null) return;
            if (string.IsNullOrEmpty(BaseModel.ModelID)) return;
            if (baseModel.ModelID.Contains("2005"))
                baseModel.ToString();
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == string.Concat("SSJ0", BaseModel.ModelID.AsSpan(2, 1)));
            if (sSJDevice == null) return;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position.Contains(BaseModel.ModelID.Substring(2,4)));
            if (sSJDeviceBlock == null) return;
            DataContext = sSJDeviceBlock;
            //SSJDeviceBlock sSJDeviceBlock1 = sSJDevice.DeviceBlockList.Find(x => x.Position.Length == 5 && x.Position.Substring(0, 4) == sSJDeviceBlock.Position && x.Position != baseModel.DeviceID);
            //if (sSJDeviceBlock1 != null)
            //{
            //    RowDefinition rd1 = new RowDefinition();
            //    rd1.Height = new GridLength(12);
            //    g.RowDefinitions.Add(rd1);
            //    PalletPlaceUC palletPlaceUC = new PalletPlaceUC();
            //    palletPlaceUC.DataContext = sSJDeviceBlock1;
            //    palletPlaceUC.Width = 12;
            //    g.Children.Add(palletPlaceUC);
            //    Grid.SetRow(palletPlaceUC,2);
            //}
        }

        private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SSJBlockOptionWindows OptionWindow = new SSJBlockOptionWindows(BaseModel);
            OptionWindow.ShowDialog();
            if (string.IsNullOrEmpty(BaseModel.ModelID)) return;
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == string.Concat("SSJ0", BaseModel.ModelID.AsSpan(2, 1)));
            if (sSJDevice == null) return;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position.Contains(BaseModel.ModelID.Substring(2, 4)));
            if (sSJDeviceBlock == null) return;
            DataContext = sSJDeviceBlock;
        }
    }
}
