   M S B A M L   `   `   `  ���� K  GBMHRI.WCS.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:BMHRI.WCS.Server.StylesBMHRI.WCS.Server.Styles  W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   TabControlStyle  �� 
TargetType �� Property �� Value �� 
TargetType � RowDefinitions K� IsItemsHost �� Padding  TabItemStyle ��Name �� CornerRadius	 ��Name
 �� RelativeSource �� Mode �� Path �� Triggers P� Property P� Value �� 
TargetName ]� 
Conditions �� Property �� Value  TabControlStyle1 �Name J� SnapsToDevicePixels-   'System.Windows.Input.KeyboardNavigation    TabNavigation � ColumnDefinitions ��Name ��Name A�Name A� Rows    TabIndex K� ZIndex	 �� X1	 �� X2     DirectionalNavigation! ��Name" �� SelectedContentTemplate# �� SelectedContent"$ �� SelectedContentStringFormat% �� ContentStringFormat  TabItemStyle1& � ToolTip' ^� 
Conditions( �� Binding) �� AncestorLevel* �� AncestorType   TabControlWithUnderLineStyle+ ��Name  TabItemExWithUnderLineStyle*   $BMHRI.WCS.Server.Styles.TabItemClose,   CornerRadius-   NormalBackground. ��Name/   LogoPadding0   LogoIcon1   LogoIconWidth2   LogoIconHeigth3 J� Opacity4   NormalForeground5 �� TextAlignment&    BMHRI.WCS.Server.Styles.ButtonEx6  Name7   	CloseIcon8   Icon9   
ButtonType�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     2controls%clr-namespace:BMHRI.WCS.Server.Styles  %�%  &	        &	 �     &	     &	 �
    &	 �    &	 �    '  �    �� "  ��.���  � �� �� " ��.R�� 
������  �� $
��Auto ��� $��* �.+��� 
������ $��0 0 0 0q�._��� $
 True���� $��1��
������ $ 0q�$��0q�#��z��._��� $��PART_SelectedContentHost��$��SelectedContent���� "  ��.���  � �� �� " ��.R�� .+��� $ bd��
������ $��30��
������ $��0 0 0 0q�$ 0,0,0,0��$��0q�._��� $		 txt��$��Center=�$
��30 0q����� 
 �� $ TemplatedParent=�$ Header� P�  �@��
IsSelected$
 true��. ��� $ bd�� � ��$ 0,0,0,0q��� $ bd�� � ��
 ���� �� $ bd�� � �� �	#33ffffff�� $	 txt�� � �� �#fff]�  ��  �@��
IsSelected$ False����  �@J�IsMouseOver$
 True��./��� $	 txt�� � �� �#fff�� "  ��.���  � ��$ 2q���  � ��$ Center=���  � ��$ Center=���  � �� �	#FF0E5097��  � �� �	#FFACACAC��  � ��$ 1q���  � ��# � ����  � �� �� " ��.R��� #��z��#��z��#��z��._�� $ templateRoot��$
}�True��$
 True��$ Local=� �� $ ColumnDefinition0���� $ ColumnDefinition1��$��0 � �� $ RowDefinition0��$
��Auto ��� $ RowDefinition1��$��* �.+�A� $ HeaderPanel��$��Center=�$ 1��
������ $��0��$
 True��$��0q�$��0��$ 1��$ 1���� $ 0��
�������$	��0.1��$��Bottom=�$��0 0 0 1q�$
 True�� �� *ActualWidth+
 �� *Self+�� $ ContentPanel��$��0q�#��z��$��0��$  	Contained=�$��1��$ 2��$ Local=�._��� $! PART_SelectedContentHost��#��z" #��z# #% z$ $��SelectedContent��$��0q�# z  P�  �@��TabStripPlacement$ Bottom=�. ��� $ HeaderPanel�� � ��$ 1���� $ ContentPanel�� � ��$ 0���� $ RowDefinition0�� � ��$ * ��� $ RowDefinition1�� � ��$
 Auto �P�  �@��TabStripPlacement$
 Left=�. ��� $ HeaderPanel�� � ��$ 0���� $ ContentPanel�� � ��$ 0���� $ HeaderPanel�� � ��$ 0���� $ ContentPanel�� � ��$ 1���� $ ColumnDefinition0�� � ��$
 Auto ��� $ ColumnDefinition1�� � ��$ * ��� $ RowDefinition0�� � ��$ * ��� $ RowDefinition1�� � ��$ 0 �P�  �@��TabStripPlacement$ Right=�. ��� $ HeaderPanel�� � ��$ 0���� $ ContentPanel�� � ��$ 0���� $ HeaderPanel�� � ��$ 1���� $ ContentPanel�� � ��$ 0���� $ ColumnDefinition0�� � ��$ * ��� $ ColumnDefinition1�� � ��$
 Auto ��� $ RowDefinition0�� � ��$ * ��� $ RowDefinition1�� � ��$ 0 �P�  � {�$ False��. ��� $ templateRoot��# � �� � ���� "  ��.���  � ��
 �������  � ��
 ���� ��  � �� �	#FFACACAC��  � ��$ 0q���  � ��$ Stretch=���  � ��$ Stretch=���  � �� �� " ��.R�� $ templateRoot��$
 True��
������ .+��� $		 txt��$y�Visible=�$��Center=�$��Center=�#��z��#& z��#��z��$��CharacterEllipsis=� ^� ' �� ( �� *IsMouseOver+
 �� *Self+ true�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* �� Top.0��� $	 txt�� � �� �#fffea1^� ' �� ( �� *	IsEnabled+
 �� *Self+	 false�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* �� Left.0��� $ templateRoot�� �@J�Opacity$
 0.56Q�^� ' �� ( �� *	IsEnabled+
 �� *Self+	 false�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* ��
 Bottom.0��� $ templateRoot�� �@J�Opacity$
 0.56Q�^� ' �� ( �� *	IsEnabled+
 �� *Self+	 false�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* ��	 Right.0��� $ templateRoot�� �@J�Opacity$
 0.56Q�^� ' �� ( �� *	IsEnabled+
 �� *Self+	 false�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* �� Top.0��� $ templateRoot�� �@J�Opacity$
 0.56Q�^� ' �� ( �� *
IsSelected+
 �� *Self+ true�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* �� Top.0���  �@K�ZIndex$ 1���� $	 txt�� � �� �#fffea1�� "  ��.���  � ��$ 2q���  � ��$ Center=���  � ��$ Center=���  � ��
 �������  � �� �	#FFACACAC��  � ��$ 1q���  � ��# � ����  � �� �� " ��.R�� $ templateRoot��$
}�True��#��z��$
 True��$ Local=� �� $ ColumnDefinition0���� $ ColumnDefinition1��$��0 � �� $ RowDefinition0��$
��Auto ��� $ RowDefinition1��$��* �.+��� $+ HeaderPanel��$��Center=�
������ $��0��$
 True��$��0q�$��0��$ 1��$ 1���� $ 0��
�������$	��0.1��$��Bottom=�$��0 0 0 1q�$
 True�� �� *ActualWidth+
 �� *Self+�� $ ContentPanel��#��z��#��z��#��z��$��0��$  	Contained=�$��1��$ 2��$ Local=�._��� $! PART_SelectedContentHost��#��z" #��z# #% z$ $��SelectedContent��$��0q�# z  P�  �@��TabStripPlacement$ Bottom=�. ��� $ HeaderPanel�� � ��$ 1���� $ ContentPanel�� � ��$ 0���� $ RowDefinition0�� � ��$ * ��� $ RowDefinition1�� � ��$
 Auto �P�  �@��TabStripPlacement$
 Left=�. ��� $ HeaderPanel�� � ��$ 0���� $ ContentPanel�� � ��$ 0���� $ HeaderPanel�� � ��$ 0���� $ ContentPanel�� � ��$ 1���� $ ColumnDefinition0�� � ��$
 Auto ��� $ ColumnDefinition1�� � ��$ * ��� $ RowDefinition0�� � ��$ * ��� $ RowDefinition1�� � ��$ 0 �P�  �@��TabStripPlacement$ Right=�. ��� $ HeaderPanel�� � ��$ 0���� $ ContentPanel�� � ��$ 0���� $ HeaderPanel�� � ��$ 1���� $ ContentPanel�� � ��$ 0���� $ ColumnDefinition0�� � ��$ * ��� $ ColumnDefinition1�� � ��$
 Auto ��� $ RowDefinition0�� � ��$ * ��� $ RowDefinition1�� � ��$ 0 �P�  � {�$ False��. ��� $ templateRoot��# � �� � ���� "  ��.���  � ��
 �������  � ��
 ���� ��  � �� �	#FFACACAC��  � ��$ 0q���  � ��$ Stretch=���  � ��$ Stretch=���  � �� �� " ��.R�� $ templateRoot��$
 True��
������ .+��� $ 
_underline�����#37aefe$��0q�#��z��� .+��� $		 txt��$y�Visible=�$��Center=�$��Center=�#��z��#& z��#��z��$��CharacterEllipsis=� ^� ' �� ( �� *IsMouseOver+
 �� *Self+ true�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* �� Top.0��� $	 txt�� � �� �#37aefe^� ' �� ( �� *	IsEnabled+
 �� *Self+	 false�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* �� Left.0��� $ templateRoot�� �@J�Opacity$
 0.56Q�^� ' �� ( �� *	IsEnabled+
 �� *Self+	 false�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* ��
 Bottom.0��� $ templateRoot�� �@J�Opacity$
 0.56Q�^� ' �� ( �� *	IsEnabled+
 �� *Self+	 false�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* ��	 Right.0��� $ templateRoot�� �@J�Opacity$
 0.56Q�^� ' �� ( �� *	IsEnabled+
 �� *Self+	 false�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* �� Top.0��� $ templateRoot�� �@J�Opacity$
 0.56Q�^� ' �� ( �� *
IsSelected+
 �� *Self+ true�� ( �� *TabStripPlacement+
 �� *FindAncestor+$) 1��"* �� Top.0���  �@K�ZIndex$ 1���� $	 txt�� � �� �#37aefe�� $ 
_underline�� � ��$ 0 0 0 2q��� "   .���  � ��$ Stretch=���  � ��$ Stretch=���  � �� �#666666��  � ��$ 0 0 0 0q���  � ��$ 0q���  � ��$ 0q���  �@ 	CloseIcon$ /Images/关闭.png����  �@ NormalBackground$ White����  �@ OverBackgound$ 	#33ca5100����  �@ SelectedBackgound$ #ca5100����  �@ NormalForeground$ #555558����  �@ OverForeground$ White����  �@ SelectedForeground$ White����  � �� �� "  .R��� $ 
_bordertop��#��z��#��z��#��z��# z, #��z- #��z��#��z��#& z��._�]� .+��� $. _logo��$
��Left=�$y�Visible=�#��z/ #��z0 $��Center=�$��Center=�$��Uniform=�#��z1 #��z2 � $��_grid��$
 True�� �� $��* ��� $ 
_col_close��$��20 �.+��� $��2��
�������$3 0Q��� $
	 _txt��$��Center=�$��CharacterEllipsis=�$��3 0 3 0q�#��z4 $5 Center=�$��Center=�#��z��� $ 
_gridclose��$��1��.+��� $ 	_borderbg��
���   �$3 0Q�  $6 PART_Close_TabItem��$��Center=�$��Center=�
������ $y�Visible=�#8 z7 $
9 Icon=� P�  Z�  �@ LogoIcon. ��� $ _logo�� � y�$ 	Collapsed=�P�  �@ 
IsCanClose$ false��. ��� $ 
_gridclose�� � y�$ 	Collapsed=��� $ 
_col_close�� � ��$ 0 �P�  �@��
IsSelected$
 true��. ��� $ 
_bordertop�� �� *SelectedBackgound+
 �� *TemplatedParent+ � ���� $
 _txt�� �� *SelectedForeground+
 �� *TemplatedParent+ � ��]�  ��  �@J�IsMouseOver$
 true����  �@��
IsSelected$ false��./��� $
 _txt�� �� *OverForeground+
 �� *TemplatedParent+ � ���� $ 
_bordertop�� �� *OverBackgound+
 �� *TemplatedParent+ � ��