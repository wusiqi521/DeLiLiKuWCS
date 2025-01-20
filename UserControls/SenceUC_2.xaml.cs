using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.ValueConverter;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// SenceUC.xaml 的交互逻辑
    /// </summary>
    public partial class SenceUC_2 : UserControl
    {
        double ScaleX = 1;
        double ScaleY = 1;
        double offsety = 0;
        double offsetx = 0;
        private double MaxX, MinX, MinY, MaxY;
        private double _currentScale = 1.0;
        private const double _minScale = 1.0;
        private const double _maxScale = 5.0;
        private Point _previousMousePosition;  // 记录鼠标上一次位置
        private bool _isDragging = false;  // 是否在拖动
        private TranslateTransform _translateTransform;  // 用于记录和应用平移
        private ScaleTransform _scaleTransform;  // 用于记录和应用缩放
        public SenceUC_2()
        {
            InitializeComponent();
            MyCanvas.Width = SystemParameters.WorkArea.Width;//得到屏幕工作区域宽度
            MyCanvas.Height = SystemParameters.WorkArea.Height - 60;//得到屏幕工作区域高度
                                                                    //Canvas.SetLeft(TitleLB, MyCanvas.Width / 2);
            _scaleTransform = new ScaleTransform();
            _translateTransform = new TranslateTransform();
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(_scaleTransform);
            transformGroup.Children.Add(_translateTransform);
            MyCanvas.RenderTransform = transformGroup;
            // 绑定鼠标事件
            outsideWrapper.MouseWheel += MainCanvas_MouseWheel;
            outsideWrapper.MouseLeftButtonDown += MainCanvas_MouseLeftButtonDown;
            outsideWrapper.MouseMove += MainCanvas_MouseMove;
            outsideWrapper.MouseLeftButtonUp += MainCanvas_MouseLeftButtonUp;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ModelsManager.Instance != null && ModelsManager.Instance.ModelsList != null && ModelsManager.Instance.ModelsList.Count > 0)
            {
                LoadGroupLayout(2);
            }
            else
            {

            }
        }
        public void LoadGroupLayout(int groupID)
        {
            // 清空当前Canvas上的所有元素
            if (ModelsManager.Instance == null || ModelsManager.Instance.ModelsList == null)
            {
                // 数据为空，直接return，不进行后续绘制
                return;
            }
            SetMaxMinXY(groupID);
            AddChildModel(groupID);
        }
        private void AddChildModel(int groupID)
        {
            var filteredModels = ModelsManager.Instance.ModelsList.FindAll(x => x.GroupID == groupID);
            if (filteredModels == null || filteredModels.Count < 1)
                return;

            ScaleX = 0.9 * MyCanvas.Width / (MaxX - MinX);
            ScaleY = 0.9 * MyCanvas.Height / (MaxY - MinY);

            if (ScaleX > ScaleY)
                ScaleX = ScaleY;
            else ScaleY = ScaleX;

            offsetx = (MyCanvas.Width - (MaxX - MinX) * ScaleX) / 2;
            offsety = (MyCanvas.Height - (MaxY - MinY) * ScaleX) / 2;

            foreach (BaseModel baseModel in filteredModels.FindAll(x => x.ModelType == ModelType.Shelves))
            {
                CreateHorizontalRacks(baseModel);
            }
            foreach (BaseModel baseModel in filteredModels.FindAll(x => x.ModelType == ModelType.FloorPlane))
            {
                CreatePlane(baseModel);
            }
            foreach (BaseModel baseModel in filteredModels.FindAll(x => x.ModelType == ModelType.PalletStackMachine))
            {
                CreateDDJUI(baseModel);
            }
            foreach (BaseModel baseModel in filteredModels.FindAll(x => x.ModelType == ModelType.PalletChainMachine))
            {
                CreateChainMUI(baseModel);
            }
            foreach (BaseModel baseModel in filteredModels.FindAll(x => x.ModelType == ModelType.PalletRolleMachine))
            {
                CreateRollerMUI(baseModel);
            }
            foreach (BaseModel baseModel in filteredModels.FindAll(x => x.ModelType == ModelType.TextDescription))
            {
                CreateTextMUI(baseModel);
            }
            foreach (BaseModel baseModel in filteredModels.FindAll(x => x.ModelType == ModelType.RGVerticalRailWay))
            {
                CreateRGVerticalRails(baseModel);
            }
            foreach (BaseModel baseModel in filteredModels.FindAll(x => x.ModelType == ModelType.RailGuidedVehicle))
            {
                CreateRGVUI(baseModel);
            }
            foreach (BaseModel baseModel in filteredModels.FindAll(x => x.ModelType == ModelType.Led))
            {
                CreateLed(baseModel);
            }
        }
        private void CreateLed(BaseModel baseModel)
        {
            if (baseModel == null) return;
            Led led = new Led(baseModel);
            led.Height = baseModel.GetHeight() * ScaleY;
            led.Width = baseModel.GetWidth() * ScaleX;
            // led.GroupID = baseModel.GroupID;
            led.BaseModel = baseModel;
            //baseModel.MinX* ScaleX +offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety
            MyCanvas.Children.Add(led);
            Canvas.SetLeft(led, (baseModel.MinX - MinX) * ScaleX + offsetx);

            Canvas.SetTop(led, ((MaxY - baseModel.MaxY) * ScaleY) + offsety);
            //  if (baseModel.GroupID != 1) chainMUI.Visibility = Visibility.Hidden;
        }
        private void SetMaxMinXY(int groupID)
        {
            var filteredModels = ModelsManager.Instance.ModelsList.FindAll(x => x.GroupID == groupID);
            if (filteredModels == null || filteredModels.Count == 0)
            {
                MaxX = MinX = MaxY = MinY = 0;
                return;
            }

            MaxX = filteredModels[0].MaxX;
            MinX = filteredModels[0].MinX;
            MaxY = filteredModels[0].MaxY;
            MinY = filteredModels[0].MinY;

            for (int i = 1; i < filteredModels.Count; i++)
            {
                BaseModel baseModel = filteredModels[i];
                if (MaxX < baseModel.MaxX) MaxX = baseModel.MaxX;
                if (MinX > baseModel.MinX) MinX = baseModel.MinX;
                if (MaxY < baseModel.MaxY) MaxY = baseModel.MaxY;
                if (MinY > baseModel.MinY) MinY = baseModel.MinY;
            }
        }
        private void AddChildModel()
        {
            if (ModelsManager.Instance.ModelsList == null || ModelsManager.Instance.ModelsList.Count < 1)
                return;
            SetMaxMinXY();
            ScaleX = 0.9 * MyCanvas.Width / (MaxX - MinX);    //ScaleX = 0.00630204081631881 CAD中单位X距离在画布中占据的长度
            ScaleY = 0.9 * MyCanvas.Height / (MaxY - MinY);   //ScaleY = 0.00630204081631881 CAD中单位Y距离在画布中占据的长度

            if (ScaleX > ScaleY)
                ScaleX = ScaleY;
            else ScaleY = ScaleX;
            //整体距离画布左边、右边的距离   MyCanvas.Width=1536
            offsetx = (MyCanvas.Width - (MaxX - MinX) * ScaleX) / 2 - 300;  //offsetx = 222.24326530678354  将CAD所有设备看成整体，放在画布中央
            //整体距离画布上边、下边的距离    MyCanvas.Height=772                              
            offsety = (MyCanvas.Height - (MaxY - MinY) * ScaleX) / 2; //offsety = 38.599999999999966  
            //ScaleY = -ScaleY;          
            foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelType == ModelType.Shelves))
            {
                CreateHorizontalRacks(baseModel);
            }
            foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelType == ModelType.FloorPlane))
            {
                CreatePlane(baseModel);
            }
            //foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelName == "车挡"))
            //    {
            //    CreateDDJStop(baseModel);
            //}
            foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelType == ModelType.PalletStackMachine))
            {
                CreateDDJUI(baseModel);
            }
            foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelType == ModelType.PalletChainMachine))
            {
                CreateChainMUI(baseModel);
            }
            foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelType == ModelType.PalletRolleMachine))
            {
                CreateRollerMUI(baseModel);
            }
            foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelType == ModelType.TextDescription))
            {
                CreateTextMUI(baseModel);
            }
            //foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelType == ModelType.PalletRolleNoPower))
            //{
            //    CreateRollerMUI(baseModel);
            //}
            //foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelName == "拆盘机"))
            //{
            //    CreateRollerMUI(baseModel);
            //}
            //foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelName == "翻转板块"))
            //{
            //    CreateRollerMUI(baseModel);
            //}
            //foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelName == "翻转板块1"))
            //{
            //    CreateRollerMUI(baseModel);
            //}
            //foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelName == "222222"))
            //{
            //    CreateRollerMUI(baseModel);
            //}
            //foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelName == "非标设备"))
            //{
            //    CreateRollerMUI(baseModel);
            //}
            //foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelName == "A$C41635D92"))
            //{
            //    CreateRollerMUI(baseModel);
            //}
            foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelType == ModelType.RGVerticalRailWay))
            {
                CreateRGVerticalRails(baseModel);
            }
            foreach (BaseModel baseModel in ModelsManager.Instance.ModelsList.FindAll(x => x.ModelType == ModelType.RailGuidedVehicle))
            {
                CreateRGVUI(baseModel);
            }
        }
        private void CreateRGVerticalRails(BaseModel baseModel)
        {
            if (baseModel == null) return;
            RGVRailWayUI rGVRailWayUI = new RGVRailWayUI(baseModel);
            rGVRailWayUI.Height = baseModel.GetHeight() * ScaleY;
            rGVRailWayUI.Width = baseModel.GetWidth() * ScaleX;
            rGVRailWayUI.GroupID = baseModel.GroupID;
            rGVRailWayUI.BaseModel = baseModel;
            //baseModel.MinX* ScaleX +offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety
            MyCanvas.Children.Add(rGVRailWayUI);
            Canvas.SetLeft(rGVRailWayUI, baseModel.MinX * ScaleX + offsetx);
            Canvas.SetTop(rGVRailWayUI, ((MaxY - baseModel.MaxY) * ScaleY) + offsety);
            //if (baseModel.GroupID != 1) rGVRailWayUI.Visibility = Visibility.Hidden;
        }
        //private void CreateRGVRails(BaseModel baseModel)
        //{
        //    Path myPath = new()
        //    {
        //        Stroke = Brushes.ForestGreen,
        //        StrokeThickness = 0.2
        //    };

        //    // Create a StreamGeometry to use to specify myPath.
        //    StreamGeometry geometry = new StreamGeometry();
        //    //geometry.FillRule = FillRule.EvenOdd;

        //    // Open a StreamGeometryContext that can be used to describe this StreamGeometry
        //    // object's contents.
        //    using (StreamGeometryContext ctx = geometry.Open())
        //    {
        //        double rheight = (baseModel.MaxY - baseModel.MinY) * ScaleY;
        //        double rwildth = (baseModel.MaxX - baseModel.MinX) * ScaleX;
        //        MaxP = baseModel.MinX * ScaleX + offsetx + rwildth;
        //        MinP = baseModel.MinX * ScaleX + offsetx;
        //        // Begin the triangle at the point specified. Notice that the shape is set to
        //        // be closed so only two lines need to be specified below to make the triangle.
        //        ctx.BeginFigure(new Point(baseModel.MinX * ScaleX + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is filled */, false /* is closed */);
        //        // Draw a line to the next specified point.
        //        ctx.LineTo(new Point(baseModel.MinX * ScaleX + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);
        //        ctx.BeginFigure(new Point(baseModel.MinX * ScaleX + offsetx + rwildth, (MaxY - baseModel.MinY) * ScaleY + offsety), true, false);
        //        // Draw another line to the next specified point.
        //        ctx.LineTo(new Point(baseModel.MinX * ScaleX + offsetx + rwildth, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is stroked */, false /* is smooth join */);

        //        ctx.Close();
        //    }

        //    // Freeze the geometry (make it unmodifiable)
        //    // for additional performance benefits.
        //    geometry.Freeze();

        //    // Specify the shape (triangle) of the Path using the StreamGeometry.
        //    myPath.Data = geometry;
        //    MyCanvas.Children.Add(myPath);
        //}
        private void CreateRGVRail(BaseModel baseModel)
        {
            if (baseModel == null) return;
            switch (baseModel.ModelID)
            {
                case "RGV11":
                    RailMinY11 = (MaxY - baseModel.MaxY) * ScaleY + offsety;
                    break;
                case "RGV21":
                    RailMinY21 = (MaxY - baseModel.MaxY) * ScaleY + offsety;
                    break;
                case "RGV22":
                    RailMinY22 = (MaxY - baseModel.MaxY) * ScaleY + offsety;
                    break;
                case "RGV32":
                    RailMinY32 = (MaxY - baseModel.MaxY) * ScaleY + offsety;
                    break;
                case "RGV31":
                    RailMinY31 = (MaxY - baseModel.MaxY) * ScaleY + offsety;
                    break;
                default:
                    break;
            }
        }
        double RailMinY11, RailMinY21, RailMinY22, RailMinY31, RailMinY32;
        private void CreateRGVUI(BaseModel baseModel)
        {
            if (baseModel == null) return;
            RGVUI rGVUI = new RGVUI(baseModel);
            double left = baseModel.MinX * ScaleX + offsetx;
            double top = ((MaxY - baseModel.MaxY) * ScaleY) + offsety;
            rGVUI.Height = baseModel.GetHeight() * ScaleY;
            rGVUI.Width = baseModel.GetWidth() * ScaleX;
            rGVUI.GroupID = baseModel.GroupID;
            rGVUI.BaseModel = baseModel;
            MyCanvas.Children.Add(rGVUI);
            Canvas.SetLeft(rGVUI, left);
            Canvas.SetTop(rGVUI, top);
            //double sSJDevice1026_mid = (ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "P1026").MaxY + ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "P1026").MinY) / 2;
            //double sSJDevice1055_mid = (ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "P1055").MaxY + ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "P1055").MinY) / 2;
            //double sSJDevice1001_mid = (ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "P1001").MaxY + ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "P1001").MinY) / 2;
            //double sSJDevice1016_mid = (ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "P1016").MaxY + ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "P1016").MinY) / 2;
            //double rGVSouthDeviceHalfHeight = (ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "car11").MaxY - ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "car11").MinY) / 2;
            //double rGVNorthDeviceHalfHeight = (ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "car31").MaxY - ModelsManager.Instance.ModelsList.Find(x => x.ModelID == "car31").MinY) / 2;
            //double rGV12DeviceTop = (MaxY - sSJDevice1001_mid - rGVSouthDeviceHalfHeight) * ScaleY + offsety;
            //double rGV12DeviceBottom = (MaxY - sSJDevice1016_mid - rGVSouthDeviceHalfHeight) * ScaleY + offsety;
            //double rGV34DeviceTop = (MaxY - sSJDevice1026_mid - rGVNorthDeviceHalfHeight) * ScaleY + offsety;
            //double rGV34DeviceBottom = (MaxY - sSJDevice1055_mid - rGVNorthDeviceHalfHeight) * ScaleY + offsety;


            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == baseModel.PLCID);
            if (sSJDevice == null) return;
            RGVDevice rGVDevice = sSJDevice.RGVDeviceList.Find(x => x.DeviceID == baseModel.ModelID);
            if (rGVDevice == null) return;
            // New binding object using the path of 'Name' for whatever source object is used
            var binding = new Binding("RGVPostionY");

            // Configure the binding
            binding.Mode = BindingMode.OneWay;
            binding.Source = rGVDevice;
            if (baseModel.ModelID == "car2")
                binding.Converter = new XRGVMontionPositionConverter(899, 1633);
            else if (baseModel.ModelID == "car3")
                binding.Converter = new XRGVMontionPositionConverter(899, 1633);
            else if (baseModel.ModelID == "car4")
                binding.Converter = new XRGVMontionPositionConverter(899, 1633);
            else
                binding.Converter = new XRGVMontionPositionConverter(899, 1633);
            binding.ConverterCulture = new CultureInfo("en-US");
            // Set the binding to a target object. The TextBlock.Name property on the NameBlock UI element
            BindingOperations.SetBinding(rGVUI, Canvas.LeftProperty, binding);
            //if (baseModel.GroupID != 1) rGVUI.Visibility = Visibility.Hidden;
        }
        //1320 1310 2220 3210 3220 2210
        private void CreateRollerMUI(BaseModel baseModel)
        {
            if (baseModel == null) return;
            RollerMUI roller = new RollerMUI(baseModel);
            roller.Height = baseModel.GetHeight() * ScaleY;
            roller.Width = baseModel.GetWidth() * ScaleX;
            roller.GroupID = baseModel.GroupID;
            roller.BaseModel = baseModel;
            //baseModel.MinX* ScaleX +offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety
            MyCanvas.Children.Add(roller);
            Canvas.SetLeft(roller, baseModel.MinX * ScaleX + offsetx);
            Canvas.SetTop(roller, ((MaxY - baseModel.MaxY) * ScaleY) + offsety);
            //if (baseModel.GroupID != 1) roller.Visibility = Visibility.Hidden;
            Canvas.SetZIndex(roller, 100);
        }
        private void CreateChainMUI(BaseModel baseModel)
        {
            if (baseModel == null) return;
            ChainMUI chainMUI = new ChainMUI(baseModel);
            chainMUI.Height = baseModel.GetHeight() * ScaleY;
            chainMUI.Width = baseModel.GetWidth() * ScaleX;
            chainMUI.GroupID = baseModel.GroupID;
            chainMUI.BaseModel = baseModel;
            //baseModel.MinX* ScaleX +offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety
            MyCanvas.Children.Add(chainMUI);
            Canvas.SetLeft(chainMUI, (baseModel.MinX - MinX) * ScaleX + offsetx);
            
           
            Canvas.SetTop(chainMUI, ((MaxY - baseModel.MaxY) * ScaleY) + offsety);
            //  if (baseModel.GroupID != 1) chainMUI.Visibility = Visibility.Hidden;
        }
        private void CreateTextMUI(BaseModel baseModel)
        {
            if (baseModel == null) return;
            TextDescription textDes = new TextDescription(baseModel);
            textDes.Height = baseModel.GetHeight() * ScaleY;
            textDes.Width = baseModel.GetWidth() * ScaleX;
            textDes.DeviceIDLabel.Content = baseModel.ModelID;
            textDes.GroupID = baseModel.GroupID;
            textDes.BaseModel = baseModel;
            //baseModel.MinX* ScaleX +offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety
            MyCanvas.Children.Add(textDes);
            Canvas.SetLeft(textDes, baseModel.MinX * ScaleX + offsetx);
            
            Canvas.SetTop(textDes, ((MaxY - baseModel.MaxY) * ScaleY) + offsety);
            //if (baseModel.GroupID != 1) textDes.Visibility = Visibility.Hidden;
            Canvas.SetZIndex(textDes, 100);
        }
        double MaxP, MinP;
        private void CreateDDJUI(BaseModel baseModel)
        {
            if (baseModel == null) return;
            if (baseModel.PLCID == "DDJ03")
            {
                StackerUI stackerUI = new StackerUI(baseModel);
                stackerUI.Height = baseModel.GetHeight() * ScaleY;
                stackerUI.Width = baseModel.GetWidth() * ScaleX;
                MyCanvas.Children.Add(stackerUI);
                double left = (baseModel.MinX - MinX) * ScaleX + offsetx;
                double top = ((MaxY - baseModel.MaxY) * ScaleY) + offsety;
                Canvas.SetLeft(stackerUI, left);
                Canvas.SetTop(stackerUI, top);
                DDJDevice dDJDevice = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == baseModel.PLCID);
                if (dDJDevice == null) return;
                // New binding object using the path of 'Name' for whatever source object is used
                var binding = new Binding("MotionPosition");

                // Configure the binding
                binding.Mode = BindingMode.OneWay;
                binding.Source = dDJDevice;
                //if (dDJDevice.PLCID ==  "DDJ03")
                //    binding.Converter = new YMontionPositionConverter(139, 832, "1");
                //else
                //    binding.Converter = new YMontionPositionConverter(88, 832, "2");
                //binding.ConverterCulture = new CultureInfo("en-US");
                //BindingOperations.SetBinding(stackerUI, Canvas.TopProperty, binding);
            }
            else
            {
                StackUIHorizontal stackerUI = new StackUIHorizontal(baseModel);

                stackerUI.Height = baseModel.GetHeight() * ScaleY;
                stackerUI.Width = baseModel.GetWidth() * ScaleX;
                MyCanvas.Children.Add(stackerUI);
                double left = (baseModel.MinX - MinX) * ScaleX + offsetx;
                double top = ((MaxY - baseModel.MaxY) * ScaleY) + offsety;
                Canvas.SetLeft(stackerUI, left);
                Canvas.SetTop(stackerUI, top);
                DDJDevice dDJDevice = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == baseModel.PLCID);
                if (dDJDevice == null) return;
                // New binding object using the path of 'Name' for whatever source object is used
                var binding = new Binding("MotionPosition");

              

                // Configure the binding
                binding.Mode = BindingMode.OneWay;
                binding.Source = dDJDevice;
                if (dDJDevice.PLCID == "DDJ01")
                    binding.Converter = new XDmMontionPositionConverter(MyCanvas.Width * 0.09619140625, MyCanvas.Width * 0.81494140625, "1");
                else
                    binding.Converter = new XDmMontionPositionConverter(MyCanvas.Width * 0.09130859375, MyCanvas.Width * 0.81982421875, "2");
                binding.ConverterCulture = new CultureInfo("en-US");
                BindingOperations.SetBinding(stackerUI, Canvas.LeftProperty, binding);
            }



        }
        private void SetMaxMinXY()
        {
            for (int i = 0; i < ModelsManager.Instance.ModelsList.Count; i++)
            {
                BaseModel baseModel = ModelsManager.Instance.ModelsList[i];
                if (i == 0)
                {
                    MaxX = baseModel.MaxX;
                    MaxY = baseModel.MaxY;
                    MinX = baseModel.MinX;
                    MinY = baseModel.MinY;
                    continue;
                }
                if (MaxX < baseModel.MaxX) MaxX = baseModel.MaxX;
                if (MinX > baseModel.MinX) MinX = baseModel.MinX;
                if (MaxY < baseModel.MaxY) MaxY = baseModel.MaxY;
                if (MinY > baseModel.MinY) MinY = baseModel.MinY;
            }
        }
        private void CreateRacks(BaseModel baseModel)
        {
            Path myPath = new()
            {
                Stroke = Brushes.ForestGreen,
                StrokeThickness = 0.2
            };

            int rank_num = 13;

            // Create a StreamGeometry to use to specify myPath.
            StreamGeometry geometry = new StreamGeometry();
            //geometry.FillRule = FillRule.EvenOdd;

            // Open a StreamGeometryContext that can be used to describe this StreamGeometry
            // object's contents.
            using (StreamGeometryContext ctx = geometry.Open())
            {
                double rheight = (baseModel.MaxY - baseModel.MinY) * ScaleY;
                double rwildth = (baseModel.MaxX - baseModel.MinX) * ScaleX;
                double sub_step = rheight / rank_num;
                MaxP = baseModel.MinX * ScaleX + offsetx + rwildth;
                MinP = baseModel.MinX * ScaleX + offsetx;
                // Begin the triangle at the point specified. Notice that the shape is set to
                // be closed so only two lines need to be specified below to make the triangle.
                ctx.BeginFigure(new Point(baseModel.MinX * ScaleX + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is filled */, true /* is closed */);

                // Draw a line to the next specified point.
                ctx.LineTo(new Point(baseModel.MinX * ScaleX + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);

                // Draw another line to the next specified point.
                ctx.LineTo(new Point(baseModel.MinX * ScaleX + offsetx + rwildth, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);
                ctx.LineTo(new Point(baseModel.MinX * ScaleX + offsetx + rwildth, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is stroked */, false /* is smooth join */);

                for (int i = 1; i < rank_num; i++)
                {
                    ctx.BeginFigure(new Point(baseModel.MinX * ScaleX + offsetx, (MaxY - baseModel.MaxY) * ScaleY + i * sub_step + offsety), false /* is filled */, false /* is closed */);
                    ctx.LineTo(new Point(baseModel.MinX * ScaleX + offsetx + rwildth, (MaxY - baseModel.MaxY) * ScaleY + i * sub_step + offsety), true /* is stroked */, false /* is smooth join */);
                }
                ctx.Close();
            }

            // Freeze the geometry (make it unmodifiable)
            // for additional performance benefits.
            geometry.Freeze();

            // Specify the shape (triangle) of the Path using the StreamGeometry.
            myPath.Data = geometry;
            MyCanvas.Children.Add(myPath);
        }

        private void CreateVerticalRacks(BaseModel baseModel)
        {
            Path myPath = new()
            {
                Stroke = Brushes.ForestGreen,
                StrokeThickness = 0.2
            };

            int rank_num_c = 34;
            int rank_num_b = 54;

            // Create a StreamGeometry to use to specify myPath.
            StreamGeometry geometry = new StreamGeometry();
            //geometry.FillRule = FillRule.EvenOdd;

            // Open a StreamGeometryContext that can be used to describe this StreamGeometry
            // object's contents.
            using (StreamGeometryContext ctx = geometry.Open())
            {
                double rheight = (baseModel.MaxY - baseModel.MinY) * ScaleY;
                double rwildth = (baseModel.MaxX - baseModel.MinX) * ScaleX;
                double sub_step_c = rwildth / rank_num_c;
                double sub_step_b = rwildth / rank_num_b;
                MaxP = baseModel.MinY * ScaleY + offsety + rheight;
                MinP = baseModel.MinY * ScaleY + offsety;
                // Begin the triangle at the point specified. Notice that the shape is set to
                // be closed so only two lines need to be specified below to make the triangle.
                ctx.BeginFigure(new Point(baseModel.MinX * ScaleX + offsetx, (MaxY - baseModel.MaxY) * ScaleY + rheight + offsety), true /* is filled */, true /* is closed */);

                // Draw a line to the next specified point.
                ctx.LineTo(new Point(baseModel.MinX * ScaleX + rwildth + offsetx, (MaxY - baseModel.MaxY) * ScaleY + rheight + offsety), true /* is stroked */, false /* is smooth join */);

                // Draw another line to the next specified point.
                ctx.LineTo(new Point(baseModel.MinX * ScaleX + rwildth + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is stroked */, false /* is smooth join */);
                ctx.LineTo(new Point(baseModel.MinX * ScaleX + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is stroked */, false /* is smooth join */);
                if (Convert.ToInt32(baseModel.ModelID.Substring(5, 2)) <= 12)
                {
                    for (int i = 1; i < rank_num_c; i++)
                    {
                        ctx.BeginFigure(new Point(baseModel.MinX * ScaleX + i * sub_step_c + offsetx, (MaxY - baseModel.MaxY) * ScaleY + rheight + offsety), false /* is filled */, false /* is closed */);
                        ctx.LineTo(new Point(baseModel.MinX * ScaleX + i * sub_step_c + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is stroked */, false /* is smooth join */);
                    }
                }
                else
                {
                    for (int i = 1; i < rank_num_b; i++)
                    {
                        ctx.BeginFigure(new Point(baseModel.MinX * ScaleX + i * sub_step_b + offsetx, (MaxY - baseModel.MaxY) * ScaleY + rheight + offsety), false /* is filled */, false /* is closed */);
                        ctx.LineTo(new Point(baseModel.MinX * ScaleX + i * sub_step_b + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is stroked */, false /* is smooth join */);
                    }
                }
                ctx.Close();
            }

            // Freeze the geometry (make it unmodifiable)
            // for additional performance benefits.
            geometry.Freeze();

            // Specify the shape (triangle) of the Path using the StreamGeometry.
            myPath.Data = geometry;
            MyCanvas.Children.Add(myPath);
        }
        private void CreateHorizontalRacks(BaseModel baseModel)
        {
            Path myPath = new()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 0.6
            };
            // Create a StreamGeometry to use to specify myPath.
            StreamGeometry geometry = new StreamGeometry();
            //geometry.FillRule = FillRule.EvenOdd;

            // Open a StreamGeometryContext that can be used to describe this StreamGeometry
            // object's contents.
            // int.TryParse(baseModel.ModelID.Substring(5,2), out int parsedGroupID)
            //int FirstTwoNum;
            int LarstTwoNum = 0;
            int rank_num = 1;
            if (int.TryParse(baseModel.ModelID.Substring(7, 2), out int TwoNum))
            {
                LarstTwoNum = TwoNum;
            }
            if (LarstTwoNum <= 2)
            {
                rank_num = 41;
            }
            else if (LarstTwoNum <= 4)
            {
                rank_num = 56;
            }
            else
            {
                rank_num = 50;

            }
            if (LarstTwoNum <= 4)
            {
                using (StreamGeometryContext ctx = geometry.Open())
                {
                    double rheight = (baseModel.MaxY - baseModel.MinY) * ScaleY;
                    double rwildth = (baseModel.MaxX - baseModel.MinX) * ScaleX;
                    double sub_step = rwildth / rank_num;
                    MaxP = (baseModel.MinX - MinX) * ScaleX + offsetx + rwildth;
                    MinP = (baseModel.MinX - MinX) * ScaleX + offsetx;
                    // Begin the triangle at the point specified. Notice that the shape is set to
                    // be closed so only two lines need to be specified below to make the triangle.
                    ctx.BeginFigure(new Point(MinP, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is filled */, true /* is closed */);

                    // Draw a line to the next specified point.
                    ctx.LineTo(new Point(MinP, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);

                    // Draw another line to the next specified point.
                    ctx.LineTo(new Point(MaxP, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);
                    ctx.LineTo(new Point(MaxP, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is stroked */, false /* is smooth join */);

                    for (int i = 1; i < rank_num; i++)
                    {
                        ctx.BeginFigure(new Point(MinP + i * sub_step, (MaxY - baseModel.MaxY) * ScaleY + offsety), false /* is filled */, false /* is closed */);
                        ctx.LineTo(new Point(MinP + i * sub_step, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);
                    }
                    ctx.Close();
                }
            }
            else
            {
                using (StreamGeometryContext ctx = geometry.Open())
                {
                    double rheight = (baseModel.MaxY - baseModel.MinY) * ScaleY;
                    double rwildth = (baseModel.MaxX - baseModel.MinX) * ScaleX;
                    double sub_step = rheight / rank_num;
                    MaxP = (baseModel.MinX - MinX) * ScaleX + offsetx + rwildth;
                    MinP = (baseModel.MinX - MinX) * ScaleX + offsetx;
                    // Begin the triangle at the point specified. Notice that the shape is set to
                    // be closed so only two lines need to be specified below to make the triangle.
                    ctx.BeginFigure(new Point(MinP, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is filled */, true /* is closed */);

                    // Draw a line to the next specified point.
                    ctx.LineTo(new Point(MinP, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);

                    // Draw another line to the next specified point.
                    ctx.LineTo(new Point(MaxP, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);
                    ctx.LineTo(new Point(MaxP, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is stroked */, false /* is smooth join */);

                    for (int i = 1; i < rank_num; i++)
                    {
                        ctx.BeginFigure(new Point(MinP, (MaxY - baseModel.MaxY) * ScaleY + i * sub_step + offsety), false /* is filled */, false /* is closed */);
                        ctx.LineTo(new Point(MinP + rwildth, (MaxY - baseModel.MaxY) * ScaleY + i * sub_step + offsety), true /* is stroked */, false /* is smooth join */);
                    }
                    ctx.Close();
                }
            }


            // Freeze the geometry (make it unmodifiable)
            // for additional performance benefits.
            geometry.Freeze();
            // Specify the shape (triangle) of the Path using the StreamGeometry.
            myPath.Data = geometry;
            MyCanvas.Children.Add(myPath);
        }



        private void CreatePlane(BaseModel baseModel)
        {
            Path myPath = new()
            {
                Stroke = Brushes.DarkBlue,
                StrokeThickness = 0.2
            };

            // Create a StreamGeometry to use to specify myPath.
            StreamGeometry geometry = new StreamGeometry();
            //geometry.FillRule = FillRule.EvenOdd;

            using (StreamGeometryContext ctx = geometry.Open())
            {
                double rheight = (baseModel.MaxY - baseModel.MinY) * ScaleY;
                double rwildth = (baseModel.MaxX - baseModel.MinX) * ScaleX;
                MaxP = baseModel.MinX * ScaleX + offsetx + rwildth;
                MinP = baseModel.MinX * ScaleX + offsetx;
                // Begin the triangle at the point specified. Notice that the shape is set to
                // be closed so only two lines need to be specified below to make the triangle.
                ctx.BeginFigure(new Point((baseModel.MinX - MinX) * ScaleX + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is filled */, true /* is closed */);

                // Draw a line to the next specified point.
                ctx.LineTo(new Point((baseModel.MinX - MinX) * ScaleX + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);

                // Draw another line to the next specified point.
                ctx.LineTo(new Point((baseModel.MinX - MinX) * ScaleX + offsetx + rwildth, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);
                ctx.LineTo(new Point((baseModel.MinX - MinX) * ScaleX + offsetx + rwildth, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is stroked */, false /* is smooth join */);

                ctx.Close();
            }

            // Freeze the geometry (make it unmodifiable)
            // for additional performance benefits.
            geometry.Freeze();

            // Specify the shape (triangle) of the Path using the StreamGeometry.
            myPath.Data = geometry;
            MyCanvas.Children.Add(myPath);
        }
        private void CreateDDJStop(BaseModel baseModel)
        {
            Path myPath1 = new Path();
            myPath1.Stroke = Brushes.DarkGreen;
            myPath1.StrokeThickness = 0.2;

            // Create a StreamGeometry to use to specify myPath.
            StreamGeometry geometry = new StreamGeometry();
            //geometry.FillRule = FillRule.EvenOdd;

            // Open a StreamGeometryContext that can be used to describe this StreamGeometry
            // object's contents.
            using (StreamGeometryContext ctx = geometry.Open())
            {
                double rheight = (baseModel.MaxY - baseModel.MinY) * ScaleY;
                double rwildth = (baseModel.MaxX - baseModel.MinX) * ScaleX;
                ctx.BeginFigure(new Point(baseModel.MinX * ScaleX + offsetx + rwildth / 4, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is filled */, true /* is closed */);

                // Draw a line to the next specified point.
                ctx.LineTo(new Point(baseModel.MinX * ScaleX + offsetx + rwildth / 4, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);

                // Draw another line to the next specified point.
                ctx.LineTo(new Point(baseModel.MinX * ScaleX + offsetx + rwildth - rwildth / 4, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight), true /* is stroked */, false /* is smooth join */);
                ctx.LineTo(new Point(baseModel.MinX * ScaleX + offsetx + rwildth - rwildth / 4, (MaxY - baseModel.MaxY) * ScaleY + offsety), true /* is stroked */, false /* is smooth join */);

                ctx.Close();
            }

            // Freeze the geometry (make it unmodifiable)
            // for additional performance benefits.
            geometry.Freeze();

            // Specify the shape (triangle) of the Path using the StreamGeometry.
            myPath1.Data = geometry;
            MyCanvas.Children.Add(myPath1);

            Path myPath2 = new Path();
            myPath2.Stroke = Brushes.Red;
            myPath2.StrokeThickness = 0.2;

            StreamGeometry geometry2 = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry2.Open())
            {
                double rheight = (baseModel.MaxY - baseModel.MinY) * ScaleY;
                double rwildth = (baseModel.MaxX - baseModel.MinX) * ScaleX;
                ctx.BeginFigure(new Point(baseModel.MinX * ScaleX + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight / 2), true /* is filled */, false /* is closed */);

                // Draw a line to the next specified point.
                ctx.LineTo(new Point(baseModel.MinX * ScaleX + offsetx + rwildth, (MaxY - baseModel.MaxY) * ScaleY + offsety + rheight / 2), true /* is stroked */, false /* is smooth join */);

                ctx.Close();
            }

            // Freeze the geometry (make it unmodifiable)
            // for additional performance benefits.
            geometry2.Freeze();

            // Specify the shape (triangle) of the Path using the StreamGeometry.
            myPath2.Data = geometry2;
            MyCanvas.Children.Add(myPath2);
        }
        private void CreateMocelUI(BaseModel baseModel)
        {
            if (baseModel == null) return;
            RectangleGeometry rectangleGeometry = new RectangleGeometry();
            double rheight = (baseModel.MaxY - baseModel.MinY) * ScaleY;
            double rwildth = (baseModel.MaxX - baseModel.MinX) * ScaleX;
            rectangleGeometry.Rect = new Rect(baseModel.MinX * ScaleX + offsetx, (MaxY - baseModel.MaxY) * ScaleY + offsety, rwildth, rheight);

            Path myPath = new Path();
            myPath.Fill = Brushes.LemonChiffon;
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;
            myPath.Data = rectangleGeometry;
            MyCanvas.Children.Add(myPath);
            if (baseModel.GroupID == 0 || baseModel.GroupID == 1)
                myPath.Visibility = Visibility.Visible;
            else myPath.Visibility = Visibility.Hidden;
        }

        #region 图形操作
        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 获取鼠标在画布中的位置
            Point mousePosition = e.GetPosition(MyCanvas);

            // 计算新的缩放比例
            double scaleFactor = e.Delta > 0 ? 1.1 : 0.9;
            double newScale = _currentScale * scaleFactor;

            // 限制缩放比例范围
            if (newScale < _minScale)
                newScale = _minScale;
            else if (newScale > _maxScale)
                newScale = _maxScale;

            // 计算缩放前的偏移（鼠标位置对应的实际坐标）
            double absoluteX = (_previousMousePosition.X - _translateTransform.X) / _currentScale;
            double absoluteY = (_previousMousePosition.Y - _translateTransform.Y) / _currentScale;

            // 更新缩放比例
            _currentScale = newScale;
            _scaleTransform.ScaleX = _currentScale;
            _scaleTransform.ScaleY = _currentScale;

            // 计算新的平移，使得缩放以鼠标为中心
            _translateTransform.X = _previousMousePosition.X - absoluteX * _currentScale;
            _translateTransform.Y = _previousMousePosition.Y - absoluteY * _currentScale;

            // 限制平移范围
            ConstrainTranslation();
        }
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _previousMousePosition = e.GetPosition(outsideWrapper);
            MyCanvas.CaptureMouse();
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // 如果鼠标左键没有按下，确保 _isDragging 为 false
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                if (_isDragging)
                {
                    _isDragging = false;
                    MyCanvas.ReleaseMouseCapture();
                }
                return;
            }

            if (_isDragging)
            {
                Point currentPosition = e.GetPosition(outsideWrapper);
                Vector delta = currentPosition - _previousMousePosition;

                _translateTransform.X += delta.X;
                _translateTransform.Y += delta.Y;

                _previousMousePosition = currentPosition;

                // 限制平移范围
                ConstrainTranslation();
            }
        }


        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            MyCanvas.ReleaseMouseCapture();
        }
        private void ConstrainTranslation()
        {
            // 获取缩放后的内容大小
            double scaledWidth = MyCanvas.ActualWidth * _currentScale;
            double scaledHeight = MyCanvas.ActualHeight * _currentScale;

            // 获取可视区域大小
            double viewportWidth = outsideWrapper.ActualWidth;
            double viewportHeight = outsideWrapper.ActualHeight;

            // 计算平移的最大和最小值
            double minX = Math.Min(0, viewportWidth - scaledWidth);
            double minY = Math.Min(0, viewportHeight - scaledHeight);
            double maxX = 0;
            double maxY = 0;

            // 限制平移值
            if (_translateTransform.X < minX)
                _translateTransform.X = minX;
            if (_translateTransform.X > maxX)
                _translateTransform.X = maxX;

            if (_translateTransform.Y < minY)
                _translateTransform.Y = minY;
            if (_translateTransform.Y > maxY)
                _translateTransform.Y = maxY;
        }





        #endregion




    }
}
