   M S B A M L   `   `   `  ���� K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem N JS7.Net, Version=0.13.0.0, Culture=neutral, PublicKeyToken=d5812d469e84c693/$clr-namespace:S7.Net;assembly=S7.NetS7.Net 2    ,BMHRI.WCS.Server.UserWindows.SSJDetailWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     HvalueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     6sys&clr-namespace:System;assembly=mscorlib     6S7Net$clr-namespace:S7.Net;assembly=S7.Net     -   $	��500��$	��800��  � WindowStyle$  
ToolWindow=� � Title �� *
Decription+ �� Mode$ OneWay=� � 	Resources ;  5BMHRI.WCS.Server.ValueConverter.DataItemValueToString   pvvToString4  .BMHRI.WCS.Server.ValueConverter.IndexConverter  MyIndexConverter  DataTypeList S� 
MethodName S� 
ObjectTypeU QSystem.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a  System.Enum S� MethodParameters M� TypeName  VarTypeList��%�   &	        &	      &	 
     &	 B         S� $ 	GetValues��"   M� $ S7Net:DataType��S� $ 	GetValues��"   M� $ S7Net:VarType��.��� 
�������.+��� $��10q�.@��� $��设备状态��.���� $��10q�.+��� $��设备状态��$	��0,5q�.��� .+���  �� Orientation$ 
Horizontal=�$��5q�.+��� $��设备名称：��$��Center=��� $	��100��$��Center=����� *PLCDecription+�� $ 
Horizontal=�$��5q�.+��� $��PLC类型：��$��Center=��� $��50��$��Center=����� *	CpuType+�� $ 
Horizontal=�$��5q�.+��� $��机架号：��$��Center=��� $��30��$��Center=����� *Rack+�� $ 
Horizontal=�$��10q�.+��� $��插槽号：��$��Center=��� $��30��$��Center=����� *Slot+�� $ 
Horizontal=�$��5q�.+��� $��IP地址：��$��Center=��� $	��100��$��Center=����� *IP+�� $ 
Horizontal=�$��5q�.+��� $��设备编号：���� $��50��$��Center=����� *PLCID+�� $ 
Horizontal=�$��5q�.+��� $��网络状态：���� $��50��$��Center=����� *PLCConnectState+�� $ 
Horizontal=�$��5q�.+��� $��工作状态：��$��Center=��� $��60��$��Center=����� *SSJWorkState+�� $ 
Horizontal=�$��5q�.+��� $��故障代码：��$��Center=��� $��50��
���  ��$��Center=����� *	FaultCode+�� $ 
Horizontal=�$��5q�.+��� $��故障内容：��$��Center=��� $��80��
���  ��$��Center=����� *FaultContent+�� $ 
Horizontal=�$��5q�.+��� -   $��LBCircleNum��$��环线托盘数：��$��Center=��� -   $��TBCircleNum��$��80��
���  ��$��Center=����� *InCircleNum+�� $ 
Horizontal=�$��5q�.+��� -   $��LBCircleNum2��$��2环线托盘数：��$��Center=�$y�	Collapsed=��� -   $��TBCircleNum2��$��80��
���  ��$��Center=�$y�	Collapsed=����� *InCircleNum2+�� $ 
Horizontal=�$��5q�.+��� -   $��LBCircleNum3��$��3环线托盘数：��$��Center=�$y�	Collapsed=��� -   $��TBCircleNum3��$��80��
���  ��$��Center=�$y�	Collapsed=����� *InCircleNum3+�� $ 
Horizontal=�$��5q�.+��� $��大小圈模式：��$��Center=��� $��80��
���  ��$��Center=����� *
CircleMode+�� $��修改保存��$��Center=�$��10q��� $��设备重启��$��Center=�$��10q�$y�	Collapsed=��� $��设备操作��$	��0,5q�.��� .+��� $ 
Horizontal=�$��5q�.+��� $��WCS TO PLC DB4 : ��$��Right=��� $	��300������ *WCSTOPLCDB4+�� $ 
Horizontal=�$��5q�.+��� $��PLC TO WCS DB5 : ��$��Right=��� $	��300������ *PLCTOWCSDB5+� 	 � Orientation$	 
Horizontal=�$
��Left=�$��5q�.+��� $��连接网络��$��5q��� $��断开网络��$��5q��� -   $��联机��$��5q��� -	   $��脱机��$��5q��� -
   $��故障解除��$��5q��� -   $��	DB4清零��$��5q��� -   $��	DB5清零��$��5q��� -   $��信息注入��$y�	Collapsed=�$��2��$��5q�$��Right=��� -   
 ��Name$

 mmun��$y�	Collapsed=�$��2��$��5q�$
��Left=�$��60���� -   $��开始报警��$��5q��� -   $��停止报警��$��5q��� $��设备列表��.���  � RowDefinitions �� $��* ��� $
��auto �.+��� -    ��Name$ blocksLV�� �� View �� .P��� $��设备编码�� �� Width$
 auto�� �� DisplayMemberBinding �� *
Position+�� $��设备状态��$
 auto�� �� *BlockStatus+�� $��设备模式��$
 auto�� �� *CurrentMode+�� $��设备类型��$
 auto�� �� *
DeviceType+�� $��系统类型��$
 auto�� �� *
SystemType+�� $��故障代码��$
 auto�� �� *	ErrorCode+�� $��故障内容��$
 auto�� �� *FaultContent1+�� $��是否故障��$
 auto�� �� *
IsFaulty+�� $��	托盘号��$
 auto�� �� *	PalletNum+�� $��是否有载荷��$
 auto�� �� *
IsLoaded+�� $��是否占用��$
 auto�� �� *
IsOccupied+� $��1��$	 
Horizontal=�.+��� -   $��故障设备列表��$��Center=�$
��Left=�$��5q�$	��100��$��30���� -   $��全部设备列表��$��Center=�$
��Left=�$��5q�$	��100��$��30���� -   $��入库站台列表��$��Center=�$
��Left=�$��5q�$	��100��$��30���� -   $��出库站台列表��$��Center=�$
��Left=�$��5q�$	��100��$��30���� -   $��取消占位��$��Center=�$
��Left=�$��5q�$	��100��$��30���� -   $��触发入库��$��Center=�$
��Left=�$��5q�$	��100��$��30��