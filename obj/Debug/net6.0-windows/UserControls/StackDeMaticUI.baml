   M S B A M L   `   `   `  ����K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:BMHRI.WCS.Server.UserControlsBMHRI.WCS.Server.UserControls  5      Q-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  6H   A%clr-namespace:BMHRI.WCS.Server.ModelsBMHRI.WCS.Server.Models  6�   2    ,BMHRI.WCS.Server.UserControls.StackDeMaticUI   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:BMHRI.WCS.Server.UserControls     5      HValueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     6H   8models%clr-namespace:BMHRI.WCS.Server.Models     6�   -   5          � 	Resources  5	      :  4BMHRI.WCS.Server.ValueConverter.BoolToValueConverter   BoolToValueConverter  StackerStatusGridStyle �� 
TargetType �� Triggers �� Binding �� Value �� Property �� Value��%^  &	        &	        5
   
   6.   �� 5   
   " �6/    5      �� 5       6   �� *DDJWorkState+	 Fault6?   .a�5      ��  � ��
 �  ��63   5      �� 5       6   �� *DDJWorkState+ None6?   .a�5      ��  � ��
 �����63   5      �� 5       6   �� *DDJWorkState+
 Online6?   .a�5      ��  � ��
 �2͚�63   5      �� 5       6   �� *DDJWorkState+ Standby6?   .a�5      ��  � ��
 � � �63   5      �� 5       6   �� *DDJWorkState+ Working6?   .a�5      ��  � ��
 � �|�63   5      �� 5       6   �� *DDJWorkState+ Offline6?   .a�5      ��  � ��
 � ���63   5      �� 5       6   �� *DDJWorkState+
 Manual6?   .a�5       ��  � ��
 �����63   5!      5#      5$      .��5%      � #��[ 6   $��1q�6;    � RowDefinitions 5&   
   �� 5'      $	��10* �6   �� 5(      $	��40* �6   �� 5)      $	��20* �6   .+�5,   
   �� ���	#FFE0C30D6   (� 5-   
   $��2��6   $��Stretch=�6   .��5.      �� -   5         ��Name$ DDJID��5.      
���������6#   �� *PLCID+6=   5/      �� 50   
   $��1��6   ���	#FF1AA7E5$��6��62   $��6��6<   y�6G   �� *	HasPallet+	 �� 	Converter#	 [  
 �� ConverterParameter$
 Visible�� �� FallbackValue$ Hidden��51      52      