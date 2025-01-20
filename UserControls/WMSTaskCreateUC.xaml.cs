using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// WMSTaskCreateUC.xaml 的交互逻辑
    /// </summary>
    public partial class WMSTaskCreateUC : UserControl
    {
        private static readonly WMSTaskCreateUC _instance = new WMSTaskCreateUC();
        public static WMSTaskCreateUC Instance => _instance;
        public List<SSJDeviceBlock> InboundBeginList;
        public List<SSJDeviceBlock> InboundSecondBeginList;
        public List<SSJDeviceBlock> InboundFinishList;
        public List<SSJDeviceBlock> OutboundFinishList;
        public List<SSJDeviceBlock> OutboundBeginList;
        public List<SSJDeviceBlock> PickingList;
        public const string WMSTaskSouce = "WMS";
        public const string WCSTaskSouce = "WCS";
        public List<string> WareHouseList = new List<string> { "1503", "1504", "1519" };
        // 在 WMSTaskCreateUC.xaml.cs 中
        public bool isAutoOutbound1Enabled = false;
        public string selectedOutboundFloor1 = string.Empty;

        public bool isAutoOutbound2Enabled = false;
        public string selectedOutboundFloor2 = string.Empty;

        public bool isAutoOutbound3Enabled = false;
        public string selectedOutboundFloor3 = string.Empty;

        

        public WMSTaskCreateUC()
        {
            InitializeComponent();
            InboundBeginList = new List<SSJDeviceBlock>();
            InboundSecondBeginList = new List<SSJDeviceBlock>();
            OutboundBeginList = new List<SSJDeviceBlock>();
            InboundFinishList = new List<SSJDeviceBlock>();
            OutboundFinishList = new List<SSJDeviceBlock>();
            PickingList = new List<SSJDeviceBlock>();
            // List<SSJDeviceBlock> ss = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == "SSJ02").DeviceBlockList.FindAll(x => x.SystemType > 0);
            foreach (SSJDevice sSJDevice in PLCDeviceManager.Instance.SSJDeviceList)
            {
                if (sSJDevice.DeviceBlockList == null) continue;
                if (sSJDevice.DeviceBlockList.Count < 1) continue;
                List<SSJDeviceBlock> special_blok_list = sSJDevice.DeviceBlockList.FindAll(x => x.SystemType > 0);
                if (special_blok_list == null || special_blok_list.Count < 1) continue;
                //WareHouseList.Add(special_blok_list.ware)
                foreach (SSJDeviceBlock ssj_device_bk in special_blok_list)
                {
                   // WareHouseList.Add(ssj_device_bk.WareHouse);
                    switch (ssj_device_bk.SystemType)
                    {
                        case DeviceSystemType.OutboundFinish:
                            OutboundFinishList.Add(ssj_device_bk);
                            break;
                        case DeviceSystemType.InboundFinish:
                            InboundFinishList.Add(ssj_device_bk);
                            break;
                        case DeviceSystemType.OutboundBegin:
                            OutboundBeginList.Add(ssj_device_bk);
                            break;
                        case DeviceSystemType.InboundBegin:
                            InboundBeginList.Add(ssj_device_bk);
                            break;
                        case DeviceSystemType.Picking:
                            PickingList.Add(ssj_device_bk);
                            InboundBeginList.Add(ssj_device_bk);
                            OutboundFinishList.Add(ssj_device_bk);
                            break;
                        case DeviceSystemType.InboundSecondBegin:
                            InboundSecondBeginList.Add(ssj_device_bk);
                            break;
                        case DeviceSystemType.InboundFinishOrOutboundBegin:
                            InboundFinishList.Add(ssj_device_bk);
                            OutboundBeginList.Add(ssj_device_bk);
                            break;
                        case DeviceSystemType.TotalPort:
                            InboundFinishList.Add(ssj_device_bk);
                            OutboundBeginList.Add(ssj_device_bk);
                            InboundBeginList.Add(ssj_device_bk);
                            OutboundFinishList.Add(ssj_device_bk);
                            break;
                        default:
                            break;
                    }
                }
            }
            IStaAddrCBB.ItemsSource = InboundBeginList;
            AStaAddrCBB.ItemsSource = InboundBeginList;
            CStaAddrCBB3.ItemsSource = InboundBeginList;
            InboundPortCBB.ItemsSource = InboundBeginList;
            OutWarehouse.ItemsSource = WareHouseList;
            OutWarehouse2.ItemsSource = WareHouseList;
            //SecondInboundPortCBB.ItemsSource = InboundSecondBeginList;
            UStaAddrCBB.ItemsSource = InboundFinishList;
            BEndAddrCBB.ItemsSource = OutboundFinishList;
            CStartAddrCBB.ItemsSource = InboundBeginList;
            CEndAddrCBB.ItemsSource = OutboundFinishList;
            CEndAddrCBB3.ItemsSource = OutboundFinishList;
            SStartAddrCBB.ItemsSource= OutboundBeginList;
            SEndAddrCBB.ItemsSource= OutboundFinishList;
            //EmptyAppPortCBB.ItemsSource = OutboundFinishList.FindAll(x => x.Position == "1032" || x.Position == "1027" || x.Position == "2028");
            BStaAddrGLUC.GoodLocationChanged += BStaAddrGLUC_GoodLocationChanged;
            MStaAddrGLUC.GoodLocationChanged+= BStaAddrGLUC_GoodLocationChanged;
        }

        private void BStaAddrGLUC_GoodLocationChanged(object sender, string e)
        {
            if (!(sender is GoodLocationUC goodLocationUC)) return;
            if (goodLocationUC.goodsLocation == null) return;
            string palletnum = goodLocationUC.goodsLocation.PalletNum;
            switch (goodLocationUC.Name)
            {
                case "BStaAddrGLUC":
                    BPaletNumTB.Text = palletnum;
                    break;
                case "MStaAddrGLUC":
                    MPaletNumTB.Text = palletnum;
                    break;
                default:
                    break;
            }
        }
        private void ApplyInboundBT_Click(object sender, RoutedEventArgs e)
        {
            if (InboundPortCBB.SelectedIndex == -1) return;
            if (PaletNumTB.Text.Length !=10&& PaletNumTB.Text.Length != 12) return;
            WMSTask wMSTask = new WMSTask();
            SSJDeviceBlock sSJDeviceBlock = InboundPortCBB.SelectedItem as SSJDeviceBlock;
           // WMSTask wMSTask = new WMSTask();
            wMSTask.TaskType = WMSTaskType.Inbound;

            wMSTask.TaskStatus = WMSTaskStatus.SSJ_APP_IN;
            wMSTask.PalletNum = PaletNumTB.Text;
            wMSTask.FmLocation= InboundPortCBB.Text;
            wMSTask.Warehouse = sSJDeviceBlock.WareHouse;//加上巷道传递给wmsapi
            wMSTask.TaskSource = "WMS";
            wMSTask.Floor = GetSSJFloor(InboundPortCBB.Text);
            if (wMSTask.Warehouse == "1503") { wMSTask.GaoDiBZ = WMSGaoDiBZ.Height; }
            else { wMSTask.GaoDiBZ = WMSGaoDiBZ.Height; }
            WMSTasksManager.Instance.AddWMSTask(wMSTask);

            PaletNumTB.Text = null;
            InboundPortCBB.SelectedIndex = -1;
        }
        //private void ApplyEmptyBT_Click(object sender, RoutedEventArgs e)
        //{
        //    if (EmptyAppPortCBB.SelectedIndex == -1) return; ;
        //    SSJDeviceBlock sSJDeviceBlock = EmptyAppPortCBB.SelectedItem as SSJDeviceBlock;
        //    WMSTask wMSTask = new WMSTask();
        //    wMSTask.TaskType = WMSTaskType.EmptyWanted;
        //    wMSTask.TaskStatus = WMSTaskStatus.SSJ_APP_EM;
        //    //wMSTask.PalletNum = PaletNumTB.Text;
        //    wMSTask.FmLocation = sSJDeviceBlock.Position;
        //    wMSTask.TaskSource = "WMS";
        //    WMSTasksManager.Instance.AddWMSTask(wMSTask);
        //    EmptyAppPortCBB.SelectedIndex = -1;
        //}
        private void CreateSSJInboundBT_Click(object sender, RoutedEventArgs e)
        {
            if (IStaAddrCBB.SelectedItem == null) return;
            if (IPaletNumTB.Text.Length != 10 && IPaletNumTB.Text.Length != 12) return;
            SSJDeviceBlock sSJDeviceBlock = IStaAddrCBB.SelectedItem as SSJDeviceBlock;
            string pallet_num = IPaletNumTB.Text;
            if (string.IsNullOrEmpty(pallet_num)) return;
            if (IEndAddrCBB.SelectedItem == null) return;
            SSJDeviceBlock sSJDeviceBlock_End = IEndAddrCBB.SelectedItem as SSJDeviceBlock;

            WMSTask wMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == IPaletNumTB.Text);
            if (wMSTask == null)
            {
                wMSTask = new WMSTask();
                wMSTask.PalletNum = pallet_num;
                wMSTask.TaskType = WMSTaskType.Inbound;
                wMSTask.FmLocation = sSJDeviceBlock.Position;
                wMSTask.ToLocation = sSJDeviceBlock_End.Position;
               // wMSTask.Warehouse = sSJDeviceBlock_End.WareHouse;
                wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
                wMSTask.TaskSource = "WCS";
                wMSTask.Floor = sSJDeviceBlock.Floor;
                wMSTask.Warehouse = sSJDeviceBlock.WareHouse;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
                IStaAddrCBB.SelectedIndex = -1;
                IPaletNumTB.Text = null;
                IEndAddrCBB.SelectedIndex = -1;
            }
            else
            {
                AlarmLabel.Content = "当前托盘有任务 托盘号：" + UPaletNumTB.Text;
            }
        }
        private void CreateOuboundBT_Click(object sender, RoutedEventArgs e)
        {
            if (BEndAddrCBB.SelectedItem == null) 
                return;
            if (BPaletNumTB.Text.Length != 10&& BPaletNumTB.Text.Length != 12) return;
            SSJDeviceBlock sSJDeviceBlock = BEndAddrCBB.SelectedItem as SSJDeviceBlock;
            string pallet_num = BPaletNumTB.Text;
            if (string.IsNullOrEmpty(pallet_num)) return;
            if (string.IsNullOrEmpty(BStaAddrGLUC.GetGoodLocation())) return;
            if (string.IsNullOrEmpty(OutWarehouse.SelectedItem.ToString())) return;
            WMSTask wMSTask = new WMSTask();
            wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
            wMSTask.Warehouse = OutWarehouse.SelectedItem.ToString();
            wMSTask.PalletNum = pallet_num;
            wMSTask.FmLocation = BStaAddrGLUC.GetGoodLocation();
            wMSTask.ToLocation = sSJDeviceBlock.Position;
            wMSTask.TaskSource = "WCS";
            wMSTask.TaskType = WMSTaskType.Outbound;
            wMSTask.Floor = sSJDeviceBlock.Floor;
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
            BEndAddrCBB.SelectedIndex = -1;
            OutWarehouse.SelectedIndex = -1;
            BPaletNumTB.Text = null;
            BStaAddrGLUC.GoodLocationEmpty();

        }

        private void CreateMvboundBT_Click(object sender, RoutedEventArgs e)
        {
            if (MPaletNumTB.Text.Length !=10&& MPaletNumTB.Text.Length != 12) return;
            if (string.IsNullOrEmpty(OutWarehouse2.SelectedItem.ToString())) return;
            WMSTask wMSTask = new WMSTask();
            wMSTask.TaskType = WMSTaskType.Moving;
            wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
            wMSTask.PalletNum = MPaletNumTB.Text;
            wMSTask.FmLocation = MStaAddrGLUC.GetGoodLocation();
            wMSTask.ToLocation = MEndAddrGLUC.GetGoodLocation();
            wMSTask.Warehouse = OutWarehouse2.SelectedItem.ToString();
            wMSTask.TaskSource = "WCS";
            WMSTasksManager.Instance.AddWMSTask(wMSTask);
            MPaletNumTB.Text = null;
            MStaAddrGLUC.GoodLocationEmpty();
            MEndAddrGLUC.GoodLocationEmpty();
            OutWarehouse2.SelectedIndex = -1;
        }
        private void AStaAddrCBB_DropDownClosed(object sender, System.EventArgs e)
        {            
            if (AStaAddrCBB.SelectedItem == null) return;;
            SSJDeviceBlock sSJDeviceBlock = AStaAddrCBB.SelectedItem as SSJDeviceBlock;
            if (sSJDeviceBlock == null) return;
           //if (StartAddr.Length != 4) return;

            if (sSJDeviceBlock == null||!sSJDeviceBlock.IsOccupied) return;
            APaletNumTB.Text = sSJDeviceBlock.PalletNum;
            // 5545 5555 5534 空托盘收集 只能去5525
            
            //if (sSJDeviceBlock.Position  Position_Jia_Colloction.c)
        }

        private void IStaAddrCBB_DropDownClosed(object sender, System.EventArgs e)
        {
            if (IStaAddrCBB.SelectedItem == null) 
                return; 
            SSJDeviceBlock sSJDeviceBlock = IStaAddrCBB.SelectedItem as SSJDeviceBlock;
            if (sSJDeviceBlock == null) return;
            IPaletNumTB.Text = sSJDeviceBlock.PalletNum;
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == sSJDeviceBlock.PLCID);
            if (sSJDevice == null) return;
            IEndAddrCBB.ItemsSource = sSJDevice.DeviceBlockList.FindAll(x => x.SystemType == DeviceSystemType.InboundFinish || x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.TotalPort);
        }
        private void UStaAddrCBB_DropDownClosed(object sender, System.EventArgs e)
        {
            if (UStaAddrCBB.SelectedItem == null) return;
            SSJDeviceBlock sSJDeviceBlock = UStaAddrCBB.SelectedItem as SSJDeviceBlock;
            if (sSJDeviceBlock == null) return;
            UPaletNumTB.Text = sSJDeviceBlock.PalletNum;
           // UEndAddrGLUC.UpdateRowNumCBBRows(sSJDeviceBlock.Tunnel);
            UEndAddrGLUC.FilterByWarehouse(sSJDeviceBlock.WareHouse);
          // BStaAddrGLUC
        }

        private void CreateMDirectBT_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CPaletNumTB3.Text)) return;
            if (CStaAddrCBB3.SelectedItem == null) return;
            SSJDeviceBlock sSJFmBlock = CStaAddrCBB3.SelectedItem as SSJDeviceBlock;
            if (CEndAddrCBB3.SelectedItem == null) return;
            SSJDeviceBlock sSJToBlock = CEndAddrCBB3.SelectedItem as SSJDeviceBlock;
            if(sSJToBlock.CurrentMode== DeviceModeType.OutboundMode)
            {
                WMSTask wMSTask = new WMSTask();
                wMSTask.TaskType = WMSTaskType.InToOut;
                wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
                wMSTask.PalletNum = CPaletNumTB3.Text;

                wMSTask.FmLocation = sSJFmBlock.Position;
                wMSTask.ToLocation = sSJToBlock.Position;
                wMSTask.TaskSource = "WCS";
                CPaletNumTB.Text = "";
                wMSTask.Warehouse = sSJFmBlock.WareHouse;
                CStaAddrCBB3.SelectedIndex = -1;
                CEndAddrCBB.SelectedIndex = -1;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
            }
            else
            {
                CAlarmLB3.Content = "目的地不符合出库模式！" ;
            }
            
        }
        private void CreateStackBT_Click(object sender, RoutedEventArgs e)
        {
            if (UStaAddrCBB.SelectedItem == null) return;
            if (UPaletNumTB.Text.Length !=10&& UPaletNumTB.Text.Length != 12) return;
            SSJDeviceBlock sSJDeviceBlock = UStaAddrCBB.SelectedItem as SSJDeviceBlock;
            string pallet_num = UPaletNumTB.Text;
            if (string.IsNullOrEmpty(pallet_num)) return;
            if (string.IsNullOrEmpty(UEndAddrGLUC.GetGoodLocation())) return;
            WMSTask wMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == UStaAddrCBB.Text);
            if (wMSTask == null)
            {
                wMSTask = new WMSTask();
                wMSTask.PalletNum = pallet_num;
                wMSTask.TaskType = WMSTaskType.Stacking;
                wMSTask.FmLocation = sSJDeviceBlock.Position;
                wMSTask.ToLocation = UEndAddrGLUC.GetGoodLocation();
                wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
                wMSTask.TaskSource = "WCS";
                wMSTask.Floor = sSJDeviceBlock.Floor;
                wMSTask.Warehouse = sSJDeviceBlock.WareHouse;//加上仓库号
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
                UPaletNumTB.Text = null;
                UEndAddrGLUC.GoodLocationEmpty();
                UStaAddrCBB.SelectedIndex = -1;
            }
            else
                UAlarmLB.Content = "当前托盘有任务 托盘号：" + UPaletNumTB.Text;
        }

        //private void BEndAddrCBB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (BEndAddrCBB.SelectedItem == null) return;
        //    SSJDeviceBlock sSJDeviceBlock = BEndAddrCBB.SelectedItem as SSJDeviceBlock;
        //    if (int.Parse(sSJDeviceBlock.Position.Substring(0, 2)) >= 41 && int.Parse(sSJDeviceBlock.Position.Substring(0, 2)) <= 48)
        //    {
        //        BEndAddrTB.Visibility = Visibility.Visible;
        //        MateriLB.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        BEndAddrTB.Visibility = Visibility.Collapsed;
        //        MateriLB.Visibility = Visibility.Collapsed;
        //    }
        //}

        //private void SecondApplyInboundBT_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SecondInboundPortCBB.SelectedIndex == -1) return;
        //    if (SecPaletNumTB.Text.Length < 8) return;
        //    if (string.IsNullOrEmpty(TaskidNumTB.Text)) return;
        //    WMSTask wMSTask = new WMSTask();
        //    wMSTask.TaskType = WMSTaskType.Inbound;
        //    wMSTask.TaskStatus = WMSTaskStatus.SSJ_APP_IN_Again;
        //    wMSTask.PalletNum = SecPaletNumTB.Text.Substring(0, 8);
        //    wMSTask.FmLocation = SecondInboundPortCBB.Text;
        //    wMSTask.TaskSource = "WMS";
        //    wMSTask.taskId = TaskidNumTB.Text;
        //    WMSTasksManager.Instance.AddWMSTask(wMSTask);

        //    SecPaletNumTB.Text = null;
        //    SecondInboundPortCBB.SelectedIndex = -1;
        //}

        private void CreateInboundBT_Click(object sender, RoutedEventArgs e)
        {
            if (AStaAddrCBB.SelectedItem == null)
                return;
            if (APaletNumTB.Text.Length != 10 && APaletNumTB.Text.Length != 12) return;
            SSJDeviceBlock sSJDeviceBlock = AStaAddrCBB.SelectedItem as SSJDeviceBlock;
            string pallet_num = APaletNumTB.Text;
            if (string.IsNullOrEmpty(pallet_num)) return;
            if (string.IsNullOrEmpty(AEndAddrGLUC.GetGoodLocation()))
            {
                AAlarmLB.Content = "当前货位不存在或已被锁定";
                return;
            }
            //if (!OperationCheck("创建联机入库任务 托盘号：" + pallet_num + "入库口：" + sSJDeviceBlock.Position + "目的地：" + AEndAddrGLUC.GetGoodLocation())) return;
            WMSTask wMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == pallet_num);
            if (wMSTask == null)
            {
                wMSTask = new WMSTask();
                wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
                wMSTask.PalletNum = pallet_num;
                wMSTask.ToLocation = AEndAddrGLUC.GetGoodLocation();
                wMSTask.FmLocation = sSJDeviceBlock.Position;
                wMSTask.TaskSource = "WCS";
                wMSTask.TaskType = WMSTaskType.AutoInbound;
                wMSTask.Floor = sSJDeviceBlock.Floor;
                wMSTask.Warehouse = sSJDeviceBlock.WareHouse;//加上仓库号
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
                AStaAddrCBB.SelectedIndex = -1;
                APaletNumTB.Text = null;
                AEndAddrGLUC.GoodLocationEmpty();
                AAlarmLB.Content = "";
            }
            else
                AAlarmLB.Content = "当前托盘有任务 托盘号：" + pallet_num;
        }
        private void SStartAddrCBB_DropDowmClosed(object sender, System.EventArgs e)
        {
            if (SStartAddrCBB.SelectedItem == null) return;
            SSJDeviceBlock sSJDeviceBlock = SStartAddrCBB.SelectedItem as SSJDeviceBlock;
            if (sSJDeviceBlock == null) return;
            SPaletNumTB.Text = sSJDeviceBlock.PalletNum;
        }
        private void CStartAddrCBB_DropDowmClosed(object sender, System.EventArgs e)
        {
            if (SStartAddrCBB.SelectedItem == null) return;
            SSJDeviceBlock sSJDeviceBlock = SStartAddrCBB.SelectedItem as SSJDeviceBlock;
            if (sSJDeviceBlock == null) return;
            SPaletNumTB.Text = sSJDeviceBlock.PalletNum;
        }
        private void CreateSSJOutboundBT_Click(object sender, RoutedEventArgs e)
        {
            if (SStartAddrCBB.SelectedItem == null) return; ;
            SSJDeviceBlock sSJDeviceBlock = SStartAddrCBB.SelectedItem as SSJDeviceBlock;
            if (SPaletNumTB.Text.Trim().Length !=10&& SPaletNumTB.Text.Trim().Length != 12) return;
            string pallet_num = SPaletNumTB.Text.Trim();
            if (string.IsNullOrEmpty(pallet_num)) return;
            if (SEndAddrCBB.SelectedItem == null) return;
            SSJDeviceBlock sSJDeviceBlock_End = SEndAddrCBB.SelectedItem as SSJDeviceBlock;
            //if (!OperationCheck("创建输送机出库任务 托盘号：" + pallet_num + "起始地：" + sSJDeviceBlock.Position + "目的地:" + sSJDeviceBlock_End.Position)) return;
            WMSTask wMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == pallet_num);
            if (wMSTask == null)
            {
                wMSTask = new WMSTask();
                wMSTask.PalletNum = pallet_num;
                wMSTask.TaskType = WMSTaskType.SSJOutbound;
                wMSTask.Warehouse = sSJDeviceBlock.WareHouse;
                wMSTask.FmLocation = sSJDeviceBlock.Position;
                wMSTask.ToLocation = sSJDeviceBlock_End.Position;
                wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
                wMSTask.TaskSource = "WCS";
                wMSTask.Floor = sSJDeviceBlock.Floor;
                WMSTasksManager.Instance.AddWMSTask(wMSTask);
                SStartAddrCBB.SelectedIndex = -1;
                SPaletNumTB.Text = null;
                SEndAddrCBB.SelectedIndex = -1;
                SAlarmLB.Content = "";
            }
            else
            {
                SAlarmLB.Content = "当前托盘有任务 托盘号：" + pallet_num;
            }
        }

        private void CreateSSJInToOutBT_Click(object sender, RoutedEventArgs e)//提升机换层
        {
            if (CStartAddrCBB.SelectedItem == null) return; ;
            SSJDeviceBlock sSJDeviceBlock = CStartAddrCBB.SelectedItem as SSJDeviceBlock;
            if (CPaletNumTB.Text.Trim().Length !=10&& CPaletNumTB.Text.Trim().Length != 12) return;
            string pallet_num = CPaletNumTB.Text.Trim();
            if (string.IsNullOrEmpty(pallet_num)) return;
            if (CEndAddrCBB.SelectedItem == null) return;
            SSJDeviceBlock sSJDeviceBlock_End = CEndAddrCBB.SelectedItem as SSJDeviceBlock;
            //if (!OperationCheck("创建输送机入转出任务 托盘号：" + pallet_num + "起始地：" + sSJDeviceBlock.Position + "目的地:" + sSJDeviceBlock_End.Position)) return;
            WMSTask wMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == pallet_num);
            if (wMSTask == null)
            {
                if (sSJDeviceBlock_End.CurrentMode == DeviceModeType.OutboundMode)
                {
                    wMSTask = new WMSTask();
                    wMSTask.PalletNum = pallet_num;
                    wMSTask.TaskType = WMSTaskType.Directe;
                    wMSTask.FmLocation = sSJDeviceBlock.Position;
                    wMSTask.ToLocation = sSJDeviceBlock_End.Position;
                    wMSTask.TaskStatus = WMSTaskStatus.TaskAssigned;
                    wMSTask.TaskSource = "WCS";
                    wMSTask.Floor = sSJDeviceBlock.Floor;
                    wMSTask.Warehouse = sSJDeviceBlock.WareHouse;
                    WMSTasksManager.Instance.AddWMSTask(wMSTask);
                    CStartAddrCBB.SelectedIndex = -1;
                    CPaletNumTB.Text = null;
                    CEndAddrCBB.SelectedIndex = -1;
                    CAlarmLB.Content = "";
                }
                else
                {
                    CAlarmLB.Content = "目的地不符合出库模式";
                }
                
            }
            else
            {
                CAlarmLB.Content = "当前托盘有任务 托盘号：" + pallet_num;
            }
        }
        private int GetSSJFloor(string toLocation)
        {
            //string ssjid = "";
            //if (string.IsNullOrEmpty(toLocation)) return null;
            //ssjid = string.Concat("SSJ0", toLocation.AsSpan(0, 1));
            //return ssjid;
            int floor;
            string sqlstr = string.Format("select Floor from dbo.ConveyerBlocks where Position='{0}'", toLocation);
            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr);
            if (obj == null) return 0;
            if (string.IsNullOrEmpty(obj.ToString())) return 0;
            floor = (int)obj;
            return floor;
        }

        private void OutWarehouse_DropDownClosed(object sender, System.EventArgs e)
        {
            if (OutWarehouse.SelectedItem == null) return;
            string warehouse = OutWarehouse.SelectedItem.ToString();
            //  SSJDeviceBlock sSJDeviceBlock = UStaAddrCBB.SelectedItem as SSJDeviceBlock;
            // if (sSJDeviceBlock == null) return;
            //  UPaletNumTB.Text = sSJDeviceBlock.PalletNum;
            // UEndAddrGLUC.UpdateRowNumCBBRows(sSJDeviceBlock.Tunnel);
            BStaAddrGLUC.FilterByWarehouse(warehouse);
            // BStaAddrGLUC
        }

        private void OutWarehouse_DropDownClosed2(object sender, System.EventArgs e)
        {
            if (OutWarehouse2.SelectedItem == null) return;
            string warehouse = OutWarehouse2.SelectedItem.ToString();
            MStaAddrGLUC.FilterByWarehouse(warehouse);
            MEndAddrGLUC.FilterByWarehouse(warehouse);
        }


        // 一号提升机
        private void StartAutoOutboundBT1_Click(object sender, RoutedEventArgs e)
        {
            if (!isAutoOutbound1Enabled)
            {
                if (OutboundFloorCBB1.SelectedItem is ComboBoxItem selectedItem)
                {
                    
                    selectedOutboundFloor1 = selectedItem.Tag.ToString();
                   SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == "SSJ03");
                    SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == GetToPosition(sSJDevice.PLCID, int.Parse(selectedOutboundFloor1)));
                    if (sSJDeviceBlock.CurrentMode != DeviceModeType.OutboundMode)
                    {
                        AlarmLabel1.Content = "目标层不是出库模式！";
                        return;
                    }
                    isAutoOutbound1Enabled = true;
                    StartAutoOutboundBT1.Content = "取消自动出库";
                    StatusLabel1.Content = $"当前状态：已开启（{selectedOutboundFloor1}）";
                    AlarmLabel1.Content = "一号提升机自动出库已开启。";
                }
                else
                {
                    AlarmLabel1.Content = "请选择出库层位。";
                }
            }
            else
            {
                isAutoOutbound1Enabled = false;
                StartAutoOutboundBT1.Content = "开启自动出库";
                StatusLabel1.Content = "当前状态：未开启";
                AlarmLabel1.Content = "";
            }
        }

        // 二号提升机
        private void StartAutoOutboundBT2_Click(object sender, RoutedEventArgs e)
        {
            if (!isAutoOutbound2Enabled)
            {
                if (OutboundFloorCBB2.SelectedItem is ComboBoxItem selectedItem)
                {
                    selectedOutboundFloor2 = selectedItem.Tag.ToString();
                    SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == "SSJ04");
                    SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == GetToPosition(sSJDevice.PLCID, int.Parse(selectedOutboundFloor2)));
                    if (sSJDeviceBlock.CurrentMode != DeviceModeType.OutboundMode)
                    {
                        AlarmLabel2.Content = "目标层不是出库模式！";
                        return;
                    }
                    isAutoOutbound2Enabled = true;
                    StartAutoOutboundBT2.Content = "取消自动出库";
                    StatusLabel2.Content = $"当前状态：已开启（{selectedOutboundFloor2}）";
                    AlarmLabel2.Content = "二号提升机自动出库已开启。";
                }
                else
                {
                    AlarmLabel2.Content = "请选择出库层位。";
                }
            }
            else
            {
                isAutoOutbound2Enabled = false;
                StartAutoOutboundBT2.Content = "开启自动出库";
                StatusLabel2.Content = "当前状态：未开启";
                AlarmLabel2.Content = "";
            }
        }

        // 三号提升机
        private void StartAutoOutboundBT3_Click(object sender, RoutedEventArgs e)
        {
            if (!isAutoOutbound3Enabled)
            {
                if (OutboundFloorCBB3.SelectedItem is ComboBoxItem selectedItem)
                {
                    selectedOutboundFloor3 = selectedItem.Tag.ToString();
                    SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == "SSJ05");
                    SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == GetToPosition(sSJDevice.PLCID, int.Parse(selectedOutboundFloor3)));
                    if (sSJDeviceBlock.CurrentMode != DeviceModeType.OutboundMode)
                    {
                        AlarmLabel3.Content = "目标层不是出库模式！";
                        return;
                    }
                    isAutoOutbound3Enabled = true;
                    StartAutoOutboundBT3.Content = "取消自动出库";
                    StatusLabel3.Content = $"当前状态：已开启（{selectedOutboundFloor3}）";
                    AlarmLabel3.Content = "三号提升机自动出库已开启。";
                }
                else
                {
                    AlarmLabel3.Content = "请选择出库层位。";
                }
            }
            else
            {
                isAutoOutbound3Enabled = false;
                StartAutoOutboundBT3.Content = "开启自动出库";
                StatusLabel3.Content = "当前状态：未开启";
                AlarmLabel3.Content = "";
            }
        }
        public string GetToPosition(string plcId, int floor)
        {
            // 定义查询语句，使用参数化查询
            string sqlstr = "SELECT Position FROM dbo.ConveyerBlocks WHERE PLCID = @plcId AND Floor = @floor";

            // 定义参数
            var parameters = new List<SqlParameter>
        {
            new SqlParameter("@plcId", plcId),
            new SqlParameter("@floor", floor)
        };

            // 执行查询
            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr, parameters);

            // 检查查询结果
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return "";

            return obj.ToString();
        }
    }
}
