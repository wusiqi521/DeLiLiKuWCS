   M S B A M L   `   `   `  ����   BMHRI.WCS.Server<    6BMHRI.WCS.Server.UserWindows.DeviceMessageDetailWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$  设备消息处理��$	��370��$	��460�� � WindowStyle$ 
ToolWindow=�.���� $��10q�$��1q�
����  �._��  � RowDefinitions �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto �.+�� $��2�� � Orientation$ 
Horizontal=�.+��� $��用户名：��$��Center=�$��5q��� -    ��Name$ 
UserNameTB��$	��100��$��Center=��� $��
密 码：��$��Center=�$��5q�G� -    G�Name$ 
PasswordTB��$	��100��$��Center=�$��5q��� -    ��Name$ AlarmLB��$0��*执行操作前请输入用户名和密码�����	#FFD41212$��5q��� -    ��Name$ ReSendBT��$��3��$��模拟重发��$��70��$��5q�$��Center=��� -   $ OperationAlarmLB��$��Center=�$��5q�$��1��� $��5q�.+��� $��PLCID:��$��5q��� $��5q����� *PLCID+�� $��
优先级:��$��5q��� $��5q����� *SendPriority+�� $��内容:��$��5q��� $��5q����� *Trans+�� $��方向:��$��5q��� $��5q����� *	Direction+�� $��消息说明:��$��5q��� $��5q����� *
MsgParse+�� $��发送时间:��$��5q��� $��5q����� *Tkdat+ �� StringFormat$ {0:yyyy-MM-dd HH:mm:sss}��