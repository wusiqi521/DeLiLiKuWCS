   M S B A M L   `   `   `  ����K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:BMHRI.WCS.Server.UserWindowsBMHRI.WCS.Server.UserWindows  5   	   >    8BMHRI.WCS.Server.UserWindows.HaiKangMessageDetailWindows   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   <local*clr-namespace:BMHRI.WCS.Server.UserWindows     5   	     � Title$  海康消息处理��5   	   $	��400��6    � WindowStyle$ 
ToolWindow=�6$    � SizeToContent$ Height=�6=    � WindowStartupLocation$ CenterScreen=�6T   .��5	      �� $��10q�6   $��1q�6   
����  �._�5
   
   �  � RowDefinitions 5      �� 5      $
��auto �6    �� 5      $
��auto �6    �� 5      $
��auto �6    �� 5      $
��auto �6    �� 5      $
��auto �6    .+�5      � $��3��6    � Orientation$ 
Horizontal=�6%   .+�5      �� $��用户名：��6   $��Center=�6'   $��5q�6B   �� 5      -   5         ��Name$ 
UserNameTB��5      $	��100��6.   $��Center=�6:   �� 5      $��
密 码：��6   $��Center=�6'   $��5q�6B   G� 5      -   5         G�Name$ 
PasswordTB��5      $	��100��62   $��Center=�6>   $��5q�6Y   �� 5      -   5         ��Name$ AlarmLB��5      $0��*执行操作前请输入用户名和密码��6)   ���	#FFD41212$��5q�6Y   5      �� 5      -   5        	 ��Name$	 ReSendBT��5      $��4��6'   $��模拟重发��64   $��70��6D   $��5q�6n   $��Center=�6y   �� 5      -   5        $ OperationAlarmLB��5      $��Center=�6.   $��5q�6I   $��2��6T   �� 5      $��
10,10,10,0q�6   ���	#FF0C7ED4$��1q�6B   ._�5      �� -   5        $ ReqPanel��5      
 �� TextWrapping$

 Wrap=�6,   5      �� 5      $��1��6   $��
10,10,10,0q�6#   ���	#FF0C7ED4$��1q�6O   ._�5       �� -   5        $ RspPanel��5       $

 Wrap=�6,   5!      5"      5#      5$      