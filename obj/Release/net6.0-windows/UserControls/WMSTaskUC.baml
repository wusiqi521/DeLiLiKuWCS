   M S B A M L   `   `   `  ���� K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:BMHRI.WCS.Server.UserControlsBMHRI.WCS.Server.UserControls  Q-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  -    'BMHRI.WCS.Server.UserControls.WMSTaskUC   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     =local+clr-namespace:BMHRI.WCS.Server.UserControls     C	converter-clr-namespace:BMHRI.WCS.Server.ValueConverter       � 	Resources  >   8BMHRI.WCS.Server.ValueConverter.EnumDescriptionConverter   EnumDescriptionConverter  ListViewItemStyle �� 
TargetType �� Property �� Value��%E   &	        &	        �� " ��.���  � ��$ Center=�-   .���  � RowDefinitions �� $��* ��� $��* �.+��� $��管理任务列表�����	#FF000000$��5q�.���  �� $��* ��� $
��auto �.+��� -    ��Name$ 	WMSTaskLV��$	��150��#���   �� View �� .P��� $��	任务号�� �� Width$	 130�� �� DisplayMemberBinding �� *taskId+�� $��	托盘号��$ 90�� �� *	PalletNum+�� $��任务类型��$ 90�� �� *
TaskType+	 �� 	Converter#	 [  �� $��起始地址��$	 110�� �� *
FmLocation+�� $��目的地址��$	 110�� �� *
ToLocation+�� $��任务状态��$	 110�� �� *
TaskStatus+#	 [  �� $��	优先级��$ 90�� �� *
Priority+�� $��任务来源��$ 90�� �� *
TaskSource+�� $��
WMS货位1��$	 130�� �� *WMSLocation+�� $��
WMS货位2��$	 130�� �� *WMSLocation2+�� $��高低标志��$ 90�� �� *	GaoDiBZ+#	 [  �� $��创建时间��$	 200�� �� *
CreateTime+
 �� StringFormat$%
 {0:yyyy-MM-dd HH:mm:ss.fffffff}���� $��WMSTaskGuid��$	 200�� �� *
WMSSeqID+� $��1��.+��� $��
任务数:��$��Center=��� -    ��Name$ 	TaskNumLB��$��Center=�$��0,0,10,0q��� -   $��刷新列表��$��5q��� -   $��修改任务��$��5q��� -    ��Name$ PalletNumTBM��$	��150��$��5q��� -   $��查找任务��$��5q��� $��1��$��设备任务列表�����	#FF000000$��5q�.���  �� $��* ��� $
��auto �.+��� -   $ 	WCSTaskLV��$	��150��#���   �� .P��� $��	托盘号��$ 90�� �� *	PalletNum+�� $��设备ID��$	 160�� �� *
DeviceID+�� $��任务类型��$ 90�� �� *
TaskType+#	 [  �� $��起始地址��$ 90�� �� *
FmLocation+�� $��目的地址��$ 90�� �� *
ToLocation+�� $��状态��$ 90�� �� *
TaskStatus+#	 [  �� $��	优先级��$ 90�� �� *	TaskPri+�� $��高低标志��$ 90�� �� *	GaoDiBZ+#	 [  �� $��创建时间��$	 200�� �� *
CreateTime+$%
 {0:yyyy-MM-dd HH:mm:ss.fffffff}���� $��WCSSeqID��$	 200�� �� *
WCSSeqID+�� $��WMS任务ID��$	 160�� �� *
WMSSeqID+� $��1��.+��� $��
任务数:��$��Center=��� -	   $ WCSTaskNumLB��$��Center=�$��0,0,10,0q��� -
   $��刷新列表��$��5q��� -   $ PalletNumTBC��$	��150��$��5q��� -   $��查找任务��$��5q��� -   $��出库尚未进行任务��$��5q��� -   $��堆垛机故障任务��$��5q�