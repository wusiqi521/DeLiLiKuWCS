   M S B A M L   `   `   `  ����   BMHRI.WCS.Server)    #BMHRI.WCS.Server.UserControls.WMSUC   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -   $��5q�  � 	Resources     WMSConnectStateStyle �� 
TargetType �� Property �� Value �� Triggers �� Binding �� Value��%�  &	        �� " ��.���  � ��$ 5q���  � ��$ Stretch=���  � ��$ Stretch=� ��  �� *WMSServerConnectStatus+ 0.a���  � ��
 �������  �@�ToolTip$ 	未连接����  �� *WMSServerConnectStatus+ -1.a���  � ��
 � �����  �@�ToolTip$ 正在连接����  �� *WMSServerConnectStatus+ 1.a���  � ��
 � � ���  �@�ToolTip$ 	已连接��.��� ���	#FF0B5E8D � ColumnDefinitions �� $��* ��� $��* ��� $��* � � RowDefinitions �� $��* ��� $��* ��� $��* ��� $
��0.5* �.+�(� $��0��$��3��$��2q�.���� -   	 ��Name$	 
DeviceName��$	��WMS��
�������(� $��1��$��2q�.���� $��网络��
��������� $��1��$��1��$��2��#���   $
��10,5q�