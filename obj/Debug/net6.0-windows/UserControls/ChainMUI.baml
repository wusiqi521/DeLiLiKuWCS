   M S B A M L   `   `   `  ����K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  5      A%clr-namespace:BMHRI.WCS.Server.ModelsBMHRI.WCS.Server.Models  6S   ,    &BMHRI.WCS.Server.UserControls.ChainMUI   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Ced4http://schemas.microsoft.com/expression/2010/drawing     5      HValueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     5      8models%clr-namespace:BMHRI.WCS.Server.Models     6S   -   5          >�Name$  userControl��5       � 	Resources 5
      :  4BMHRI.WCS.Server.ValueConverter.BoolToValueConverter   BoolToValueConverter7   1BMHRI.WCS.Server.ValueConverter.MultiplyConverter  MultiplyConverter  BorderStyle �� 
TargetType �� Triggers �� Binding �� Value �� Property �� Value��%�  &	        &	      &	 &       5   
   6.     5   
   6+   �� 5   
   " ��6$    5      �� 5       6   �� *
IsFaulty+ true6;   .a�5      ��  � ��
 �  ��64   5      �� 5       6   �� *
IsFaulty+	 false6;   .a�5      ��  � ��
 ��  �64   5      �� 5       6   �� *BlockStatus+ 26>   .a�5      ��  � ��
 � ���64   5      5      5       .��5!      �� $	��0.7q�6   #��[ 6#    � ToolTip 5"   
   �� 5#      ��6   �� *DeviceDescription+._�5%   
   � -   5        	 �Name$	 g��5%      ���	#FFEBF3F4.+�5&      �� ���	#FF1AA7E5$��16��6)   $��16��64   $��Center=�6@   $��Center=�6�   y�6[   �� *
IsOccupied+
 �� 	Converter#
 [   �� ConverterParameter$ Visible�� �� FallbackValue$ 	Collapsed��6�   (� 5)      $��Center=�6   $��Center=�62   ��5*      �� *ActualWidth+ �� RelativeSource ��  �� AncestorType UserControl#
 [ $	 0.5����5+      �� *ActualHeight+ ��  UserControl#
 [ $	 0.6��.��5,      �� -   5         ��Name$ DeviceIDLabel��5,      $��Bottom=�6L   ��6/   �� *
Position+6L   5-      5/      50      51      