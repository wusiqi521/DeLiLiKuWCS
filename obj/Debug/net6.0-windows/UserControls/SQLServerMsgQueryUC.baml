   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5       BMHRI.WCS.Server7   1BMHRI.WCS.Server.UserControls.SQLServerMsgQueryUC   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6sys&clr-namespace:System;assembly=mscorlib     5      -   5          � 	Resources  5	         ListViewItemStyle �� 
TargetType �� Property �� Value��%g   &	        �� 5
   
   " ��6*   .�5      ��  � ��$
 Left=�6;   6J   -   5        5      5      .��5      � ���	#FFE5E5E5 � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��* �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $
��10,5q�6.   .+�5      �� -   5         ��Name$ TitlLB��5      
���  ��$��Center=�65   $
��10,0q�6P   �� 5      $��起始时间:��6   $��Center=�6$   $��Center=�6?   $	��5,0q�6\   (  "System.Windows.Controls.DatePicker  5      -   5          Name$ StartDP��5      $��Center=�6*   
 �� Now	   SelectedDate#	 Z 6E   $	��5,0q�6p   $��95��6}   �� 5      $	��---��6   $��Center=�6"   $	��5,0q�6=   �� 5      $��结束时间:��6   $��Center=�6$   $��Center=�6?   $	��5,0q�6\     5      -   5        $ EndDP��5      #	 Z 6(   $��Center=�6S   $	��5,0q�6n   $��95��6{   �� 5      -   5        
 ��Name$
 
DeviceIDCB��5      $��Center=�6+   $	��5,0q�6F   $��80��6S   $y�	Collapsed=�6a   �� 5      $��包含字符串��6   $��Center=�6$   $	��5,0q�6?   �� 5      -   5         ��Name$ StrTB��5      $��Center=�6%   $��80��6B   $��Center=�6P   $	��5,0q�6k   �� 5      -   5        $��查询��5      $��80��6"   $��Center=�60   $	��5,0q�6K   6X   �� 5       -	   5        $ AlarmLB��5       
���  ��$��Center=�66   5!      �� 5"   
   -
   5         ��Name$ 
SqlQueryLV��5"      $��1��6'   #��[  64    �� View 5#      �� 5$      .P�5&      �� $��方向��6&    �� Width$
 auto��6`    �� DisplayMemberBinding 62   �� *MsgDirection+6`   �� 5'      $��消息说明��6&   $
 auto��6]    64   �� *	MsgDesc+6]   �� 5(      $��请求消息��6&   $
 auto��6\    64   �� *ReqMsg+6\   �� 5)      $��反馈消息��6&   $
 auto��6\    64   �� *RspMsg+6\   �� 5*      $��时间��6&   $
 auto��6�    62   �� *	MsgTime+ �� StringFormat$! {0:yyyy-MM-dd HH:mm:ss:fff}��6�   5,      5.      5/      50      