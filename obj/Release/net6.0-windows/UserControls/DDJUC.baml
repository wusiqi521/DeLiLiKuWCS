   M S B A M L   `   `   `  ���� K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  A%clr-namespace:BMHRI.WCS.Server.ModelsBMHRI.WCS.Server.Models  )    #BMHRI.WCS.Server.UserControls.DDJUC   DmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     HValueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     8models%clr-namespace:BMHRI.WCS.Server.Models     -   $��5q�  � 	Resources  :  4BMHRI.WCS.Server.ValueConverter.BoolToValueConverter   BoolToValueConverter  DDJWorkStateStyle �� 
TargetType �� Property �� Value �� Triggers �� Binding �� Value  DDJConnectStateStyle  DDJWorkStatusStyle  DDJWorkStatusOperationStyle  DDJWorkStatusRecallStyle  ConnectMenuItemHeaderStyle��%�  &	        &	      &	     &	 �    &	 �    &	 �
    &	 V      �� " ��.���  � ��$ 5q���  � ��$ Stretch=���  � ��$ Stretch=� ��  �� *DDJWorkState+ 0.a���  � ��
 �������  �@�ToolTip$ 未知����  �� *DDJWorkState+ 1.a���  � ��
 �2͚���  �@�ToolTip$ 联机����  �� *DDJWorkState+ 2.a���  � ��
 � � ���  �@�ToolTip$ 待机����  �� *DDJWorkState+ 3.a���  � ��
 �� ���  �@�ToolTip$ 工作����  �� *DDJWorkState+ 4.a���  � ��
 �  ����  �@�ToolTip$ 故障����  �� *DDJWorkState+ 5.a���  � ��
 � �����  �@�ToolTip$ 脱机����  �� *DDJWorkState+ 6.a���  � ��
 �������  �@�ToolTip$ 手动���� " ��.���  � ��$ 5q���  � ��$ Stretch=���  � ��$ Stretch=� ��  �� *PLCConnectState+ 0.a���  � ��
 �������  �@�ToolTip$ 未知����  �� *PLCConnectState+ 1.a���  � ��
 � �����  �@�ToolTip$ 正在联机����  �� *PLCConnectState+ 2.a���  � ��
 � � ���  �@�ToolTip$ 	已连接���� " m� ��  �� *DDJWorkState+ None.a���  � ��$ 联机����  �� *DDJWorkState+
 Online.a���  � ��$ 脱机����  �� *DDJWorkState+ Standby.a���  � ��$ 脱机����  �� *DDJWorkState+ Working.a���  � ��$ 脱机����  �� *DDJWorkState+	 Fault.a���  � ��$ 脱机����  �� *DDJWorkState+ Offline.a���  � ��$ 联机����  �� *DDJWorkState+
 Manual.a���  � ��$ 联机����  � {�$ False����  �� *DDJWorkState+ Working.a���  � {�$ False����  �� *PLCConnectState+ 0.a���  � {�$ False����  �� *PLCConnectState+ 1.a���  � {�$ False����  �� *PLCConnectState+ 2.a���  � {�$
 True���� " m� ��  �� *DDJWorkState+ None.a���  � {�$ False����  �� *DDJWorkState+
 Online.a���  � {�$
 True����  �� *DDJWorkState+ Standby.a���  � {�$
 True����  �� *DDJWorkState+ Working.a���  � {�$ False����  �� *DDJWorkState+	 Fault.a���  � {�$ False����  �� *DDJWorkState+ Offline.a���  � {�$ False����  �� *DDJWorkState+
 Manual.a���  � {�$ False����  �� *DDJWorkState+ Working.a���  � {�$ False����  �� *PLCConnectState+ 0.a���  � {�$ False����  �� *PLCConnectState+ 1.a���  � {�$ False���� " m� ��  �� *DDJWorkState+ None.a���  � {�$ False����  �� *DDJWorkState+
 Online.a���  � {�$
 True����  �� *DDJWorkState+ Standby.a���  � {�$
 True����  �� *DDJWorkState+ Working.a���  � {�$ False����  �� *DDJWorkState+	 Fault.a���  � {�$
 True����  �� *DDJWorkState+ Offline.a���  � {�$ False����  �� *DDJWorkState+
 Manual.a���  � {�$ False����  �� *DDJWorkState+ Working.a���  � {�$ False����  �� *PLCConnectState+ 0.a���  � {�$ False����  �� *PLCConnectState+ 1.a���  � {�$ False���� " m� ��  �� *PLCConnectState+ 1.a���  � ��$ 断开连接����  �� *PLCConnectState+ 0.a���  � ��$ 连接设备����  �� *PLCConnectState+ 5.a���  � ��$ 连接设备����  �� *PLCConnectState+ 2.a���  � ��$ 断开连接�� � ContextMenu �� .@�m� -   #��[ m� -   $��故障解除��#��[ m� -   $��一楼召回��#��[ �� m� -   #��[ m� -   $��设备属性��.��� ���	#FF0B5E8D � ColumnDefinitions �� $��* ��� $��* ��� $��* �	 � RowDefinitions	 �� $��* ��� $��* ��� $��* ��� $
��0.5* �.+�(� $��0��$��3��$��2q�.���� -   
 ��Name$
 
DeviceName��$��	PLC设备��
��������� $��0��$��2��$��2q�$��X��$��Center=�$��Center=�$��18(�$��0q�
���  ��y��� *	Available+ �� 	Converter# [   �� ConverterParameter$ 	Collapsed�� �� FallbackValue$ Visible��(� $��1��$��2q�.���� $��状态��
�������(� $��2��$��2q�.���� $��网络��
��������� $��1��$��1��$��2��#���  $
��10,5q��� $��2��$��1��$��2��#���  $
��10,5q�