   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   
   K GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:BMHRI.WCS.Server.ModelsBMHRI.WCS.Server.Models 5   	   K*clr-namespace:BMHRI.WCS.Server.UserWindowsBMHRI.WCS.Server.UserWindows 5   	   Q-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter 6B   7   1BMHRI.WCS.Server.UserWindows.AgvPositionOptionWin   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7core&clr-namespace:System;assembly=mscorlib     5   
   8models%clr-namespace:BMHRI.WCS.Server.Models     5   	   <local*clr-namespace:BMHRI.WCS.Server.UserWindows     5   	   HValueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     6B     � Title$  Agv位置操作窗口��5
   	    � SizeToContent$ WidthAndHeight=�6    � WindowStyle$ 
ToolWindow=�6:    � WindowStartupLocation$ CenterScreen=�6S   $��1,1,1,1q�6x    � 	Resources 5      : 4BMHRI.WCS.Server.ValueConverter.BoolToValueConverter   BoolToValueConverter  AgvPositionStatusKey S� 
MethodName S� 
ObjectTypeU QSystem.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a  System.Enum S� MethodParameters M� Type/  )BMHRI.WCS.Server.Models.AgvPositionStatus  AgvPositionSystemTypeKey3  -BMHRI.WCS.Server.Models.AgvPositionSystemType��%�   &	        &	      &	 n       5   
   6.   S� 5   
   $ 	GetValues��6:   "  6Q    5      M� 5      "  6   5      S� 5   
   $ 	GetValues��6>   "  6U    5      M� 5      "  6   5      5      .��5      � $��10q�6   	 � ColumnDefinitions	 5   
   �� 5      $��* �6   �� 5      $��* �6   
 � RowDefinitions
 5   
   �� 5      �� 5      �� 5       �� 5!      �� 5"      �� 5#      �� 5$      �� 5%      �� 5&      �� 5'      .+�5)   
   �� $��0��6   $��0��6   $��位置编号��6-   $
��Left=�6<   �� 5*   
   $��0��6   $��1��6   $	��180��6-   $��Center=�6Y   $
��Left=�6t   ��6<   �� *
Position+6t   �� 5+   
   $��1��6   $��0��6   $��	WMS区域��6-   $
��Left=�6=   �� 5,   
   $��1��6   $��1��6   $	��180��60   $��Center=�6[   $
��Left=�6w   ��6?   �� *
FLPosition+6w   �� 5-   
   $��2��6   $��0��6   $��	WMS编号��6-   $
��Left=�6=   �� 5.   
   $��2��6   $��1��6   $	��180��60   $��Center=�6\   $
��Left=�6x   ��6?   �� *FLLPosition+6x   �� 5/   
   $��3��6   $��0��6   $��对应SSJ编号��6-   $
��Left=�6?   �� 50   
   $��3��6   $��1��6   $	��180��6/   $��Center=�6U   $
��Left=�6p   ��6>   �� *SSJID+6p   �� 51   
   $��4��6   $��0��6   $��	托盘号��6-   $
��Left=�6;   �� 52   
   $��4��6   $��1��6   $	��180��6/   $��Center=�6X   $
��Left=�6s   ��6>   �� *
PalletNo+6s   �� 53   
   $��5��6   $��0��6   $��位置类型��6-   $
��Left=�6<   �� 54   
   $��5��6   $��1��6    $	��180��60   $��Center=�6�   $
��Left=�6�    �� SelectedItem 6?   �� *PositionType+��6e   ��  �� Source# [ 6�   �� 55   
   $��6��6   $��0��6   $��位置状态��6-   $
��Left=�6<   �� 56   
   $��6��6   $��1��6    $	��180��60   $��Center=�6�   $
��Left=�6�    6?   �� *PositionStatus+��6g   �� # [ 6�   �� 57   
   $��7��6   $��0��6   $��是否可用��6-   $
��Left=�6<   �� 58   
   -   5         ��Name$ AvailableCB��58   0   $��7��6   $��1��6    $	��100��6E   $��Center=�6�    �� Text 6T   �� *IsAvailable+ �� 	Converter# [   �� ConverterParameter$ 当前可用�� �� FallbackValue$ 长期故障��.@�59      �� $��当前可用��6   
 � Tag$ 1��6*   $��Center=�62   �� 5:      $��长期故障��6   $ 0��6*   5;      �� 5<   
   $��8��6   $��0��6   $��位置描述��6-   $
��Left=�6<   �� 5=   
   $��8��6   $��1��6   $	��180��60   $��Center=�6Y   $
��Left=�6u   ��6?   �� *
Describe+6u   � 5>   
   $��9��6   $��2��6!   .+�5?      �� -   5        $��80��5?      $��5q�6    $��保存设置��6+   6:   5@      5A      5B      