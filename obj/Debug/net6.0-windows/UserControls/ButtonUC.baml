   M S B A M L   `   `   `  ����K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:BMHRI.WCS.Server.UserControlsBMHRI.WCS.Server.UserControls  5      ,    &BMHRI.WCS.Server.UserControls.ButtonUC   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:BMHRI.WCS.Server.UserControls     5      .��5	      �   � ColumnDefinitions  5
   
   �� 5      $
��0.1* �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   � $��1��6   $��0��6$    � Orientation$ 
Horizontal=�61   .+�5      �� $��用户名：��6   $��Center=�6#   $��5q�6>   �� 5      -   5         ��Name$ 
UserNameTB��5      $��80��6*   $��Center=�65   �� 5      $��
密 码：��6   $��Center=�6#   $��5q�6>   G� 5      -   5         G�Name$ 
PasswordTB��5      $��80��6.   $��Center=�69   $��5q�6T   �� 5      -   5         ��Name$ 
AlarmLabel��5      $0��*执行操作前请输入用户名和密码��6(   
���  ��$��5q�5      $��Center=�6(   $	��200��6C   5      � 5   
   $��1��6   $��1��6     5      �� 5      $��* �6#   �� 5      $��* �6#   .+�5      �� -   5        $��一键联机��5      $��0��6$   $��5q�65   $	��100��6[   $��30��6g   �� 5       -   5        $��一键脱机��5       $��1��6$   $��5q�65   $	��100��6\   $��30��6h   $
��Left=�6t   5!      5"      5#      