   M S B A M L   `   `   `  ����K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:BMHRI.WCS.Server.UserWindowsBMHRI.WCS.Server.UserWindows  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      A%clr-namespace:BMHRI.WCS.Server.ModelsBMHRI.WCS.Server.Models  5      Q-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  5	   	   8    2BMHRI.WCS.Server.UserWindows.SSJBlockOptionWindows   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   <local*clr-namespace:BMHRI.WCS.Server.UserWindows     5   	   7core&clr-namespace:System;assembly=mscorlib     5      8models%clr-namespace:BMHRI.WCS.Server.Models     5      HValueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     5	   	   -   5          � Title$  DeviceOptionWindow��5   	   $
��auto��6$   $	��650��62    � WindowStyle$ 
ToolWindow=�6>    � WindowStartupLocation$ CenterScreen=�6X    � 	Resources 5      >   8BMHRI.WCS.Server.ValueConverter.EnumDescriptionConverter   EnumDescriptionConverter:  4BMHRI.WCS.Server.ValueConverter.BoolToValueConverter  BoolToValueConverter  DeviceSysTypeKey S� 
MethodName S� 
ObjectTypeU QSystem.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a  System.Enum S� MethodParameters M� Type.   (BMHRI.WCS.Server.Models.DeviceSystemType  DeviceTypeKey%   BMHRI.WCS.Server.Models.EqpType  MotionDirKey8   2BMHRI.WCS.Server.Models.DeviceBlockMotionDirection��%i  &	        &	      &	 &     &	 �     &	 �       5   
   62     5   
   6.   S� 5      $ 	GetValues��6:   "  6Q    5      M� 5      "  6   5      S� 5   
   $ 	GetValues��63   "  6J    5      M� 5      "  6   5      S� 5   
   $ 	GetValues��62   "  6I    5      M� 5      "  6   5      5      .��5       �� $��10q�6   .+�5!   
   �� $��设备基本配置��6   $��5q�6#   .��5"      � $	��Top=�6   .+�5#      ��  �� Orientation$ 
Horizontal=�6   $��5q�66   $��25��6A   .+�5$      �� $��设备编号：��6   $��70��6-   �� 5%      -   5        	 ��Name$	 
PositionTB��5%      $	��100��62   $��Center=�6[   ��6A   �� *
Position+6[   5&      �� 5'      $ 
Horizontal=�6   $��5q�66   $��25��6A   .+�5(      �� $��PLC编号：��6   $��70��6.   �� 5)      -   5        $	 PLCIDTB��5)      $	��100��60   $��Center=�6V   ��6?   �� *PLCID+6V   5*      �� 5+      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5,      �� $��设备类型：��6   $��70��6,   $��Center=�67   �� 5.      -   5        
 ��Name$
 	DevTypeCB��5.      $	��100��62   $��Center=�6f    �� SelectedItem 6B   �� *
DeviceType+��6�   ��  �� Source# [ 5/      �� 50      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�51      �� $��流程类型：��6   $��70��6,   $��Center=�67   �� 53      -   5        $
 	SystypeCB��53      $	��100��62   $��Center=�6e    6A   �� *
SystemType+��6�   �� # [ 54      �� 55      $ 
Horizontal=�6   $��5q�66   $��25��6A   .+�56      �� $��DB地址：��6   $��70��6,   �� 57      -   5        $	 DBaddrTB��57      $	��100��60   $��Center=�6`   ��6?   �� *PalletNumDBAddr+6`   58      �� 5=      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5>      �� $��是否可用：��6   $��70��6,   $��Center=�67   �� 5?      -   5        $
 AvailableCB��5?      $	��100��64   $��Center=�6�    �� Text 6C   �� *	Available+ �� 	Converter# [  �� ConverterParameter$ 当前可用�� �� FallbackValue$ 长期故障��.@�5@      �� $��当前可用��6'   
 � Tag$ 1��66   $��Center=�6>   �� 5A      $��长期故障��6'   $ 0��66   5B      5C      �� 5D      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5E      �� $��默认运行方向：��6   $	��100��6.   $��Center=�6:   �� 5F      -   5        $
 MotionDirectionCB��5F      $	��100��6:   $��Center=�6r    6I   �� *MotionDirection+��6�   �� # [ 5G      �� 5H      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5I      �� $��当前运行方向：��6   $	��100��6.   $��Center=�6:   �� 5J      $	��100��6D   $��Center=�6P   ��6   �� *CurrMotionDirection+6P   5K      �� 5L      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5M      �� $��位置间隔：��6   $��70��6,   $��Center=�67   �� 5N      -	   5        $	 MarginTB��5N      $��40��6H   $��Center=�6S   ��60   �� *Margin+6S   5O      �� 5P      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5Q      �� $��Q值读取地址：��6   $��Center=�6.   �� 5R      -
   5        $	 	MotionQTB��5R      $��30��61   $��Center=�6\   ��6?   �� *MotionQAddr+6\   �� 5S      $��位地址1：��6   $��Center=�6,   �� 5T      -   5        $	 
BitAddr1TB��5T      $��30��62   $��Center=�6[   ��6@   �� *	QAddrBit1+6[   �� 5U      $��位地址2：��6   $��Center=�6,   �� 5V      -   5        $	 
BitAddr2TB��5V      $��30��62   $��Center=�6[   ��6@   �� *	QAddrBit2+6[   5W      �� 5X      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5Y      �� -   5         ��Name$ 
AlarmLable��5Y      $	��200��60   $��Center=�6?   
���  ��6Z   �� 5Z      -   5        $��保存设备设置��5Z      $��80��6H   $	��5,0q�6V   �� 5[      -   5        $��更改设备编号��5[      $��80��6J   $��5,0,0,0q�6X   5\      5]      5^      �� 5_   
   $��设备状态��6   $��5q�6!   .��5`      � .+�5a      �� $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5b      �� $��托盘号：��6   $��70��6+   �� 5c      $	��100��6   ��6+   �� *	PalletNum+5d      �� 5e      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5f      �� $��起始地址：��6   $��70��6,   �� 5g      $	��100��6   ��6,   �� *
FmLocation+5h      �� 5i      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5j      �� $��目的地址：��6   $��70��6,   �� 5k      $	��100��6   ��6,   �� *
ToLocation+5l      �� 5m      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5n      �� $��故障内容：��6   $��70��6,   �� 5o      $	��100��6   ��6+   �� *FaultContent1+5p      �� 5q      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5r      �� $��故障代码：��6   $��70��6,   �� 5s      $	��100��6   ��6+   �� *	ErrorCode+5t      �� 5u      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5v      �� -   5        $ AllowLoadLB��5v      $��允许卸货：��61   $��70��6A   �� 5w      -   5        $ AllowLoadLB2��5w      $	��100��62   ��6A   �� *AllowUnloading+5x      �� 5y      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5z      �� -   5        $ TPHorizonLB��5z      $��入库旋转台横向：��61   $	��100��6D   �� 5{      -   5        $ TPHorizonLB2��5{      $	��100��63   ��6B   �� *	TPHorizon+5|      �� 5}      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5~      �� -   5        $ modeLB��5~      $��当前模式：��6,   $��70��6=   �� 5      -   5        $ modeLBs��5      $	��100��6-   ��6<   �� *CurrentMode+5�      �� 5�      $ 
Horizontal=�6   $��5q�66   $��30��6C   .+�5�      �� -   5         ��Name$ ClearBT��5�      $��清除占位��6.   $	��5,0q�6=   $��80��6J   $��30��6s   $	��Top=�6   5�      �� 5�      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5�      �� $��	提示：��6   $��70��6*   �� 5�      -   5        $ ClearOccupyAlarmLabel��5�      
���  ��$	��200��6M   5�      �� 5�      $ 
Horizontal=�6   $��5q�66   $��30��6A   .+�5�      �� -   5        $ NotAGV��5�      $��允许AGV放货：��6,   $	��100��6?   �� 5�      -   5        $ NotAGVs��5�      
���   �$	��100��6A   ��6M   �� *Allow_AGV_Put+5�      5�      5�      5�      5�      