   M S B A M L   `   `   `  ����K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	   N JS7.Net, Version=0.13.0.0, Culture=neutral, PublicKeyToken=d5812d469e84c693/$clr-namespace:S7.Net;assembly=S7.NetS7.Net 5   	   2    ,BMHRI.WCS.Server.UserWindows.FZJDetailWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   HvalueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     5   	   6sys&clr-namespace:System;assembly=mscorlib     5   	   6S7Net$clr-namespace:S7.Net;assembly=S7.Net     5   	   -   5        $	��300��5
   3   $	��800��6@     � WindowStyle$  
ToolWindow=�6d    � Title 6	   �� *
Decription+ �� Mode$ OneWay=� � 	Resources 5      ;  5BMHRI.WCS.Server.ValueConverter.DataItemValueToString   pvvToString4  .BMHRI.WCS.Server.ValueConverter.IndexConverter  MyIndexConverter  DataTypeList S� 
MethodName S� 
ObjectTypeU QSystem.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a  System.Enum S� MethodParameters M� TypeName  VarTypeList��%#  &	        &	      &	 &     &	 �       5   
   6/     5   
   6(   S� 5   
   $ 	GetValues��62   "  6I    5      M� 5      $ S7Net:DataType��6   5      S� 5   
   $ 	GetValues��61   "  6H    5      M� 5      $ S7Net:VarType��6   5      5      .��5      � 
�������.+�5   
   �� $��10q�6   .@�5      �� $��设备状态��6   .��5      �� $��10q�6   .+�5      �� $��设备状态��6   $	��0,5q�6-   .��5      � .+�5      ��  �� Orientation$ 
Horizontal=�6)   $��5q�6B   .+�5    "   �� $��设备名称：��6(   $��Center=�68   �� 5!   "   $	��100��6(   $��Center=�6Y   ��67   �� *PLCDecription+6Y   5"      �� 5#      $ 
Horizontal=�6)   $��5q�6B   .+�5$   "   �� $��液压站电机故障：��6(   $��Center=�6;   �� 5%   "   $��50��6(   $��Center=�6T   ��66   �� *	FaultCode+6T   5&      �� 5'      $ 
Horizontal=�6)   $��5q�6B   .+�5(   "   �� $��紧急停止：��6(   $��Center=�68   �� 5)   "   $��30��6(   $��Center=�6X   ��66   �� *EmergencyStop+6X   5*      �� 5+      $ 
Horizontal=�6)   $��10q�6B   .+�5,   "   �� $��自动/手动：��6(   $��Center=�69   �� 5-   "   $��30��6(   $��Center=�6S   ��66   �� *
ManuAuto+6S   5.      �� 5/      $ 
Horizontal=�6)   $��5q�6B   .+�50   "   �� $��IP地址：��6)   $��Center=�69   �� 51   "   $	��100��6*   $��Center=�6M   ��69   �� *IP+6M   52      �� 53      $ 
Horizontal=�6)   $��5q�6B   .+�54   "   �� $��设备编号：��6(   �� 55   "   $��50��6(   $��Center=�6P   ��66   �� *PLCID+6P   56      �� 57      $ 
Horizontal=�6)   $��5q�6B   .+�58   "   �� $��网络状态：��6(   �� 59   "   $��50��6(   $��Center=�6Z   ��66   �� *PLCConnectState+6Z   5:      �� 5;      $ 
Horizontal=�6)   $��5q�6B   .+�5<   "   �� $��翻转机立位：��6(   �� 5=   "   $��50��6(   $��Center=�6S   ��66   �� *
Vertival+6S   5>      �� 5@      $ 
Horizontal=�6)   $��5q�6B   .+�5A   "   �� $��翻转机卧位：��6(   �� 5B   "   $��50��6(   $��Center=�6U   ��66   �� *
Horizontal+6U   5C      �� 5D      $ 
Horizontal=�6)   $��5q�6B   .+�5E   "   �� $��推料缸顶出：��6(   $��Center=�69   �� 5F   "   $��60��6(   $��Center=�6[   ��66   �� *PutMateriStretch+6[   5G      �� 5H      $ 
Horizontal=�6)   $��5q�6B   .+�5I   "   �� $��推料缸缩回：��6(   $��Center=�69   �� 5J   "   $��50��6(   $��Center=�6[   ��66   �� *PutMateriShrink+6[   5K      �� 5L      $ 
Horizontal=�6)   $��5q�6B   .+�5M   "   �� $!��托盘前进到位光电：��6(   $��Center=�6<   �� 5N   "   $��80��6(   $��Center=�6Y   ��66   �� *PalletForward+6Y   5O      �� 5P      $ 
Horizontal=�6)   $��5q�6B   .+�5Q   "   �� $!��托盘后退到位光电：��6(   $��Center=�6<   �� 5R   "   $��80��6(   $��Center=�6V   ��66   �� *
PalletBack+6V   5S      �� 5T      $ 
Horizontal=�6)   $��5q�6B   .+�5U   "   �� $��气缸压紧：��6(   $��Center=�68   �� 5V   "   $��80��6(   $��Center=�6X   ��66   �� *CylinderHold+6X   5W      �� 5X      $ 
Horizontal=�6)   $��5q�6B   .+�5Y   "   �� $��气缸松开：��6(   $��Center=�68   �� 5Z   "   $��80��6(   $��Center=�6[   ��66   �� *CylinderRelease+6[   5[      5\      5]      5^      5_      5`      5a      5b      