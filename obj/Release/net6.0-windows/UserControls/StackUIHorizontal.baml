   M S B A M L   `   `   `  ���� K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:BMHRI.WCS.Server.UserControlsBMHRI.WCS.Server.UserControls  Q-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  A%clr-namespace:BMHRI.WCS.Server.ModelsBMHRI.WCS.Server.Models  5    /BMHRI.WCS.Server.UserControls.StackUIHorizontal   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     =local+clr-namespace:BMHRI.WCS.Server.UserControls     HValueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     8models%clr-namespace:BMHRI.WCS.Server.Models     -     � 	Resources  :  4BMHRI.WCS.Server.ValueConverter.BoolToValueConverter   BoolToValueConverter  StackerStatusGridStyle �� 
TargetType �� Triggers �� Binding �� Value �� Property �� Value��%
  &	        &	        �� " � ��  �� *DDJWorkState+	 Fault.a���  � ��
 �  ����  �� *DDJWorkState+ None.a���  � ��
 �������  �� *DDJWorkState+
 Online.a���  � ��
 �2͚���  �� *DDJWorkState+ Standby.a���  � ��
 � � ���  �� *DDJWorkState+ Working.a���  � ��
 � �|���  �� *DDJWorkState+ Offline.a���  � ��
 � �����  �� *DDJWorkState+
 Manual.a���  � ��
 �����.��� #��[ $��1q� � ColumnDefinitions �� $	��20* ��� $	��40* ��� $	��10* �.+��� $��2�����	#FFE0C30D(� $��Stretch=�.���� -    ��Name$ DDJID��
����������� *PLCID+�� $��1�����	#FF1AA7E5$��6��$��6��y��� *	HasPallet+	 �� 	Converter#	 [  
 �� ConverterParameter$
 Visible�� �� FallbackValue$ Hidden��