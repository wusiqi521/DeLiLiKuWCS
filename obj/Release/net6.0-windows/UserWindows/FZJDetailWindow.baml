   M S B A M L   `   `   `  ���� K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem N JS7.Net, Version=0.13.0.0, Culture=neutral, PublicKeyToken=d5812d469e84c693/$clr-namespace:S7.Net;assembly=S7.NetS7.Net 2    ,BMHRI.WCS.Server.UserWindows.FZJDetailWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     HvalueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     6sys&clr-namespace:System;assembly=mscorlib     6S7Net$clr-namespace:S7.Net;assembly=S7.Net     -   $	��300��$	��800��  � WindowStyle$  
ToolWindow=� � Title �� *
Decription+ �� Mode$ OneWay=� � 	Resources ;  5BMHRI.WCS.Server.ValueConverter.DataItemValueToString   pvvToString4  .BMHRI.WCS.Server.ValueConverter.IndexConverter  MyIndexConverter  DataTypeList S� 
MethodName S� 
ObjectTypeU QSystem.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a  System.Enum S� MethodParameters M� TypeName  VarTypeList��%�   &	        &	      &	 
     &	 B         S� $ 	GetValues��"   M� $ S7Net:DataType��S� $ 	GetValues��"   M� $ S7Net:VarType��.��� 
�������.+��� $��10q�.@��� $��设备状态��.���� $��10q�.+��� $��设备状态��$	��0,5q�.��� .+���  �� Orientation$ 
Horizontal=�$��5q�.+��� $��设备名称：��$��Center=��� $	��100��$��Center=����� *PLCDecription+�� $ 
Horizontal=�$��5q�.+��� $��液压站电机故障：��$��Center=��� $��50��$��Center=����� *	FaultCode+�� $ 
Horizontal=�$��5q�.+��� $��紧急停止：��$��Center=��� $��30��$��Center=����� *EmergencyStop+�� $ 
Horizontal=�$��10q�.+��� $��自动/手动：��$��Center=��� $��30��$��Center=����� *
ManuAuto+�� $ 
Horizontal=�$��5q�.+��� $��IP地址：��$��Center=��� $	��100��$��Center=����� *IP+�� $ 
Horizontal=�$��5q�.+��� $��设备编号：���� $��50��$��Center=����� *PLCID+�� $ 
Horizontal=�$��5q�.+��� $��网络状态：���� $��50��$��Center=����� *PLCConnectState+�� $ 
Horizontal=�$��5q�.+��� $��翻转机立位：���� $��50��$��Center=����� *
Vertival+�� $ 
Horizontal=�$��5q�.+��� $��翻转机卧位：���� $��50��$��Center=����� *
Horizontal+�� $ 
Horizontal=�$��5q�.+��� $��推料缸顶出：��$��Center=��� $��60��$��Center=����� *PutMateriStretch+�� $ 
Horizontal=�$��5q�.+��� $��推料缸缩回：��$��Center=��� $��50��$��Center=����� *PutMateriShrink+�� $ 
Horizontal=�$��5q�.+��� $!��托盘前进到位光电：��$��Center=��� $��80��$��Center=����� *PalletForward+�� $ 
Horizontal=�$��5q�.+��� $!��托盘后退到位光电：��$��Center=��� $��80��$��Center=����� *
PalletBack+�� $ 
Horizontal=�$��5q�.+��� $��气缸压紧：��$��Center=��� $��80��$��Center=����� *CylinderHold+�� $ 
Horizontal=�$��5q�.+��� $��气缸松开：��$��Center=��� $��80��$��Center=����� *CylinderRelease+