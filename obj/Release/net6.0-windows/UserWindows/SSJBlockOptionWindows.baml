   M S B A M L   `   `   `  ���� K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:BMHRI.WCS.Server.UserWindowsBMHRI.WCS.Server.UserWindows  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem A%clr-namespace:BMHRI.WCS.Server.ModelsBMHRI.WCS.Server.Models  Q-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  8    2BMHRI.WCS.Server.UserWindows.SSJBlockOptionWindows   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     <local*clr-namespace:BMHRI.WCS.Server.UserWindows     7core&clr-namespace:System;assembly=mscorlib     8models%clr-namespace:BMHRI.WCS.Server.Models     HValueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     -     � Title$  DeviceOptionWindow��$
��auto��$	��650�� � WindowStyle$ 
ToolWindow=� � WindowStartupLocation$ CenterScreen=� � 	Resources >   8BMHRI.WCS.Server.ValueConverter.EnumDescriptionConverter   EnumDescriptionConverter:  4BMHRI.WCS.Server.ValueConverter.BoolToValueConverter  BoolToValueConverter  DeviceSysTypeKey S� 
MethodName S� 
ObjectTypeU QSystem.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a  System.Enum S� MethodParameters M� Type.   (BMHRI.WCS.Server.Models.DeviceSystemType  DeviceTypeKey%   BMHRI.WCS.Server.Models.EqpType  MotionDirKey8   2BMHRI.WCS.Server.Models.DeviceBlockMotionDirection��%�   &	        &	      &	 
     &	 2     &	 Z         S� $ 	GetValues��"   M� "  S� $ 	GetValues��"   M� "  S� $ 	GetValues��"   M� "  .���� $��10q�.+��� $��设备基本配置��$��5q�.��� $	��Top=�.+���  �� Orientation$ 
Horizontal=�$��5q�$��25��.+��� $��设备编号：��$��70���� -   	 ��Name$	 
PositionTB��$	��100��$��Center=����� *
Position+�� $ 
Horizontal=�$��5q�$��25��.+��� $��PLC编号：��$��70���� -   $	 PLCIDTB��$	��100��$��Center=����� *PLCID+�� $ 
Horizontal=�$��5q�$��30��.+��� $��设备类型：��$��70��$��Center=��� -   
 ��Name$
 	DevTypeCB��$	��100��$��Center=� �� SelectedItem �� *
DeviceType+����  �� Source# [ �� $ 
Horizontal=�$��5q�$��30��.+��� $��流程类型：��$��70��$��Center=��� -   $
 	SystypeCB��$	��100��$��Center=� �� *
SystemType+���� # [ �� $ 
Horizontal=�$��5q�$��25��.+��� $��DB地址：��$��70���� -   $	 DBaddrTB��$	��100��$��Center=����� *PalletNumDBAddr+�� $ 
Horizontal=�$��5q�$��30��.+��� $��是否可用：��$��70��$��Center=��� -   $
 AvailableCB��$	��100��$��Center=� �� Text �� *	Available+ �� 	Converter# [  �� ConverterParameter$ 当前可用�� �� FallbackValue$ 长期故障��.@��� $��当前可用��
 � Tag$ 1��$��Center=��� $��长期故障��$ 0���� $ 
Horizontal=�$��5q�$��30��.+��� $��默认运行方向：��$	��100��$��Center=��� -   $
 MotionDirectionCB��$	��100��$��Center=� �� *MotionDirection+���� # [ �� $ 
Horizontal=�$��5q�$��30��.+��� $��当前运行方向：��$	��100��$��Center=��� $	��100��$��Center=����� *CurrMotionDirection+�� $ 
Horizontal=�$��5q�$��30��.+��� $��位置间隔：��$��70��$��Center=��� -	   $	 MarginTB��$��40��$��Center=����� *Margin+�� $ 
Horizontal=�$��5q�$��30��.+��� $��Q值读取地址：��$��Center=��� -
   $	 	MotionQTB��$��30��$��Center=����� *MotionQAddr+�� $��位地址1：��$��Center=��� -   $	 
BitAddr1TB��$��30��$��Center=����� *	QAddrBit1+�� $��位地址2：��$��Center=��� -   $	 
BitAddr2TB��$��30��$��Center=����� *	QAddrBit2+�� $ 
Horizontal=�$��5q�$��30��.+��� -    ��Name$ 
AlarmLable��$	��200��$��Center=�
���  ���� -   $��保存设备设置��$��80��$	��5,0q��� -   $��更改设备编号��$��80��$��5,0,0,0q��� $��设备状态��$��5q�.��� .+��� $ 
Horizontal=�$��5q�$��30��.+��� $��托盘号：��$��70���� $	��100������ *	PalletNum+�� $ 
Horizontal=�$��5q�$��30��.+��� $��起始地址：��$��70���� $	��100������ *
FmLocation+�� $ 
Horizontal=�$��5q�$��30��.+��� $��目的地址：��$��70���� $	��100������ *
ToLocation+�� $ 
Horizontal=�$��5q�$��30��.+��� $��故障内容：��$��70���� $	��100������ *FaultContent1+�� $ 
Horizontal=�$��5q�$��30��.+��� $��故障代码：��$��70���� $	��100������ *	ErrorCode+�� $ 
Horizontal=�$��5q�$��30��.+��� -   $ AllowLoadLB��$��允许卸货：��$��70���� -   $ AllowLoadLB2��$	��100������ *AllowUnloading+�� $ 
Horizontal=�$��5q�$��30��.+��� -   $ TPHorizonLB��$��入库旋转台横向：��$	��100���� -   $ TPHorizonLB2��$	��100������ *	TPHorizon+�� $ 
Horizontal=�$��5q�$��30��.+��� -    ��Name$ ClearBT��$��清除占位��$	��5,0q�$��80��$��30��$	��Top=��� $ 
Horizontal=�$��5q�$��30��.+��� $��	提示：��$��70���� -   $ ClearOccupyAlarmLabel��
���  ��$	��200��