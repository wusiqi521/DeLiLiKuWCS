   M S B A M L   `   `   `  ����K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:BMHRI.WCS.Server.UserControlsBMHRI.WCS.Server.UserControls  5      Q-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  5      )    #BMHRI.WCS.Server.UserControls.RGVUI   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:BMHRI.WCS.Server.UserControls     5      HValueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     5      -   5          � 	Resources  5
      :  4BMHRI.WCS.Server.ValueConverter.BoolToValueConverter   BoolToValueConverter  PalletOcupppBorderStyle �� 
TargetType �� Triggers �� Binding �� Value �� Property �� Value  RGVStatusGridStyle��%�  &	        &	      &	 �       5   
   6.   �� 5   
   " ��60    5      �� 5       6   �� *IsOccupying+ 16>   .a�5      ��  � ��
 ��� �63   5      5      �� 5   
   " �6+    5      �� 5       6   �� *	RGVStatus+ None6<   .a�5      ��  � ��
 �����63   5      �� 5       6   �� *	RGVStatus+ Standby6<   .a�5      ��  � ��
 � � �63   5      �� 5       6   �� *	RGVStatus+ Working6<   .a�5      ��  � ��
 � �|�63   5      �� 5       6   �� *	RGVStatus+	 Fault6<   .a�5      ��  � ��
 �  ��63   5       5"      5#      .��5$      � #��[ 6    � RowDefinitions 5&   
   �� 5(      $��8* �6   �� 5)      $��4* �6   .+�5,   
   �� $��0��6   ._�5-      � -   5         �Name$ g��5-      ���	#FFADCEE4.+�5.      �� ���	#FF1AA7E5$��19��6-   $��16��68   $��Center=�6D   $��Center=�6�   y�6_   �� *IsOccupying+	 �� 	Converter#	 [  
 �� ConverterParameter$
 Visible�� �� FallbackValue$ 	Collapsed��6�   (� 5/      .��50      �� -   5        $��RGVNum��50      
���   ���6>   �� *
DeviceID+51      52      53      �� 55   
   $��1��6   ���	#FFADCEE4$	��0.5q�66   #��[ 6L    � ToolTip 56      �� 57      ��6   �� *Description+._�59      (� .��5:      �� -   5        $��RGVPalletNum��5:      
����  ���6@   �� *	PalletNum+5;      5<      5=      5>      