   M S B A M L   `   `   `  ���� K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:BMHRI.WCS.Server.UserControlsBMHRI.WCS.Server.UserControls  3clr-namespace:BMHRI.WCS.ServerBMHRI.WCS.Server  !    BMHRI.WCS.Server.MainWindow   <UserControls+clr-namespace:BMHRI.WCS.Server.UserControls  W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     0localclr-namespace:BMHRI.WCS.Server     -     � Title$?  9ASRS WCS.Server - 北京起重运输机械设计研究院�� � Icon$ /Images/logo2.png�� � WindowState$ 	Maximized=�.��� $
��True��
������� � ColumnDefinitions �� $��50 ��� $��* �.+��� -    ��Name$ ToolBarGrid��$��50��$
��Left=� K� ZIndex$ 10��
���   �.+��� -    ��Name$
 img0��$��/Images/报警.png��$��5q� � ToolTip$ 设备管理���� -   $
 img1��$��/Images/在线监控.png��$��5q�$ 在线监控���� -   $
 img2��$��/Images/日志记录.png��$��5q�$ 日志记录���� -   $
 img3��$��/Images/任务列表.png��$��5q�$ 任务列表���� -   $
 img4��$��/Images/库存统计.png��$��5q�$ 库存统计���� -   $
 img6��$��/Images/任务设定.png��$��5q�$ 任务设置���� -	   $
 img7��$��/Images/关于我们.png��$��5q�$ 关于我们��� -
    �Name$ ContainerListGrid��$��1�� �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+�-   'BMHRI.WCS.Server.UserControls.WMSTaskUC  $��3��$��1q����#3fb0f9+   %BMHRI.WCS.Server.UserControls.SenceUC  $��1�����	#FF3FB0F9$��1q�� $��2��.+���    TabControlStyle1#��[  	 �� TabStripPlacement$	 Bottom=����	#FF0C7ED4$��1q����#3fb0f9.@��� $��WMS消息日志��$��Center=�  TabItemStyle#��[ ���	#ddffffff$��34��.��� $
}�True��.+�7   1BMHRI.WCS.Server.UserControls.SQLServerMsgQueryUC  -   
  Name$
 FlMsgUC��   SQLServerQueryName$ WMS消息日志��   SQLServerTableName$ WCS2WMSJsonLog���� $��北起设备消息日志��$��Center=�#��[ ���	#ddffffff$��34��.��� $
}�True��.+�=   7BMHRI.WCS.Server.UserControls.SQLServerDeviceMsgQueryUC     SQLServerQueryName$ 北起设备消息日志��   SQLServerTableName$ PLC_Message_Log��   IsDeviceMsgQuery$
 True���� $��德马泰克消息日志��$��Center=�#��[ ���	#ddffffff$��34��.��� $
}�True��.+�>   8BMHRI.WCS.Server.UserControls.SQLServerDematicMsgQueryUC     SQLServerQueryName$ 德马泰克消息日志��   SQLServerTableName$ DEMATIC_Send_Buffer_Log��   IsDeviceMsgQuery$
 True��:   4BMHRI.WCS.Server.UserControls.GoodsLocationManagerUC  $��4�����	#FF3FB0F9$��1q�3   -BMHRI.WCS.Server.UserControls.DeviceManagerUC  $��0�����	#FF3FB0F9$��1q�1   +BMHRI.WCS.Server.UserControls.UserManagerUC  $��5�����	#FF3FB0F9$��1q�-	   'BMHRI.WCS.Server.UserControls.AboutUSUC	  $��7�����	#FF3FB0F9$��1q�3
   -BMHRI.WCS.Server.UserControls.WMSTaskCreateUC
  $��6�����	#FF3FB0F9$��1q�