   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem   BMHRI.WCS.Server=   7BMHRI.WCS.Server.UserControls.SQLServerDeviceMsgQueryUC   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6sys&clr-namespace:System;assembly=mscorlib     -     � 	Resources     ListViewItemStyle �� 
TargetType �� Property �� Value��%4   &	        �� " ��.���  � ��$
 Left=�-   .��� ���	#FFE5E5E5 � RowDefinitions �� $��40 ��� $��* �.+���  �� Orientation$ 
Horizontal=�$
��10,5q�.+��� -    ��Name$ TitlLB��
���  ��$��Center=�$
��10,0q��� $��起始时间:��$��Center=�$��Center=�$	��5,0q�(  "System.Windows.Controls.DatePicker  -     Name$ StartDP��$��Center=�
 �� Now	   SelectedDate#	 Z $	��5,0q�$��95���� $	��---��$��Center=�$	��5,0q��� $��结束时间:��$��Center=�$��Center=�$	��5,0q�  -   $ EndDP��#	 Z $��Center=�$	��5,0q�$��95���� -   
 ��Name$
 
DeviceIDCB��$��Center=�$	��5,0q�$��80��$y�	Collapsed=��� $��包含字符串��$��Center=�$	��5,0q��� -    ��Name$ StrTB��$��Center=�$��80��$��Center=�$	��5,0q��� -   $��查询��$��80��$��Center=�$	��5,0q��� -	   $ AlarmLB��
���  ��$��Center=��� -
    ��Name$ 
SqlQueryDG��$��1��#��[   �� View �� .P��� $��	PLC编号�� �� Width$	 150�� �� DisplayMemberBinding �� *PLCID+�� $��消息方向��$	 150�� �� *	Direction+�� $��消息内容��$
 auto�� �� *Trans+�� $��消息说明��$
 auto�� �� *
MsgParse+�� $��时间��$
 auto�� �� *Tkdat+ �� StringFormat$! {0:yyyy-MM-dd HH:mm:ss:fff}���� $��	序列号��$
 auto�� �� *
MsgSeqID+