   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   	   K GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter 5   	   N JS7.Net, Version=0.13.0.0, Culture=neutral, PublicKeyToken=d5812d469e84c693/$clr-namespace:S7.Net;assembly=S7.NetS7.Net 5   	   M+clr-namespace:BMHRI.WCS.Server.UserControlsBMHRI.WCS.Server.UserControls 5	   	   2   ,BMHRI.WCS.Server.UserWindows.DDJDetailWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6sys&clr-namespace:System;assembly=mscorlib     5   	   HvalueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     5   	   6S7Net$clr-namespace:S7.Net;assembly=S7.Net     5   	   =local+clr-namespace:BMHRI.WCS.Server.UserControls     5	   	   -   5          � Title$  	堆垛机��5   	   $	��600��6    � WindowStyle$ 
ToolWindow=�6"    � SizeToContent$ Height=�6R    � 	Resources 5      ; 5BMHRI.WCS.Server.ValueConverter.DataItemValueToString   pvvToString9 3BMHRI.WCS.Server.ValueConverter.F20ToValueConverter  F20ToValueConverter9 3BMHRI.WCS.Server.ValueConverter.F21ToValueConverter  F21ToValueConverter9 3BMHRI.WCS.Server.ValueConverter.F19ToValueConverter  F19ToValueConverter4 .BMHRI.WCS.Server.ValueConverter.IndexConverter  MyIndexConverter  DataTypeList S� 
MethodName S� 
ObjectTypeU QSystem.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a  System.Enum S� MethodParameters M� TypeName  VarTypeList��%z  &	        &	      &	 &     &	 9     &	 L     &	 _     &	 �       5   
   6/     5   
   6-     5   
   6-     5   
   6-     5   
   6(   S� 5   
   $ 	GetValues��62   "  6I    5      M� 5      $ S7Net:DataType��6   5      S� 5   
   $ 	GetValues��61   "  6H    5      M� 5      $ S7Net:VarType��6   5      5      .��5      � ���	#FFFFFBFB.+�5   
   �� $��10q�6   .+�5      �� $��设备状态��6   $	��0,5q�6%   .��5       � .+�5!      �� $��名   称：��6   �� 5"      $	��100��6   ��6+   �� *PLCDecription+�� 5#      $��PLC类型：��6   �� 5$      $��50��6   ��6*   �� *	CpuType+�� 5%      $��机架号：��6   �� 5&      $��30��6   ��6*   �� *Rack+�� 5'      $��IP地址：��6   $��Center=�6-   �� 5(      $	��100��6   $��Center=�6A   ��6-   �� *IP+6A   �� 5)      $��插槽号：��6   �� 5*      $��30��6   ��6*   �� *Slot+�� 5+      $��设备编号：��6   �� 5,      $��50��6   ��6*   �� *PLCID+�� 5-      $��网络状态：��6   �� 5.      $��50��6   ��6*   �� *PLCConnectState+�� 5/      $��工作状态：��6   �� 50      $��60��6   ��6*   �� *DDJWorkState+�� 51      $��是否可用：��6   �� 52      $��30��6   ��6*   �� *	Available+�� 55      $��水平位置：��6   �� 56      $��30��6   ��6*   �� *MotionPosition+�� 57      $��升降高度：��6   �� 58      $��30��6   ��6*   �� *LiftingPosition+�� 5=      $��货叉位置：��6   �� 5>      $��30��6   ��6*   �� *ForkPosition+�� 5?      $��是否有盘：��6   �� 5@      $��40��6   ��6*   �� *	HasPallet+�� 5A      $��托盘号：��6   �� 5B      $��80��6   ��6*   �� *	PalletNum+�� 5C      $��故障代码：��6   �� 5D      $��50��6   
���  ����6*   �� *	FaultCode+6H   �� 5E      $��故障内容：��6   �� 5F      $��80��6   
���  ����6*   �� *FaultContent+6K   �� 5G      $��CannotDo：��6   �� 5H      $��80��6   
���  ����6*   �� *
CannotDo+6G   5L      5M      �� 5N      $��设备操作��6   $	��0,5q�6%   .��5O      � .+�5P      ��  �� Orientation$ 
Horizontal=�6!   $
��10,5q�6:   .+�5Q      �� $��WCS TO PLC DB4 : ��6    $��Right=�6<   �� 5R      $	��300��6    ��6/   �� *WCSTOPLCDB4+5S      �� 5T      $ 
Horizontal=�6!   $
��10,5q�6:   .+�5U      �� $��PLC TO WCS DB5 : ��6    $��Right=�6<   �� 5V      $	��300��6    ��6/   �� *PLCTOWCSDB5+5W      � 5X      	 � Orientation$	 
Horizontal=�6    $
��Left=�6:   $
��10,5q�6V   .+�5Y      �� -   5        $��连接网络��5Y   !   $��5q�62   $��80��6U   $��30��6`   �� 5Z      -   5        $��断开网络��5Z   !   $��5q�62   $��80��6U   $��30��6`   �� 5[      -   5        $��联机��5[   !   $��5q�60   $��80��6R   $��30��6]   �� 5\      -   5        $��脱机��5\   !   $��5q�60   $��80��6S   $��30��6^   �� 5]      -   5        $��故障解除��5]   !   $��5q�62   $��80��6[   $��30��6f   �� 5^      -   5        $��	DB4清零��5^   !   $��5q�63   $��80��6Y   $��30��6d   �� 5_      -   5        $��	DB5清零��5_   !   $��5q�63   $��80��6Y   $��30��6d   �� 5`      -	   5        
 ��Name$
 FloorRecallButton1��5`   !   $��一楼南召回��6=   $��5q�6O   $��80��6q   $��30��6|   �� 5a      -
   5        $
 FloorRecallButton2��5a   !   $��二楼南召回��6=   $��5q�6O   $��80��6q   $��30��6|   �� 5b      -   5        $
 FloorRecallButton3��5b   !   $��一楼北召回��6=   $��5q�6O   $��80��6q   $��30��6|   �� 5c      -   5        $
 FloorRecallButton4��5c   !   $��二楼北召回��6=   $��5q�6O   $��80��6q   $��30��6|   �� 5d      -   5        $
 FloorRecallButton5��5d   !   $��三楼北召回��6=   $��5q�6O   $��80��6q   $��30��6|   �� 5e      -   5        $
 FloorRecallButton6��5e   !   $��三楼南召回��6=   $��5q�6O   $��80��6q   $��30��6|   �� 5f      -   5        $
 RecallButton��5f   !   $��模具库堆垛机召回��67   $��5q�6L   $	��100��6n   $��30��6z   �� 5g      -   5        $��准备进入巷道��5g   !   
���  ��
�������$��2��6V   $��5q�6c   $��Right=�6�   $��80��6�   $��30��6�   �� 5h      -   5        $
 send0j��5h   !   
���  ��$��安全门打开��6C   
�������$��5q�6g   $��80��6�   $��30��6�   �� 5j      -   5        $��设备可用��5j   !   $��2��61   $��5q�6>   $��Right=�6f   $��80��6�   $��30��6�   
��� � �
�������6�   5k      5l      5m      5n      5o      5p      