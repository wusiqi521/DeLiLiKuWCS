   M S B A M L   `   `   `  ���� K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:BMHRI.WCS.Server.UserControlsBMHRI.WCS.Server.UserControls  3    -BMHRI.WCS.Server.UserControls.WMSTaskCreateUC   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     =local+clr-namespace:BMHRI.WCS.Server.UserControls     .���� $
��Auto=�$
��Auto=�.���   � RowDefinitions  �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto �.+��� $��0��$��10q� �� Orientation$ 
Horizontal=�$
��Left=�.+��� $��用户名：��$
��10,5q��� -    ��Name$ 
UserNameTB��$	��200��$
��10,5q�$��Center=��� $��密  码：��$
��10,5q�G� -    G�Name$ 
PasswordTB��$	��200��$
��10,5q�$��Center=��� -    ��Name$ AuthorizedDetectionAlarm��$0��*执行操作前请输入用户名和密码��
���  ��$��Center=��� $��1��$��入库申请��
���   �$��10q�.���� $ 
Horizontal=�.+��� $��入库位置��$��10q�$��Center=�$��Center=��� -    ��Name$ InboundPortCBB�� �� DisplayMemberPath$ Position��$��10q�$	��160��$��Center=�$��Center=��� $��	托盘号��$��10q�$��Center=�$��Center=��� -   $ 
PaletNumTB��$��10q�$	��150��$��Center=�$��Center=��� -    ��Name$ ApplyInboundBT��$��创建申请��$��10q�   BtnInfoStyle#��[  $��Center=�$��Center=��� -   $ 
AlarmLabel��
���  ��$��10q��� $��8��$��空托盘申请��
���   �$��10q�.���� $ 
Horizontal=�.+��� $��申请位置��$��10q�$��Center=�$��Center=��� -   $ EmptyAppPortCBB��$ Position��$��10q�$	��160��$��Center=�$��Center=��� -	   $ EmptyApplyBT��$��创建申请��$��10q�#��[  $��Center=�$��Center=��� -
   $ EmptyApplyAlarmLabel��
���  ��$��10q��� $��3��$!��创建输送机入库任务��
���   �$��10q�.���� $ 
Horizontal=�.+��� $��起始位置��$��10q�$��Center=�$��Center=��� -   $ IStaAddrCBB��$ Position��$��10q�$	��160��$��Center=�$��Center=��� $��	托盘号��$��10q�$��Center=�$��Center=��� -   $ IPaletNumTB��$��10q�$	��150��$��Center=�$��Center=��� $��目的地址��$��10q�$��Center=�$��Center=��� -   $ IEndAddrCBB��$ Position��$��10q�$	��160��$��Center=�$��Center=��� -   $ CreateITaskBT��$!��创建输送机入库任务��$��10q�#��[  $��Center=�$��Center=��� -   $ IAlarmLB��
���  ��$��10q��� $��4��$��创建上架任务��
���   �$��10q�.���� $ 
Horizontal=�.+��� $��起始位置��$��10q�$��Center=�$��Center=��� -   $ UStaAddrCBB��$ Position��$��10q�$	��160��$��Center=�$��Center=��� $��	托盘号��$��10q�$��Center=�$��Center=��� -   $ UPaletNumTB��$��10q�$	��150��$��Center=�$��Center=��� $��目的地址��$��10q�$��Center=�$��Center=�2   ,BMHRI.WCS.Server.UserControls.GoodLocationUC  -     Name$ UEndAddrGLUC���� -   $ CreateUTaskBT��$��创建上架任务��$��10q�#��[  $��Center=�$��Center=��� -   $ UAlarmLB��
���  ��$��10q��� $��5��$��创建出库任务��
���   �$��10q�.���� $ 
Horizontal=�.+��� $��起始位置��$��10q�$��Center=�$��Center=�  -   $ BStaAddrGLUC���� $��	托盘号��$��10q�$��Center=�$��Center=��� -   $ BPaletNumTB��$��10q�$	��150��$��Center=�$��Center=��� $��目的地址��$��10q�$��Center=�$��Center=��� -   $ BEndAddrCBB��$ Position��$��10q�$	��160��$��Center=�$��Center=��� -   $ CreateOutboundBT��$��创建出库任务��$��10q�#��[  $��Center=�$��Center=��� -   $ BAlarmLB��
���  ��$��10q��� $��6��$��创建移库任务��
���   �$��10q�.���� $ 
Horizontal=�.+��� $��起始位置��$��10q�$��Center=�$��Center=�  -   $ MStaAddrGLUC���� $��	托盘号��$��10q�$��Center=�$��Center=��� -   $ MPaletNumTB��$��10q�$	��150��$��Center=�$��Center=��� $��目的地址��$��10q�$��Center=�$��Center=�  -   $ MEndAddrGLUC���� -   $ CreateMoveboundBT��$��创建移库任务��$��10q�#��[  $��Center=�$��Center=��� -   $ MAlarmLB��
���  ��$��10q��� $��9��$!��输送机入库二次申请��
���   �$��10q�.���� $ 
Horizontal=�.+��� $��入库位置��$��10q�$��Center=�$��Center=��� -   $ SecondInboundPortCBB��$ Position��$��10q�$	��160��$��Center=�$��Center=��� $��	托盘号��$��10q�$��Center=�$��Center=��� -    $ SecPaletNumTB��$��10q�$	��150��$��Center=�$��Center=��� $��	任务号��$��10q�$��Center=�$��Center=��� -!   $ TaskidNumTB��$��10q�$	��150��$��Center=�$��Center=��� -"   $ SecondApplyInboundBT��$��创建申请��$��10q�#��[  $��Center=�$��Center=��� -#   $ SecAlarmLabel��
���  ��$��10q��� $��10��$��创建联机入库任务��	 � ToolTip$	 创建联机入库任务��
���   �$��10q�.���� $ 
Horizontal=�.+��� $��起始位置��$��10q�$��Center=�$��Center=��� -$   $ AStaAddrCBB��$ Position��$��10q�$	��160��$��Center=�$��Center=��� $��	托盘号��$��10q�$��Center=�$��Center=��� -%   $ APaletNumTB��$��10q�$	��150��$��Center=�$��Center=��� $��目的地址��$��10q�$��Center=�$��Center=�  -&   $ AEndAddrGLUC���� -'   $ CreateInboundBT��$��创建联机入库任务��$��10q�#��[  $��Center=�$��Center=��� -(   $ AAlarmLB��
���  ��$��10q��� $��11��$!��创建输送机出库任务��
���   �$��10q�.���� $ 
Horizontal=�.+��� $��起始位置��$��10q�$��Center=�$��Center=��� -)   $ SStartAddrCBB��$ Position��$��10q�$	��160��$��Center=�$��Center=��� $��	托盘号��$��10q�$��Center=�$��Center=��� -*   $ SPaletNumTB��$��10q�$	��150��$��Center=�$��Center=��� $��目的地址��$��10q�$��Center=�$��Center=��� -+   $ SEndAddrCBB��$ Position��$��10q�$	��160��$��Center=�$��Center=��� -,   $ CreateSSJOutTaskBT��$!��创建输送机出库任务��$��10q�#��[  $��Center=�$��Center=��� --   $ SAlarmLB��
���  ��$��10q�