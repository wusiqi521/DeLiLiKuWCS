   M S B A M L   `   `   `  ����  BMHRI.WCS.Server<    6BMHRI.WCS.Server.UserWindows.DeviceMessageDetailWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -   5          � Title$  设备消息处理��5   	   $	��370��6   $	��460��6%    � WindowStyle$ 
ToolWindow=�61   .��5      �� $��10q�6   $��1q�6   
����  �._�5	   
   �  � RowDefinitions 5
      �� 5      $
��auto �6    �� 5      $
��auto �6    �� 5      $
��auto �6    �� 5      $
��auto �6    .+�5      � $��2��6    � Orientation$ 
Horizontal=�6%   .+�5      �� $��用户名：��6   $��Center=�6'   $��5q�6B   �� 5      -   5         ��Name$ 
UserNameTB��5      $	��100��6.   $��Center=�6:   �� 5      $��
密 码：��6   $��Center=�6'   $��5q�6B   G� 5      -   5         G�Name$ 
PasswordTB��5      $	��100��62   $��Center=�6>   $��5q�6Y   �� 5      -   5         ��Name$ AlarmLB��5      $0��*执行操作前请输入用户名和密码��6)   ���	#FFD41212$��5q�6Y   5      �� 5      -   5         ��Name$ ReSendBT��5      $��3��6'   $��模拟重发��64   $��70��6D   $��5q�6n   $��Center=�6y   �� 5      -   5        $ OperationAlarmLB��5      $��Center=�6.   $��5q�6I   $��1��6T   � 5      $��5q�6   .+�5      �� $��PLCID:��6   $��5q�6)   �� 5      $��5q�6   ��6#   �� *PLCID+�� 5      $��
优先级:��6   $��5q�6'   �� 5      $��5q�6   ��6#   �� *SendPriority+�� 5      $��内容:��6   $��5q�6&   �� 5       $��5q�6   ��6#   �� *Trans+�� 5!      $��方向:��6   $��5q�6&   �� 5"      $��5q�6   ��6$   �� *	Direction+�� 5#      $��消息说明:��6   $��5q�6(   �� 5$      $��5q�6   ��6$   �� *
MsgParse+�� 5%      $��发送时间:��6   $��5q�6(   �� 5&      $��5q�6   ��6#   �� *Tkdat+ �� StringFormat$ {0:yyyy-MM-dd HH:mm:sss}��5'      5(      5)      5*      