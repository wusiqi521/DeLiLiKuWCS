﻿#pragma checksum "..\..\..\..\UserWindows\DDJDetailWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "07D288D4154692B3C0CA2565E88CE590CE315C78"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using BMHRI.WCS.Server.UserControls;
using BMHRI.WCS.Server.ValueConverter;
using S7.Net;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace BMHRI.WCS.Server.UserWindows {
    
    
    /// <summary>
    /// DDJDetailWindow
    /// </summary>
    public partial class DDJDetailWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 96 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button FloorRecallButton1;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button FloorRecallButton2;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button FloorRecallButton3;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button FloorRecallButton4;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button FloorRecallButton5;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button FloorRecallButton6;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RecallButton;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button send0j;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.6.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/BMHRI.WCS.Server;component/userwindows/ddjdetailwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.6.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 11 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            ((BMHRI.WCS.Server.UserWindows.DDJDetailWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 89 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ConnectBT_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 90 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ConnectBT_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 91 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnlineBT_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 92 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OfflineBT_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 93 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.RecallInPlaceBT_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 94 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ClearDB400BT_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 95 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ClearDB500BT_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.FloorRecallButton1 = ((System.Windows.Controls.Button)(target));
            
            #line 96 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            this.FloorRecallButton1.Click += new System.Windows.RoutedEventHandler(this.RecallBT_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.FloorRecallButton2 = ((System.Windows.Controls.Button)(target));
            
            #line 97 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            this.FloorRecallButton2.Click += new System.Windows.RoutedEventHandler(this.RecallBT_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.FloorRecallButton3 = ((System.Windows.Controls.Button)(target));
            
            #line 98 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            this.FloorRecallButton3.Click += new System.Windows.RoutedEventHandler(this.RecallBT_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.FloorRecallButton4 = ((System.Windows.Controls.Button)(target));
            
            #line 99 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            this.FloorRecallButton4.Click += new System.Windows.RoutedEventHandler(this.RecallBT_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.FloorRecallButton5 = ((System.Windows.Controls.Button)(target));
            
            #line 100 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            this.FloorRecallButton5.Click += new System.Windows.RoutedEventHandler(this.RecallBT_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            this.FloorRecallButton6 = ((System.Windows.Controls.Button)(target));
            
            #line 101 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            this.FloorRecallButton6.Click += new System.Windows.RoutedEventHandler(this.RecallBT_Click);
            
            #line default
            #line hidden
            return;
            case 15:
            this.RecallButton = ((System.Windows.Controls.Button)(target));
            
            #line 102 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            this.RecallButton.Click += new System.Windows.RoutedEventHandler(this.RecallBT_Click);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 103 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.UnavailabilityBT_Click);
            
            #line default
            #line hidden
            return;
            case 17:
            this.send0j = ((System.Windows.Controls.Button)(target));
            
            #line 104 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            this.send0j.Click += new System.Windows.RoutedEventHandler(this.send0j_Click);
            
            #line default
            #line hidden
            return;
            case 18:
            
            #line 106 "..\..\..\..\UserWindows\DDJDetailWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AvailabilityBT_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

