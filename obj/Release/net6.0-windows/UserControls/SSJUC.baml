   M S B A M L   `   `   `  ����   BMHRI.WCS.Server)    #BMHRI.WCS.Server.UserControls.SSJUC   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -   $��5q�  � 	Resources     SSJWorkStateStyle �� 
TargetType �� Property �� Value �� Triggers �� Binding �� Value  SSJConnectStateStyle  SSJMenuItemWorkStateStyle  ConnectMenuItemHeaderStyle��%�  &	        &	 
    &	 �    &	 �    �� " ��.���  � ��$ 5q���  � ��$ Stretch=���  � ��$ Stretch=� ��  �� *SSJWorkState+ 0.a���  � ��
 �������  �@�ToolTip$ 未知����  �� *SSJWorkState+ 1.a���  � ��
 �2͚���  �@�ToolTip$ 在线����  �� *SSJWorkState+ 2.a���  � ��
 � � ���  �@�ToolTip$ 待机����  �� *SSJWorkState+ 3.a���  � ��
 �� ���  �@�ToolTip$ 工作����  �� *SSJWorkState+ 4.a���  � ��
 �  ����  �@�ToolTip$ 故障����  �� *SSJWorkState+ 5.a���  � ��
 � �����  �@�ToolTip$ 手动����  �� *SSJWorkState+ 6.a���  � ��
 �������  �@�ToolTip$ 未知���� " ��.���  � ��$ 5q���  � ��$ Stretch=���  � ��$ Stretch=� ��  �� *PLCConnectState+ 0.a���  � ��
 �������  �@�ToolTip$ 	未连接����  �� *PLCConnectState+ 1.a���  � ��
 � �����  �@�ToolTip$ 正在连接����  �� *PLCConnectState+ 2.a���  � ��
 � � ���  �@�ToolTip$ 	已连接���� " m� ��  �� *SSJWorkState+ None.a���  � ��$ 联机����  �� *SSJWorkState+
 Online.a���  � ��$ 脱机����  �� *SSJWorkState+ Standby.a���  � ��$ 脱机����  �� *SSJWorkState+ Working.a���  � ��$ 脱机����  �� *SSJWorkState+	 Fault.a���  � ��$ 脱机����  �� *SSJWorkState+ Offline.a���  � ��$ 联机����  �� *SSJWorkState+
 Manual.a���  � ��$ 联机����  � {�$ False����  �� *PLCConnectState+ 0.a���  � {�$ False����  �� *PLCConnectState+ 1.a���  � {�$ False����  �� *PLCConnectState+ 2.a���  � {�$
 True���� " m� ��  �� *PLCConnectState+ 0.a���  � ��$ 连接网络����  �� *PLCConnectState+ 1.a���  � ��$ 断开网络����  �� *PLCConnectState+ 2.a���  � ��$ 断开网络�� � ContextMenu �� .@�m� -   #���  m� -   #���  m� -   $��设备状态��.��� ���	#FF0B5E8D � ColumnDefinitions �� $��* ��� $��* ��� $��* �	 � RowDefinitions	 �� $��* ��� $��* ��� $��* ��� $
��0.5* �.+�(� $��0��$��3��$��2q�.���� -   
 ��Name$
 
DeviceName��$��	PLC设备��
�������(� $��1��$��2q�.���� $��状态��
�������(� $��2��$��2q�.���� $��网络��
��������� $��1��$��1��$��2��#���   $
��10,5q��� $��2��$��1��$��2��#���  $
��10,5q�