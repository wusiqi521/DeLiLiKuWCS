   M S B A M L   `   `   `  ����K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:BMHRI.WCS.Server.UserControlsBMHRI.WCS.Server.UserControls  5      3    -BMHRI.WCS.Server.UserControls.WMSTaskCreateUC   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:BMHRI.WCS.Server.UserControls     5      .��5	      �� $
��Auto=�6   $
��Auto=�66   .��5
   
   �   � RowDefinitions  5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6    �� 5      $
��auto �6    �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6    �� 5      $
��auto �6    �� 5      $
��auto �6    �� 5      $
��auto �6    .+�5   
   �� $��0��6   $��10q�6"    �� Orientation$ 
Horizontal=�6.   $
��Left=�6G   .+�5      �� $��用户名：��6   $
��10,5q�6#   �� 5      -   5         ��Name$ 
UserNameTB��5      $	��200��6*   $
��10,5q�69   $��Center=�6G   �� 5      $��密  码：��6   $
��10,5q�6$   G� 5      -   5         G�Name$ 
PasswordTB��5      $	��200��6.   $
��10,5q�6=   $��Center=�6K   �� 5      -   5         ��Name$ AuthorizedDetectionAlarm��5      $0��*执行操作前请输入用户名和密码��66   
���  ��$��Center=�6`   5       �� 5!   
   $��1��6   $��入库申请��6    
���   �$��10q�6A   .��5"      �� $ 
Horizontal=�6   .+�5#      �� $��入库位置��6   $��10q�6'   $��Center=�63   $��Center=�6N   �� 5$      -   5         ��Name$ InboundPortCBB��5$       �� DisplayMemberPath$ Position��63   $��10q�6P   $	��160��6\   $��Center=�6k   $��Center=�6�   �� 5%      $��	托盘号��6   $��10q�6&   $��Center=�62   $��Center=�6O   �� 5&      -   5        $ 
PaletNumTB��5&      $��10q�6.   $	��150��6:   $��Center=�6F   $��Center=�6c   �� 5'      -   5         ��Name$ ApplyInboundBT��5'      $��创建申请��61   $��10q�6@      BtnInfoStyle#��[  6i   $��Center=�6�   $��Center=�6�   �� 5(      -   5        $ 
AlarmLabel��5(      
���  ��$��10q�6=   5)      5*      �� 53      $��2��6   $!��创建输送机入库任务��6$   
���   �$��10q�6J   .��54      �� $ 
Horizontal=�6   .+�55      �� $��起始位置��6   $��10q�6+   $��Center=�67   $��Center=�6R   �� 56      -   5        $ IStaAddrCBB��56      $ Position��64   $��10q�6Q   $	��160��6]   $��Center=�6l   $��Center=�6�   6�   �� 57      $��	托盘号��6   $��10q�6*   $��Center=�66   $��Center=�6S   �� 58      -	   5        $ IPaletNumTB��58      $��10q�63   $	��150��6?   $��Center=�6K   $��Center=�6h   �� 59      $��目的地址��6   $��10q�6+   $��Center=�67   $��Center=�6T   �� 5:      -
   5        $ IEndAddrCBB��5:      $ Position��64   $��10q�6Q   $	��160��6]   $��Center=�6l   $��Center=�6�   �� 5;      -   5        $ CreateITaskBT��5;      $!��创建输送机入库任务��64   $��10q�6H   #��[  6u   $��Center=�6�   $��Center=�6�   �� 5<      -   5        $ IAlarmLB��5<      
���  ��$��10q�6?   5=      5>      �� 5?      $��3��6   $��创建上架任务��6$   
���   �$��10q�6G   .��5@      �� $ 
Horizontal=�6   .+�5A      �� $��起始位置��6   $��10q�6'   $��Center=�63   $��Center=�6N   �� 5B      -   5        $ UStaAddrCBB��5B      $ Position��64   $��10q�6Q   $	��160��6]   $��Center=�6l   $��Center=�6�   6�   �� 5C      $��	托盘号��6   $��10q�6&   $��Center=�62   $��Center=�6O   �� 5D      -   5        $ UPaletNumTB��5D      $��10q�6/   $	��150��6;   $��Center=�6G   $��Center=�6d   �� 5E      $��目的地址��6   $��10q�6'   $��Center=�63   $��Center=�6P   2   ,BMHRI.WCS.Server.UserControls.GoodLocationUC  5F      -   5          Name$ UEndAddrGLUC��5F   '   �� 5G      -   5        $ CreateUTaskBT��5G      $��创建上架任务��60   $��10q�6A   #��[  6i   $��Center=�6�   $��Center=�6�   �� 5H      -   5        $ UAlarmLB��5H      
���  ��$��10q�6;   5I      5J      �� 5L   
   $��4��6   $��创建出库任务��6    
���   �$��10q�6C   .��5M      �� $ 
Horizontal=�6   .+�5N      �� $��仓库编号��6   $��10q�6+   $��Center=�67   $��Center=�6R   �� 5O      -   5        $ OutWarehouse��5O      $��10q�65   $	��160��6A   $��Center=�6P   $��Center=�6k   6�   �� 5Q      $��起始位置��6   $��10q�6+   $��Center=�67   $��Center=�6R     5R      -   5        $ BStaAddrGLUC��5R   '   �� 5S      $��	托盘号��6   $��10q�6&   $��Center=�62   $��Center=�6O   �� 5T      -   5        $ BPaletNumTB��5T      $��10q�6/   $	��150��6;   $��Center=�6G   $��Center=�6d   �� 5U      $��目的地址��6   $��10q�6'   $��Center=�63   $��Center=�6N   �� 5V      -   5        $ BEndAddrCBB��5V      $ Position��64   $��10q�6Q   $	��160��6]   $��Center=�6l   $��Center=�6�   �� 5Y      -   5        $ CreateOutboundBT��5Y      $��创建出库任务��63   $��10q�6D   #��[  6n   $��Center=�6�   $��Center=�6�   �� 5Z      -   5        $ BAlarmLB��5Z      
���  ��$��10q�6;   5[      5\      �� 5]   
   $��5��6   $��创建移库任务��6    
���   �$��10q�6C   .��5^      �� $ 
Horizontal=�6   .+�5_      �� $��仓库编号��6   $��10q�6+   $��Center=�67   $��Center=�6R   �� 5`      -   5        $ OutWarehouse2��5`      $��10q�66   $	��160��6B   $��Center=�6Q   $��Center=�6l   6�   �� 5a      $��起始位置��6   $��10q�6+   $��Center=�67   $��Center=�6R     5b      -   5        $ MStaAddrGLUC��5b   '   �� 5c      $��	托盘号��6   $��10q�6&   $��Center=�62   $��Center=�6O   �� 5d      -   5        $ MPaletNumTB��5d      $��10q�6/   $	��150��6;   $��Center=�6G   $��Center=�6d   �� 5e      $��目的地址��6   $��10q�6'   $��Center=�63   $��Center=�6N     5f      -   5        $ MEndAddrGLUC��5f   '   �� 5h      -   5        $ CreateMoveboundBT��5h      $��创建移库任务��64   $��10q�6E   #��[  6o   $��Center=�6�   $��Center=�6�   �� 5i      -   5        $ MAlarmLB��5i      
���  ��$��10q�6;   5j      5k      �� 5l   
   $��6��6   $��创建直出搬运任务��6    
���   �$��10q�6E   .��5m      �� $ 
Horizontal=�6   .+�5n      �� $��起始位置��6   $��10q�6'   $��Center=�63   $��Center=�6N   �� 5o      -   5        $ CStaAddrCBB3��5o      $ Position��65   $��10q�6R   $	��160��6^   $��Center=�6m   $��Center=�6�   6�   �� 5p      $��	托盘号��6   $��10q�6&   $��Center=�62   $��Center=�6O   �� 5q      -   5        $ CPaletNumTB3��5q      $��10q�60   $	��150��6<   $��Center=�6H   $��Center=�6e   �� 5r      $��目的地址��6   $��10q�6'   $��Center=�63   $��Center=�6N   �� 5s      -    5        $ CEndAddrCBB3��5s      $ Position��65   $��10q�6R   $	��160��6^   $��Center=�6m   $
��Left=�6�   �� 5t      -!   5        $ CreateCTaskBT3��5t      $��创建直行搬运任务��61   $��10q�6D   #��[  6n   $��Center=�6�   $��Center=�6�   �� 5u      -"   5        $ 	CAlarmLB3��5u      
���  ��$��10q�6<   5v      5w      �� 5�      $��7��6   $��创建联机入库任务��6$   	 � ToolTip$	 创建联机入库任务��66   
���   �$��10q�6\   .��5�      �� $ 
Horizontal=�6   .+�5�      �� $��起始位置��6   $��10q�6,   $��Center=�68   $��Center=�6S   �� 5�      -#   5        $ AStaAddrCBB��5�      $ Position��64   $��10q�6Q   $	��160��6]   $��Center=�6l   $��Center=�6�   6�   �� 5�      $��	托盘号��6   $��10q�6+   $��Center=�67   $��Center=�6T   �� 5�      -$   5        $ APaletNumTB��5�      $��10q�63   $	��150��6?   $��Center=�6K   $��Center=�6h   �� 5�      $��目的地址��6   $��10q�6,   $��Center=�68   $��Center=�6S     5�      -%   5        $ AEndAddrGLUC��5�   +   �� 5�      -&   5        $ CreateInboundBT��5�      $��创建联机入库任务��66   $��10q�6J   #��[  6t   $��Center=�6�   $��Center=�6�   �� 5�      -'   5        $ AAlarmLB��5�      
���  ��$��10q�6?   5�      5�      �� 5�      $��8��6   $!��创建输送机出库任务��6$   
���   �$��10q�6J   .��5�      �� $ 
Horizontal=�6   .+�5�      �� $��起始位置��6   $��10q�6+   $��Center=�67   $��Center=�6R   �� 5�      -(   5        $ SStartAddrCBB��5�      $ Position��66   $��10q�6S   $	��160��6_   $��Center=�6n   $��Center=�6�   6�   �� 5�      $��	托盘号��6   $��10q�6*   $��Center=�66   $��Center=�6S   �� 5�      -)   5        $ SPaletNumTB��5�      $��10q�63   $	��150��6?   $��Center=�6K   $��Center=�6h   �� 5�      $��目的地址��6   $��10q�6+   $��Center=�67   $��Center=�6T   �� 5�      -*   5        $ SEndAddrCBB��5�      $ Position��64   $��10q�6Q   $	��160��6]   $��Center=�6l   $��Center=�6�   �� 5�      -+   5        $ CreateSSJOutTaskBT��5�      $!��创建输送机出库任务��69   $��10q�6M   #��[  6{   $��Center=�6�   $��Center=�6�   
 � Cursor$
  ��6�   �� 5�      -,   5        $ SAlarmLB��5�      
���  ��$��10q�6?   5�      5�      �� 5�      $��9��6   $!��创建提升机换层任务��6$   
���   �$��10q�6J   .��5�      �� $ 
Horizontal=�6   .+�5�      �� $��起始位置��6   $��10q�6+   $��Center=�67   $��Center=�6R   �� 5�      --   5        $ CStartAddrCBB��5�      $ Position��66   $��10q�6S   $	��160��6_   $��Center=�6n   $��Center=�6�   6�   �� 5�      $��	托盘号��6   $��10q�6*   $��Center=�66   $��Center=�6S   �� 5�      -.   5        $ CPaletNumTB��5�      $��10q�63   $	��150��6?   $��Center=�6K   $��Center=�6h   �� 5�      $��目的地址��6   $��10q�6+   $��Center=�67   $��Center=�6T   �� 5�      -/   5        $ CEndAddrCBB��5�      $ Position��64   $��10q�6Q   $	��160��6]   $��Center=�6l   $��Center=�6�   �� 5�      -0   5        $ CreateSSJInToOutTaskBT��5�      $!��创建提升机换层任务��6=   $��10q�6Q   #��[  6~   $��Center=�6�   $��Center=�6�   �� 5�      -1   5        $ CAlarmLB��5�      
���  ��$��10q�6?   5�      5�      �� 5�      $��10��6   $)��#一号提升机自动出库(北侧)��6%   
���   �$��10q�6O   .��5�      �� $ 
Horizontal=�6   .+�5�      �� $��出库层位��6   $��10q�6+   $��Center=�67   $��Center=�6R   �� 5�      -2   5        $ OutboundFloorCBB1��5�      $��10q�6:   $	��160��6F   $��Center=�6U   $��Center=�6p   .@�5�      �� 
 � Tag$ 1��6'   $��一层��6/   �� 5�      $ 2��6'   $��二层��6/   �� 5�      $ 3��6'   $��三层��6/   5�      �� 5�      -3   5        $ StartAutoOutboundBT1��5�      $��开启自动出库��6;   $��10q�6L   #��[  6{   $��Center=�6�   $��Center=�6�   �� 5�      -4   5        $ StatusLabel1��5�      $��当前状态：未开启��62   
��� � �$��10q�6X   $��Center=�6d   �� 5�      -5   5        $ AlarmLabel1��5�      
���  ��$��10q�6B   5�      5�      �� 5�      $��11��6   $)��#二号提升机自动出库(中部)��6%   
���   �$��10q�6O   .��5�      �� $ 
Horizontal=�6   .+�5�      �� $��出库层位��6   $��10q�6+   $��Center=�67   $��Center=�6R   �� 5�      -6   5        $ OutboundFloorCBB2��5�      $��10q�6:   $	��160��6F   $��Center=�6U   $��Center=�6p   .@�5�      �� $ 1��6'   $��一层��6/   �� 5�      $ 2��6'   $��二层��6/   �� 5�      $ 3��6'   $��三层��6/   5�      �� 5�      -7   5        $ StartAutoOutboundBT2��5�      $��开启自动出库��6;   $��10q�6L   #��[  6{   $��Center=�6�   $��Center=�6�   �� 5�      -8   5        $ StatusLabel2��5�      $��当前状态：未开启��62   
��� � �$��10q�6X   $��Center=�6d   �� 5�      -9   5        $ AlarmLabel2��5�      
���  ��$��10q�6B   5�      5�      �� 5�      $��12��6   $)��#三号提升机自动出库(南侧)��6%   
���   �$��10q�6O   .��5�      �� $ 
Horizontal=�6   .+�5�      �� $��出库层位��6   $��10q�6+   $��Center=�67   $��Center=�6R   �� 5�      -:   5        $ OutboundFloorCBB3��5�      $��10q�6:   $	��160��6F   $��Center=�6U   $��Center=�6p   .@�5�      �� $ 1��6'   $��一层��6/   �� 5�      $ 2��6'   $��二层��6/   �� 5�      $ 3��6'   $��三层��6/   5�      �� 5�      -;   5        $ StartAutoOutboundBT3��5�      $��开启自动出库��6;   $��10q�6L   #��[  6{   $��Center=�6�   $��Center=�6�   �� 5�      -<   5        $ StatusLabel3��5�      $��当前状态：未开启��62   
��� � �$��10q�6X   $��Center=�6d   �� 5�      -=   5        $ AlarmLabel3��5�      
���  ��$��10q�6B   5�      5�      5�      5�      5�      