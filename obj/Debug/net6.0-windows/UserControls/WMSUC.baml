   M S B A M L   `   `   `  ����  BMHRI.WCS.Server)    #BMHRI.WCS.Server.UserControls.WMSUC   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5        $��5q�5   7     � 	Resources  5         WMSConnectStateStyle �� 
TargetType �� Property �� Value �� Triggers �� Binding �� Value��%�  &	        �� 5	   
   " ��6-   .�5
      ��  � ��$ 5q�6'   �� 5       � ��$ Stretch=�62   �� 5       � ��$ Stretch=�64    5      �� 5       6   �� *WMSServerConnectStatus+ 06I   .a�5      ��  � ��
 �����6-   �� 5       �@�ToolTip$ 	未连接��60   5      �� 5       6   �� *WMSServerConnectStatus+ -16I   .a�5      ��  � ��
 � ���6-   �� 5       �@�ToolTip$ 正在连接��60   5      �� 5       6   �� *WMSServerConnectStatus+ 16I   .a�5      ��  � ��
 � � �6-   �� 5       �@�ToolTip$ 	已连接��60   5      5      5      .��5      � ���#DDDDDD � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5       $��* �6   �� 5!      $��* �6    � RowDefinitions 5#   
   �� 5$      $��* �6   �� 5%      $��* �6   �� 5&      $��* �6   �� 5'      $
��0.3* �6   .+�5)   
   (� $��0��6   $��2��6   $��2q�63   .��5*      �� -   5        	 ��Name$	 
DeviceName��5*      $	��WMS��6(   
���   �66   5+      (� 5,   
   $��1��6   $��2q�6   .��5-      �� $��网络��6   
���   �6!   5.      �� 5/   
   $��1��6   $��1��6!   $��2��61   #���   6E   $
��10,5q�6t   50      51      