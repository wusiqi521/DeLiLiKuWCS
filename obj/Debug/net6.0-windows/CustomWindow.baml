   M S B A M L   `   `   `  ����K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:BMHRI.WCS.ServerBMHRI.WCS.Server  5   	   #    BMHRI.WCS.Server.CustomWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:BMHRI.WCS.Server     5   	     � Title$  CustomWindow��5   	   $	��450��6   $	��800��6+   '  !System.Windows.Shell.WindowChrome   WindowChrome 5	        5
   
      CornerRadius$ 0��6      CaptionHeight$ 0Q�5         GlassFrameThickness$ 1q�5         UseAeroCaptionButtons$ False��5         NonClientFrameEdges$
 None=�5      ��5      �� 5   
    �� 
TargetType" �6   .�5      ��  �� Property � ��	 �� Value	 5      �� 5      
 �� 
TargetType"
 �6&   .R�5      �� ._�5      � .+�5   "   �� .u�5   &   �� 5   #   �� 5   "    ��Name$ WindowResizeGrip��6-   $��Right=�5   )   $��false��5   )   $y�	Collapsed=�5   )   $��Bottom=�5   )   5      5        �� Triggers 5!      ]� 5"       ]� 
Conditions 5#   "   �� 5$   &    �� Property �@�
ResizeMode �� Value$ CanResizeWithGrip=�6F   �� 5%   &    �@�WindowState$ Normal=�6G   ./�5'   "   ��  �� 
TargetName$ WindowResizeGrip��6?    � y�$	 Visible=�6]   5(      5*      5,      5-      .��5/      � ���	#FFF00D0D � RowDefinitions 50   
   �� 51      $��* �6   �� 52      $��* �6    � ColumnDefinitions 54   
   �� 55      $��* �6   �� 56      $��* �6   .+�58   
   �� ���	#FF23BB69$��10q�6)   �� 59   
   ���	#FF23BB69$��10q�6)   $��1��6>   �� 5:   
   ���	#FF23BB69$��10q�6)   $��1��6>   �� 5;   
   ���	#FF23BB69$��10q�6)   $��1��6>   $��1��6N   5<      5>      