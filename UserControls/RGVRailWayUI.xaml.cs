using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.UserWindows;
using System.Windows;
using System.Windows.Controls;
using System;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// RGVRailWayUI.xaml 的交互逻辑
    /// </summary>
    public partial class RGVRailWayUI : UserControl
    {
        public int GroupID = 0;
        public BaseModel BaseModel;
        public RGVRailWayUI(BaseModel baseModel)
        {
            InitializeComponent();
        }
    }
}
