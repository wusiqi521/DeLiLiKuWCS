   M S B A M L   `   `   `  ����K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:BMHRI.WCS.Server.ModelsBMHRI.WCS.Server.Models  5      Q-clr-namespace:BMHRI.WCS.Server.ValueConverterBMHRI.WCS.Server.ValueConverter  5      '    !BMHRI.WCS.Server.UserControls.Led   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      8models%clr-namespace:BMHRI.WCS.Server.Models     5      HValueConverter-clr-namespace:BMHRI.WCS.Server.ValueConverter     5      $
��1500��5
      $
��1500��6     � 	Resources  5      :  4BMHRI.WCS.Server.ValueConverter.BoolToValueConverter   BoolToValueConverter  UpperLightStyle �� 
TargetType �� Property �� Value �� Triggers �� Binding �� Value  LowerLightStyle��%�  &	        &	      &	 l      5   
   6.   �� 5   
   " F�6(   .�5      ��  � ��
 �����6%   �� 5       � ��
 �   �6'   �� 5       � ��$ 2��60   �� 5       � ��$ Center=�64   �� 5       � ��$ Center=�62    5      �� 5       6   �� *CurrentMode+ 16>   .a�5      ��  � ��
 �� �6-   5      5      �� 5   
   " F�6(   .�5      ��  � ��
 �����6%   �� 5!       � ��
 �   �6'   �� 5"       � ��$ 2��60   �� 5#       � ��$ Center=�64   �� 5$       � ��$ Center=�62    5%      �� 5&       6   �� *CurrentMode+ 06>   .a�5'      ��  � ��
 �  ��6-   5(      5*      5+      .��5-      � -   5         �Name$
 grid��5-      .+�5.   
   (�  (� Stretch$ Uniform=�6   	 (� StretchDirection$
	 Both=�6$   .��50      � $
��1500��6   $
��1500��6    
 � RowDefinitions
 51      �� 52      $��* �6$   �� 53      $��* �6$   .+�57      � $��0��6   $��0,0,0,0q�6$    � ColumnDefinitions 58      �� 59      $	��750 �6+   �� 5;      $��* �6+   .+�5@      �� $��0��6    $	��入��5A       $	��750(�5B       $
��Bold"�5C       
���   �$��Center=�5F       $��Center=�5G       $��0,0,0,0q�5H       F� 5K      $��1��6   #��[ 5L      $	��750��5M      $	��750��6'   C�����Z�Z�    �Z׀�,�Z�Z�Z�    �Z�Z�Z	$��Center=�5O      $��Center=�5P      5Q      � 5T      $��1��6   $��0,0,0,0q�6$    5U      �� 5V      $	��750 �6+   �� 5X      .+�5]      �� $��0��6    $	��出��5^       $	��750(�5_       
���   �$
��Bold"�5b       $��Center=�5c       $��Center=�5d       $��0,0,0,0q�5e       F� 5h      $��1��6   #��[ 5i      $	��750��5j      $	��750��6'   C�����Z�Z�    �Z׀�,�Z�Z�Z�    �Z�Z�Z	$��Center=�5l      $��Center=�5m      5n      5o      5p      5q      5r      