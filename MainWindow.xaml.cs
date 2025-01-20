using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Styles;
using BMHRI.WCS.Server.UserControls;
using BMHRI.WCS.Server.WebApi;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BMHRI.WCS.Server.Models;
namespace BMHRI.WCS.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double ScreenWidth { get; set; }
        private double Screenheight { get; set; }
        //private double SideBarWidth = 50;
        //private int ViewCount = 7;
        public MainWindow()
        {
            InitializeComponent();
         // ScreenWidth = SystemParameters.WorkArea.Width-SideBarWidth;//得到屏幕工作区域宽度
           /// Screenheight = SystemParameters.WorkArea.Height;//得到屏幕工作区域高度
            //double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            //double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度
            //ContainerListGrid.Width = ViewCount * ScreenWidth;
            //ContainerListGrid.Height = Screenheight-32;
        }
        private bool SideBarComeOn = false;
        //private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    Point ptMouse = e.GetPosition(this);
        //    if (ptMouse.X < SideBarWidth && SideBarComeOn == false)
        //    {
        //        ChangeToolBarGridMarginLeft(0);
        //        SideBarComeOn = true;
        //        ChangeContainerListGridMarginLeft(SideBarWidth);
        //        //System.Diagnostics.Debug.WriteLine("ptMouse.X = " + ptMouse.X.ToString()+ " SideBarComeOn = " + SideBarComeOn);
        //    }
        //    else if(ptMouse.X > SideBarWidth&&SideBarComeOn == true)
        //    {
        //        ChangeToolBarGridMarginLeft(-SideBarWidth);
        //        SideBarComeOn = false;
        //        ChangeContainerListGridMarginLeft(-SideBarWidth);
        //        //System.Diagnostics.Debug.WriteLine("ptMouse.X = " + ptMouse.X.ToString() + " SideBarComeOn = " + SideBarComeOn);
        //    }
        //}
       
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //WMSDBManager.Instance.Start();
            foreach (Systems systemm in SystemManager.Instance.SystemList)
            {
                WMSUC wMSStatusUC = new WMSUC();
                wMSStatusUC.DeviceName.Content = systemm.SystemType;
                wMSStatusUC.SystemNum = systemm.ID;
                wMSStatusUC.Width = 100;
                wMSStatusUC.Height = 80;
                Device_wrap_panel.Children.Add(wMSStatusUC);
            }

            foreach (SSJDevice ssj_device in PLCDeviceManager.Instance.SSJDeviceList)
            {
                SSJUC ssj_uc = new SSJUC();
                ssj_uc.DeviceName.Content = ssj_device.PLCDecription;
                ssj_uc.DeviceNum = ssj_device.PLCID;
                ssj_uc.Width = 100;
                ssj_uc.Height = 80;
                Device_wrap_panel.Children.Add(ssj_uc);

            }
            foreach (DDJDevice ddj_device in PLCDeviceManager.Instance.DDJDeviceList)
            {
                DDJUC ddj_uc = new DDJUC(ddj_device);
                ddj_uc.DeviceName.Content = ddj_device.PLCDecription;
                ddj_uc.DeviceNum = ddj_device.PLCID;
                ddj_uc.Width = 100;
                ddj_uc.Height = 80;
                DDJ_wrap_panel.Children.Add(ddj_uc);
            }

            ButtonUC button_uc = new ButtonUC();
            button_uc.Width = 600;
            button_uc.Height = 80;
            DDJ_wrap_panel.Children.Add(button_uc);
            WebSockectServer.Instance.Start();
            WebApiServer.Instance.Start();
            WMSWebApiClient.Instance.Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("您确定退出WCS系统？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)

            {
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
