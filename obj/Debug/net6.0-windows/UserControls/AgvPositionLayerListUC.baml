   M S B A M L   `   `   `  ����K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:BMHRI.WCS.Server.UserControlsBMHRI.WCS.Server.UserControls  5      K*clr-namespace:BMHRI.WCS.Server.UserWindowsBMHRI.WCS.Server.UserWindows  5      :    4BMHRI.WCS.Server.UserControls.AgvPositionLayerListUC   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:BMHRI.WCS.Server.UserControls     5      @	converter*clr-namespace:BMHRI.WCS.Server.UserWindows     5        � 	Resources  5
      ;   5BMHRI.WCS.Server.UserWindows.EnumDescriptionConverter   EnumDescriptionConverter  ListViewItemStyle �� 
TargetType �� Property �� Value��%�   &	        &	        5   
   6-   �� 5   
   " ��6*   .�5      ��  � ��$
 Left=�6;   6J   -   5        5      5      .��5      �� -   5         ��Name$ Gp��5      $��一楼AGV位置管理��6   ���	#FF000000$��5q�6E   $��0��6P   .��5   
   � .+�5      �� -   5         ��Name$ AgvPositionLLV��5      $	��150��60   #���  6A    �� View 5      �� 5      .P�5      �� $��位置编号��6*    �� Width$
 auto��6b    �� DisplayMemberBinding 68   �� *
Position+6b   �� 5      $��	WMS编号��6*   $
 auto��6e    69   �� *
FLPosition+6e   �� 5      $��	WMS区域��6*   $
 auto��6f    69   �� *FLLPosition+6f   �� 5      $��对应SSJ编号��6*   $
 auto��6b    6;   �� *SSJID+6b   �� 5      $��SSJ位置编号��6*   $
 auto��6g    6;   �� *
SSJPositon+6g   �� 5      $��位置描述��6*   $
 auto��6b    68   �� *
Describe+6b   �� 5      $��位置类型��6*   $
 auto��6�    68   �� *PositionType+	 �� 	Converter#	 [  6�   �� 5      $��	托盘号��6*   $
 auto��6a    67   �� *
PalletNo+6a   �� 5      $��可用��6*    66   �� *IsAvailable+�� 5      $��位置状态��6*   $
 auto��6�    68   �� *PositionStatus+#	 [  6�   5       5"      5#      5$      5%      