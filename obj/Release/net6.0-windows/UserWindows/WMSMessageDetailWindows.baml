   M S B A M L   `   `   `  ����   BMHRI.WCS.Server:    4BMHRI.WCS.Server.UserWindows.WMSMessageDetailWindows   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006       � Title$  WMS消息处理��$	��400�� � WindowStyle$ 
ToolWindow=� � SizeToContent$ Height=� � WindowStartupLocation$ CenterScreen=�.���� $��10q�$��1q�
����  �._��  � RowDefinitions �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto �.+�� $��4�� � Orientation$ 
Horizontal=�.+��� $��用户名：��$��Center=�$��5q��� -    ��Name$ 
UserNameTB��$	��100��$��Center=��� $��
密 码：��$��Center=�$��5q�G� -    G�Name$ 
PasswordTB��$	��100��$��Center=�$��5q��� -    ��Name$ AlarmLB��$0��*执行操作前请输入用户名和密码�����	#FFD41212$��5q��� -   	 ��Name$	 ReSendBT��$��5��$��模拟重发��$��70��$��5q�$��Center=��� -   $ OperationAlarmLB��$��Center=�$��6q�$��2���� $��
10,10,10,0q����	#FF0C7ED4$��1q�$��1��._��� -   $ ReqPanel��
 �� TextWrapping$

 Wrap=��� $��2��$��
10,10,10,0q����	#FF0C7ED4$��1q�._��� -   $ RspPanel��$

 Wrap=��� $��
10,10,10,0q����	#FF0C7ED4$��1q�._�� .+��� -   $ 	MsgDescLB��$��消息说明：���� -	   $ SdTimeLB��$��发送时间：���� -
   $ MsgDirLB��$��消息方向：��